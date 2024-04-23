# Introduction

This solution contains a Code Generator named Modeller, and it's associated Global tool.  See below on how to install and use the tool.

The generator uses templates, basically a DLL that implements a specific interface `IGenerator` to create output.  The way the output is created
is up to the imagination of the developer creating the output.  The examples used here use a string builder and file components to generate files
in a specified structure.  But there is nothing stopping the developer from using a database or any other method to generate the output, i.e.  Liquid templates.

The modeller currently supports 2 json structures to model the data, the first is the OpenApi v3 json schema,
the second is a custom schema. The custom schema is described in another README.file.

Generally the OpenApi v3 schema is used to model the public data structures and api definitions, whilst the custom schema is used to model the data store and backend behaviours.
More on that later.

# Getting Started

## Installation

`dotnet tool install -g modeller.tool`

If you are using the Credentials Manager to access the ADO repo:
`dotnet tool install -g modeller.tool --interactive`

If you are using the Credentials Manager to access the ADO repo and have the .NET 6.0+ framework installed:
`dotnet tool install -g modeller.tool --interactive --verbosity minimal`

Check out the [Microsoft documentation](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools) for full details.

# CLI
``` cmd
USAGE:
    entity [OPTIONS] <COMMAND>

EXAMPLES:
    entity convert c:\jjs\set\MyProject c:\temp\myProjectTemplate MyProject
    entity build ApiSolution CaseDomain --definitions ../src/Modeller.Definitions --templates ../src/Modeller.Templates --output c:/playschool
    entity settings --filepath c:/playschool/Settings.json
    entity validate CaseDomain --definitions ../src/Modeller.Definitions
    entity definition NewBranch --definitions ../src/Modeller.Definitions

OPTIONS:
    -h, --help       Prints help information
    -v, --version    Prints version information

COMMANDS:
    list                             List the available definitions or templates
    convert                          Convert a folder structure to a template project
    build <template> <definition>    Build a definition using a specific template
    settings                         Manage the settings for the tool
    validate <definition>            Validate a definition
    definition <definition>          Write the definition to a directory
```

## Build

Usage:
`model build [options] <Generator> <SourceModel>`

Arguments:
-  `Generator`                         The generator to use.
-  `SourceModel`                       The filename for the source model to use during code generation.

Options:
-  `-l|--local-folder <LOCAL_FOLDER>`  Path to the locally cached generators
-  `-m|--model <MODEL>`                Model name. If included then the output will be limited to the specified model
-  `-o|--output <OUTPUT>`              Output folder
-  `--overwrite`                       Overwrite
-  `-t|--target <TARGET>`              Target framework. Defaults to netstandard2.0
-  `-s|--settings <SETTINGS>`          Settings file to use when generating code. Settings in the file will override
                                    arguments on the command line
-  `--version <VERSION>`               Specific version to use for the generator
-  `--verbose`                         Verbose
-  `-?|-h|--help`                      Show help information

## Generators

Usage:
`model generators [options] [command]`

Options:
  `-?|-h|--help  Show help information`

Commands:
-  `list`          List available generators
-  `update`        Update generators

Run `'generators [command] --help'` for more information about a command.

NB:
The generators are not initially in the tool, they must be referenced either by adding them to the tool or for each usage.

The generators are found in the Modeller source code under the Modeller.Generators folder.

e.g.
`model build --local-folder ../../../modeller/modeller.generators --output d:\temp WebApiProject .\Events.json`
