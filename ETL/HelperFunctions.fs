namespace HelperFunctions
open Types

/// Contém funções para converter dados brutos em tipos de domínio.
module Convert=
    
    /// <summary> Converte um array de strings (uma linha do CSV) em um registro do tipo Order. </summary>
    /// <param name="fields">Um array de strings representando os campos de um pedido.</param>
    /// <returns>Um registro 'Order' preenchido.</returns>
    
    let elementToOrder (fields: string[]) : Order =
        let id = int fields.[0]
        let clientId = int fields.[1]
        let orderDate = System.DateTime.Parse(fields.[2])
        let status = match fields.[3] with
                     | "Complete" -> Status.Complete
                     | "Pending" -> Status.Pending
                     | "Cancelled" -> Status.Cancelled
                     | _ -> failwith "Invalid status"
        let origin = match fields.[4] with
                     | "O" -> Origin.Online
                     | "P" -> Origin.Physical
                     | _ -> failwith "Invalid origin"
        { Order.Id = id
          ClientID = clientId
          OrderDate = orderDate
          Status = status
          Origin = origin }

    /// <summary>
    /// Converte um array de linhas de um arquivo CSV de pedidos em um array de registros 'Order'.
    /// Ignora a primeira linha (cabeçalho).
    /// </summary>
    /// <param name="lines">Todas as linhas do arquivo CSV de pedidos.</param>
    /// <returns>Um array de registros 'Order'.</returns>      
    let ArrayToOrders (lines: string[]) : Order[] =
        lines
        |> Array.tail
        |> Array.map (fun line -> line.Split(','))
        |> Array.map elementToOrder


    /// <summary> Converte um array de strings (uma linha do CSV) em um registro do tipo OrderItem. </summary>
    /// <param name="fields">Um array de strings representando os campos de um item de pedido.</param>
    /// <returns>Um registro 'OrderItem' preenchido.</returns>
    let elementToItem (fields: string[]) : OrderItem =
        let orderId = int fields.[0]
        let productId = int fields.[1]
        let quantity = int fields.[2]
        let price = decimal fields.[3]
        let tax = decimal fields.[4]
        { OrderItem.OrderID = orderId
          ProductID = productId
          Quantity = quantity
          Price = price
          Tax = tax
          }


    /// <summary>
    /// Converte um array de linhas de um arquivo CSV de pedidos em um array de registros 'OrderItem'.
    /// Ignora a primeira linha (cabeçalho).
    /// </summary>
    /// <param name="lines">Todas as linhas do arquivo CSV de pedidos.</param>
    /// <returns>Um array de registros 'OrderItem'.</returns>
    let ArrayToItems (lines: string[]) : OrderItem[] =
        lines
        |> Array.tail
        |> Array.map (fun line -> line.Split(','))
        |> Array.map elementToItem

/// Contém funções para realizar transformações e cálculos nos dados.
module Transform=
    /// <summary>
    /// Calcula os totais de valor e impostos para cada pedido, com filtros opcionais.
    /// </summary>
    /// <param name="orders">Um array de todos os pedidos.</param>
    /// <param name="items">Um array de todos os itens de pedido.</param>
    /// <param name="statusFilter">Filtro opcional para o status do pedido.</param>
    /// <param name="originFilter">Filtro opcional para a origem do pedido.</param>
    /// <returns>Um array de 'OrderSummary' contendo os totais para cada pedido filtrado.</returns>
    let calculateSummaries (orders: Order[]) (items: OrderItem[]) (statusFilter: Status option) (originFilter: Origin option) : OrderSummary[] =
        let filteredOrders =
            orders
            |> Array.filter (fun order ->
                let statusMatch =
                    match statusFilter with
                    | Some s -> order.Status = s
                    | None -> true
                let originMatch =
                    match originFilter with
                    | Some o -> order.Origin = o
                    | None -> true
                statusMatch && originMatch)

        filteredOrders
        |> Array.map (fun order ->
            let itemsForOrder =
                items
                |> Array.filter (fun item -> item.OrderID = order.Id)

            let totalAmount =
                itemsForOrder
                |> Array.sumBy (fun item -> item.Price * (decimal item.Quantity))

            let totalTaxes =
                itemsForOrder
                |> Array.sumBy (fun item -> item.Tax)

            { OrderSummary.OrderID = order.Id
              TotalAmount = totalAmount
              TotalTaxes = totalTaxes })
    
    /// <summary>
    /// Calcula os resumos mensais de vendas, agrupando por ano e mês, e calculando as médias de receita e impostos.
    /// </summary>
    /// <param name="orders">Um array de todos os pedidos.</param>
    /// <param name="items">Um array de todos os itens de pedido.</param>
    /// <returns>Um array de 'MonthlySummary' contendo as médias de receita e impostos
    let calculateMonthlySummaries (orders: Order[]) (items: OrderItem[]) : MonthlySummary[] =
        let orderSummaries = calculateSummaries orders items None None

        orderSummaries
        |> Array.groupBy (fun summary ->
            let order = orders |> Array.find (fun o -> o.Id = summary.OrderID)
            (order.OrderDate.Year, order.OrderDate.Month))
        |> Array.map (fun ((year, month), summaries) ->
            let averageRevenue =
                summaries
                |> Array.averageBy (fun s -> s.TotalAmount)

            let averageTaxes =
                summaries
                |> Array.averageBy (fun s -> s.TotalTaxes)

            { MonthlySummary.Year = year
              Month = month
              AverageRevenue = averageRevenue
              AverageTaxes = averageTaxes })

