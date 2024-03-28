namespace sharpinoCounter
open System
open FSharpPlus
open FsToolkit.ErrorHandling
open Sharpino.Definitions
open Sharpino.Utils
open Sharpino
open Sharpino.Core
open Sharpino.Utils
open Sharpino.CommandHandler
open Sharpino.StateView
open Sharpino.Definitions

open Sharpino.Storage
open sharpinoCounter.Counter
open sharpinoCounter.CounterEvents
open sharpinoCounter.CounterCommands

module SharpinoCounterApi =

    let doNothingBroker =
        { 
            notify = None
            notifyAggregate = None
        }

    type SharpinoCounterApi(storage: IEventStore) =
        // let storage = MemoryStorage()
        let counterStateViewer =
            getStorageFreshStateViewer<Counter, CounterEvents> storage

        let runCounterCommand cmd =
            cmd 
            |> runCommand<Counter, CounterEvents> storage doNothingBroker counterStateViewer

        member this.Increment() =
            result
                {
                    return!
                        Increment ()
                        |> runCounterCommand 
                }
        member this.Decrement() =
            result
                {
                    return! 
                        Decrement ()
                        |> runCounterCommand
                }
        member this.Clear() =
            result
                {
                    return!
                        Clear ()
                        |> runCounterCommand
                }
        member this.GetState() =
            result
                {
                    return! 
                        counterStateViewer ()
                        |> Result.map (fun (_, state, _, _) -> state.State)
                }





