# GCop

GCop is a set up code analysis rules for improving C# code.

## Getting started

### Installing GCop in Visual Studio 2017

#### Prerequisite

First of all make sure you have installed visual studio 2017 version 15.5.5 or a newer one.

#### Adding GCop to a project

1. Open the **Manage NuGet Packages...** window for your solution.
2. Next to the ⚙ icon, change the drop-down list to ***All***
3. If your project has a reference to M#, Zebble or Olive nuget packages, then install `GCop.All.Geeks`
4. Otherwise, select `GCop.All.Common`
5. Install it on all projects in your solution (or the ones that you need analysed)

> **Note :** You need to do this once for each project / solution.

### Running GCop

1. After adding GCop to a project, **build** that project in Visual Studio in the normal way.
2. Open the *Error List* window
   - Ensure *Errors*, *Warnings* and *Messages* tabs are all selected
   - Ensure the last drop down is selected as **Build + IntelliSense**
3. Make sure you see items in the *Error List* window whose Code starts with *GCop*.
   - If you don't see any GCop warnings, right click on each project and select **Properties** > **Code Analysis** and ensure **Enable Code Analysis on Build** is ticked.

### GCop Error
If GCop thorws error related to NETStandard, Please update your 'Microsoft.Net.Compilers' Package, if you are using 'Microsoft.CodeDom' please update that either. 

#### Disabling specific GCop rules

If you disagree with any of the GCop rules, or if you believe they are not applicable to your project, you can *Escape the Cop*, i.e. disable it, using any of the following methods.

#### Disabling entire project

Unfortunately Visual Studio doesn't allow you to disable analysis rules at the solution level. But you can disable them for every project in your solution.

1. Right click on the project and select **Properties**
2. Select the **Build** tab
3. Update the **Suppress warnings** field to include the warning codes you want to disable.
   - Separate multiple warning codes with a semicolon, e.g: `GCop316; GCop140; GCop179`

### Disabling one class

You can disable GCop for specific C# classes by adding the `[EscapeGCop]` attribute on top of it. You can optionally provide a reason description as well. This is particularly useful for auto-generated code.

> **Note :** For having **EscapeGCop**, you need to install **MSharp** or **Olive** from NuGet for each project or solution as well.


For example:

```csharp
[EscapeGCop("This class is auto generated.")]
public class MyClass
{
    ...
}
```

#### Disabling one method

You can disable GCop for specific methods by adding the `[EscapeGCop]` attribute on top of it. For example:

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

## Contributing

As this documentation is a kind of Open-Doc, contributions are welcome. We are currently focused on documenting GCop errors. Feel free to review, contribute and pull request to this documentation.
we'll review your PR as soon as we can and we will do the merge if everything was just fine. If there was any kind of issues, please post it in the [issues](https://github.com/Geeksltd/GCop/issues) section. Make sure that you follow our [writing styles](Rules/_Template.md).

## Authors

This project is maintained and supported by the GeeksLtd.

See also the list of [contributors](https://github.com/Geeksltd/GCop/contributors) who participated in this project.
