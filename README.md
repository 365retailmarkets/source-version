# SourceVersion.WebForms

SourceVersion is used to provide useful version information harvested from Source Control and then provided at runtime. The primary driver of this project is to make it simpler for developers and testers to accurately know what application build they are interacting with and whether the build is clean or not.

## Usage

Add SourceVersion to your WebForms application by doing the following.

1. Add a reference to [SourceVersion.WebForms](https://www.nuget.org/packages/SourceVersion.WebForms/) package via NuGet. 
2. Remove any `AssemblyVersion`, `AssemblyFileVersion` or `AssemblyInformationalVersion` assembly attributes normally found in `AssemblyInfo.cs` file (AssemblyInfo.fs or AssemblyInfo.vb for F# or VB.NET projects).
3. Build your project.
4. Run the project and navigate to `Version.ashx` in the browser.

```
{
  "changeset": 1234,
  "serverPath": "$/The/Next/Best/Thing",
  "localPath": "C:\\Users\\fry\\Source\\MyProject",
  "machine": "BUILDSERVER-1234",
  "user": "BUILDSERVER-1234\\fry",
  "hasPendingChanges": true
}
```

Also notice that when looking at the version information for your assembly that the build portion of the version will be set automatically to the changeset number. Plans are to support additional information here similar to [GitVersion](https://github.com/GitTools/GitVersion).

## How does it work
SourceVersion adds specific build targets and properties to the project file and an HTTP Handler to `Web.config` file 


## Known limitations

The `SourceVersion.WebForms` package only works for web applications that target .NET Framework 4.5 and above and that use integrated pipeline to host in IIS 7+. Currently the only source control implementation is TFS and the output contains specific details for TFS workspace. Plans are to support git and perhaps Subversion as well in later versions.

## Security consideration

The information disclosed by the Version.ashx handler may be considered sensitive depending on the project. Be aware that by default there is nothing that prevents unauthenticated users from accessing this information.

## Contribute

To build open `/src/SourceVersion.sln` in Visual Studio and build the solution. Packaging and publishing to NuGet.org is currently handled per project such as with SourceVersion.WebForms by using the PowerShell script `/src/SourceVersion.WebForms/Package.ps1`.