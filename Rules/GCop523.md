# GCop 523

> *"Database search is case insensitive. Simply use `{LambdaIdentifier}.{Property} == yourValue` for a faster execution."*

## Rule description

Science the M# database search is case insensitive, there is no need to use string comparer objects which cause lower execution. You can simply use `==` operator instead.

## Example1

```csharp
Database.GetList<Customer>(s=>s.LastName.Equals("SomeValue", StringComparison.OrdinalIgnoreCase));
```

*should be* 🡻

```csharp
Database.GetList<Customer>(s=>s.LastName == "SomeValue");
```

## Example2

```csharp
Database.GetList<Customer>(s=>s.LastName.ToLower() == "SomeValue");
```

*should be* 🡻

```csharp
Database.GetList<Customer>(s=>s.LastName == "SomeValue");
```