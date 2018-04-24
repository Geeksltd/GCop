# GCop 134

> *"This method should not contain complex code, Instead call other focused methods to perform the complex logic"*

## Rule description

You should avoid writing too much logic in the event handler methods such as `OnValidating`, `OnSaving`, `OnSaved`, 'OnDeleting', etc. Instead, break the logic into one or more methods that are properly named, and just invoke them in your 
The first rule of functions is that they should be small. Functions should do one thing. They should do it well. They should do it only, so long methods are against OOP rules and reduce the code readability.

## Example

```csharp
public override void OnSaved(SaveEventArgs args)
{
    if (args.Mode == SaveMode.Insert)
    {
        Database.Save(new EmailQueueItem 
        {
           To = Config.Get("Order.Receiver.Email"),
           Html = true,
           Subject = "New order submitted",
           Body = @".....
                   .....
                   ....
                   ...."
        });        
    }
}        
```

*should be* 🡻

```csharp
public override void OnSaved(SaveEventArgs args)
{
    if (args.Mode == SaveMode.Insert)
    {
         SendNotificationEmail();
    }
}        

void SendNotificationEmail()
{
        Database.Save(new EmailQueueItem 
        {
           To = Config.Get("Order.Receiver.Email"),
           Html = true,
           Subject = "New order submitted",
           Body = @".....
                   .....
                   ....
                   ...."
        });        
}
```

