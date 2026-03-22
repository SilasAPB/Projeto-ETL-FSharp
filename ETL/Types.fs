namespace Types

/// <summary> diferentes status de um pedido. </summary> 
type Status = 
    | Complete
    | Pending
    | Cancelled

/// <summary> diferentes canais de origem de um pedido. </summary> 
type Origin =
    | Online
    | Physical

/// <summary> Estrutura que representa um pedido. </summary> 
type Order = {
    Id: int
    ClientID: int
    OrderDate: System.DateTime
    Status: Status
    Origin: Origin
}

/// <summary> Estrutura que representa um item de pedido. </summary>
type OrderItem = {
    OrderID: int
    ProductID: int
    Quantity: int
    Price: decimal
    Tax: decimal
}

/// <summary> Estrutura que representa um resumo de pedido. </summary>
type OrderSummary = {
    OrderID: int
    TotalAmount: decimal
    TotalTaxes: decimal
}