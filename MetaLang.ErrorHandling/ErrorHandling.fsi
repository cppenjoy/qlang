



namespace MetaLang.ErrorHandling
    
    type ErrorLevel =
        | Note
        | Warning
        | Error
    
    type Error =
        
        new: what: string * line: int * pos: int * ?level: ErrorLevel -> Error
        
        member Throw: unit -> unit
        
        member Level: ErrorLevel
        
        member Line: int
        
        member Pos: int
        
        member What: string

