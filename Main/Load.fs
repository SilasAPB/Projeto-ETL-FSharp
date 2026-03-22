namespace Main

module Load =
    open Types

    let saveSummariesToCsv (summaries: OrderSummary[]) (filePath: string) =
        let header = "order_id,total_amount,total_taxes"
        let lines =
            summaries
            |> Array.map (fun s ->
                sprintf "%d,%.2f,%.2f" s.OrderID s.TotalAmount s.TotalTaxes)
        let content = System.String.Join(System.Environment.NewLine, Array.concat [| [|header|]; lines |])
        System.IO.File.WriteAllText(filePath, content)