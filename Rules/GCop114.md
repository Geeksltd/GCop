# GCop 114

> *"DateTime.Now is not TDD friendly. Use LocalTime.Now instead."*

## Rule description

Sanity/Pangolin have commands to inject the current system date and time to enable testing time dependent processes. For example ***assume date 22/01/2021***. The underlying mechanism will use `LocalTime` provided features.

Fo your code to work correctly in the test running mode, instead of `DateTime.Now` or `DateTime.Today` you should use `LocalTime.Now` or `LocalTime.Today` to make sure injected date/time commands are respected.

## Example

```csharp
var endDate = DateTime.Today.AddDays(1);
```

*should be* 🡻

```csharp
var endDate = LocalTime.Now.AddDays(1);
```