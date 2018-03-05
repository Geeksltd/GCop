# GCop201

> *"Use camelCasing when declaring local variables / a parameter."*


## Rule description

Use the following three conventions for capitalizing identifiers.

##### Pascal case

The first letter in the identifier and the first letter of each subsequent concatenated word are capitalized. You can use Pascal case for identifiers of three or more characters. For example:
```
BackColor
```
##### Camel case

The first letter of an identifier is lowercase and the first letter of each subsequent concatenated word is capitalized. For example:
```
backColor
```
##### Uppercase

All letters in the identifier are capitalized. Use this convention only for identifiers that consist of two or fewer letters. For example:
```
System.IO
System.Web.UI
```

## Example 1

For these identifiers we should use CamelCasing :


|Identifier|Violating code|Compliant code|
|---|---|---|
| Protected instance field|RedValue|redValue <br> **Note**   Rarely used. A property is preferable to using a protected instance field.|
| Parameter|Int|int|
 
