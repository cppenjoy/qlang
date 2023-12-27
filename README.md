# MetaLang

`MetaLang` is a *high-level* programming language that focuses on *supercompilation*. 

Supercompilation is an optimization technique that allows you to transform programs into more efficient versions of yourself. MetaLang is a toy language, study its source code if you want to understand how supercompilers work.

## Compiler and Language Status

### Color Table
| Color | Means |
|-|-|
| 🟢 | It's already done |
| 🟡 | Partially finished |
| ⚪ | Planned, It is planned to start doing after the almost finished stage |
| ⚫ | Not Ready, will be done in the future |
| 🟠 | Thinking About It, Maybe it will, maybe not |
| 🟣 | Refused, and will not even be considered |

### Compiler Status

| Stage | Status | Color Status
|-|-|-|
| CLI | Ready | 🟢 |
| Lexer | Ready | 🟢 |
| Parser | Partly | 🟡 |
| Semantic Analyzer | Partly | 🟡 
| Intermediate Representation | Planned | ⚪ |
| Optimizations | Not Ready | ⚫ |
| Supercompilation | Planned | ⚪ |
| CodeGen to x86 | Planned | ⚪ |
| CodeGen to Wasm | Thinking About It, Maybe it will, maybe not | 🟠 |
| CodeGen to x86-x64 | Thinking About It | 🟠 |
| CodeGen to ARM | Thinking About It | 🟠 |
| Bootstrapping | Thinking About It | 🟠 |


### Language Status

| Stage | Status | Color Status
|-|-|-|
| Grammar | Ready | 🟢 |
| Type System Rules | Ready | 🟢 |
| API For Code Analysis | Ready | 🟢 |
| Documentation | Planned | ⚫ |
| Playground site for MetaLang(Include Online Code Editor, Compilation, Decompilation)  |  Not Ready | 🟠 |
| Standart Library (with IO Module for Linux and Windows) | Thinking About It | 🟠 |
| OOP | Thinking About It | 🟠 |
| Reflection | Thinking About It | 🟠 |
| Extension for Vim and VS Code | Thinking About It | 🟠 |
| Powerful IDE | Thinking About It | 🟠 |
| Debugger | Denied | 🟣 |
| Package Manager | Denied | 🟣 |
| Change license from MIT to GNU GPL | Denied | 🟣 |

If you want to contribute to the development of the project, then scroll below or open contributing.md

## Small code examples

Here are some code examples on MetaLang that demonstrate its main features and syntax:

```cs
// Declaring an alias for a type
using args = array of string

// Aliases for C-Like types
using char = int8
using short = int16
using int = int32
using long = int64

// Extern linking example
extern printf(string, ...)

// Global variable (without type annotation, by default is int32)
let variableWithoutTypeAnnotation = 0

// Global variable (with type annotation)
let variableWithTypeAnnotation: int = 0 

// Lexical scope demonstration
{
    let variableWithoutTypeAnnotation = 0
    let variableWithTypeAnnotation: int = 0 

    // Nested scope demonstration
    {
        let variableWithoutTypeAnnotation = 0
        let variableWithTypeAnnotation: int = 0
    }
}
// Entry Point
fn main(int argc, args argv) int
{
    // Local variable(syntax same as declaring a global variable)
    let someVariable = 0

    // Declaring global variable in local scope
    global someVariable = 0

    // One line comment (C-Style)

    /*
        Multi line comment
    */

    for i in range(1, argc) 
    {
        if (argv[i] == "-h")
        {
            print "Help information: ...."
            return 0
        }

        print "Undefined arg"
    }
}

// Extended examples can be found in the examples folder
```
# Contribute

If you want to contribute to the development of MetaLang, you can follow these steps:

- Make a fork of this repository on GitHub
- Clone your fork to your computer using the `git clone` command https://github.com/Lofinaf/MetaLang/MetaLang.git`
- Create a new branch for your changes using the command `git checkout -b new_branch`
- Make your changes to the code and add them to the index using the `git add command.`
- Commit your changes using the command `git commit -m "your message"`
- Push your branch to GitHub using the `git push origin new_branch` command
- Create a pull request on GitHub and wait for the review

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
- Run `dotnet build --no-self-contained -c "Release" -o bin` to build the project in release mode and output the binaries to the `bin` folder.

Using Visual Studio (Windows only):

- Make sure you have Visual Studio 2019 or later installed on your machine, with the F# [language support](https://learn.microsoft.com/en-us/dotnet/fsharp/get-started/get-started-visual-studio) enabled.
- Open the `MetaLang.sln` file in Visual Studio.
- Select `Release` as the configuration and `Any CPU` as the platform from the toolbar.
- Build the solution by pressing `Ctrl+Shift+B` or selecting `Build -> Build Solution` from the menu.