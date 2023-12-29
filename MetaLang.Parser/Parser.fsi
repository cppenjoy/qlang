



namespace MetaLang.Parser
    
    module TypeDefinition =
        
        type TypeVariant =
            | TInt8
            | TInt16
            | TInt32
            | TInt64
            | TFloat
            | TDouble
            | TString
            | TBool
            | TArray
            | TNumber
            | TAny
            | TBad

namespace MetaLang.Parser.Lexer
    
    module TokenDefinition =
        
        type TokenType =
            | Operator
            | LPair
            | RPair
            | LBrace
            | RBrace
            | Comma
            | Semicolon
            | Punctuator
            | Identifier
            | StringLiteral
            | NumberLiteral
            | BooleanLiteral
            | KeywordLet
            | KeywordPrint
            | KeywordString
            | KeywordInt8
            | KeywordInt16
            | KeywordInt32
            | KeywordInt64
            | KeywordFloat
            | KeywordDouble
            | KeywordArray
            | KeywordStatic
            | KeywordBool
            | KeywordFn
            | KeywordVoid
            | KeywordUsing
            | KeywordReturn
            | Error
            | EOF
        
        [<Struct>]
        type LiteralVariant =
            | None = 0
            | Integer8 = 1
            | Integer16 = 2
            | Integer = 3
            | Integer64 = 4
            | Float = 5
        
        type Token =
            
            new: TokenType: TokenType * TokenLexeme: string * ?TokenLine: int *
                 ?TokenPos: int * ?LiteralType: LiteralVariant -> Token
            
            member Lexeme: string
            
            member Line: int
            
            member LiteralType: LiteralVariant
            
            member Pos: int
            
            member Type: TokenType
        
        val KeywordAsToken: Map<string,TokenType>
    
    module LexerDefinition =
        
        type Error = ErrorHandling.Error
        
        type LexerResults =
            
            new: unit -> LexerResults
            
            member Errors: System.Collections.Generic.List<Error>
            
            member
              Tokens: System.Collections.Generic.List<TokenDefinition.Token>
        
        type Lexer =
            
            new: _source: string -> Lexer
            
            member Tokenize: unit -> LexerResults
            
            member private is_end: offset: int -> bool
            
            member private next: unit -> char
            
            member private peek: offset: int -> char
            
            member private tokenize_identifier: results: LexerResults -> unit
            
            member
              private tokenize_number: results: LexerResults * ?prefix: char ->
                                         unit
            
            member
              private tokenize_string_literal: results: LexerResults -> unit
            
            member Source: string

namespace MetaLang.Parser
    
    module AST =
        
        type IVisitable =
            
            abstract Accept: IVisitor -> unit
        
        and IVisitor =
            
            abstract Visit: EmptyNode -> unit
            
            abstract Visit: CastExpression -> unit
            
            abstract Visit: ReturnStmt -> unit
            
            abstract Visit: UsingDeclStmt -> unit
            
            abstract Visit: DeclArrayStmt -> unit
            
            abstract Visit: DeclFnStmt -> unit
            
            abstract Visit: DeclVarStmt -> unit
            
            abstract Visit: PrintStmt -> unit
            
            abstract Visit: AssignStmt -> unit
        
        and Identifier = | Identifier of string * string
        
        and Literal =
            | StringLiteral of string
            | NumberLiteral of Lexer.TokenDefinition.Token
            | BooleanLiteral of bool
        
        and Expression =
            | Expression of Expression
            | CastExpression of CastExpression
            | BinaryExpression of BinaryExpression
            | Identifier of Identifier
            | Literal of Literal
            | EmptyNode of EmptyNode
        
        and CastExpression =
            | CastExpression of TypeDefinition.TypeVariant * Identifier
        
        and ReturnStmt =
            | ReturnStmt of Expression
            interface IVisitable
        
        and UsingDeclStmt =
            | UsingDeclStmt of Identifier * TypeDefinition.TypeVariant
            interface IVisitable
        
        and BinaryExpression =
            | BinaryExpression of
              Primary * Lexer.TokenDefinition.Token * Expression
        
        and Primary =
            | Primary of Literal
            | PrimaryIdentifier of Identifier
            | EmptyNode of unit
        
        and AssignStmt =
            | AssignStmt of Identifier * Expression
            interface IVisitable
        
        and DeclArrayStmt =
            | DeclArrayStmt of
              Identifier * TypeDefinition.TypeVariant *
              System.Collections.Generic.List<Expression>
            interface IVisitable
        
        and PrintStmt =
            | PrintStmt of Expression
            interface IVisitable
        
        and EmptyNode =
            | EmptyNode of unit
            interface IVisitable
        
        and DeclVarStmt =
            | VarStmt of Identifier * TypeDefinition.TypeVariant * Expression
            interface IVisitable
        
        and DeclFnStmt =
            | FnDeclNode of
              Identifier * TypeDefinition.TypeVariant * FnParamDecl * FnBody
            interface IVisitable
        
        and FnParamDecl =
            | Void of Lexer.TokenDefinition.Token
            | ParamListNode of FnParamList
        
        and FnParamList =
            | FnParamsNode of
              System.Collections.Generic.List<TypeDefinition.TypeVariant *
                                              Identifier>
        
        and FnBody = | FnBody of System.Collections.Generic.List<IVisitable>

namespace MetaLang.Parser.SymbolTable
    
    module SymbolDefinition =
        
        type SymbolType =
            | Variable
            | Fn
            | Array
            | Alias
        
        type ArrayElements =
            
            new: unit -> ArrayElements
            
            member Elements: System.Collections.Generic.List<Symbol>
            
            member TypeOfElem: TypeDefinition.TypeVariant
        
        and AliasData =
            
            new: identifier: AST.Identifier *
                 ?typeVar: TypeDefinition.TypeVariant -> AliasData
            
            member Identifier: AST.Identifier
            
            member TypeOfElem: TypeDefinition.TypeVariant
        
        and FnData =
            
            new: typeofReturn: TypeDefinition.TypeVariant *
                 signature: System.Collections.Generic.List<TypeDefinition.TypeVariant *
                                                            AST.Identifier> ->
                   FnData
            
            member
              Signature: System.Collections.Generic.List<TypeDefinition.TypeVariant *
                                                         AST.Identifier>
            
            member TypeofReturn: TypeDefinition.TypeVariant
        
        and Symbol =
            
            new: _type: SymbolType * _info: TypeDefinition.TypeVariant *
                 _decl: uint32 * ?_context: string * ?_arrayElem: ArrayElements *
                 ?_aliasData: AliasData * ?_fnData: FnData -> Symbol
            
            member Alias: AliasData
            
            member ArrayElements: ArrayElements
            
            member Context: string
            
            member IsAlias: bool
            
            member LineOfDeclaration: uint32
            
            member Type: SymbolType
            
            member TypeInfo: TypeDefinition.TypeVariant
    
    module SymbolTable =
        
        /// <summary>
        /// Provide a wrapper on list of symbols
        /// </summary>
        type SymbolTable =
            
            new: unit -> SymbolTable
            
            member Exist: identifier: AST.Identifier -> bool
            
            member Exist: symbol: SymbolDefinition.Symbol -> bool
            
            member ExistAlias: identifier: AST.Identifier -> bool
            
            member Get: identifier: AST.Identifier -> SymbolDefinition.Symbol
            
            member
              GetIfExist: identifier: AST.Identifier ->
                            bool * SymbolDefinition.Symbol
            
            member
              PushAlias: identifier: AST.Identifier ->
                           refType: TypeDefinition.TypeVariant ->
                           line: int -> unit
            
            member
              PushFunction: scope: string ->
                              identifier: AST.Identifier ->
                              line: int ->
                              fnData: SymbolDefinition.FnData -> unit
            
            member PushSymbol: symbol: SymbolDefinition.Symbol -> unit
            
            member
              PushVariable: scope: string ->
                              identifier: AST.Identifier ->
                              typeofVariable: TypeDefinition.TypeVariant ->
                              line: int -> unit
            
            member
              Symbols: System.Collections.Generic.List<SymbolDefinition.Symbol>

namespace MetaLang.Parser
    
    module ParserDefinition =
        
        type ParserResults =
            
            new: unit -> ParserResults
            
            member
              Errors: System.Collections.Generic.List<Lexer.LexerDefinition.Error>
            
            /// <summary>
            /// lexical scope identifeir * symboltable
            /// </summary>
            member
              SymbolTables: System.Collections.Generic.Dictionary<string,
                                                                  SymbolTable.SymbolTable.SymbolTable>
            
            member Tree: System.Collections.Generic.List<AST.IVisitable>
        
        type Parser =
            
            new: _tokens: System.Collections.Generic.List<Lexer.TokenDefinition.Token> ->
                   Parser
            
            member Parse: ?trace: bool -> ParserResults
            
            member
              Tokens: System.Collections.Generic.List<Lexer.TokenDefinition.Token>

