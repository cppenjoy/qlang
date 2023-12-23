# decl-fn

decl-fn -> 'fn' identifier fn-param-decl type? fn-body
oneline-fn -> 'fn' identifier fn-param-decl type? '->' fn-short-body
forward-decl-fn -> 'extern' 'fn' identifier fn-param-decl type? ';'

fn-param-decl -> '(' ('void' | fn-param-list) ')'
fn-param-list -> fn-param (',' fn-param)*
fn-param -> type identifier
fn-body -> '{' stmt* '}'
fn-short-body -> stmt