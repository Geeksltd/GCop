# GCop 182

> *"For a `AtionType` Web Api, specify the parameter binding source explicitly. Did you mean to add [FromBody]?"*

## Rule description

If you do not decorate the Web API method parameter with `[FromBody]` attribute and you send the model (raw javascript object, not in JSON format) without specifying the `ContentType` property value, model binding will work for the flat properties on the model, not the properties where the type is complex or another type.

Also To force Web API to read a simple type from the request body, we should add the `[FromBody]` attribute to the parameter.


## Example

```csharp
[HttpPost]
public void Foo(Bar bar)
{
    ...
}
```

*should be* 🡻

```csharp
[HttpPost]
public void Foo([FromBody]Bar bar)
{
    ...
}
```
