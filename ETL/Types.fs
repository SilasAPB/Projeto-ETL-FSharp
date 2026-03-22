namespace Types

type Status = 
    | Complete
    | Pending
    | Cancelled

type Origin =
    | Online
    | Physical

type Order = {
    Id: int
    ClientID: int
    OrderDate: System.DateTime
    Status: Status
    Origin: Origin
}

type OrderItem = {
    OrderID: int
    ProductID: int
    Quantity: int
    Price: decimal
    Tax: decimal
}

type OrderSummary = {
    OrderID: int
    TotalAmount: decimal
    TotalTaxes: decimal
}