# GCop 205

>*"Interfaces must start with the letter 'I'."* 
>
>*"Interfaces must start with a capital 'I'."*
> 
>*"Interfaces names must be Pascal Case."*

## Rule description

Prefix your C# interfaces with *'I'* because that is the .NET convention used and advocated by Microsoft. Also naming convention easily tells you a lot about your object without you having to investigate further. You can easily see what you are inheriting vs what you are implementing.

## Example 1

```csharp
public interface Sample
{
    ...
}
```

*should be* 🡻

```csharp
public interface ISample
{
    ...
}
```

## Example 2

```csharp
public interface iSample
{
    ...
}
```

*should be* 🡻

```csharp
public interface ISample
{
    ...
}
```

## Example 3

```csharp
public interface Isample
{
    ...
}
```

*should be* 🡻

```csharp
public interface ISample
{
    ...
}
```