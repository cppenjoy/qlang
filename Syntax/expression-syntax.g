# expression

expression ->
    | '(' expression ')'
    | cast-expression
    | binary-expression
    | literal
    | identifier
    | if-expression
    | call-expression

literal ->
    number
    | string    
    | boolean

cast-expression ->
    '(' type ')' identifier

call-expression ->
    identifier '(' call-param ')'

call-param ->
    'void' | expression (',' expression)*

if-expression ->
    match expression ':' expression

binary-expression ->
    primary-expr op expression

primary-expr ->
    number 
    | identifier
    | cast-expression

op ->
    '+'
    | '-'
    | '*'
    | '/'

integer-suffix ->
    'b' // int8
    | 's' // int16
    | 'l' // int64

identifier -> ([a-zA-Z][0..9]?)*
number -> integer-suffix? [0..9]+ (. [0..9])?
string -> '"' [A-Z] '"'