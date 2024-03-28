
namespace sharpinoCounter
open System
open Sharpino
open FSharpPlus
open FsToolkit.ErrorHandling
open Sharpino.Definitions
open Sharpino.Utils
open Sharpino


module Counter =
    type Counter(state: int) =
        let stateId = Guid.NewGuid()

        static member Deserialize (serializer: ISerializer, json: Json) =
            serializer.Deserialize<Counter>(json)

        member this.Serialize (serializer: ISerializer) =
            serializer.Serialize(this)

        member this.Increment() =   
            ResultCE.result
                {
                    let! noUpTo100 =
                        this.State < 99
                        |> Result.ofBool "must be lower than 99"

                    let newState = Counter(state + 1)
                    return newState
                }
        member this.Decrement() =
            ResultCE.result
                {
                    let! mustBeGreaterThan0 = 
                        this.State > 0
                        |> Result.ofBool "must be greater than 0"
                    let newState = Counter(state - 1)
                    return newState
                }
        member this.Clear() =
            ResultCE.result
                {
                    let newState = Counter(0)
                    return newState
                }

        member this.State = state
        member this.StateId = stateId
        static member Zero = Counter (0)
        static member StorageName = "_counter"
        static member Version = "_01"

        static member SnapshotsInterval = 15
        static member Lock = new Object()
