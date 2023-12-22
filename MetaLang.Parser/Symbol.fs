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

    and AliasData(identifier: Identifier, ?typeVar: TypeVariant) =

        let _typeVar = defaultArg typeVar (TypeVariant.TAny)

        member val Identifier: Identifier = identifier with get
        member val TypeOfElem: TypeVariant = _typeVar with get 

    and FnData(typeofReturn: TypeVariant, signature: List<TypeVariant>) =

        member val TypeofReturn: TypeVariant = typeofReturn with get
        member val Signature: List<TypeVariant> = signature with get 

    and Symbol(_type: SymbolType, _info: TypeVariant, _decl: uint32, ?_arrayElem: ArrayElements
                                                                    , ?_aliasData: AliasData
                                                                    , ?_fnData: FnData) =

        let arrayElem = defaultArg _arrayElem (ArrayElements())
        let aliasData = defaultArg _aliasData (AliasData((Identifier.Identifier ""), TInt32))
        let fnData = defaultArg _fnData (FnData(TBad, List<TypeVariant>()))

        member val Type: SymbolType = _type with get
        member val TypeInfo: TypeVariant = _info with get
        
        member val LineOfDeclaration: uint32 = _decl with get

        member val ArrayElements: ArrayElements = arrayElem with get
        member val Alias: AliasData = aliasData with get


[<AutoOpen>]
module SymbolTable =

    open SymbolDefinition

    /// <summary>
    /// Provide a wrapper on list of symbols
    /// </summary>
    type SymbolTable() =

        member val Symbols: List<Symbol> = new List<Symbol>() with get

        member public this.PushSymbol symbol =
            this.Symbols.Add symbol

        member public this.Exist (symbol: Symbol) =
            List.exists (fun (x: Symbol) -> x.Alias.Identifier = symbol.Alias.Identifier) (this.Symbols |> Seq.toList)

        member public this.Exist (identifier: Identifier) =
            List.exists (fun (x: Symbol) -> x.Alias.Identifier = identifier) (this.Symbols |> Seq.toList)

        member public this.GetIfExist (identifier: Identifier) : (bool * Symbol) =
            let symbol = List.tryFind (fun (x: Symbol) -> x.Alias.Identifier = identifier) (this.Symbols |> Seq.toList)

            match symbol  with
            | None -> (false, Symbol(SymbolType.Variable, TBad, uint32 0))
            | _ -> 
                (true, symbol.Value)            

        member public this.Get (identifier: Identifier) =
            List.find (fun (x: Symbol) -> x.Alias.Identifier = identifier) (this.Symbols |> Seq.toList)

        member public this.PushAlias identifier refType line =
            let aliasData: AliasData = new AliasData(identifier, refType)
            let aliasSymbol: Symbol = new Symbol(SymbolType.Alias, TBad, uint32 line, _aliasData = aliasData)

            this.PushSymbol aliasSymbol

        member public this.PushVariable identifier typeofVariable line =
            let aliasData: AliasData = new AliasData(identifier, TAny)
            let variableSymbol = new Symbol(SymbolType.Variable, typeofVariable, uint32 line, _aliasData = aliasData)

            this.PushSymbol variableSymbol

        member public this.PushFunction identifier line (fnData: FnData) =
            let aliasData: AliasData = new AliasData(identifier, TAny)
            let fnSymbol: Symbol = new Symbol(SymbolType.Fn, TBad, uint32 line, _aliasData = aliasData, _fnData = fnData)

            this.PushSymbol fnSymbol