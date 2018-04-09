# GCop 117

> *"The meaning of **true / false** is not obvious. Specify the parameter's name explicitly."*

## Rule description

Using true or false for `boolean` arguments is not meaningful enough. Instead, specify the parameters name that the reader will immediately understand its purpose.

## Example 1

```csharp
Response.Redirect("URL Address", true);
```

*should be* 🡻

```csharp
Response.Redirect("URL Address", endResponse: true);
```

## Example 2

```csharp
protected void ReFetch(object sender, EventArgs e) => FetchProducts(true);
```

*should be* 🡻

```csharp
protected void ReFetch(object sender, EventArgs e) => FetchProducts(refresh: true);
```