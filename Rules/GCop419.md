# GCop 419

> *"This statement is too long and hard to read. Press Enter at logical breaking points to split it into multiple lines."*

## Rule description

Long statements reduce code readability. 
Break logical operations to align it in a way that shows which operands are related to each other via the operator.

Helpful rules:

* New line after an open paren.
* Line breaks after commas.
* Indent the inner method calls.
* Line up parameters to a method that are on new lines.

## Example 1

```csharp
mykeyboard.AddButton(new InlineKeyboardButton(CustomWork.ReturnPreviousStep, "$" + Cover.CoverCallback.BackAdvertiseAreas + "," + CurrentButtonState.FK_CurrentBtnId), width + 1 == 2 ? ++height : height, width = width + 1 == 2 ? 0 : ++width);
```

*should be* 🡻

```csharp
mykeyboard.AddButton(
    new InlineKeyboardButton(
        CustomWork.ReturnPreviousStep, 
        "$" + Cover.CoverCallback.BackAdvertiseAreas + "," + CurrentButtonState.FK_CurrentBtnId),
        width + 1 == 2 ? ++height : height,
        width = width + 1 == 2 ? 0 : ++width);
```

## Example 2

```csharp
shippedItems.AddRange(OrderItem.Fetch(market: this.MARKET, shipConfirmState: ORDERITEMSHIPCONFIRMSTATE.NONE, orderPlacedAfter: serverTime.AddDays(-7), orderPlacedBefore: serverTime.AddHours(-85)));
```

*should be* 🡻

```csharp
shippedItems.AddRange(
    OrderItem.Fetch(market: this.MARKET,
                    shipConfirmState: ORDERITEMSHIPCONFIRMSTATE.NONE,
                    orderPlacedAfter: serverTime.AddDays(-7),
                    orderPlacedBefore: serverTime.AddHours(-85)));
```