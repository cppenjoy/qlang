```ocaml
type Def =

    // Field declaration
    let someField = 0

    // Constructor declaration. Behaviour as a basic function
    ctor Def(int value) -> this.someField = value

    // Method declaration
    fn SomeFieldSetter(int value) -> this.someField = value

let obj: Def = Def(0) // Creating new instance of class Def

// Calling method
obj.SomeFieldSetter(0)

// Changing field value
Def.someField = 0
```

# Status
waiting