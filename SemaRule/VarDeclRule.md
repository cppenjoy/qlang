This rule is known as the Variable Declaration Typing Rule. It is used in type systems of programming languages to ensure that the type of an expression matches the type of the variable it is being assigned to.

The rule states that if e is an expression of type T, then in the declaration let x : T = e, x will also be of type T. The : symbol is used to annotate the type of the variable x. If the type T is not explicitly annotated, it defaults to number-type.

This rule is crucial for maintaining the consistency and safety of a programming language by preventing type mismatches during variable assignment. It helps in catching potential type errors during the compilation phase rather than at runtime, making the debugging process easier and the code more reliable. It also allows programmers to explicitly state their intent, making the code easier to understand and maintain.

$$
\frac{{\Gamma \vdash e : T}}{{\Gamma \vdash \text{{let }} x : T = e : T}}
$$
