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
    open Sharpino.MemoryStorage

    let doNothingBroker =
        { 
            notify = None
            notifyAggregate = None
        }

    type SharpinoCounterApi() =
        let storage = MemoryStorage()
        let counterStateViewer =
            getStorageFreshStateViewer<Counter, CounterEvents> storage
        member this.Increment() =
            ResultCE.result
                {
                    let command = Increment()
                    let! result = 
                        command
                        |> runCommand<Counter, CounterEvents> storage doNothingBroker counterStateViewer
                    return result
                }
        member this.Decrement() =
            ResultCE.result
                {
                    let command = Decrement()
                    let! result = 
                        command
                        |> runCommand<Counter, CounterEvents> storage doNothingBroker counterStateViewer
                    return result
                }
        member this.Clear() =
            ResultCE.result
                {
                    let command = Clear()
                    let! result = 
                        command
                        |> runCommand<Counter, CounterEvents> storage doNothingBroker counterStateViewer
                    return result
                }
        member this.GetState() =
            ResultCE.result
                {
                    let! (_, state, _, _) = 
                        counterStateViewer ()
                    return state.State
                }





