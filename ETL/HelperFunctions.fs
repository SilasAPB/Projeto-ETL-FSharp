namespace HelperFunctions
open Types

module Convert=
    
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
    
    let ArrayToOrders (lines: string[]) : Order[] =
        lines
        |> Array.tail
        |> Array.map (fun line -> line.Split(','))
        |> Array.map elementToOrder

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

    let ArrayToItems (lines: string[]) : OrderItem[] =
        lines
        |> Array.tail
        |> Array.map (fun line -> line.Split(','))
        |> Array.map elementToItem


module Transform=
    // Funções de transformação podem ser adicionadas aqui
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
                |> Array.sumBy (fun item -> item.Price * (decimal item.Quantity) * item.Tax)

            { OrderSummary.OrderID = order.Id
              TotalAmount = totalAmount
              TotalTaxes = totalTaxes })

