namespace MetaLang.ErrorHandling

open System

type ErrorLevel =
    | Note
    | Warning
    | Error

type Error(what: string, line: int, pos: int, ?level: ErrorLevel) = 

    let errorValue = defaultArg level ErrorLevel.Error

    member val What: string = what with get
    member val Line: int = line with get 
    member val Pos: int = pos with get 

    member val Level: ErrorLevel = errorValue with get

    member this.Throw(): unit = 
        
        match this.Level with
        | Note -> Console.ResetColor()
        | Warning -> Console.ForegroundColor <- ConsoleColor.Yellow
        | Error -> Console.ForegroundColor <- ConsoleColor.Red

        printf "\nIn line %d, Position %d\n %s\n" this.Line this.Pos this.What

        Console.ResetColor()
        ()