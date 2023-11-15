namespace MetaLang.Service

module CompilerDefinition =

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

    type CompilerOptions(?_lexerTrace: bool, ?_parserTrace: bool, ?_semaTrace: bool) = 

        let lexerTrace: bool = defaultArg _lexerTrace false
        let parserTrace: bool = defaultArg _parserTrace false
        let semaTrace: bool = defaultArg _semaTrace false

        member val LexerTrace: bool = lexerTrace with get, set
        member val ParserTrace: bool = parserTrace with get, set
        member val SemaTrace: bool = semaTrace with get, set

    type CompilerInstance(?_options: CompilerOptions) =
        
        let baseCompilerOptions: CompilerOptions = defaultArg _options (CompilerOptions())

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
                if this.Options.LexerTrace
                    then
                        printf "====Tokens from module %s====" moduleInst.Name

                        for i in lexerResult.Tokens do
                            printfn $"\nToken: [type {i.Type}, lexeme {i.Lexeme}, line {i.Line}, position {i.Pos}]"

            // Parser

                let parser: Parser = Parser(tokens)
                let parserResults: ParserResults = parser.Parse()

                moduleInst.Errors.AddRange(parserResults.Errors)

            // Parser Trace
                if this.Options.ParserTrace
                then
                    let printVisitor: AstPrinter = AstPrinter()

                    for node in parserResults.Tree do
                        node.Accept(printVisitor)

            // Semantic
                let semanticAnalyzer: SemaAnalyzer = SemaAnalyzer(parserResults.SymbolTable)

                for node in parserResults.Tree do
                    node.Accept(semanticAnalyzer)

                let semaResults: SemaResults = semanticAnalyzer.Results

                
                moduleInst.Errors.AddRange(semaResults.Errors)
            ()