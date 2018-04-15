# GCop 149

> *"Instead of using string literal as parameter name use nameof(variableName)"*

## Rule description

With the introduction of the `nameof` operator, the hard-coded string to be specified in our code can be avoided. The `nameof` operator accepts the name of code elements and returns a string literal of the same element.

Using string literals for the purpose of throwing an `ArgumentNullException` is simple, but error prone because we may spell it wrong, or a refactoring may leave it stale. The `nameof` operator expressions are a special kind of string literal where the compiler checks that you have something of the given name and Visual Studio knows where it refers to, so navigation and refactoring will work easily.

## Example1

```csharp
if (title == null)
    throw new ArgumentNullException("title");
```

*should be* 🡻

```csharp
if (title == null)
    throw new ArgumentNullException(nameof(title));
```

## Example2

```csharp
this.something = theArgument.ThrowIfNull("theArgument");

public static T ThrowIfNull<T>(this T argument, string argumentName)
{
    if (argument == null)
    {
        throw new ArgumentNullException(argumentName);
    }
    return argument;
}
```

*should be* 🡻

```csharp
this.something = theArgument.ThrowIfNull(nameof(theArgument));

public static T ThrowIfNull<T>(this T argument, string argumentName)
{
    if (argument == null)
    {
        throw new ArgumentNullException(argumentName);
    }
    return argument;
}
```
