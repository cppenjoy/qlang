namespace MetaLang.Parser.SymbolTable

open System.Collections.Generic
open MetaLang.Parser
open TypeDefinition
open AST

module SymbolDefinition =
    
    type SymbolType =
        | Variable
        | Fn
        | Array
        | Alias

    type ArrayElements() =

        member val TypeOfElem: TypeVariant = TInt32 with get
        member val Elements: List<Symbol> = List<Symbol>() with get

    and AliasData(identifier: Identifier, typeVar: TypeVariant) =

        member val Identifier: Identifier = identifier with get
        member val TypeOfElem: TypeVariant = typeVar with get 

    and Symbol(_type: SymbolType, _info: TypeVariant, _decl: uint32, ?_arrayElem: ArrayElements
                                                                    , ?_aliasData: AliasData) =

        let arrayElem = defaultArg _arrayElem (ArrayElements())
        let aliasData = defaultArg _aliasData (AliasData((Identifier.Identifier ""), TInt32))

        member val Type: SymbolType = _type with get
        member val TypeInfo: TypeVariant = _info with get
        
        member val LineOfDeclaration: uint32 = _decl with get

        member val ArrayElements: ArrayElements = arrayElem with get
        member val Alias: AliasData = aliasData with get