

namespace FSharp

namespace MetaLang.Sema
    
    module SemaDefinition =
        
        type SemaResults =
            
            new: unit -> SemaResults
            
            member
              Errors: System.Collections.Generic.List<MetaLang.Parser.Lexer.LexerDefinition.Error>
        
        type SemaAnalyzer =
            interface MetaLang.Parser.AST.IVisitor
            
            new: _symbolTables: System.Collections.Generic.Dictionary<string,
                                                                      MetaLang.Parser.SymbolTable.SymbolTable.SymbolTable> *
                 ?_semaTrace: bool -> SemaAnalyzer
            
            member
              private ToTypeVariant: identifier: MetaLang.Parser.AST.Identifier ->
                                       MetaLang.Parser.TypeDefinition.TypeVariant
            
            member
              private ToTypeVariant: token: MetaLang.Parser.Lexer.TokenDefinition.Token ->
                                       MetaLang.Parser.TypeDefinition.TypeVariant
            
            member
              private TypeCheckExpression: expression: MetaLang.Parser.AST.Expression *
                                           ?_excepted: MetaLang.Parser.TypeDefinition.TypeVariant ->
                                             unit
            
            member
              private throwError: what: string -> line: int -> pos: int -> unit
            
            member Results: SemaResults
            
            member
              SymbolTables: System.Collections.Generic.Dictionary<string,
                                                                  MetaLang.Parser.SymbolTable.SymbolTable.SymbolTable>
            
            member Trace: bool

