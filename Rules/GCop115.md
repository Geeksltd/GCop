# GCop 115

> *"This `IF` statement is too long for one line. Break the body of the `IF` into another line"*

## Rule description

Long statements reduce code readability. Break down the body of `if` into another line to have a more readable code.

## Example1

```csharp
if (someBooleanExpression) Response.Redirect(string.Format("~/someAddress/someFolders/anotherFolder/{0}/", CurrentShop.Domain));
```

*should be* 🡻

```csharp
if (someBooleanExpression)
   Response.Redirect(string.Format("~/someAddress/someFolders/anotherFolder/{0}/", CurrentShop.Domain));
```
*OR* 🡻

```csharp
if (someBooleanExpression)
{
   Response.Redirect(string.Format("~/someAddress/someFolders/anotherFolder/{0}/", CurrentShop.Domain));
}
```