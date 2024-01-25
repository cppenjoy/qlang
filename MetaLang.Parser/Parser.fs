#nowarn "67"

namespace MetaLang.Parser

open System
open System.IO
open System.Linq
open System.Text
open System.Collections.Generic
open MetaLang.Parser.Lexer
open MetaLang.Parser.SymbolTable
open MetaLang.ErrorHandling
open MetaLang.Parser.Lexer
open MetaLang.Parser.SymbolTable
open Fastenshtein
open TokenDefinition
open SymbolDefinition
open LexerDefinition
open TypeDefinition
open AST

module ParserDefinition =

    type ParserResults() = 

        member val Tree: List<IVisitable> = new List<IVisitable>() with get
        member val Errors: List<Error> = new List<Error>() with get

        /// <summary>
        /// lexical scope identifeir * symboltable
        /// </summary>
        member val SymbolTables: Dictionary<string, SymbolTable> = new Dictionary<string, SymbolTable>() with get

    type Parser(_tokens: List<Token>) =

        member val Tokens: List<Token> = _tokens with get, set

        member this.Parse(?trace: bool): ParserResults =

            let _trace = defaultArg trace false

            let parserResults: ParserResults = ParserResults()
            let mutable pos = 0

            let inline lookback() =
                    this.Tokens.[pos - 2]

            let inline throwError(what: string): Token =
                parserResults.Errors.Add ( Error(what, this.Tokens.[pos - 1].Line, this.Tokens.[pos - 1].Pos) )
                Token(TokenType.Error, "")

            let inline throwWarning(what: string): unit =
                parserResults.Errors.Add( Error(what, this.Tokens.[pos - 1].Line, this.Tokens.[pos - 1].Pos, ErrorLevel.Warning) )

            let mutable currentScope = ""
            let mutable latestScope = Stack<string>()

            let inline pushScope identifier =
                if latestScope.Count >= 0  then
                    latestScope.Push currentScope

                currentScope <- identifier

                parserResults.SymbolTables.Add(identifier, new SymbolTable())

            let inline recoveryScope () =
                if latestScope.Count > 1 then
                    currentScope <- latestScope.Pop()
                else
                    throwError "You tried to close the scope without opening it" |> ignore
                    

            let inline getScope identifier =
                parserResults.SymbolTables.[identifier]

            let generateScopeIdentifier (signature: string) length =

                let charTable = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_"

                let randomGenerator = new Random()
                let resultContainer = new StringBuilder()

                resultContainer.Append(signature) |> ignore

                for i in 1..length do
                    resultContainer.Append(charTable.[randomGenerator.Next(0, charTable.Length)]) |> ignore

                resultContainer.ToString()

            pushScope "global"            

            let levenshteinDistance (word1: string) (word2: string) =
                let m, n = word1.Length, word2.Length
                let dp = Array2D.create (m + 1) (n + 1) 0

                for i in 0..m do
                    dp.[i, 0] <- i

                for j in 0..n do
                    dp.[0, j] <- j

                for i in 1..m do
                    for j in 1..n do
                        let cost = if word1.[i - 1] = word2.[j - 1] then 0 else 1
                        dp.[i, j] <- List.min [ dp.[i - 1, j] + 1; dp.[i, j - 1] + 1; dp.[i - 1, j - 1] + cost ]

                dp.[m, n]

            let findMostSimilarWord (word: string) (words: string[]) =
                words
                |> Array.map (fun w -> (w, levenshteinDistance word w))
                |> Array.sortBy (fun (_, distance) -> distance)
                |> Array.head
                |> fst

            let inline getMirrorType typeof =
                findMostSimilarWord typeof [|"char"; "float"; "double"; "string"; "int8"; "int16"; "int32"; "int64"|]

            let next(): Token =

                if pos <= (this.Tokens.Count - 1) then
                    let token = this.Tokens.[int pos]
                    pos <- pos + 1

                    token
                else 
                    Token(EOF, "")

            let inline report (message): unit =
                if _trace then
                    printfn message

            let inline isOp(token: Token): bool =
                match token.Type with
                | Operator -> true
                | _ -> false

            let inline isType(token: Token): bool =
                match token.Type with
                | KeywordString | KeywordBool | KeywordArray | KeywordInt8 | KeywordInt16 | KeywordInt32 | KeywordInt64 | KeywordFloat | KeywordDouble -> true
                | _ when (getScope "global").ExistAlias(Identifier.Identifier(token.Lexeme, "global")) -> true
                | _ when (getScope currentScope).ExistAlias(Identifier.Identifier(token.Lexeme, currentScope)) -> true
                | _ -> false

            let rec parseBinaryExpression(): Expression =

                report "parsing binary expression....."

                pos <- pos - 1

                let firstExpression = parsePrimary()
                next() |> ignore
                let secondExpression: Expression = parseExpression()



                let Node = BinaryExpression(firstExpression, this.Tokens.[pos - 1], secondExpression)

                Expression.BinaryExpression Node

            and parsePrimary(): Primary =

                report "parsing primary....."

                let primary: Token = next()

                match primary.Type with
                | TokenType.NumberLiteral ->
                    Primary.Primary (Literal.NumberLiteral primary)
                
                | TokenType.Identifier ->                     
                    Primary.PrimaryIdentifier (Identifier.Identifier (primary.Lexeme, currentScope))

                | _ -> 
                    throwError $"You cannot use NaN in binary expressions. Ref: {primary.Type.ToString()}" |> ignore
                    Primary.EmptyNode ()

            and parseNumber(): Token =

                report "parsing number....."

                let primary: Token = next()

                match primary.Type with
                | TokenType.NumberLiteral ->                     
                    primary

                | _ -> throwError $"You cannot use NaN in binary expressions. Ref: {primary.Type.ToString()}"

            and parseType(): TypeVariant =

                report "parsing type....."

                let primary: Token = next()

                match primary.Type with
                | KeywordFloat -> TFloat
                | KeywordDouble -> TDouble
                | KeywordInt8 -> TInt8
                | KeywordInt16 -> TInt16
                | KeywordInt32 -> TInt32
                | KeywordInt64 -> TInt64
                | KeywordString -> TString
                | KeywordBool -> TBool
                    
                | _ ->

                    let existOrNot = (getScope "global").Exist (Identifier.Identifier(primary.Lexeme, "global"))

                    match existOrNot with
                    | true ->
                        let typeof = (getScope "global").Get (Identifier.Identifier(primary.Lexeme, "global"))

                        typeof.Alias.TypeOfElem

                    | _ ->

                        if (getScope currentScope).Exist (Identifier.Identifier(primary.Lexeme, currentScope)) then
                            let typeof = (getScope currentScope).Get (Identifier.Identifier(primary.Lexeme, currentScope))
                            typeof.Alias.TypeOfElem

                        else
                            throwError $"The type {primary.Lexeme} is undefined, did you man {getMirrorType primary.Lexeme}\n Note: This type caused was not recognized\n\t{primary.Line} | {this.Tokens.[pos - 4].Lexeme} {this.Tokens.[pos - 3].Lexeme} {lookback().Lexeme} {primary.Lexeme} <- {getMirrorType primary.Lexeme}?" |> ignore
                            TInt32

            and parseIdentifier(): Identifier =

                report "parsing identifier....."

                let primary: Token = next()

                match primary.Type with
                | TokenType.Identifier -> 
                
                    let identifier: Identifier = Identifier.Identifier(primary.Lexeme, currentScope)

                    identifier

                | _ -> 
                    throwError $"Unrecognized identifier. An identifier can only contain letters, _ and digit (after the letter)\n Note: This identifier caused was not recognized\n\t{primary.Line} | {lookback().Lexeme} {primary.Lexeme} <- this is a bad identifier" |> ignore
                    Identifier.Identifier("", "")

            and parseExpression(): Expression =

                report "parsing expression....."

                let primary: Token = next()

                match primary.Type with

                | LPair -> 

                    if isType(next())
                    then
                        pos <- pos - 1

                        let typeOf = parseType()

                        next() |> ignore // Skip the ) symbol
                        let identifier = parseIdentifier()

                        Expression.CastExpression (CastExpression(typeOf, identifier))
                    
                    else
                        let expression = parseExpression()
                        next() |> ignore

                        expression

                | TokenType.Identifier -> 
                
                    let identifier: Identifier = Identifier.Identifier(primary.Lexeme, currentScope)

                    match next().Type with
                    //| LPair ->

                      //  let argumentList = parseArgumentList()

                        // Expression.

                    | Operator ->

                        pos <- pos - 1
                        parseBinaryExpression()

                    | _ ->
                        pos <- pos - 1
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
                        pos <- pos - 2
                        
                        let literal: Token = parseNumber()
                        Expression.Literal (NumberLiteral(literal))

                | _ ->

                    throwError $"{primary.Lexeme} - incomplete expression\n Note: link to the statement\n\t| {lookback().Lexeme}" |> ignore
                    Expression.EmptyNode (EmptyNode ())

            let parseUsingDeclStmt() : IVisitable =

                report "parsing using statement....."


                let identifier: Identifier = parseIdentifier()
                let primary: Token = next()

                match primary.Type with
                | TokenType.Operator when primary.Lexeme = "=" ->

                    let Type = parseType()

                    match (getScope currentScope).Exist identifier with
                    | true -> 
                        throwError $"The identifier {identifier} already defined {Type.ToString()}" |> ignore
                        EmptyNode()

                    | _ ->
                        (getScope currentScope).PushAlias identifier Type primary.Line
                        UsingDeclStmt(identifier, Type)

                | _ -> 
                    throwError "Operator expected =" |> ignore
                    EmptyNode ()

            let parseVarDeclStmt(): IVisitable =            

                report "parsing var statement....."

                let identifier: Identifier = parseIdentifier()
                let optionalCheck: Token = next()

                let mutable typeOfVar: TypeVariant = TInt32

                match optionalCheck.Type with
                | Punctuator when optionalCheck.Lexeme = ":" ->

                    typeOfVar <- parseType()

                    next() |> ignore

                    let expression = parseExpression()

                    match (getScope currentScope).Exist identifier with
                    | true -> 
                        let symbol = (getScope currentScope).Get identifier

                        throwError $"Some error occurred. The {symbol.Alias.Identifier} already defined in line {symbol.LineOfDeclaration}" |> ignore
                        EmptyNode()

                    | _ ->
                        (getScope currentScope).PushVariable currentScope identifier typeOfVar optionalCheck.Line
                        VarStmt(identifier, typeOfVar, expression)

                | Operator when optionalCheck.Lexeme = "=" -> 
                    
                    let expression = parseExpression()

                    match (getScope currentScope).Exist identifier with
                    | true -> 
                        let symbol = (getScope currentScope).Get identifier

                        throwError $"Some error occurred. The {symbol.Alias.Identifier} already defined in line {symbol.LineOfDeclaration}" |> ignore
                        EmptyNode()

                    | _ ->
                        (getScope currentScope).PushVariable currentScope identifier typeOfVar optionalCheck.Line
                        VarStmt(identifier, typeOfVar, expression)

                | _ ->

                    throwError "After the identifier in the let construct, you can explicitly specify the type, or assign a value" |> ignore
                    EmptyNode ()

            let parseAssignExpression(): IVisitable =

                report "parsing assign expression....."

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

                report "parsing return expression....."

                let expression: Expression = parseExpression()

                PrintStmt.PrintStmt(expression)

            let parsePrintStmt(): IVisitable =

                report "parsing print statement....."
                let expression: Expression = parseExpression()

                PrintStmt.PrintStmt(expression)

            let parsePPInclude(): IVisitable =

                report "including some shit......"

                let path =

                    match next().Type with
                    | TokenType.StringLiteral -> this.Tokens[pos - 1]
                    | _ ->
                        throwError "Bad literal"

                let mutable source: string = ""

                try 
                    source <- File.ReadAllText path.Lexeme
                with
                | :? Exception as (excep: Exception) ->
                    throwError "File Not Found" |> ignore

                this.Tokens[pos - 1] <- Token(Empty, "")

                // Tokenizing include text
                let lexer: Lexer = Lexer(source)
                let lexerResult: LexerResults = lexer.Tokenize()

                this.Tokens.InsertRange(0, lexerResult.Tokens)
                parserResults.Errors.AddRange(lexerResult.Errors)

                pos <- pos - 2
                printfn "%s" (this.Tokens[pos].Type.ToString())

                EmptyNode()

            let rec parseFnDeclStmt(): IVisitable =

                report "parsing fn decl statement....."

                let identifier: Identifier = parseIdentifier()

                let parseArgumentList(): List<TypeVariant * Identifier> =

                    let result = List<TypeVariant * Identifier>()

                    let rec parseParam() = 
                          
                        match isType(next()) with
                        | true ->
                            pos <- pos - 1

                            let typeof = parseType()
                            let argumentIdentifier = parseIdentifier()

                            match next().Type with
                            | RPair ->

                                result.Add (typeof, argumentIdentifier)
                                result
                            
                            | Comma ->
                                result.Add (typeof, argumentIdentifier)
                                parseParam()

                            | _ ->
                                throwError $"Unfinished expression" |> ignore
                                new List<TypeVariant * Identifier>()


                        | _ ->

                            if this.Tokens.[pos - 1].Type = TokenType.RPair then
                                throwError $"Missing void in function argument specifier" |> ignore
                            else
                                throwError $"The identifier {this.Tokens.[pos - 1].Lexeme} - not type" |> ignore

                            new List<TypeVariant * Identifier>()


                    match next().Type with
                    | TokenType.KeywordVoid ->
                        next() |> ignore
                        new List<TypeVariant * Identifier>()
                
                    | _ ->
                        pos <- pos - 1
                        parseParam()

                match next().Type with
                | LPair ->
                    let argumentList = parseArgumentList()
                    let optionalCheck = next()

                    let mutable returnType = TInt32

                    match optionalCheck.Type with
                    | LBrace ->

                        let fnData = FnData(returnType, argumentList)

                        match (getScope currentScope).Exist (identifier) with
                        | true -> 
                            throwError $"The identifier {identifier} already defined in line {(getScope currentScope).Get(identifier).LineOfDeclaration }" |> ignore
                        | _ ->

                            (getScope currentScope).PushFunction currentScope identifier (_tokens.[pos - 1].Line) fnData 

                            pushScope (generateScopeIdentifier $"{identifier.ToString()}" 9)


                        let body = parseBody()

                        let fnBody = FnBody.FnBody(body)
                        let fnArgumentList = FnParamList.FnParamsNode(argumentList)
                        let fnParams = FnParamDecl.ParamListNode(fnArgumentList)
                        let fnNode = DeclFnStmt.FnDeclNode(identifier, returnType, fnParams, fnBody) 
                           
                        fnNode

                    | _ when isType(optionalCheck) ->
                        
                        pos <- pos - 1
                        returnType <- parseType()

                        match next().Type with
                        | LBrace ->

                            let fnData = FnData(returnType, argumentList)

                            match (getScope currentScope).Exist (identifier) with
                            | true -> 
                                throwError $"The identifier {identifier} already defined in line {(getScope currentScope).Get(identifier).LineOfDeclaration}" |> ignore
                            | _ ->

                                (getScope currentScope).PushFunction currentScope identifier (_tokens.[pos - 1].Line) fnData 

                                pushScope (generateScopeIdentifier $"{identifier.ToString()}" 9)


                            let body = parseBody()

                            let fnBody = FnBody.FnBody(body)
                            let fnArgumentList = FnParamList.FnParamsNode(argumentList)
                            let fnParams = FnParamDecl.ParamListNode(fnArgumentList)
                            let fnNode = DeclFnStmt.FnDeclNode(identifier, returnType, fnParams, fnBody) 

                            let fnData = FnData(returnType, argumentList)
                            fnNode

                        | _ ->
                            throwError "stupid idiot, where fn body?????" |> ignore
                            EmptyNode()

                    | _ ->
                        throwError "Except return type or function body" |> ignore
                        EmptyNode()

                | _ ->
                    throwError "unfinished declaration" |> ignore
                    EmptyNode()

            and parseBody(): List<IVisitable> =

                report "parsing body....."

                let result = List<IVisitable>()

                let rec parseStmt () =

                    match next().Type with
                    | EOF | RBrace ->
                        ()

                    | _ -> 

                        pos <- pos - 1

                        let stmt: IVisitable = parseStatement()
                        
                        result.Add stmt
                        parseStmt()

                result

            and parseStatement() =

                report "parsing statement....."

                match next().Type with
                | KeywordPrint ->
                    parsePrintStmt()
                
                | LBrace ->
                    pushScope (generateScopeIdentifier "" 20)
                    parseStatement()

                | RBrace ->
                    recoveryScope()
                    EmptyNode ()

                | KeywordLet -> parseVarDeclStmt()
                | PP_KeywordInclude ->
                    this.Tokens.[pos - 1] <- Token(TokenType.Empty, "")
                    parsePPInclude()

                | KeywordFn -> parseFnDeclStmt()
                | KeywordUsing -> parseUsingDeclStmt()
                | KeywordReturn -> parseReturnStmt()
                | TokenType.Identifier -> 
                    pos <- pos - 1
                    parseAssignExpression()
                
                | Empty ->
                    EmptyNode()

                | _ -> 
                    throwError $"The keyword or identifier {this.Tokens.[pos - 1].Lexeme} is undefined" |> ignore
                    EmptyNode ()

            let rec parse() =

                report "start parsing....."

                match next().Type with
                | EOF -> ()

                | _ -> 

                    pos <- pos - 1

                    let stmt: IVisitable = parseStatement()
                    
                    parserResults.Tree.Add stmt
                    parse()


            parse()
            
            report "parsing done....."
            parserResults