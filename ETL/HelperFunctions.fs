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
    /// Combina os arrays de pedidos e itens em uma única lista "achatada", similar a um INNER JOIN de SQL.
    /// </summary>
    /// <param name="orders">Um array de todos os pedidos.</param>
    /// <param name="items">Um array de todos os itens de pedido.</param>
    /// <returns>Um array de 'JoinedOrderItem' contendo os dados combinados.</returns>
    let innerJoin (orders: Order[]) (items: OrderItem[]) : JoinedOrderItem[] =
        // Cria um mapa para busca rápida de pedidos por ID
        let orderMap = orders |> Array.map (fun o -> (o.Id, o)) |> Map.ofArray
        
        items
        |> Array.choose (fun item -> // 'choose' é como 'map' mas para 'Option', descartando os 'None'
            match Map.tryFind item.OrderID orderMap with
            | Some order ->
                Some {
                    OrderID = order.Id
                    ClientID = order.ClientID
                    OrderDate = order.OrderDate
                    Status = order.Status
                    Origin = order.Origin
                    ProductID = item.ProductID
                    Quantity = item.Quantity
                    Price = item.Price
                    Tax = item.Tax
                }
            | None -> None) // Descarta itens que não têm um pedido correspondente


    /// <summary>
    /// Calcula os totais de valor e impostos para cada pedido, com filtros opcionais.
    /// </summary>
    /// <param name="orders">Um array de todos os pedidos.</param>
    /// <param name="items">Um array de todos os itens de pedido.</param>
    /// <param name="statusFilter">Filtro opcional para o status do pedido.</param>
    /// <param name="originFilter">Filtro opcional para a origem do pedido.</param>
    /// <returns>Um array de 'OrderSummary' contendo os totais para cada pedido filtrado.</returns>
    let calculateSummaries (joinedData: JoinedOrderItem[]) (statusFilter: Status option) (originFilter: Origin option) : OrderSummary[] =
        joinedData
        |> Array.filter (fun row ->
            let statusMatch =
                match statusFilter with
                | Some s -> row.Status = s
                | None -> true 
            let originMatch =
                match originFilter with
                | Some o -> row.Origin = o
                | None -> true
            statusMatch && originMatch)
        |> Array.groupBy (fun row -> row.OrderID) // Agrupa todos os itens pelo ID do pedido
        |> Array.map (fun (orderId, items) ->
            let totalAmount =
                items
                |> Array.sumBy (fun item -> item.Price * (decimal item.Quantity))

            let totalTaxes =
                items
                |> Array.sumBy (fun item -> item.Price * (decimal item.Quantity) * item.Tax)

            { OrderSummary.OrderID = orderId
              TotalAmount = totalAmount
              TotalTaxes = totalTaxes })
    
    /// <summary>
    /// Calcula os resumos mensais de vendas, agrupando por ano e mês, e calculando as médias de receita e impostos.
    /// </summary>
    /// <param name="orders">Um array de todos os pedidos.</param>
    /// <param name="items">Um array de todos os itens de pedido.</param>
    /// <returns>Um array de 'MonthlySummary' contendo as médias de receita e impostos
    let calculateMonthlySummaries (joinedData: JoinedOrderItem[]) : MonthlySummary[] =
        let orderSummaries = calculateSummaries joinedData None None
        // Precisamos dos dados originais dos pedidos para obter a data
        // Esta é uma limitação do design atual, mas podemos contorná-la.
        // Uma abordagem mais avançada seria passar um mapa de datas.
        // Por simplicidade, vamos agrupar os dados juntados diretamente.
        joinedData
        |> Array.groupBy (fun row -> (row.OrderDate.Year, row.OrderDate.Month))
        |> Array.map (fun ((year, month), monthData) ->
            // Agrupa por pedido dentro do mês para calcular a receita de cada pedido
            let monthlyOrderRevenues =
                monthData
                |> Array.groupBy (fun row -> row.OrderID)
                |> Array.map (fun (_, orderItems) ->
                    let revenue = orderItems |> Array.sumBy (fun i -> i.Price * (decimal i.Quantity))
                    let taxes = orderItems |> Array.sumBy (fun i -> i.Price * (decimal i.Quantity) * i.Tax)
                    (revenue, taxes)
                )

            let averageRevenue = monthlyOrderRevenues |> Array.averageBy fst
            let averageTaxes = monthlyOrderRevenues |> Array.averageBy snd

            { MonthlySummary.Year = year
              Month = month
              AverageRevenue = averageRevenue
              AverageTaxes = averageTaxes })

