# BlockCounterCLI

Project to count the total of blocks in a given minecraft world. 

## Installation

### Windows

Download and extract the BlockCounterCLI-win64.zip file and run BlockCounterCLI.exe. No additional libraries are needed.

## Build

To create a new build from source, the dotnet sdk has to be installed.

### Windows

A new build is created by running
```bash
dotnet publish -r win-x64 --self-contained
```
in the projects root directory.

To create a zip file, navigate to 
```bash
<project_folder>/bin/Release/net8.0/win-x64/
```
and compress the contents of the publish folder to a new zip file.
