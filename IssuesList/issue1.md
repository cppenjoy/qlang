# Issue

At the moment, float and int are generalized to one type: number, which can be confusing, because the compiler generates code depending on the value

# How to solve this problem

Divide the number type to integers: int16, int32, int64, and into real numbers: float, double

# What needs to be changed

 * grammar, 
 * sema rules, 
 * lexer, 
 * parser, 
 * types,
 * sema

# Status

*created branch issue1*
*grammar changed*
*sema rules changed*
*lexer changed*
*parser changed*
*some changes in specifics: rejection of implicit conversions*
*sema changed*
*commit all changes*
*merge issue1 to master*
*deleted branch issue1*