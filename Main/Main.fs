namespace Main

open System
open Types
open HelperFunctions

module Program =
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

    [<EntryPoint>]
    let main argv =
        try
            // Extract
            let orders = Extract.getOrders ()
            let items = Extract.getItems ()

            // Transform
            let statusFilter, originFilter = parseArgs argv
            let summaries = Transform.calculateSummaries orders items statusFilter originFilter

            // Load
            let outputPath = System.IO.Path.Combine(__SOURCE_DIRECTORY__, "..", "dados", "report.csv")
            Load.saveSummariesToCsv summaries outputPath

            printfn "Report successfully generated at %s" outputPath
            0 // Return 0 for success
        with
        | ex ->
            printfn "An error occurred: %s" ex.Message
            1 // Return 1 for failure