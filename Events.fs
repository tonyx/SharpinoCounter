
namespace sharpinoCounter
open System
open FSharpPlus
open FsToolkit.ErrorHandling
open Sharpino.Definitions
open Sharpino.Utils
open Sharpino
open Sharpino.Core
open sharpinoCounter.Counter

module CounterEvents =
    type CounterEvents =
        | Incremented of unit
        | Decremented of unit
        | Cleared of unit
            interface Event<Counter> with
                member this.Process (counter: Counter) =
                    match this with
                    | Incremented _ -> counter.Increment()
                    | Decremented _ -> counter.Decrement()
                    | Cleared _ -> counter.Clear()
        static member Deserialize (serializer: ISerializer, json: Json) =
            serializer.Deserialize<CounterEvents>(json)
        member this.Serialize (serializer: ISerializer) =
            this
            |> serializer.Serialize

