namespace Main
/// <summary>Exportação dos dados.</summary>
module Load =
    open Types

    /// <summary>Função para salvar resumos de pedidos em um arquivo CSV.</summary>
    /// <param name="summaries">Um array de resumos de pedidos a serem salvos.</param>
    /// <param name="filePath">O caminho do arquivo onde os resumos serão salvos.</param>
    let saveSummariesToCsv (summaries: OrderSummary[]) (filePath: string) =
        let header = "order_id,total_amount,total_taxes"
        let lines =
            summaries
            |> Array.map (fun s ->
                sprintf "%d,%.2f,%.2f" s.OrderID s.TotalAmount s.TotalTaxes)
        let content = System.String.Join(System.Environment.NewLine, Array.concat [| [|header|]; lines |])
        System.IO.File.WriteAllText(filePath, content)

    let saveMonthlySummariesToCsv (summaries: MonthlySummary[]) (filePath: string) =
        let header = "year,month,average_revenue,average_taxes"
        let lines =
            summaries
            |> Array.map (fun s ->
                sprintf "%d,%d,%.2f,%.2f" s.Year s.Month s.AverageRevenue s.AverageTaxes)
        let content = System.String.Join(System.Environment.NewLine, Array.concat [| [|header|]; lines |])
        System.IO.File.WriteAllText(filePath, content)