# GCop 309

> *"It should be written as `f.FooId == foo.FooId`, Because the criteria will be faster and eliminate unnecessary database fetches"*

## Rule description

To find a special record we can compare the ID of objects instead of comparing all object properties. This way is faster and eliminate unnecessary database fetches.

## Example1

```csharp
var foo = Database.Get<Foo>(ID);
var res = bar.Where(f => f.Foo == foo);
```

*should be* 🡻

```csharp
var foo = Database.Get<Foo>(ID);
var res = bar.Where(f => f.FooId == foo.FooId);
```

## Example2

```csharp
var myList = new List<PasswordResetTicket>();
var res = myList.Where(rec => rec.User == User);
```

*should be* 🡻

```csharp
var myList = new List<PasswordResetTicket>();
var res = myList.Where(rec => rec.UserId == UserId);
```