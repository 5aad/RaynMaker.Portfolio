﻿namespace RaynMaker.Portfolio.Gateways


module WebApp =
    open Suave
    open Suave.Successful
    open Suave.Operators
    open Suave.Filters
    open Newtonsoft.Json
    open Newtonsoft.Json.Serialization
    open RaynMaker.Portfolio.Interactors
    open Suave.Redirection

    let JSON v =
        let jsonSerializerSettings = new JsonSerializerSettings()
        jsonSerializerSettings.ContractResolver <- new CamelCasePropertyNamesContractResolver()

        JsonConvert.SerializeObject(v, jsonSerializerSettings)
        |> OK
        >=> Writers.setMimeType "application/json; charset=utf-8"

    module private Handlers =
        open System
        open RaynMaker.Portfolio
        open RaynMaker.Portfolio.Interactors.PositionsInteractor

        let (=>) k v = k,v |> box
        let formatDate (date:DateTime) = date.ToString("yyyy-MM-dd")
        let formatTimespan (span:TimeSpan) = 
            match span.TotalDays with
            | x when x > 365.0 -> sprintf "%.2f years" (span.TotalDays / 365.0)
            | x when x > 90.0 -> sprintf "%.2f months" (span.TotalDays / 30.0)
            | x -> sprintf "%.0f days" span.TotalDays
        let formatMoney = sprintf "%.2f"
        let formatPercentage = sprintf "%.2f%%"
        
        let createSummaryViewModel (p:PositionSummary) =
            dict [
                "name" => p.Name
                "isin" => p.Isin
                "open" => (p.Open |> formatDate)
                "close" => (p.Close |> Option.map formatDate |? "-")
                "duration" => ((p.Close |? DateTime.Today) - p.Open |> formatTimespan)
                "marketProfit" => (p.MarketProfit |> formatMoney)
                "dividendProfit" => (p.DividendProfit |> formatMoney)
                "totalProfit" => (p.MarketProfit + p.DividendProfit |> formatMoney)
                "marketRoi" => (p.MarketRoi |> formatPercentage)
                "dividendRoi" => (p.DividendRoi |> formatPercentage)
                "totalRoi" => (p.MarketRoi + p.DividendRoi |> formatPercentage)
                "marketRoiAnual" => (p.MarketRoiAnual |> formatPercentage)
                "dividendRoiAnual" => (p.DividendRoiAnual |> formatPercentage)
                "totalRoiAnual" => (p.MarketRoiAnual + p.DividendRoiAnual |> formatPercentage)
            ]
        
        let summarizePositions f = PositionsInteractor.getPositions >> f >> List.map createSummaryViewModel >> JSON
        let closedPositions = summarizePositions PositionsInteractor.summarizeClosedPositions
        let openPositions = summarizePositions PositionsInteractor.summarizeOpenPositions

    let createApp home store =
        let log = request (fun r -> printfn "%s" r.path; succeed)

        choose [ 
            GET >=> log >=> choose
                [
                    path "/" >=> redirect "/Content/index.html"
                    pathScan "/Content/%s" (fun f -> Files.file (sprintf "%s/Content/%s" home f))
                    pathScan "/Scripts/%s" (fun f -> Files.file (sprintf "%s/Scripts/%s" home f))
                    path "/api/closedPositions" >=> Handlers.closedPositions store
                    path "/api/openPositions" >=> Handlers.openPositions store
                ]
        ]

module ExcelEventStore =
    open System
    open FSharp.ExcelProvider
    open RaynMaker.Portfolio
    open RaynMaker.Portfolio.Entities

    [<Literal>] 
    let private template = @"../../etc/Events.xlsx"

    type private EventsSheet = ExcelFile<template>

    type ParsedEvent = 
        | Event of DomainEvent
        | Unknown of string * DateTime * payload:obj list

    let load path =
        let sheet = new EventsSheet(path)

        sheet.Data
        |> Seq.filter(fun r -> String.IsNullOrEmpty(r.Event) |> not)
        |> Seq.map(fun r -> 
            try
                match r.Event with
                | EqualsI "StockBought" _ -> 
                    { StockBought.Date = r.Date
                      Isin = r.ID
                      Name = r.Name
                      Count = r.Count |> int
                      Price = (r.Value |> decimal) * 1.0M<Currency>
                      Fee = (r.Fee |> decimal) * 1.0M<Currency>} |> StockBought |> Event
                | EqualsI "StockSold" _ -> 
                    { StockSold.Date = r.Date
                      Isin = r.ID
                      Name = r.Name
                      Count = r.Count |> int
                      Price = (r.Value |> decimal) * 1.0M<Currency>
                      Fee = (r.Fee |> decimal) * 1.0M<Currency>} |> StockSold |> Event
                | EqualsI "DividendReceived" _ -> 
                    { DividendReceived.Date = r.Date
                      Isin = r.ID
                      Name = r.Name
                      Value = (r.Value |> decimal) * 1.0M<Currency>
                      Fee = (r.Fee |> decimal) * 1.0M<Currency>} |> DividendReceived |> Event
                | EqualsI "DepositAccounted" _ -> 
                    { DepositAccounted.Date = r.Date
                      Value = (r.Value |> decimal) * 1.0M<Currency>} |> DepositAccounted |> Event
                | EqualsI "DisbursementAccounted" _ -> 
                    { DisbursementAccounted.Date = r.Date
                      Value = (r.Value |> decimal) * 1.0M<Currency>} |> DisbursementAccounted |> Event
                | EqualsI "InterestReceived" _ -> 
                    { InterestReceived.Date = r.Date
                      Value = (r.Value |> decimal) * 1.0M<Currency>} |> InterestReceived |> Event
                | EqualsI "PositionClosed" _ -> 
                    { PositionClosed.Isin = r.ID
                      Name = r.Name
                      Price = (r.Value |> decimal) * 1.0M<Currency>
                      Fee = (r.Fee |> decimal) * 1.0M<Currency>} |> PositionClosed |> Event
                | x -> Unknown(r.Event,r.Date,[r.ID; r.Name; r.Value; r.Fee; r.Count; r.Comment])
            with 
                | ex -> failwithf "Failed reading event store at %s with %A" r.Event ex 
        )
        |> List.ofSeq
