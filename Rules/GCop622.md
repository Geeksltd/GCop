# GCop622

> *"Reverse your IF condition and return. Then move the nested statements to after the IF."*


## Rule description
To avoid unnecessary nesting, it's better to get out of the method immediately.

## Example 1
```csharp
void SomeMethod()
{
   if (someCondition)
   {
       ...
       ...
       ...
       ...
       ...
   }
}
```
*should be* 🡻

```csharp
void SomeMethod()
{
    if (!someCondition) return;
   
    ...
    ...
    ...
    ...
    ...   
}
```
