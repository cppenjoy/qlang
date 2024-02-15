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

        let funcionCallStack: Stack<DeclFnStmt> = Stack()

        member val Results = SemaResults() with get
        member val SymbolTables: Dictionary<string, SymbolTable> = _symbolTables with get

        member val Trace = semaTrace with get
        
        member private this.throwError what line pos : unit =
            this.Results.Errors.Add (Error(what, line, pos))

        member private this.ToTypeVariant (token: Token): TypeVariant =

            match token.LiteralType with
                | LiteralVariant.Integer8 -> TInt8
                | LiteralVariant.Integer16 -> TInt16
                | LiteralVariant.Integer -> TInt32
                | LiteralVariant.Integer64 -> TInt64
                | LiteralVariant.Float -> TDouble
                | _ -> TBad

        member private this.ToTypeVariant (identifier: Identifier): TypeVariant =

            match identifier with
            | Identifier.Identifier (text, scope) ->

                let symbolTable = this.SymbolTables.[scope]

                match (symbolTable.Exist(identifier) || this.SymbolTables.["global"].Exist(Identifier.Identifier(text, "global")))  with
                | true when symbolTable.Exist(identifier) ->

                    let symbol = symbolTable.Get identifier

                    match symbol.Type with
                    | Variable ->
                        symbol.TypeInfo

                    | _ ->
                        this.throwError $"The identifier {text} is not variable" 0 0
                        TBad

                | true when this.SymbolTables.["global"].Exist (Identifier.Identifier(text, "global")) ->

                    let symbol = this.SymbolTables.["global"].Get (Identifier.Identifier(text, "global"))

                    match symbol.Type with
                    | Variable ->
                        symbol.TypeInfo

                    | _ ->
                        this.throwError $"The identifier {text} is not variable" 0 0
                        TBad
                
                | _ ->
                    this.throwError $"The identifier {text} don`t exist in current context" 0 0
                    TBad


        member private this.TypeCheckExpression(expression: Expression, ?_excepted: TypeVariant, ?_token: Token): unit =

            let excepted = defaultArg _excepted TAny
            let token = defaultArg _token (Token(TokenType.Empty, ""))

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
            
                let typeOfIdentifier = this.ToTypeVariant(x)

                if not(typeOfIdentifier = excepted) || typeOfIdentifier = TAny
                then 
                    this.throwError $"Type incompatibility. The {typeOfIdentifier.ToString()} is incompatible with the type {excepted.ToString()}\n Note: link to the literal\n\t|   " token.Line token.Pos


            | Expression.Literal x ->

                let mutable lexemeOfLiteral: string = ""
                let mutable typeOfLiteral: TypeVariant = TAny

                let mutable pos = 0
                let mutable line = 0

                match x with

                | Literal.StringLiteral (token) -> 
                    pos <- token.Pos
                    line <- token.Line
                    lexemeOfLiteral <- token.Lexeme

                    typeOfLiteral <- TString

                | Literal.NumberLiteral (token) ->
                
                    typeOfLiteral <- this.ToTypeVariant(token)

                    pos <- token.Pos
                    line <- token.Line
                    lexemeOfLiteral <- token.Lexeme

                | Literal.BooleanLiteral (token) -> 
                    pos <- token.Pos
                    line <- token.Line
                    lexemeOfLiteral <- token.Lexeme

                    typeOfLiteral <- TBool

                if not(typeOfLiteral = excepted) && not (excepted = TAny)
                then 
                    this.throwError $"Type incompatibility. The type {typeOfLiteral.ToString()} is incompatible with the type {excepted.ToString()}\n Note: link to the literal\n\t| {lexemeOfLiteral}  " token.Line token.Pos

            | Expression.BinaryExpression (BinaryExpression (primary, _, expr)) -> 
            
                match primary with

                | Primary.Primary (literal) ->

                    match literal with
                    | Literal.NumberLiteral (token) ->
                        this.TypeCheckExpression(expr, this.ToTypeVariant(token))

                    | _ ->
                        ()
                
                | Primary.PrimaryIdentifier (identifier) ->
                    this.TypeCheckExpression(expr, this.ToTypeVariant(identifier))

                | _ ->
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
                    | ReturnStmt (token, expr) -> 
                        
                        match funcionCallStack.Count > 0 with
                        | true ->

                            match funcionCallStack.Pop() with
                            | FnDeclNode (_, returnType, _, _, _, _) -> 
                                this.TypeCheckExpression(expr, returnType, token)


                        | _ ->
                        this.throwError "the return statement is invalid at this point" token.Line token.Pos



                
            member this.Visit(castExpression: CastExpression): unit =
                
                ()

            member this.Visit(declFnStmt: DeclFnStmt): unit =

                funcionCallStack.Push declFnStmt

                match declFnStmt with
                    | FnDeclNode (identifier, returnType, signature, body, line, pos) -> 
                        
                        match identifier with
                        | Identifier.Identifier (id, _) ->
                            
                            if id = "main" && not(returnType = TypeVariant.TInt32) then
                                this.throwError "the main function should be return int32" (int line) (int pos) 
                                 
                        match body with
                        | FnBody x ->

                            for node in x do
                                node.Accept this
                () 