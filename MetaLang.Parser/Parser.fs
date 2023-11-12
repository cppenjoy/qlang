namespace MetaLang.Parser

open Fastenshtein
open System.Collections.Generic
open MetaLang.Parser.Lexer
open MetaLang.Parser.SymbolTable
open MetaLang.ErrorHandling
open TokenDefinition
open SymbolDefinition
open LexerDefinition
open AST

module ParserDefinition =

    type ParserResults() = 

        member val Tree: List<IVisitable> = List<IVisitable>() with get
        member val Errors: List<Error> = List<Error>() with get
        member val SymbolTable: List<Symbol> = List<Symbol>() with get

    type Parser(_tokens: List<Token>) =

        member val Tokens: List<Token> = _tokens with get

        member this.Parse(): ParserResults =

            let parserResults: ParserResults = ParserResults()

            let mutable pos = 0

            let getMirrorType(src: string): string =

                ""

            let next(): Token =

                if pos <= (this.Tokens.Count - 1) then
                    let token = this.Tokens.[int pos]
                    pos <- pos + 1

                    token
                else 
                    Token(EOF, "")

            let inline isOp(token: Token): bool =

                match token.Type with
                | Operator -> true
                | _ -> false

            let inline throwError(what: string): Token =

                parserResults.Errors.Add ( Error(what, this.Tokens.[pos - 1].Line, this.Tokens.[pos - 1].Pos) )
                Token(TokenType.Error, "")

            let inline throwWarning(what: string): unit =

                parserResults.Errors.Add( Error(what, this.Tokens.[pos - 1].Line, this.Tokens.[pos - 1].Pos, ErrorLevel.Warning) )

            let rec parseBinaryExpression(): Expression =

                pos <- pos - 1

                let firstExpression: Token = parseNumber()
                next() |> ignore
                let secondExpression: Expression = parseExpression()

                let Node = BinaryExpression(Literal.NumberLiteral firstExpression, this.Tokens.[pos - 1], secondExpression)

                Expression.BinaryExpression Node

            and parseNumber(): Token =

                let primary: Token = next()

                match primary.Type with
                | TokenType.NumberLiteral -> primary

                | _ -> throwError "You cannot use NaN in binary expressions"

            and parseType(): TypeVariant =

                let primary: Token = next()

                match primary.Type with
                | KeywordNumber -> TNumber
                | KeywordString -> TString
                | KeywordBool -> TBool
                | _ ->

                    let existOrNot: bool = List.exists (fun (x: Symbol) -> x.Alias.Identifier = (Identifier.Identifier(primary.Lexeme))) (parserResults.SymbolTable |> Seq.toList)

                    match existOrNot with
                    | true ->
                        let typeOf = List.find (fun (x: Symbol) -> x.Alias.Identifier = (Identifier.Identifier(primary.Lexeme))) (parserResults.SymbolTable |> Seq.toList)

                        typeOf.Alias.TypeOfElem

                    | _ ->
                        throwError $"The type {primary.Lexeme} is undefined, did you man {getMirrorType primary.Lexeme}" |> ignore
                        TNumber

            and parseIdentifier(): Identifier =

                let primary: Token = next()

                match primary.Type with
                | TokenType.Identifier -> 
                
                    let identifier: Identifier = Identifier.Identifier(primary.Lexeme)

                    identifier

                | _ -> 
                    throwError "Unrecognized identifier" |> ignore
                    Identifier.Identifier("")

            and parseExpression(): Expression =

                let primary: Token = next()

                match primary.Type with

                | LPair -> parseExpression()

                | TokenType.Identifier -> 
                
                    let identifier: Identifier = Identifier.Identifier(primary.Lexeme)

                    Expression.Identifier(identifier)
                
                | TokenType.StringLiteral -> 

                    let literal: Literal = Literal.StringLiteral(primary.Lexeme)

                    Expression.Literal(literal)

                | TokenType.BooleanLiteral -> 

                    let literal: Literal = Literal.BooleanLiteral(bool.Parse(primary.Lexeme))

                    Expression.Literal(literal)

                | TokenType.NumberLiteral -> 
                
                    match next().Type with
                    
                    | Operator ->

                        pos <- pos - 1
                        parseBinaryExpression()

                    | _ ->
                        pos <- pos - 1
                        
                        let literal = Literal.NumberLiteral this.Tokens.[pos]

                        Expression.Literal(literal)

                | _ -> 
                    throwError $"{primary.Lexeme} - incomplete expression" |> ignore
                    Expression.EmptyNode (EmptyNode ())

//    
//            let parseArrayDeclStmt(): IVisitable =
//
  //              let identifier: Identifier = parseIdentifier()
//
  //              match next().Type with
    //////            | TokenType.Punctuator when this.Tokens.[pos - 1].Lexeme = ":" ->
////
    //                let typeOfArrayElem: TypeVariant = parseType()
//
  //                  match next().Type with
    //                | _ -> throwError "Expected array initialization"
//
  //              | _ -> throwError "When declaring an array, an explicit annotation of the type of elements of this array is required"

            let parseUsingDeclStmt() : IVisitable =

                let identifier: Identifier = parseIdentifier()
                let primary: Token = next()

                match primary.Type with
                | TokenType.Operator when primary.Lexeme = "=" ->

                    let Type = parseType()

                    let AliasData: AliasData = AliasData(identifier, Type)
                    let AliasSymbol: Symbol = Symbol(Alias, TNumber, uint32 primary.Line, _aliasData = AliasData)

                    parserResults.SymbolTable.Add AliasSymbol

                    UsingDeclStmt(identifier, Type)

                | _ -> 
                    throwError "Operator expected =" |> ignore
                    EmptyNode ()

            let parseVarDeclStmt(): IVisitable =            

                let identifier: Identifier = parseIdentifier()
                let optionalCheck: Token = next()

                let mutable typeOfVar: TypeVariant = TNumber

                match optionalCheck.Type with
                | Punctuator when optionalCheck.Lexeme = ":" ->

                    typeOfVar <- parseType()

                    next() |> ignore

                    let expression = parseExpression()

                    let Symbol = Symbol(SymbolType.Variable, typeOfVar, uint32 optionalCheck.Line)
                    VarStmt(identifier, typeOfVar, expression)

                | Operator when optionalCheck.Lexeme = "=" -> 
                    
                    let expression = parseExpression()

                    let Symbol = Symbol(SymbolType.Variable, typeOfVar, uint32 optionalCheck.Line)
                    VarStmt(identifier, typeOfVar, expression)

                | _ ->

                    throwError "After the identifier in the let construct, you can explicitly specify the type, or assign a value" |> ignore
                    EmptyNode ()

            let parseAssignExpression(): IVisitable =

                let identifier = parseIdentifier()
                let primary: Token = next()

                match primary.Type with
                | Punctuator when primary.Lexeme = "<-" -> 

                    let expression: Expression = parseExpression()

                    AssignStmt(identifier, expression)

                | _ -> 
                    throwError "undefined expression" |> ignore
                    EmptyNode ()

            let parseReturnStmt(): IVisitable =

                let expression: Expression = parseExpression()

                PrintStmt.PrintStmt(expression)

            let parsePrintStmt(): IVisitable =

                let expression: Expression = parseExpression()

                PrintStmt.PrintStmt(expression)

            let parseStatement() =

                match next().Type with
                | KeywordPrint -> parsePrintStmt()
                | KeywordLet -> parseVarDeclStmt()
                | KeywordUsing -> parseUsingDeclStmt()
                | KeywordReturn -> parseReturnStmt()
                | TokenType.Identifier -> 
                
                    pos <- pos - 1
                    parseAssignExpression()
                
                | _ -> 
                    throwError $"The keyword or identifier {this.Tokens.[pos - 1].Lexeme} is undefined" |> ignore
                    EmptyNode ()

            let rec parse() =

                match next().Type with
                | EOF -> ()

                | _ -> 

                    pos <- pos - 1

                    let stmt: IVisitable = parseStatement()
                    
                    parserResults.Tree.Add stmt
                    parse()

            parse()
            parserResults