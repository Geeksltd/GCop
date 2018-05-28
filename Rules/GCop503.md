# GCop 503

> *"Don't set one document to another without calling clone."*

## Rule description

Always `Clone` the entity instance before changing and then call `Database.Save` or `Database.Update`. The reason is that you are changing the object that is already referenced on data cache so If `Save` action fails the object on cache would be different than the one in database. (Memory Cache is not transactional).

## Example
> See https://github.com/Geeksltd/GCop/issues/163
```csharp
var result = Database.Update(applicant, x => x.MyFile = file);
```

*should be* 🡻

```csharp
var file = MyFile.Clone();
var result = Database.Update(applicant, x => x.MyFile = file);
```