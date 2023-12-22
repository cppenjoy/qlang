namespace MetaLang.Service

open MetaLang.Parser
open AST

type AstPrinter() =

    interface IVisitor with 

        member this.Visit(assignStmt: AssignStmt): unit =

            match assignStmt with
            | AssignStmt (identifier: Identifier, _) -> printfn $"Visited AssignStmt, Identifier: {identifier}"
            ()

        member this.Visit(printStmt: PrintStmt): unit =
            printfn "Visited PrintStmt"
            ()

        member this.Visit(castExpression: CastExpression): unit =
            ()

        member this.Visit(declVarStmt: DeclVarStmt): unit =
            match declVarStmt with
            | VarStmt (identifier, typeOf, expr) -> 
                printfn $"Visited DeclVarStmt, Identifier: {identifier}, Type: {typeOf.ToString()}"
            ()

        member this.Visit(emptyNode: EmptyNode): unit =

            ()

        member this.Visit(declArrayStmt: DeclArrayStmt): unit =

            ()

        member this.Visit(usingDeclStmt: UsingDeclStmt): unit =
            match usingDeclStmt with
            | UsingDeclStmt (identifier, typeOf) -> printfn $"Visited UsingDeclStmt, Identifier: {identifier}, Type: {typeOf.ToString()}"
            ()
        
        member this.Visit(returnStmt: ReturnStmt): unit =
            printfn "Visited ReturnStmt"
            ()