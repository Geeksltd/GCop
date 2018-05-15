# GCop 320

> *"While \{IEnumerable\<T>} is being enumerated at run-time, saving of each of its items can potentially affect the source expression from which this IEnumerable object is define upon. To avoid unintended side effects, evaluate myobjects into an Array or List before passing it to Database.XXX."*

## Rule description

M# runs bulk saves in “Transaction”. Meaning, failure to save one records results rolling back all the saved instances of the current collection. Whenever an entity instance is saved or updated, it goes through a series of events, which are used to implement core business logic e.g. validation. Saving of each of its items can potentially affect the source expression from which this `IEnumerable` object is define upon. To avoid unintended side effects, evaluate myobjects into an Array or List before passing it to `Database.Save()` or `Database.Update()`.

## Example1

```csharp
Database.Save<Customer>(customerCollection);
```

*should be* 🡻

```csharp
Database.Save<Customer>(customerlist);
```

## Example2

```csharp
Database.Update<Customer>(customerCollection);
```

*should be* 🡻

```csharp
Database.Update<Customer>(customerlist);
```