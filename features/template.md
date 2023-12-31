
Includes namespace, class. Introduces new if, throw, return?, metaof, typeof, modifies using, offers static parametric polymorphism

# Fast Example
```cpp
fn<T,R> get(T a, R b) auto
{
    using type_traits

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
               | metaof<T>("Integer")) // Exists tag or not
    }
}
```

# Typeof

typeof(T) return text representation of type T

# Metaof

metaof<T>(V) return true, if type has a meta V. Otherwise returning false

# Meta for class

Now each class will be able to have one meta tag, which you can use in template functions

## example

```ocaml
["Integer"] // Static metatag (can be used only in compile time)
<"Integer"> // Dynamic metatag (can be used in compile time and it runtime)
type Int128;

You can assign multiple meta tags to one type

<"Functor"> | ["Integer"]
type Int128;
```

# Concepts

Concepts offer the mechanic of type constraints through meta tags. You can declare a meta tag

for example

```ocaml
type ["Test"] with
| field m // Required field named M
| fn op // Required method named M
| meta Callable // Required metatag named Callable

```

# Example: Creating Wrapper on Int

```ocaml
#include "lib/type_traits.h"

<"Integer">
type Int =

    let private value = 0

    ctor Int(int init_value) -> this.value = init_value
```