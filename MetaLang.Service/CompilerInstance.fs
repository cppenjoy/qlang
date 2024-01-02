namespace MetaLang.Service

module CompilerDriverDefinition =

    open System
    open FSharp.Json
    open System.Collections.Generic
    open MetaLang.Parser
    open MetaLang.Sema
    open SemaDefinition
    open MetaLang.Parser.Lexer
    open MetaLang.ErrorHandling
    open TokenDefinition
    open ParserDefinition
    open LexerDefinition
    open ModuleDefinition
    open AST

    type CompilerOptions = { mutable LexerTrace: bool; mutable ParserTrace: bool; mutable DumpAst : bool }

    type CompilerInstance(?_options: CompilerOptions) =
        
        let baseCompilerOptions: CompilerOptions = defaultArg _options { LexerTrace = false; ParserTrace = false; DumpAst = false } 

        member val Options: CompilerOptions = baseCompilerOptions with get, set
        member val Modules: List<Module> = List<Module>() with get, set

        member this.CompileAllModules() =

            let tokens: List<Token> = List<Token>()

            for moduleInst: Module in this.Modules do

            // Lexer

                let lexer: Lexer = Lexer(moduleInst.Source)
                let lexerResult: LexerResults = lexer.Tokenize()

                tokens.AddRange(lexerResult.Tokens)
                moduleInst.Errors.AddRange(lexerResult.Errors)

            // Lexer Trace
                if this.Options.LexerTrace then
                    printf "\n====Tokens from module %s====" moduleInst.Name

                    for i in lexerResult.Tokens do
                        printfn $"\nToken: [type {i.Type}, lexeme {i.Lexeme}, line {i.Line}, position {i.Pos}, literal-type {i.LiteralType}]"

            // Parser

                let parser: Parser = Parser(tokens)
                let parserResults: ParserResults = parser.Parse(if this.Options.ParserTrace then true else false)

                moduleInst.Errors.AddRange(parserResults.Errors)

                if this.Options.DumpAst then
                    let tree = List.ofSeq parserResults.Tree

                    let jsonString = Json.serialize tree
                    Console.WriteLine(jsonString);

            // Parser Trace
                if this.Options.ParserTrace then
                    let printVisitor: AstPrinter = AstPrinter()

                    for node in parserResults.Tree do
                        node.Accept(printVisitor)

            // Semantic
                let semanticAnalyzer: SemaAnalyzer = SemaAnalyzer(parserResults.SymbolTables)

                for node in parserResults.Tree do
                    node.Accept(semanticAnalyzer)

                let semaResults: SemaResults = semanticAnalyzer.Results

                
                moduleInst.Errors.AddRange(semaResults.Errors)
            ()