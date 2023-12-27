namespace MetaLang.Sema

open System.Collections.Generic
open MetaLang.Parser
open MetaLang.Parser.Lexer
open MetaLang.Parser.SymbolTable
open MetaLang.ErrorHandling
open TokenDefinition
open SymbolDefinition
open LexerDefinition
open TypeDefinition
open AST

module SemaDefinition =

    type SemaResults() =
        member val Errors: List<Error> = List<Error>() with get

    type SemaAnalyzer(_symbolTables: Dictionary<string, SymbolTable>, ?_semaTrace: bool) =

        let semaTrace = defaultArg _semaTrace false

        member val Results = SemaResults() with get
        member val SymbolTables: Dictionary<string, SymbolTable> = _symbolTables with get

        member val Trace = semaTrace with get
        
        member private this.toTypeVariant (token: Token): TypeVariant =

            match token.LiteralType with
                | LiteralVariant.Integer8 -> TInt8
                | LiteralVariant.Integer16 -> TInt16
                | LiteralVariant.Integer -> TInt32
                | LiteralVariant.Integer64 -> TInt64
                | LiteralVariant.Float -> TDouble
                | _ -> 
                
                    match token.Type with
                    | TokenType.Identifier
                    
                    | _ ->
                        TBad

        member private this.TypeCheckExpression(expression: Expression, ?_excepted: TypeVariant): unit =

            let excepted = defaultArg _excepted TAny

            let inline throwError(what, line, pos): unit =
                this.Results.Errors.Add (Error(what, line, pos))

            let inline toNumberType(typeOf): TypeVariant =
                match typeOf with
                | TInt8 | TInt16 | TInt32 | TInt64 | TFloat | TDouble -> TNumber
                | _ -> TBad // Bad Cast Type

            let inline isNumberType(typeOf): bool =
                not(toNumberType(typeOf) = TBad)

            match expression with
            
            | Expression.CastExpression x ->

                (this :> IVisitor).Visit x

            | Expression.Identifier x ->

                match x with
                | Identifier.Identifier (lexeme, scope) ->

                    match this.SymbolTables.[scope].Exist (Identifier.Identifier(lexeme, "")) with
                    | true ->
                        let typeofIdentifier = this.SymbolTables.[scope].Get (Identifier.Identifier(lexeme, ""))

                        if not(typeofIdentifier.TypeInfo = excepted) || typeofIdentifier.TypeInfo = TAny
                        then 
                            throwError ($"Type incompatibility. The type {typeofIdentifier.ToString()} is incompatible with the type {excepted.ToString()}\n Note: link to the literal\n\t| {lexeme}  ", 0, 0)

                    | _ -> throwError ($"The identifier {lexeme} does not exist in the current context", 0, 0)
                ()

            | Expression.Literal x ->

                let mutable lexemeOfLiteral: string = ""
                let mutable typeOfLiteral: TypeVariant = TAny

                let mutable pos = 0
                let mutable line = 0

                match x with

                | Literal.StringLiteral x -> typeOfLiteral <- TString
                | Literal.NumberLiteral (token) ->
                
                    typeOfLiteral <- this.toTypeVariant(token)

                    pos <- token.Pos
                    line <- token.Line
                    lexemeOfLiteral <- token.Lexeme

                | Literal.BooleanLiteral x -> typeOfLiteral <- TBool

                if not(typeOfLiteral = excepted) || typeOfLiteral = TAny
                then 
                    throwError ($"Type incompatibility. The type {typeOfLiteral.ToString()} is incompatible with the type {excepted.ToString()}\n Note: link to the literal\n\t| {lexemeOfLiteral}  ", line, pos)

            | Expression.BinaryExpression (BinaryExpression (token, _, expr)) -> 
            
                this.TypeCheckExpression(expr, this.toTypeVariant(token))
                ()

            | _ -> ()

        interface IVisitor with

            member this.Visit(assignStmt: AssignStmt): unit =
                
                ()

            member this.Visit(printStmt: PrintStmt): unit =

                match printStmt with
                | PrintStmt (expr) -> this.TypeCheckExpression(expr)
                ()

            member this.Visit(declVarStmt: DeclVarStmt): unit =

                match declVarStmt with
                | VarStmt (_, typeOf, expr) -> this.TypeCheckExpression(expr, typeOf)
                ()

            member this.Visit(emptyNode: EmptyNode): unit =

                ()

            member this.Visit(declArrayStmt: DeclArrayStmt): unit =

                ()

            member this.Visit(usingDeclStmt: UsingDeclStmt): unit =

                ()
            
            member this.Visit(returnStmt: ReturnStmt): unit =
                match returnStmt with
                | ReturnStmt (expr) -> this.TypeCheckExpression(expr)
                ()

            member this.Visit(castExpression: CastExpression): unit =
                
                ()

            member this.Visit(declFnStmt: DeclFnStmt): unit =
                
                ()