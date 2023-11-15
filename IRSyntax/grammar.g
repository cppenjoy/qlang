
S -> (decl)

decl -> (fn-decl | var-decl)*

global-var-decl ->
    identifier '=' mnemonic

mnemonic -> 

# Arithmetic 
    add-mnemonic
    sub-mnemonic
    mul-mnemonic
    div-mnemonic

    deref-mnemonic


value -> (identifier | number-literal | string-literal)

number-literal -> [0..9]*
string-literal -> \' [any] \'
identifier -> [a-zA-Z][0..9]*