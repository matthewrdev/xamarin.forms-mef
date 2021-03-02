# xamarin.forms-mef
Using MEF, Managed Extensibility Framework, for dependency injection in Xamarin.Forms.

## What Is MEF?

MEF, Managed Extensibility Framework. https://docs.microsoft.com/en-us/dotnet/framework/mef

 * Microsoft backed DI framework. It's used as the backing DI infrastrucutre in the Visual Studio family.
 * MEF is feature rich and is supported on all platforms.
 * Very easy to use (arguably simplier that other frameworks as it does not require a set of registrations).

## Using MEF

Exporting a class (Export attribute)
Exporting an interface implementation (Export(typeof()) .
Exporting a class against multiple interfaces (Export(typeof).

Creation policies (shared vs non-shared).

Declaring importing constructors.
Importing enumerable collections.

## Implementation Overview

TODO: Discuss how we can use MEF as out DI framework in Xamarin.Forms.

Note that this implementation is used by MFractor for both Visual Studio Mac and Windows plus so it well tested.

 * Resolver -> A service locator pattern for 
 * DeclareExportResolver -> An attribute that provides the ExportResolver implementation for the Resolver.
 * BaseExportResolver -> The base class for an `IExportResolver` that can resolve parts. When first used, the Resolver find the DeclareExportResolver attribute in the app domain and automatically creates an instance of the provided type.
 * AssemblyCompositionExportResolver -> An IExportResolver that can build a part catalog from several assemblies.

USage:

Declare an export or two

**MyService.cs**
```
[Export(typeof(IMyService))]
[PartCreationPolicy(CreationPolicy.Shared))] // Singleton instance
public class MyService : IMyService
{
}
```

**MainPage.cs**
```
[Export]
public class MainPage()
{
  [ImportingConstructor]
  public MainPage(IMyService myService, 
                  IUserDialogs userDialogs)
  {
    // ...
  }
```


```
MainPage = Resolver.Resolve<MainPage>();
```

## Summary

