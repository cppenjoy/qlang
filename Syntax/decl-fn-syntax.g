# decl-fn

decl-fn -> 'fn' identifier fn-param-decl type? fn-body

fn-param-decl -> '(' ('void' | fn-param-list) ')'
fn-param-list -> fn-param (',' fn-param)*
fn-param -> type identifier
fn-body -> '{' stmt* '}'