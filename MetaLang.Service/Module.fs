namespace MetaLang.Service

open VersionDefinition
open System.Collections.Generic
open MetaLang.ErrorHandling

module ModuleDefinition = 

/// <summary>
/// A module is a translation unit that contains: a language version, a code. 
/// Each module has its own identifier. The module ID is usually equal to the path to the file
/// </summary>
    type Module(_name: string, _version: int, _source: string) =  

        member val Name: string = _name with get
        member val Version: int = _version with get
        member val Source: string = _source with get

        member val Errors: List<Error> = List<Error>() with get

        member this.ThrowAllErrors() =

            for error in this.Errors do

                printf $"\n From module {this.Name}"
                error.Throw()

            ()
