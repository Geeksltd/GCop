# GCop414

> *"Remove .ToString() as it's unnecessary."*


## Rule description
#### Null reference exception
The + concatenation ("arg0" + arg1) compiles to a call to String.Concat(object arg0, object arg1). The method concatenates arg0 and arg1 by calling the parameterless ToString() method of arg0 and arg1; it does not add any delimiters.
String.Empty is used in place of any null argument (From the .Net reference source):
```csharp
public static String Concat(Object arg0, Object arg1) {
        if (arg0==null) {
            arg0 = String.Empty; 
        }

        if (arg1==null) { 
            arg1 = String.Empty;
        } 
        return Concat(arg0.ToString(), arg1.ToString());
    }
```
This calls String.Concat(string, string):
```csharp
public static String Concat(String str0, String str1) { 
        if (IsNullOrEmpty(str0)) { 
            if (IsNullOrEmpty(str1)) {
                return String.Empty; 
            }
            return str1;
        }

        if (IsNullOrEmpty(str1)) {
            return str0; 
        } 

        int str0Length = str0.Length; 

        String result = FastAllocateString(str0Length + str1.Length);

        FillStringChecked(result, 0,        str0); 
        FillStringChecked(result, str0Length, str1);

        return result; 
    }
```
Whereas calling ("arg0" + arg1.ToString()) directly will avoid boxing and call the Concat(string, string) overload. 
 
Here the code ("arg0" + arg1) will not throw exception even if arg1 is a null reference. But "arg0" + arg1.ToString() will throw if arg1 is null, of course.  


#### String.ToString()
The ToString() method is found on Object. The implementation of Object.ToString() is to print the the fully qualified name of the object's type.  
```csharp
public virtual string ToString() {
    return this.GetType().ToString();
} 
```
The type String inherits from Object, so overrides this method to return itself.  
```csharp
public override string ToString() {
    return this;
}  
```
String.ToString() Returns this instance of String; no actual conversion is performed.
Because this method simply returns the current string unchanged, there is no need to call it.  

So the code "someText".ToString() has an unnecessary call to ToString.


## Example 1
**Violating code:**
```csharp
var result = "someText" + myObject.ToString() + " ...";
```
🡻

**Compliant code**
```csharp
var result = "someText" + myObject + " ...";
```
 
## Example 2
**Violating code:**
```csharp
public string SomeMethod() => "This year is " + DateTime.Today.Year.ToString();
```
🡻

**Compliant code**
```csharp
public string SomeMethod() => "This year is " + DateTime.Today.Year;
```
 
