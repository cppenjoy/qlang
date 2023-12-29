



namespace MetaLang.Sema
    
    module SemaDefinition =
        
        type SemaResults =
            
            new: unit -> SemaResults
            
            member
              Errors: System.Collections.Generic.List<Parser.Lexer.LexerDefinition.Error>
        
        type SemaAnalyzer =
            interface Parser.AST.IVisitor
            
            new: _symbolTables: System.Collections.Generic.Dictionary<string,
                                                                      Parser.SymbolTable.SymbolTable.SymbolTable> *
                 ?_semaTrace: bool -> SemaAnalyzer
            
            member
              private ToTypeVariant: identifier: Parser.AST.Identifier ->
                                       Parser.TypeDefinition.TypeVariant
            
            member
              private ToTypeVariant: token: Parser.Lexer.TokenDefinition.Token ->
                                       Parser.TypeDefinition.TypeVariant
            
            member
              private TypeCheckExpression: expression: Parser.AST.Expression *
                                           ?_excepted: Parser.TypeDefinition.TypeVariant ->
                                             unit
            
            member
              private throwError: what: string -> line: int -> pos: int -> unit
            
            member Results: SemaResults
            
            member
              SymbolTables: System.Collections.Generic.Dictionary<string,
                                                                  Parser.SymbolTable.SymbolTable.SymbolTable>
            
            member Trace: bool

