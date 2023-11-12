namespace MetaLang.Service

module VersionDefinition =

/// <summary>
/// Each module has a MetaLang version. 
/// This enumeration describes each version of MetaLang, for backward compatibility, and version control of MetaLang during compilation
/// </summary>
    type MetaLangVersion =
        | ML1 = 0
        | ML2 = 1
