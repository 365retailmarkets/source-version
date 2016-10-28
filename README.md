# SourceVersion.WebForms

SourceVersion is used to provide useful version information harvested from Source Control and then provided at runtime. The primary driver of this project is to make it simpler for developers and testers to accurately know what application build they are interacting with and whether the build is clean or not.

## Usage

Add SourceVersion to your WebForms application by doing the following.

1. Add a reference to [SourceVersion.WebForms]() package via NuGet. 
2. Build your project.
3. Run the project and navigate to `Version.ashx` in the browser.

```
{
  "localInfo": {
    "localPath": "C:\\Users\\fry\\Source\\MyProject",
    "serverPath": "$/The/Next/Best/Thing",
    "changeset": "1234",
    "change": "none",
    "type": "folder"
  },
  "serverInfo": {
    "serverPath": "$/The/Next/Best/Thing",
    "changeset": "1234",
    "deletionId": "0",
    "lock": "none",
    "lockOwner": "",
    "lastModified": "Tuesday, March 8, 2016 8:46:52 AM",
    "type": "folder\r"
  }
}
```

## How does it work
SourceVersion adds specific build targets and properties to the project file and an HTTP Handler to `Web.config` file 


## Known limitations

The `SourceVersion.WebForms` package only works for web applications that target .NET Framework 4.0 and above and that use integrated pipeline to host in IIS 7+. Currently the only source control implementation is TFS and the output contains specific details for TFS workspace. Plans are to support git and perhaps Subversion as well in later versions. Because of the way that the path is used to find TF.exe this solution will likely only work in it's current form with Visual Studio 2015.

## Security consideration

The information disclosed by the Version.ashx handler may be considered sensitive depending on the project. Be aware that by default there is nothing that prevents unauthenticated users from accessing this information.

## Contribute

To build open `/src/SourceVersion.sln` in Visual Studio and build the solution. Packaging and publishing to NuGet.org is currently handled per project such as with SourceVersion.WebForms by using the PowerShell script `/src/SourceVersion.WebForms/Package.ps1`.