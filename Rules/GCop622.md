# GCop 622

> *"Reverse your `if` condition and return. Then move the nested statements to after the `if`."*

## Rule description

To avoid unnecessary nesting, it's better to get out of the method immediately.

## Example

```csharp
void Foo()
{
   if (condition)
   {
       ...
       ...
       ...
       ...
       ...
   }
}
```

*should be* 🡻

```csharp
void Foo()
{
    if (!condition) return;
   
    ...
    ...
    ...
    ...
    ...   
}
```