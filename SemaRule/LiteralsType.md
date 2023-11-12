These rules are known as the Literal Typing Rules. They are used in type systems of programming languages to ensure that literals (i.e., values that are written exactly as they’re meant to be interpreted) are assigned the correct type.

The rules state that:

A number literal n is of type number-type.
A string literal s is of type string-type.
A boolean literal b (true or false) is of type boolean-type.
These rules are crucial for maintaining the consistency and safety of a programming language by ensuring that literals are treated as the correct type. This helps in catching potential type errors during the compilation phase rather than at runtime, making the debugging process easier and the code more reliable. It also allows the language to optimize operations based on the type of the literals.

$$
\begin{align*}
\text{{number}} & : \quad \Gamma \vdash n : \text{{number-type}} \\
\text{{string}} & : \quad \Gamma \vdash s : \text{{string-type}} \\
\text{{boolean}} & : \quad \Gamma \vdash b : \text{{boolean-type}}
\end{align*}
$$