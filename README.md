# CS0757 Error Repro

To reproduce the issue, run the following command:

```text
> dotnet build /p:IncludeTelemetry=true
MSBuild version 17.8.0-preview-23401-01+b3989dc43 for .NET
  Determining projects to restore...
  Restored src\MyApplication\MyApplication.csproj (in 869 ms).
src\MyApplication\Microsoft.Extensions.Logging.Generators\Microsoft.Extensions.Logging.Generators.LoggerMessageGenerator\LoggerMessage.g.cs(15,40): error CS0757: A partial method may not have multiple implementing declarations [src\MyApplication\MyApplication.csproj]
src\MyApplication\Microsoft.Extensions.Logging.Generators\Microsoft.Extensions.Logging.Generators.LoggerMessageGenerator\LoggerMessage.g.cs(27,40): error CS0757: A partial method may not have multiple implementing declarations [src\MyApplication\MyApplication.csproj]
src\MyApplication\Microsoft.Extensions.Logging.Generators\Microsoft.Extensions.Logging.Generators.LoggerMessageGenerator\LoggerMessage.g.cs(39,40): error CS0757: A partial method may not have multiple implementing declarations [src\MyApplication\MyApplication.csproj]
src\MyApplication\Microsoft.Extensions.Logging.Generators\Microsoft.Extensions.Logging.Generators.LoggerMessageGenerator\LoggerMessage.g.cs(51,40): error CS0757: A partial method may not have multiple implementing declarations [src\MyApplication\MyApplication.csproj]
src\MyApplication\Microsoft.Extensions.Logging.Generators\Microsoft.Extensions.Logging.Generators.LoggerMessageGenerator\LoggerMessage.g.cs(63,40): error CS0757: A partial method may not have multiple implementing declarations [src\MyApplication\MyApplication.csproj]

Build FAILED.

src\MyApplication\Microsoft.Extensions.Logging.Generators\Microsoft.Extensions.Logging.Generators.LoggerMessageGenerator\LoggerMessage.g.cs(15,40): error CS0757: A partial method may not have multiple implementing declarations [src\MyApplication\MyApplication.csproj]
src\MyApplication\Microsoft.Extensions.Logging.Generators\Microsoft.Extensions.Logging.Generators.LoggerMessageGenerator\LoggerMessage.g.cs(27,40): error CS0757: A partial method may not have multiple implementing declarations [src\MyApplication\MyApplication.csproj]
src\MyApplication\Microsoft.Extensions.Logging.Generators\Microsoft.Extensions.Logging.Generators.LoggerMessageGenerator\LoggerMessage.g.cs(39,40): error CS0757: A partial method may not have multiple implementing declarations [src\MyApplication\MyApplication.csproj]
src\MyApplication\Microsoft.Extensions.Logging.Generators\Microsoft.Extensions.Logging.Generators.LoggerMessageGenerator\LoggerMessage.g.cs(51,40): error CS0757: A partial method may not have multiple implementing declarations [src\MyApplication\MyApplication.csproj]
src\MyApplication\Microsoft.Extensions.Logging.Generators\Microsoft.Extensions.Logging.Generators.LoggerMessageGenerator\LoggerMessage.g.cs(63,40): error CS0757: A partial method may not have multiple implementing declarations [src\MyApplication\MyApplication.csproj]
    0 Warning(s)
    5 Error(s)

Time Elapsed 00:00:02.65
```

Without the reference included, the application compiles:

```text
> dotnet build
MSBuild version 17.8.0-preview-23401-01+b3989dc43 for .NET
  Determining projects to restore...
  Restored src\MyApplication\MyApplication.csproj (in 189 ms).
  MyApplication -> src\MyApplication\bin\Debug\net8.0\MyApplication.dll

Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:02.02
```
