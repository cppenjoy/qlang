namespace MetaLang.Sema

open System.Collections.Generic
open MetaLang.Parser
open MetaLang.Parser.Lexer
open MetaLang.Parser.SymbolTable
open MetaLang.ErrorHandling
open TokenDefinition
open SymbolDefinition
open LexerDefinition
open AST

module SemaDefinition =

    type SemaResults() =
        member val Errors: List<Error> = List<Error>() with get

    type SemaAnalyzer(_symbolTable: List<Symbol>) =

        member val Results = SemaResults() with get
        member val SymbolTable = _symbolTable with get
        
        member private this.TypeCheckExpression(expression: Expression, ?_excepted: TypeVariant): unit =

            let excepted = defaultArg _excepted TAny

            let throwError(what): unit =
                this.Results.Errors.Add (Error(what, 0, 0))

            match expression with 
            | Expression.Identifier x ->

                ()

            | Expression.Literal x ->

                match x with

                | StringLiteral y ->

                    if not(excepted = TAny) && not(excepted = TString) 
                    then
                        throwError $"Type {excepted} was expected, but type string was received"
                        ()
                    ()
                
                | NumberLiteral y ->

                        if not(excepted = TAny) && not(excepted = TNumber) 
                        then
                            throwError $"Type {excepted} was expected, but type number was received"
                            ()
                        ()

                | BooleanLiteral y ->

                        if not(excepted = TAny) && not(excepted = TBool) 
                        then
                            throwError $"Type {excepted} was expected, but type bool was received"
                            ()
                        ()

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