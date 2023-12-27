# Build MetaLang

This is a guide on how to build the MetaLang compiler. There are two ways to build the project, using dotnet CLI or Visual Studio.

To build the MetaLang compiler, you will need to have the following tools installed on your machine:

## Prerequisites

Before you start, you need to have the following tools installed on your system:

- Git: a version control system that lets you clone the MetaLang repository. You can download it from https://git-scm.com/.
  
- .NET 5.0 SDK or newest: a software development kit that provides the dotnet CLI and the libraries needed to build .NET applications. You can download it from https://dotnet.microsoft.com/download/dotnet/5.0.
  
- Visual Studio 2019 (optional): an integrated development environment that provides a graphical interface for building .NET applications. You can download it from https://visualstudio.microsoft.com/vs/.

## Clone the repository

The first step is to clone the MetaLang repository from GitHub. You can do this by opening a terminal and typing the following command:

```bash
git clone https://github.com/MetaLang/MetaLang.git
```

This will create a folder called MetaLang in your current directory, containing the source code of the compiler.


## Building

There are two ways to build the project, using dotnet CLI or Visual Studio.

Using dotnet CLI (Any):

- Make sure you have the latest version of .NET SDK installed on your machine. You can check the version by running `dotnet --version` in a terminal.
- Navigate to the root folder of the project, where the `MetaLang.sln` file is located.
- Run `./release.sh` to build the project in release mode and output the binaries to the `bin` folder.

Using Visual Studio (Windows only):

- Make sure you have Visual Studio 2019 or later installed on your machine, with the F# [language support](https://learn.microsoft.com/en-us/dotnet/fsharp/get-started/get-started-visual-studio) enabled.
- Open the `MetaLang.sln` file in Visual Studio.
- Select `Release` as the configuration and `Any CPU` as the platform from the toolbar.
- Build the solution by pressing `Ctrl+Shift+B` or selecting `Build -> Build Solution` from the menu.