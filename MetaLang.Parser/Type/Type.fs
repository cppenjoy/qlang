namespace MetaLang.Parser

module TypeDefinition =
    
    type TypeVariant  =
        | TInt8
        | TInt16
        | TInt32
        | TInt64

        | TFloat
        | TDouble

        | TString
        | TBool
        | TArray

        | TNumber
        | TAny
        | TBad