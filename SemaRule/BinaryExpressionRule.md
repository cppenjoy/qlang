
This rule is known as the Binary Operation Typing Rule. It is used in type systems of programming languages to ensure that binary operations are only performed on operands of the correct type, in this case, numbers.

The rule states that if n1 and n2 are expressions of type number-type, then the result of a binary operation (op) on n1 and n2 is also of type number-type. The binary operators include addition (+), subtraction (-), multiplication (*), division (/).

This rule is crucial for maintaining the consistency and safety of a programming language by preventing operations between incompatible types, which could lead to undefined behavior or runtime errors. It helps in catching potential type errors during the compilation phase rather than at runtime, making the debugging process easier and the code more reliable.

$$
\text
    {op = + | - | * | / }
$$

$$
\frac{{\Gamma \vdash n_1 : \text{{number-type}} \quad \Gamma \vdash n_2 : \text{{number-type}}}}{{\Gamma \vdash n_1 \, \text{{op}} \, n_2 : \text{{number-type}}}}
$$

