# GCop
GCop is a set up code analysis rules for improving C# code.


## Installing GCop in Visual Studio 2017
First of all make sure you have installed visual studio 2017 version 15.5.5 or a newer one.

*You need to do this only once.*

1. Open any solution in Visual Studio
2. Right click on the solution, and select **Manage Nuget Packages...**
3. At the top right corner of the window, click on the ⚙ icon.
4. Click the ➕ icon
   - Set Name to *GCop*
   - Set source to *http://nuget.gcop.co/nuget*
   - Click OK
   
## Adding GCop to a project
*You need to do this once for each project / solution.*

1. Open the **Manage Nuget Packages...** window for your solution.
1. Next to the ⚙ icon, change the dropdown list to ***All***
2. Under the *Browse* tab, search for *"Gcop"* and select **Gcop.All**
3. Install it on all projects in your solution (or the ones that you need analysed)


## Running GCop
1. After adding GCop to a project, **build** that project in Visual Studio in the normal way.
2. Open the *Error List* window
   - Ensure *Errors*, *Warnings* and *Messages* tabs are all selected
   - Ensure the last drop down is selected as **Build + IntelliSense**
3. Make sure you see items in the *Error List* window whose Code starts with *GCop*.
   - If you don't see any GCop warnings, right click on each project and select **Properties** > **Code Analysis** and ensure **Run Code Analysis on Build** is ticked.

* For getting ride of Microsoft.CodeAnalysis warnings, You have to update your codeDOM.
* For getting ride of NETStandard warning , you need to install the NETStandard.Library NuGet into the website project and then add the netstandard.dll as an anlyzer.
* For adding a dll as analyzer you can right click on the website project , go to Add -> Analyzer -> then find the netstandrd all in the installed Package folder.

---

## Disabling specific GCop rules
If you disagree with any of the GCop rules, or if you believe they are not applicable to your project, you can *Escape the Cop*, i.e. disable it, using any of the following methods.

### Entire project?
Unfortunately Visual Studio doesn't allow you to disable analysis rules at the solution level. But you can disable them for every project in your solution.

1. Right click on the project and select **Properties**
2. Select the **Build** tab
3. Update the **Suppress warnings** field to include the warning codes you want to disable. 
   - Seperate multiple warning codes with a semicolon, e.g: *GCop316; GCop140; GCop179*
   
### One class?
You can disable GCop for specific C# classes by adding the *[EscapeGCop]* attribute on top of it. You can optionally provide a reason description as well. This is particularly useful for auto-generated code.

For example:
```csharp
[EscapeGCop("This class is auto generated.")]
public class MyClass
{
    ...
}
```

### One method?
You can disable GCop for specific methods by adding the *[EscapeGCop]* attribute on top of it. For example:
```csharp
  ...
  [EscapeGCop("It's not applicable because ....")]
  public void SomeMethod()
  {
       ...
  }
  ...
```

You should use it in the following scenarios:
- In cases GCop is giving you a false alarm
- Where a rule is not applicable in a particular scenario in your opinion.
