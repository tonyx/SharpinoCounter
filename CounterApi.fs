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
    let connection = 
        "Server=127.0.0.1;"+
        "Database=es_01;" +
        "User Id=safe;"+
        "Password=safe;"

    open Sharpino.MemoryStorage

    let doNothingBroker =
        { 
            notify = None
            notifyAggregate = None
        }

    type SharpinoCounterApi(storage: IEventStore) =
        // let storage = MemoryStorage()
        let counterStateViewer =
            getStorageFreshStateViewer<Counter, CounterEvents> storage
        member this.Increment() =
            ResultCE.result
                {
                    return!
                        Increment ()
                        |> runCommand<Counter, CounterEvents> storage doNothingBroker counterStateViewer
                }
        member this.Decrement() =
            ResultCE.result
                {
                    return! 
                        Decrement ()
                        |> runCommand<Counter, CounterEvents> storage doNothingBroker counterStateViewer
                }
        member this.Clear() =
            ResultCE.result
                {
                    return!
                        Clear ()
                        |> runCommand<Counter, CounterEvents> storage doNothingBroker counterStateViewer
                }
        member this.GetState() =
            ResultCE.result
                {
                    let! (_, state, _, _) = 
                        counterStateViewer ()
                    return state.State
                }





