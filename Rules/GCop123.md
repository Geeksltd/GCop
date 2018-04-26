# GCop 123

> *"Transaction is created but the method.Complete() is never called."*

## Rule description

When your application completes all the work it wants to perform in a transaction, you should call the Complete method only once to inform the transaction manager that it is acceptable to commit the transaction. It is very good practice to put the call to Complete as the last statement in the using block.

Failing to call this method aborts the transaction, because the transaction manager interprets this as a system failure, or equivalent to an exception thrown within the scope of the transaction. However, calling this method does not guarantee that the transaction wil be committed. It is merely a way of informing the transaction manager of your status. After calling the Complete method, you can no longer access the ambient transaction by using the Current property, and attempting to do so will result in an exception being thrown.

## Example

```csharp
using (var myScope = new Database().CreateTransactionScope())
{
    try
    {
        //...Code that does not call myScop.Complete()
    }
    catch {...}
}
```

*should be* 🡻

```csharp
using (var myScope = new Database().CreateTransactionScope())
{
    try
    {
        ...
        myScop.Complete();
    }
    catch {...}
}
```