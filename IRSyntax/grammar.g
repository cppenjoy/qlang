
S -> (decl)

decl -> (extern-decl | fn-decl | var-decl)*

extern-decl -?
    'extern' identifier type-pack

fn-decl -> 
    'fn' identifier

var-decl ->
    identifier '=' mnemonic | value

mnemonic -> 
# Magic
    print-magic
# Functions
    stackframe-mnemonic
    arg-mnemonic
    endstackframe-mnemonic
# Arithmetic 
    add-mnemonic
    sub-mnemonic
    mul-mnemonic
    div-mnemonic
# control-flow
    call-mnemonic
    ret-mnemonic
# stack-manip
    push-mnemonic
    pop-mnemonic

print-magic ->
    'print' value

stackframe-mnemonic ->
    'stackframe'

arg-mnemonic ->
    'arg' type identifier

endstackframe-mnemonic ->
    'endstackframe'

add-mnemonic ->
    'add' value value

sub-mnemonic ->
    'sub' value value

mul-mnemonic ->
    'mul' value value

div-mnemonic ->
    'div' value value

ret-mnemonic ->
    'ret' value

push-mnemonic ->
    'push' value

pop-mnemonic ->
    'pop' value

value -> (identifier | number-literal | string-literal)

type -> '@'? typeOf

typeOf -> 
    'int8'
    'int16'
    'int32'
    'int64'
    'string'
    'int128'
    'float16'
    'float32'
    'float64'
    'float128'

number-literal -> [0..9]*
string-literal -> ' [any] '
identifier -> [a-zA-Z][0..9]*