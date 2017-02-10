mkdir bin/packages/ -ErrorAction Ignore
nuget pack .\SourceVersion.WebForms.csproj -outputdirectory bin/packages -includereferencedprojects
nuget init bin/packages/ c:\deleteme\nuget-packages
#nuget push SourceVersion.WebForms.1.0.0.41879.nupkg $env:NUGETAPIKEY -Source https://www.nuget.org/api/v2/package