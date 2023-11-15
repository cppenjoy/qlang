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

    type SemaAnalyzer(_symbolTable: List<Symbol>, ?_semaTrace: bool) =

        let semaTrace = defaultArg _semaTrace false

        member val Results = SemaResults() with get
        member val SymbolTable = _symbolTable with get

        member val Trace = semaTrace with get
        
        member private this.TypeCheckExpression(expression: Expression, ?_excepted: TypeVariant): unit =

            let excepted = defaultArg _excepted TAny

            let throwError(what): unit =
                this.Results.Errors.Add (Error(what, 0, 0))

            let toNumberType(typeOf): TypeVariant =
                match typeOf with
                | TInt16 | TInt32 | TInt64 | TFloat | TDouble -> TNumber
                | _ -> TBad // Bad Cast Type

            let isNumberType(typeOf): bool =
                not(toNumberType(typeOf) = TBad)

            match expression with 
            | Expression.Identifier x ->

                ()

            | Expression.Literal x ->

                let mutable typeOfLiteral: TypeVariant = TAny

                match x with

                | Literal.StringLiteral x -> typeOfLiteral <- TString
                | Literal.NumberLiteral (token) ->
                    
                    match token.LiteralType with
                    | Integer -> typeOfLiteral <- TInt32
                    | Float -> typeOfLiteral <- TDouble

                | Literal.BooleanLiteral x -> typeOfLiteral <- TBool
                | _ -> throwError $"The type {(typeOfLiteral.ToString())} is undefined"

                if not(typeOfLiteral = TAny) && not(typeOfLiteral = excepted)
                then 
                    throwError $"Type incompatibility. The Type {typeOfLiteral.ToString()} is incompatible with the type {excepted.ToString()}"

            | Expression.BinaryExpression (BinaryExpression (_, _, expr)) -> 
            
                this.TypeCheckExpression(expr, TNumber)
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