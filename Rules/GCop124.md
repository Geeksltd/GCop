# GCop 124

> *"Use **numeric string.TryParseAs< data type >()** instead of **data type.TryParse(numeric string)**"*

## Rule description

The TryParseAs<...>() extension method on the string type allows you to make safe type conversions in a uniform way for many types. If the value cannot be converted to the target type then it returns null instead of throwing an exception. It's briefer and more readable than alternative styles of code for handling this.

## Example 1

```csharp
decimal myDecimal;

if (decimal.TryParse(txtUserInput.Text, out myDecimal))
{
   someObject.SomeNullableProperty = myDecimal;
}
else
{
   someObject.SomeNullableProperty = null;
}
```

*should be* 🡻

```csharp
someObject.SomeNullableProperty = txtUserInput.Text.TryParseAs<decimal>();
```

## Example 2

```csharp
try
{
    someObject.SomeNullableProperty = decimal.Parse(txtUserInput.Text);
}
catch
{
    someObject.SomeNullableProperty = null;
}
```

*should be* 🡻

```csharp
someObject.SomeNullableProperty = txtUserInput.Text.TryParseAs<decimal>();
```