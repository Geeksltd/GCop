# GCop117

> *"The meaning of 'true/false' is not obvious. Specify the parameter's name explicitly."*


## Rule description
Named parameters are a wonderful way to make program self-documented. Using true or false for Boolean arguments is not meaningfull enough, so try to specify the parameters name that the reader will immediately understand its purpose.
## Example 1
```csharp
Response.Redirect("URL Address", true);
```
*should be* 🡻

```csharp
Response.Redirect("URL Address", endResponse:true);
```

## Example 2
```csharp
protected void ReFetch(object sender, EventArgs e) => FetchProducts(true);
```
*should be* 🡻

```csharp
protected void ReFetch(object sender, EventArgs e) => FetchProducts(Refetch: true);
```
