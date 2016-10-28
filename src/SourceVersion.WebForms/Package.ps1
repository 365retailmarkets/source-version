mkdir bin/packages/ -ErrorAction Ignore
nuget pack .\SourceVersion.csproj -outputdirectory bin/packages
nuget init bin/packages/ c:\deleteme\nuget-packages