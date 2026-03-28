module Tests

open System
open Xunit
open Types 
open HelperFunctions.Convert 
open HelperFunctions.Transform

[<Fact>]
let ``elementToOrder TestTrue`` () =
    let input = [| "1"; "101"; "2023-01-15T00:00:00"; "Complete"; "O" |]
    
    let expected : Order = {
        Id = 1
        ClientID = 101
        OrderDate = System.DateTime(2023, 1, 15)
        Status = Status.Complete
        Origin = Origin.Online
    }

    let actual = elementToOrder input

    Assert.Equal(expected, actual)

[<Fact>]
let ``elementToOrder TestFalse`` () =
    let badInput = [| "1"; "101"; "2023-01-15T00:00:00"; "Shipped"; "O" |]

    Assert.Throws<Exception>(System.Func<obj>(fun () -> elementToOrder badInput))


[<Fact>]
let ``elementToItem TestTrue`` ()=
    let input = [| "67"; "23"; "42"; "32.02"; "3.32" |]
    
    let expected : OrderItem = {
        OrderID = 67
        ProductID = 23
        Quantity = 42
        Price = decimal(32.02)
        Tax = decimal(3.32)
    }

    // Act: Executamos a função a ser testada
    let actual = elementToItem input

    // Assert: Verificamos se o resultado é o esperado
    Assert.Equal(expected, actual)


[<Fact>]
let ``elementToItem TestFalse`` () =
    let badInput = [| "67"; "23"; "42"; "Fabio Ayres"; "3.32" |]

    Assert.Throws<FormatException>(System.Func<obj>(fun () -> elementToItem badInput))



[<Fact>]
let ``calculateSummaries TestTrue`` () =
    // Arrange
    let orders = [| 
        { Id = 1; ClientID = 101; OrderDate = System.DateTime(2023, 1, 15); Status = Status.Complete; Origin = Origin.Online }
    |]
    let items = [| 
        { OrderID = 1; ProductID = 23; Quantity = 2; Price = decimal 10.00; Tax = decimal 0.10 }
        { OrderID = 1; ProductID = 24; Quantity = 1; Price = decimal 20.00; Tax = decimal 0.20 }
    |]
    let expected = [| 
        { OrderID = 1; TotalAmount = decimal 40.00; TotalTaxes = decimal 6.00 }
    |]
    
    // Act
    // CORREÇÃO: Primeiro faz o join, depois calcula
    let joinedData = innerJoin orders items
    let actual = calculateSummaries joinedData (Some Status.Complete) None

    // Assert
    Assert.Equal<OrderSummary seq>(expected, actual)

[<Fact>]
let ``calculateSummaries TestFalse`` () =
    // Arrange
    let orders = [| 
        { Id = 1; ClientID = 101; OrderDate = System.DateTime(2023, 1, 15); Status = Status.Complete; Origin = Origin.Online }
    |]
    let items = [| 
        { OrderID = 1; ProductID = 23; Quantity = 2; Price = decimal 10.00; Tax = decimal 1.00 }
    |]
    let expected = [||]

    // Act
    // CORREÇÃO: Primeiro faz o join, depois calcula
    let joinedData = innerJoin orders items
    let actual = calculateSummaries joinedData (Some Status.Pending) None

    // Assert
    Assert.Equal<OrderSummary seq>(expected, actual)

[<Fact>]
let ``calculateMonthlySummaries TestTrue`` () =
    let orders = [| 
        { Id = 1; ClientID = 101; OrderDate = System.DateTime(2023, 1, 15); Status = Status.Complete; Origin = Origin.Online }
        { Id = 2; ClientID = 102; OrderDate = System.DateTime(2023, 1, 20); Status = Status.Complete; Origin = Origin.Online }
        { Id = 3; ClientID = 103; OrderDate = System.DateTime(2023, 2, 5); Status = Status.Complete; Origin = Origin.Online }
    |]
    
    let items = [| 
        { OrderID = 1; ProductID = 23; Quantity = 2; Price = decimal 10.0; Tax = decimal 0.10 }
        { OrderID = 1; ProductID = 24; Quantity = 1; Price = decimal 20.0; Tax = decimal 0.20 }
        { OrderID = 2; ProductID = 23; Quantity = 2; Price = decimal 10.0; Tax = decimal 0.10 }
        { OrderID = 2; ProductID = 24; Quantity = 1; Price = decimal 20.0; Tax = decimal 0.20 }
        { OrderID = 3; ProductID = 23; Quantity = 2; Price = decimal 10.0; Tax = decimal 0.10 }
        { OrderID = 3; ProductID = 24; Quantity = 1; Price = decimal 20.0; Tax = decimal 0.20 }
    |]

    // CORREÇÃO: Criamos o array primeiro
    let expectedUnsorted = [| 
        { Year = 2023; Month = 1; AverageRevenue = decimal 40.00; AverageTaxes = decimal 6.00 }
        { Year = 2023; Month = 2; AverageRevenue = decimal 40.00; AverageTaxes = decimal 6.00 }
    |]
    // E depois ordenamos em uma variável separada
    let expected = expectedUnsorted |> Array.sortBy (fun s -> s.Month)

    // Act
    let joinedData = innerJoin orders items
    let actualUnsorted = calculateMonthlySummaries joinedData
    let actual = actualUnsorted |> Array.sortBy (fun s -> s.Month)

    // Assert
    Assert.Equal<MonthlySummary seq>(expected, actual)

[<Fact>]
let ``calculateMonthlySummaries TestFalse`` () =
    // Arrange
    let orders = [||]
    let items = [| 
        { OrderID = 1; ProductID = 23; Quantity = 2; Price = decimal 10.00; Tax = decimal 1.00 }
    |]
    let expected = [||]

    // Act
    // CORREÇÃO: Primeiro faz o join, depois calcula
    let joinedData = innerJoin orders items
    let actual = calculateMonthlySummaries joinedData

    // Assert
    Assert.Equal<MonthlySummary seq>(expected, actual)

[<Fact>]
let ``My test`` () =
    Assert.True(true)