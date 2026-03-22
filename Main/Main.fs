namespace Main

open System
open Types
open HelperFunctions
/// <summary>Ponto de entrada do programa.</summary>
module Program =
    /// <summary>Função para captura de argumentos da linha de comando.</summary>
    /// <param name="args">Um array de strings representando os argumentos passados na linha de comando.</param>
    /// <returns>Uma tupla contendo um Status opcional e um Origin opcional, dependendo dos argumentos fornecidos.</returns>
    let parseArgs (args: string[]) : (Status option * Origin option) =
        let status =
            args
            |> Array.tryFind (fun arg -> arg.StartsWith("status="))
            |> Option.map (fun s -> s.Substring(7))
            |> Option.bind (fun s ->
                match s with
                | "Complete" -> Some Status.Complete
                | "Pending" -> Some Status.Pending
                | "Cancelled" -> Some Status.Cancelled
                | _ -> None)

        let origin =
            args
            |> Array.tryFind (fun arg -> arg.StartsWith("origin="))
            |> Option.map (fun s -> s.Substring(7))
            |> Option.bind (fun s ->
                match s with
                | "Online" -> Some Origin.Online
                | "Physical" -> Some Origin.Physical
                | _ -> None)

        (status, origin)

    
    /// <summary>Ponto de entrada do programa. Orquestra as etapas de Extração, Transformação e Carga (ETL).</summary>
    /// <param name="argv">Um array de strings representando os argumentos passados na linha de comando.</param>
    /// <returns>Um inteiro representando o código de saída do programa (0 para sucesso, 1 para falha).</returns>
    [<EntryPoint>]
    let main argv =
        let etlWorkflow =
            async {
                // Extract
                let! orders = Extract.getOrders ()
                let! items = Extract.getItems ()

                // Transform
                let statusFilter, originFilter = parseArgs argv
                let summaries = Transform.calculateSummaries orders items statusFilter originFilter

                // Load
                let outputPath = System.IO.Path.Combine(__SOURCE_DIRECTORY__, "..", "saida", "order_totals.csv")
                Load.saveSummariesToCsv summaries outputPath

                printfn "Report successfully generated at %s" outputPath
            }
        try
            Async.RunSynchronously etlWorkflow
            0 // Return 0 for success
        with
        | ex ->
            printfn "An error occurred: %s" ex.Message
            1 // Return 1 for failure