# GCop 124

> *"Use `numericString.TryParseAs<DataType>()` instead of `DataType.TryParse(numericString)`"*

## Rule description

The `TryParseAs<...>()` extension method on the string type allows you to make safe type conversions in a uniform way for many types. If the value cannot be converted to the target type then it returns null instead of throwing an exception. It's briefer and more readable than alternative styles of code for handling this.

## Example 1

```csharp
if (decimal.TryParse(txtUserInput.Text, out myDecimal))
{
   foo.NullableBar = myDecimal;
}
else
{
   foo.NullableBar = null;
}
```

*should be* 🡻

```csharp
foo.NullableBar = txtUserInput.Text.TryParseAs<decimal>();
```

## Example 2

```csharp
try
{
    foo.NullableBar = decimal.Parse(inputText);
}
catch
{
    foo.NullableBar = null;
}
```

*should be* 🡻

```csharp
foo.NullableBar = inputText.TryParseAs<decimal>();
```