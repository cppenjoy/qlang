



namespace MetaLang.Service
    
    type AstPrinter =
        interface Parser.AST.IVisitor
        
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
            
            member Errors: System.Collections.Generic.List<ErrorHandling.Error>
            
            member Name: string
            
            member Source: string
            
            member Version: int

namespace MetaLang.Service
    
    module CompilerDriverDefinition =
        
        type CompilerOptions =
            {
              mutable LexerTrace: bool
              mutable ParserTrace: bool
              mutable DumpAst: bool
            }
        
        type CompilerInstance =
            
            new: ?_options: CompilerOptions -> CompilerInstance
            
            member CompileAllModules: unit -> unit
            
            member
              Modules: System.Collections.Generic.List<ModuleDefinition.Module>
            
            member Options: CompilerOptions

