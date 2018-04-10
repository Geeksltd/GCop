# GCop 535

> *"Use ToLocal() method instead, so you get control over it using via LocalTime.CurrentTimeZone."*
> 
> *"Use ToUniversal() method instead, so you get control over it using via LocalTime.CurrentTimeZone."*

## Rule description

...

## Example1

```csharp
DateTime? fromDate = LocalTime.Now;
var myTestVar = fromDate.Value.ToLocalTime();
```

*should be* 🡻

```csharp
DateTime? fromDate = LocalTime.Now;
var myTestVar = fromDate.Value.ToLocal();
```

## Example2

```csharp
DateTime? fromDate = LocalTime.Now;
var myTestVar = fromDate.Value.ToUniversalTime();
```

*should be* 🡻

```csharp
DateTime? fromDate = LocalTime.Now;
var myTestVar = fromDate.Value.ToUniversal();
```