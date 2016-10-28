mkdir bin/packages/ -ErrorAction Ignore
nuget pack .\SourceVersion.WebForms.csproj -outputdirectory bin/packages
nuget init bin/packages/ c:\deleteme\nuget-packages