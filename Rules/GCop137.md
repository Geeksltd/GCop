# GCop 137

> *"Avoid locking on a Type or on the current object instance."*

## Rule description

The `lock` keyword ensures that one thread does not enter a critical section of code while another thread is in the critical section. If another thread tries to enter a locked code, it will wait, block, until the object is released.

`lock(this)` can be problematic if the instance can be accessed publicly, because code beyond your control may lock on the object as well. This could create deadlock situations where two or more threads wait for the release of the same object. 

A private field is usually a better option as the compiler will enforce access restrictions to it, and it will encapsulate the locking mechanism. 

## Example

```csharp
public void Foo()
{
    if (condition)
    {
        lock (this)
        {
            ...
        }
    }
}
```

*should be* 🡻

```csharp
object syncLock = new Object(); 
 
public void Foo()
{
    if (condition)
    {
        lock (syncLock)
        {
            ...
        }
    }
}
```