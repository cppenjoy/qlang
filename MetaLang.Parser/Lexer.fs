namespace MetaLang.Parser.Lexer

open System
open System.Collections.Generic
open MetaLang.ErrorHandling

module TokenDefinition = 

    type TokenType =
        | Operator

        | LPair // (
        | RPair // )

        | LBrace // {
        | RBrace // }

        | Semicolon
        | Punctuator

        | Identifier
        | StringLiteral
        | NumberLiteral
        | BooleanLiteral // void

        | KeywordLet // let
        | KeywordPrint // print
        | KeywordString // string
        | KeywordNumber // number
        | KeywordArray // array
        | KeywordBool // bool
        | KeywordFn // fn
        | KeywordVoid // void
        | KeywordUsing // using
        | KeywordReturn // return

        | Error
        | EOF

    type Token(TokenType: TokenType, TokenLexeme: string, ?TokenLine: int, ?TokenPos: int) = 

        let tokenLine = defaultArg TokenLine 0
        let tokenPos = defaultArg TokenPos 0

        member this.Type = TokenType
        member this.Lexeme = TokenLexeme
        member this.Line = tokenLine
        member this.Pos = tokenPos

    let KeywordAsToken = Map.ofList [
            ("let", TokenType.KeywordLet);
            ("print", TokenType.KeywordPrint);
            ("number", TokenType.KeywordNumber);
            ("bool", TokenType.KeywordBool);
            ("string", TokenType.KeywordString);
            ("fn", TokenType.KeywordFn);
            ("void", TokenType.KeywordVoid);
            ("array", TokenType.KeywordArray);
            ("using", TokenType.KeywordUsing);
            ("return", TokenType.KeywordReturn);

            ("true", TokenType.BooleanLiteral);
            ("false", TokenType.BooleanLiteral);
        ]

module LexerDefinition =

    open TokenDefinition

    type Error = MetaLang.ErrorHandling.Error

    type LexerResults() =
        member val Tokens: List<Token> = List<Token>() with get
        member val Errors: List<Error> = List<Error>() with get

    type Lexer(_source: string) =

        let mutable line: int = 1
        let mutable pos: int = 0
        let mutable offset: int = 0

        member val Source = _source with get
        
        member private this.is_end(offset: int): bool =
            ((pos + offset) >= this.Source.Length)

        member private this.next(): char =
            if (this.is_end(0))
            then
                "\\0".[0]
            else
                let element = this.Source.[pos]
                pos <- pos + 1
                element

        member private this.peek(offset: int): char =
            if (this.is_end(offset))
            then
                "\\0".[0]
            else
                this.Source.[pos + offset]

        member private this.tokenize_string_literal(results: LexerResults): unit = 

            while ( not(this.peek(0) = '"') && not(this.is_end(0)) ) do

                if this.peek(0) = '\n'
                then
                    line <- line + 1

                this.next() |> ignore

            if (this.is_end(0) && not(this.next() = '"'))
            then
                results.Errors.Add( Error("incomplete string literal", line, pos) )
            else 
                this.next() |> ignore

                let value: string = this.Source.Substring(offset, pos - offset)

                results.Tokens.Add(Token(TokenType.StringLiteral, value, line, pos))
                ()

        member private this.tokenize_identifier(results: LexerResults): unit = 

            while ( Char.IsLetterOrDigit(this.peek(0)) && not(this.is_end(0)) ) do
                this.next() |> ignore

            let value: string = this.Source.Substring(offset, pos - offset)

            if Map.containsKey value KeywordAsToken 
            then
                results.Tokens.Add(Token(KeywordAsToken.[value], value, line, pos))
            else
                results.Tokens.Add(Token(TokenType.Identifier, value, line, pos))
            ()

        member private this.tokenize_number(results: LexerResults): unit = 

            while ( Char.IsDigit(this.peek(0)) && not(this.is_end(0)) ) do
                this.next() |> ignore

            if this.peek(0) = '.'
            then
                this.next() |> ignore

                while ( Char.IsDigit(this.peek(0)) && not(this.is_end(0)) ) do
                    this.next() |> ignore
                  
            let value: string = this.Source.Substring(offset, pos - offset)

            results.Tokens.Add(Token(TokenType.NumberLiteral, value, line, pos))
            ()

        member this.Tokenize(): LexerResults = 

            let results: LexerResults = LexerResults()

            let pushToken(_type: TokenType, _lexeme: string): unit = 
                results.Tokens.Add(Token(_type, _lexeme, line, pos))
                ()

            while not (this.is_end(0)) do
                
                offset <- pos

                let ch: char = this.next()

                match ch with
                | '"' -> this.tokenize_string_literal(results)

                | '/' when this.peek(0) = '/' -> 

                    while (not(this.is_end(0)) && not(this.peek(0) = '\n')) do
                        this.next() |> ignore

                | '\n' -> line <- line + 1

                | '/' when this.peek(0) = '*' ->

                    while not(this.is_end(0)) && not((this.peek(0)) = '*' && this.peek(1) = '/') do

                        if this.peek(0) = '\n'
                        then
                            line <- line + 1

                        this.next() |> ignore

                    this.next() |> ignore
                    this.next() |> ignore
                    

                | '+' -> pushToken(TokenType.Operator, "+")
                | '-' -> pushToken(TokenType.Operator, "-")
                | '/' when not(this.peek(0) = '/') -> pushToken(TokenType.Operator, "/")
                | '*' -> pushToken(TokenType.Operator, "*")

                | '<' when this.peek(0) = '-' -> 
                        pushToken(Punctuator, "<-")
                        this.next() |> ignore

                | ':' -> pushToken(TokenType.Punctuator, ":")
                | '|' -> pushToken(TokenType.Punctuator, "|")

                | ';' -> pushToken(TokenType.Semicolon, ";")

                | '=' -> pushToken(TokenType.Operator, "=")

                | '(' -> pushToken(TokenType.LPair, "(")
                | ')' -> pushToken(TokenType.RPair, ")")
                
                | _ when Char.IsLetter(ch) -> this.tokenize_identifier(results)
                | _ when Char.IsDigit(ch) -> this.tokenize_number(results)
                | _ -> ()


            pushToken(EOF, "")
            results