# GCop 141

> *"Avoid using goto command"*

## Rule description

The `goto` transfers control to a named label. It can be used to exit a deeply nested loop or transfer control to a specific `switch-case` label or the default label in a `switch` statement. `Goto` may look fast and convenient. But it can confuse the runtime and result in poorer performance. Thus `goto` is not only more complex for programmers. It is more difficult for the JIT compiler to optimize.

Of course, there are other ways around this problem as well, such as refactoring the code into a function, using a dummy block around it, etc.

## Example

```csharp
for (...) 
{
    for (...)
    {
        ...
        if (something)
            goto end_of_loop;
    }
}

end_of_loop:
            ...
```

*should be* 🡻

```csharp
for (...)
{
    bool ok = true;
    for (...)
    {
        ...
        if (something)
        {
            ok = false;
            break;
        }
    }
    if (!ok)
    {
        break;
    }
}
```

In this example, alternatively, you can refactor the big loop into a method, and simply `return` instead of `goto`.
