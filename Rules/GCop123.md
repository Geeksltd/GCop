# GCop 123

> *"Transaction is created but the method.Complete() is never called."*

## Rule description

When your application completes all the work it wants to perform in a transaction, you should call the `.Complete()` method only once to inform the transaction manager that it is acceptable to commit the transaction. It is very good practice to put the call to `.Complete()` as the last statement in the using block.

Failing to call this method **aborts the transaction**, because the transaction manager interprets this as a system failure, or equivalent to an exception thrown within the scope of the transaction. 

*Note: Calling `.Complete()` does not guarantee that the transaction wil be committed. It is merely a way of informing the transaction manager of your status. For example if there is a parent transaction scope open, the ultimate committing will be done by that.*

## Example

```csharp
using (var scope = Database.CreateTransactionScope())
{
      MyDataOperation1();
      MyDataOperation2();      
}
```

*should be* 🡻

```csharp
using (var scope = Database.CreateTransactionScope())
{
      MyDataOperation1();
      MyDataOperation2();
      scope.Complete();
}
```
