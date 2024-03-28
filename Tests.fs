module Tests

open System
open Sharpino
open FSharpPlus
open FsToolkit.ErrorHandling
open Sharpino.Definitions
open Sharpino.Utils
open Sharpino
open Expecto
open Expecto.Tests
open sharpinoCounter
open sharpinoCounter.SharpinoCounterApi
open Sharpino.Cache
open sharpinoCounter.Counter
open Sharpino
open Sharpino.MemoryStorage
open Sharpino.PgStorage
open Sharpino.TestUtils
open Sharpino.Storage

[<Tests>]
let tests =
    let connection = 
        "Server=127.0.0.1;"+
        "Database=es_counter;" +
        "User Id=safe;"+
        "Password=safe;"

    let memoryStorage: IEventStore = MemoryStorage()
    let pgStorage = PgEventStore(connection)

    let testConfigs = [
        (memoryStorage, 0, 0)
        // (pgStorage, 1, 1) // uncomment to test with Postgres
    ]

    let Setup(eventStore: IEventStore) =
        StateCache<Counter>.Instance.Clear()
        eventStore.Reset Counter.Version Counter.StorageName

    testList "samples" [
        multipleTestCase "initialize counter and state is zero " testConfigs <| fun (eventStore, _, _) ->
            Setup eventStore
            // given
            let counterApi = SharpinoCounterApi eventStore

            // when
            let state = counterApi.GetState()

            // then
            Expect.isOk state "should be ok"
            Expect.equal state.OkValue 0 "should be zero"

        multipleTestCase "initialize, increment the counter and the state is one " testConfigs <| fun (eventStore, _, _) ->
            Setup eventStore
            // given
            let counterApi = SharpinoCounterApi eventStore
            let state = counterApi.GetState()

            // when
            let _ = counterApi.Increment()

            // then
            let state = counterApi.GetState()
            Expect.isOk state "should be ok"
            Expect.equal state.OkValue 1 "should be zero"

        multipleTestCase "initialize, increment and decrement the counter and the state is zero - OK " testConfigs <| fun (eventStore, _, _) ->
            Setup eventStore
            // given
            let counterApi = SharpinoCounterApi eventStore
            let state = counterApi.GetState()

            // when
            let _ = counterApi.Increment()
            let _ = counterApi.Decrement()

            // then
            let state = counterApi.GetState()
            Expect.isOk state "should be ok"
            Expect.equal state.OkValue 0 "should be zero"

        multipleTestCase "increment up to 99 - Ok" testConfigs <| fun (eventStore, _, _) ->
            Setup eventStore
            // given
            let counterApi = SharpinoCounterApi eventStore

            // when
            let _ =
                [ 1 .. 99 ]
                |> List.iter (fun _ -> counterApi.Increment() |> ignore)

            // then
            let state = counterApi.GetState()
            Expect.isOk state "should be ok"
            Expect.equal state.OkValue 99 "should be 99"

        multipleTestCase "can't increment from 99 to 100 - Error" testConfigs <| fun (eventStore, _, _) ->
            Setup eventStore
            // given
            let counterApi = SharpinoCounterApi eventStore

            // when
            let _ =
                [ 1 .. 99 ]
                |> List.iter (fun _ -> counterApi.Increment() |> ignore)

            let result = counterApi.Increment()

            // then
            Expect.isError result "should be error"
            Expect.equal (getError result) "must be lower than 99" "should be 'must be lower than 99'"

        multipleTestCase "can't decrement from 0 to -1 - Error" testConfigs <| fun (eventStore, _, _) ->
            Setup eventStore
            // given
            let counterApi = SharpinoCounterApi eventStore

            // when
            let result = counterApi.Decrement()

            // then
            Expect.isError result "should be error"
            Expect.equal (getError result) "must be greater than 0" "should be 'must be greater than 0'"

        multipleTestCase "increment and clear" testConfigs <| fun (eventStore, _, _) ->
            Setup eventStore
            // given
            let counterApi = SharpinoCounterApi  eventStore

            // when
            let _ = counterApi.Increment()
            let _ = counterApi.Clear()

            // then
            let state = counterApi.GetState()
            Expect.isOk state "should be ok"
            Expect.equal state.OkValue 0 "should be zero"

    ] 
    |> testSequenced
