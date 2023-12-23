
```cpp
// Basic define
namespace SomeSpace
{
    fn test(void) int {}
}

// Shorthand define
namespace SomeSpace;

// using content from namespace
using SomeSpace

// get an element from namespace(using special operator - ::)
SomeSpace::test()
```

# Status
waiting............

# What need to changes
syntax
parser
lexer
(symbol table driver)