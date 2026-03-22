namespace Main

/// <summary>Leitura de dados.</summary>
module Extract =
    open System.Net.Http
    open Types
    open HelperFunctions

    let client = new HttpClient()

    /// <summary>Leitura do arquivo de pedidos.</summary>
    let getOrders () : Async<Order[]> =
        async {
            let url = "https://raw.githubusercontent.com/SilasAPB/Projeto-ETL-FSharp/main/dados/order.csv"
            let! csvData = client.GetStringAsync(url) |> Async.AwaitTask
            let ordersArray = csvData.Split([| System.Environment.NewLine |], System.StringSplitOptions.RemoveEmptyEntries)
            return HelperFunctions.Convert.ArrayToOrders ordersArray
        }

    /// <summary>Leitura do arquivo de itens de pedidos.</summary>
    let getItems () : Async<OrderItem[]> =
        async {
            let url = "https://raw.githubusercontent.com/SilasAPB/Projeto-ETL-FSharp/main/dados/order_item.csv"
            let! csvData = client.GetStringAsync(url) |> Async.AwaitTask
            let itemsArray = csvData.Split([| System.Environment.NewLine |], System.StringSplitOptions.RemoveEmptyEntries)
            return HelperFunctions.Convert.ArrayToItems itemsArray
        }

