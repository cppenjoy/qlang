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
    | number
    | string    
    | boolean

cast-expression ->
    type expression

call-expression ->
    identifier '(' call-param ')'

call-param ->
    'void' | expression (',' expression)*

if-expression ->
    '(' expression ')' ':' expression

binary-expression ->
    number op expression

op ->
    | '+'
    | '-'
    | '*'
    | '/'

identifier -> ([a-zA-Z][0..9]?)*
number -> [0..9]+ (. [0..9])?
string -> '"' [any]* '"'