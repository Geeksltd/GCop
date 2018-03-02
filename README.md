# GCop
GCop is a set up code analysis rules for improving C# code.


## Installing GCop in Visual Studio 2017
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
1. After adding GCop to a project, *build* that project in Visual Studio in the normal way.
2. 
When you add the nuget packages to a project, and then build that project, 
