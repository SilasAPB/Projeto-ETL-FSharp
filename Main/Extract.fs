namespace Main

/// <summary>Leitura de dados.</summary>
module Extract =
    open Types
    open HelperFunctions
    
    /// <summary>Leitura do arquivo de pedidos.</summary>
    let getOrders () =
        let orderPath = System.IO.Path.Combine(__SOURCE_DIRECTORY__, "..", "dados", "order.csv")
        let ordersArray = System.IO.File.ReadAllLines(orderPath)
        HelperFunctions.Convert.ArrayToOrders ordersArray

    /// <summary>Leitura do arquivo de itens de pedidos.</summary>
    let getItems () =
        let itemPath = System.IO.Path.Combine(__SOURCE_DIRECTORY__, "..", "dados", "order_item.csv")
        let itemsArray = System.IO.File.ReadAllLines(itemPath)
        HelperFunctions.Convert.ArrayToItems itemsArray
