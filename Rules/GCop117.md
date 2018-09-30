# GCop 117

> *"The meaning of `true / false` is not obvious. Specify the parameter's name explicitly."*

## Rule description

Using `true` or `false` for `bool` arguments is not meaningful enough. Instead, specify the parameters name that the reader will immediately understand its purpose.

## Example 1

```csharp
Response.Redirect("Address", true);
```

*should be* 🡻

```csharp
Response.Redirect("Address", endResponse: true);
```

## Example 2

```csharp
protected void Foo(object sender, EventArgs e) => Bar("something", true);
```

*should be* 🡻

```csharp
protected void Foo(object sender, EventArgs e) => Bar("something", refresh: true);
```
