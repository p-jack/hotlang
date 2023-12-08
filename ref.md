# Hot Language Reference

## Types

### Boolean

The boolean type is just `bool`.

### Integers

0123456789012345678901234567890123456789012345678901234567890123456789
The system signed integer is `int`, as per tradition. The system unsigned 
integer is `uint.`

You can follow `int` or `uint` with a width, eg `uint16` or `int64`.

By default, operations on integers are not allowed to overflow. You can
prepend `overflowable_` to the type allow overflow, eg `overflowabe_uint`
for a hash code.

### Floating-Point

The two floating point types are `float32` and `float64`.

### Structuress

In hot, names always come first in a declaration. A name followed by an
opening brace declares a struct. A struct can contain other top-level
declarations, including functions and other structs.

Fields can be declared in three ways:

1. A name followed by a type and an optional initial value
2. A type and an optional initial value
3. An equals sign followed by an initial value

For #2, the name will be derived from the type. For #3, the type is 
derived from the initial value, and the name then derived from the type.

Here's a simple struct that defines a 2D point:

    point {
      x float64 = 0.0
      y float64 = 0.0
    }

Here's a struct that wraps a string:

    wrapper {
      = ""
    }

The `wrapper` struct has one field, whose name is string, whose type is
string, and whose initial value is an empty string.

In a function, you can create a struct using braces:

    let point = point{x:0.0, y:0.0}

The field names are optional:

    let point = point{0.0, 0.0}

And the values are optional if the fields have initial values:

    let point = point{}

You will commonly want to name a variable after its type. You can do so
quickly by omitting the name:

    let = point{}

The above statement creates a local variable named `point`.

### Classes

You can define a class by following its name with a slash and an optional
superclass. If no superclass is given, you define a new class hierarchy:

    root/ {
    }

    sub/super {
    }

### Arrays

Arrays are declared with an element type surrounded by braces. 

### Tins

A *tin* is a generic concreate data structure, such as a hash table.
A tin is declared with a # followed by the data structure name,
followed by the element type in braces, followed by an optional set
of tin-specific tweaks::

    #hash[string]{collide:"linear-probe"}

The above declares that you want a hash table that uses linear 
probing. The various tins define default tweaks. For instance, if you
didn't specify the collide tweak, it would default to "open-chaining".

### Enums

An enum is declared by following its name with the contextual keyword
`enum`, followed by an optional type. If the type is not specified,
it defaults to `int`.

   status enum {
     ready = 0
     progressing = 1
     success = 2
     failure = 3
   }

For `int` enums, you can remove the initial values from its members,
and they will start at 0 and increase by 1.

## Type Lenses

Types in hot are "blurry" by default. You can focus them using lenses.
The lenses are specified by following a type with a colon and various
letters.

If you don't specify lenses, default lenses are used. The defaults vary
depending on context.

There are four lenses:

1. Nullability (:n, :N)
2. Variability (:f, :v)
3. Mutabilty (:i, :m, :l)
4. Memory Management Scheme (:d, :j, :p, :r, :s, :u, :w, :W)

### Nullability

Nullability determines whether a pointer is allowed to be null or not.
The possible lenses are:

- :N - non-null; the value is guaranteed to never be null
- :n - nullable; the value may or may not be null

Primitive values can never be null. Arrays can never be null, only empty.

Pointer parameters are non-null by default. For fields in a struct, the
rules are:

- Reference-counted objects (:r) are non-null by default
- Primitive values and arrays are always non-null
- Unique pointers (:u) and weak references (:w, :W) are nullable by default

### Variability

Variability determines whether an lvalue can change. The two values are:

- :f - fixed; the lvalue will not change
- :v - variable; the lvalue is allowed to change

Almost everything is :v by default. The one exception is the `this`
parameter of a method, which is always :f.

### Mutability

Mutability determines whether an rvalue can change. The three values are;

- :i - immutable; the value will never change
- :m - mutable; the value is allowed to change
- :l - locked; the value will not be changed by the function parameter or
  field declaring the value, but might be changed by some other function
  or some other aggregate containing the value

### Memory Management Scheme

The scheme determines how memory is managed. The values are:

- :d - data; the value is declared in the .DATA or .BSS section of the
  object file, and will therefore never be freed
- :e - embedded; the object is nested, and copied on assignment
- :j - jailed; the pointer will not escape the stack
- :p - primitive; the value is copied on assignment
- :r - reference counted; the value is a pointer with a reference count
  that is automatically managed on assignment. When the reference count
  reaches zero, the object is freed
- :s - stack; the object is allocated on the stack
- :u - unique; the object is a unique pointer. When set to null, the 
  object is freed
- :w - weak; a bidirectional weak reference
- :W - weak; a unidirectional weak reference, only available in limited
  scenarios

The difference between :j and :s is that a :j pointer *might* be allocated
on the stack, and a :s pointer is *definitely* allocated on the stack.

The default scheme depends on the type:

- Primitive types always use :p
- Structures that contain :j fields must use :s
- Structures that contain no reference cycles default to :r
- Structures that do contain reference cycles default to :w
- Arrays of primitives use :r
- Arrays of :r also use :r
- Tins of :w use :e

## Functions

You declare function with its name followed by its parameters, followed by
its return type (if any), followed by its block:

    sum(x int, y int) int {
      return x + y
    }

For one-line functions, you can use a comma instead of a block:

   sum(x int, y int), x + y

### this

If you define a function inside a struct, you can add a 'this' parameter:

   point {
   
     x float64
     y float64
   
     add(this point, that point) point, {x + that.x, y + that.y}
   
   }

You will want to do that a lot, so you can shorthand it by just specifying
a single colon:

   point {
   
     x float64
     y float64
   
     add(:, that point) point, {x + that.x, y + that.y}
   
   }
   
## Methods

Methods are similar to functions, but begin with a dot:

    animal/ {
      .sound() string, ""
    }

You override a method with a slash:

   cat/animal {
     /sound, "meow"
   }

Note that you can omit the parameters and return type for an overriden
method if you don't need to change them. (Parameters are contravariant,
and return types are covariant, so you *can* change them if you need to.)

Finally, you declare an abstract method with a backslash. A better
version of the above `animal` class would be:

    animal/ {
      \sound() string
    }
