
Includes namespace, class. Introduces new if, throw, return?, metaof, typeof, modifies using, offers static parametric polymorphism

# Fast Example
```cpp
fn<T,R> get(T a, R b) auto
{
    using type_traits;

    if !(is_integer(T) && is_integer(R))
        throw "Bad Type" // Throw compile error

    return? a > b ? a : b 
}
```

# Type traits lib

```cpp

namespace type_traits
{
    fn<T> is_integer(T type) bool
    {
        return (typeof(T) == "int8"
               | typeof(T) == "int16"
               | typeof(T) == "int32"
               | typeof(T) == "int64"
               | metaof(T) == "Integer") // Get tag
    }
}
```

# Meta for class

Now each class will be able to have one meta tag, which you can use in template functions

## example

```ocaml
["Integer"] // Static metatag (can be used only in compile time)
<"Integer"> // Dynamic metatag (can be used in compile time and it runtime)
type Int128;


```