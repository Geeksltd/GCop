﻿# GCop 692

> *"Merge `switch` sections with equivalent content."*

## Rule description

There is a valid syntax for matching multiple cases to a single executable code block to have a more readable and simplified code.

## Example

```csharp
switch (foo)
{
    case "a":
        break;
    case "b":
        break;
}
```

*should be* 🡻

```csharp
switch (foo)
{
    case "a":
    case "b":
        break;
}
```
