# <img align="left" src="http://i1.img.969g.com/dota2/imgx2013/10/18/258_111015_8c209.gif" width="180px"><div style="margin-left: 100px; padding-top: 12px; font-size: 70px; padding-top: 25px; color: #283593">DotaScraper</div>
Tool that scrapes Dotabuff to retreive information about Dota.
<hr>

<br>

## How To Use

Windows:
1) Download [Visual Studio](https://www.visualstudio.com/) if an IDE is needed.
2) Download and Install the [Nuget Package Manager](https://www.nuget.org/) for Visual Studio.
3) If you dont want to install Visual Studio, then do ahead and download the [latest .NET Framework](https://www.microsoft.com/en-us/download/details.aspx?id=30653) for running the program.
4) If you downloaded Visual Studio then you can configure your required packages, build and run your program through the IDE otherwise follow the cmd process:
```bash
# Open a command prompt and change into the solution directory.
> cd "C:\Github\DotaScraper"

# Reference your .NET framework directory and use the executables to run the program
> C:\Windows\Microsoft.NET\Framework\v4.5.xxxxx\csc.exe Program.cs
or
> C:\Windows\Microsoft.NET\Framework\v4.5.xxxxx\msbuild.exe MetadataScraper.sln
or
> C:\Windows\Microsoft.NET\Framework\v4.5.xxxxx\msbuild.exe MetadataScraper.csproj
```
