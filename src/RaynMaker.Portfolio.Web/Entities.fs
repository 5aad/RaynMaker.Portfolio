﻿namespace RaynMaker.Portfolio.Entities

open System

[<Measure>] type Currency
[<Measure>] type Percentage

type StockBought = {
    Date : DateTime
    Isin : string
    Name : string
    Count : int
    Price : decimal<Currency>
    Fee : decimal<Currency>
    }

type StockSold = {
    Date : DateTime
    Isin : string
    Name : string
    Count : int
    Price : decimal<Currency>
    Fee : decimal<Currency>
    }

type DividendReceived = {
    Date : DateTime
    Isin : string
    Name : string
    Value : decimal<Currency>
    Fee : decimal<Currency>
    }

type DepositAccounted = {
    Date : DateTime
    Value : decimal<Currency>
    }

type SavingsPlanRateAccounted = {
    Date : DateTime
    Value : decimal<Currency>
    }

type DisbursementAccounted = {
    Date : DateTime
    Value : decimal<Currency>
    }

type InterestReceived = {
    Date : DateTime
    Value : decimal<Currency>
    }

type DomainEvent = 
    | StockBought of StockBought
    | StockSold of StockSold
    | DividendReceived of DividendReceived
    | DepositAccounted of DepositAccounted
    | SavingsPlanRateAccounted of SavingsPlanRateAccounted
    | DisbursementAccounted of DisbursementAccounted
    | InterestReceived of InterestReceived


