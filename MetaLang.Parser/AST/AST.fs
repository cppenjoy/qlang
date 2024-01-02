namespace MetaLang.Parser

open FSharp.Json
open System.Collections.Generic
open MetaLang.Parser.Lexer
open TokenDefinition
open TypeDefinition

module AST =

    type IVisitable =
        abstract member Accept : IVisitor -> unit

    and IVisitor =
        abstract member Visit: AssignStmt -> unit
        abstract member Visit: PrintStmt -> unit
        abstract member Visit: DeclVarStmt -> unit
        abstract member Visit: DeclFnStmt -> unit
        abstract member Visit: DeclArrayStmt -> unit
        abstract member Visit: UsingDeclStmt -> unit
        abstract member Visit: ReturnStmt -> unit
        abstract member Visit: CastExpression -> unit
        abstract member Visit: EmptyNode -> unit

    and Identifier =
        | Identifier of string * string // Text * Context(In Symbol Table)

    and Literal =
        | StringLiteral of string
        | NumberLiteral of Token
        | BooleanLiteral of bool

    and Expression =
        | Expression of Expression

        | CastExpression of CastExpression
        | BinaryExpression of BinaryExpression

        | Identifier of Identifier
        | Literal of Literal

        | EmptyNode of EmptyNode

    and CastExpression =
        | CastExpression of TypeVariant * Identifier

    and ReturnStmt =
        | ReturnStmt of Expression

        interface IVisitable with

            member this.Accept(visitor: IVisitor): unit =

                visitor.Visit this
                ()

    and UsingDeclStmt =
        | UsingDeclStmt of Identifier * TypeVariant

        interface IVisitable with

            member this.Accept(visitor: IVisitor): unit =

                visitor.Visit this
                ()

    and BinaryExpression =
        | BinaryExpression of Primary * Token * Expression

    and Primary =
        | Primary of Literal
        | PrimaryIdentifier of Identifier
        | EmptyNode of unit

    and AssignStmt = 
        | AssignStmt of Identifier * Expression

        interface IVisitable with

            member this.Accept(visitor: IVisitor): unit =

                visitor.Visit this
                ()

    and DeclArrayStmt =
        | DeclArrayStmt of Identifier * TypeVariant * List<Expression>

        interface IVisitable with

            member this.Accept(visitor: IVisitor): unit =

                visitor.Visit this
                ()

    and PrintStmt =
        | PrintStmt of Expression

        interface IVisitable with

            member this.Accept(visitor: IVisitor): unit =

                visitor.Visit this
                ()

    and EmptyNode =
        | EmptyNode of unit

        interface IVisitable with

            member this.Accept(visitor: IVisitor): unit =

                visitor.Visit this
                ()

    and DeclVarStmt =
        | VarStmt of Identifier * TypeVariant * Expression

        interface IVisitable with

            member this.Accept(visitor: IVisitor): unit =

                visitor.Visit this
                ()

    and DeclFnStmt =
        | FnDeclNode of Identifier * TypeVariant * FnParamDecl * FnBody

        interface IVisitable with

            member this.Accept(visitor: IVisitor): unit =

                visitor.Visit this
                ()

    and FnParamDecl =
        | Void of Token
        | ParamListNode of FnParamList

    and FnParamList =
        | FnParamsNode of List<TypeVariant * Identifier>

    and FnBody =
        | FnBody of List<IVisitable>