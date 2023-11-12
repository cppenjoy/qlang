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

        member this.Visit(declVarStmt: DeclVarStmt): unit =

            ()

        member this.Visit(emptyNode: EmptyNode): unit =

            ()

        member this.Visit(declArrayStmt: DeclArrayStmt): unit =

            ()

        member this.Visit(usingDeclStmt: UsingDeclStmt): unit =

            ()
        
        member this.Visit(returnStmt: ReturnStmt): unit =

                ()