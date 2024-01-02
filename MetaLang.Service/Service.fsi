

namespace FSharp

namespace MetaLang.Service
    
    type AstPrinter =
        interface MetaLang.Parser.AST.IVisitor
        
        new: unit -> AstPrinter

namespace MetaLang.Service
    
    module VersionDefinition =
        
        /// <summary>
        /// Each module has a MetaLang version. 
        /// This enumeration describes each version of MetaLang, for backward compatibility, and version control of MetaLang during compilation
        /// </summary>
        [<Struct>]
        type MetaLangVersion =
            | ML1 = 0
            | ML2 = 1

namespace MetaLang.Service
    
    module ModuleDefinition =
        
        /// <summary>
        /// A module is a translation unit that contains: a language version, a code. 
        /// Each module has its own identifier. The module ID is usually equal to the path to the file
        /// </summary>
        type Module =
            
            new: _name: string * _version: int * _source: string -> Module
            
            member ThrowAllErrors: unit -> unit
            
            member
              Errors: System.Collections.Generic.List<MetaLang.ErrorHandling.Error>
            
            member Name: string
            
            member Source: string
            
            member Version: int

namespace MetaLang.Service
    
    module CompilerDriverDefinition =
        
        type CompilerOptions =
            
            new: ?_lexerTrace: bool * ?_parserTrace: bool * ?_semaTrace: bool ->
                   CompilerOptions
            
            member LexerTrace: bool with get, set
            
            member ParserTrace: bool with get, set
            
            member SemaTrace: bool with get, set
        
        type CompilerInstance =
            
            new: ?_options: CompilerOptions -> CompilerInstance
            
            member CompileAllModules: unit -> unit
            
            member
              Modules: System.Collections.Generic.List<ModuleDefinition.Module> with get, set
            
            member Options: CompilerOptions with get, set

