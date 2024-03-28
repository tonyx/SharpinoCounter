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



[<Tests>]
let tests =
    let Setup() =
        StateCache<Counter>.Instance.Clear()

    testList "samples" [
        testCase "initialize counter and state is zero " <| fun _ ->
            Setup()
            // given
            let counterApi = SharpinoCounterApi()

            // when
            let state = counterApi.GetState()

            // then
            Expect.isOk state "should be ok"
            Expect.equal state.OkValue 0 "should be zero"

        testCase "initialize, increment the counter and the state is one " <| fun _ ->
            Setup()
            // given
            let counterApi = SharpinoCounterApi()
            let state = counterApi.GetState()

            // when
            let _ = counterApi.Increment()

            // then
            let state = counterApi.GetState()
            Expect.isOk state "should be ok"
            Expect.equal state.OkValue 1 "should be zero"

        testCase "initialize, increment and decrement the counter and the state is zero " <| fun _ ->
            Setup()
            // given
            let counterApi = SharpinoCounterApi()
            let state = counterApi.GetState()

            // when
            let _ = counterApi.Increment()
            let _ = counterApi.Decrement()

            // then
            let state = counterApi.GetState()
            Expect.isOk state "should be ok"
            Expect.equal state.OkValue 0 "should be zero"

        testCase "increment up to 99 - Ok" <| fun _ ->
            Setup()
            // given
            let counterApi = SharpinoCounterApi()

            // when
            let _ =
                [ 1 .. 99 ]
                |> List.iter (fun _ -> counterApi.Increment() |> ignore)

            // then
            let state = counterApi.GetState()
            Expect.isOk state "should be ok"
            Expect.equal state.OkValue 99 "should be 99"

        testCase "can't increment from 99 to 100 - Error" <| fun _ ->
            Setup()
            // given
            let counterApi = SharpinoCounterApi()

            // when
            let _ =
                [ 1 .. 99 ]
                |> List.iter (fun _ -> counterApi.Increment() |> ignore)

            let result = counterApi.Increment()

            // then
            Expect.isError result "should be error"
            Expect.equal (getError result) "must be lower than 99" "should be 'must be lower than 99'"

        testCase "can't decrement from 0 to -1 - Error" <| fun _ ->
            Setup()
            // given
            let counterApi = SharpinoCounterApi()

            // when
            let result = counterApi.Decrement()

            // then
            Expect.isError result "should be error"
            Expect.equal (getError result) "must be greater than 0" "should be 'must be greater than 0'"

        testCase "increment and clear" <| fun _ ->
            Setup()
            // given
            let counterApi = SharpinoCounterApi()

            // when
            let _ = counterApi.Increment()
            let _ = counterApi.Clear()

            // then
            let state = counterApi.GetState()
            Expect.isOk state "should be ok"
            Expect.equal state.OkValue 0 "should be zero"

    ] 
    |> testSequenced
