# How to build

These instructions are *only* for building from the command line, which includes compilation, test execution and packaging. This is the simplest way to build.
It also replicates the build on the Continuous Integration build server and is the best indicator of whether a pull request will build.

You can also build the solution using Visual Studio 2022 17.0 or later, but this doesn't provide the same assurances as the command line build.

At the time of writing the full build (including all target frameworks) can only run on Windows.

[Partial builds](#building-only-a-subset-of-the-supported-target-frameworks) are supported on Linux (Ubuntu 18.04). This might also run on macOS, but hasn't been tested.

## Prerequisites

The build requires that a few pieces of software be installed on the host computer. We're somewhat aggressive about adoptiong new language features and the like, so rather than specifying exactly which versions are required, we'll tend toward "latest" or "at least" forms of guidance. If it seems you have an incompatible version of the software, prefer to upgrade rather than downgrade.

### On Windows

The default [build profile](#building-only-a-subset-of-the-supported-target-frameworks) on Windows builds FakeItEasy for .NET Framework 4.5, .NET Standard 2.0, .NET Standard 2.1 and .NET 5.0, and runs the tests on .NET Framework 4.6.1, .NET Core 2.1, .NET Core 3.1 and .NET 5.0.

Ensure that the following are installed:

1. The .NET Core 2.1 and 3.1 runtimes

2. The .NET 5.0 runtime

3. The .NET Framework 4.6.1 or higher

4. An up-to-date version of the .NET 6.0 SDK (currently this means 6.0.100 or later)

You might not need everything to run a [partial build](#building-only-a-subset-of-the-supported-target-frameworks).

In order to build from Visual Studio, you will also need:

1. Visual Studio 2022 (17.0 or later)
2. The .NET Framework 4.5 targeting pack. Unfortunately, this targeting pack is no longer available for download from Microsoft. As a workaround, you can get the necessary files from the `Microsoft.NETFramework.ReferenceAssemblies.net45` package:
   1. Download the [Microsoft.NETFramework.ReferenceAssemblies.net45](https://www.nuget.org/packages/Microsoft.NETFramework.ReferenceAssemblies.net45/) package from NuGet
   2. Open it as a ZIP
   3. Extract the contents of `build\.NETFramework\v4.5` to the `C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5` directory

### On Linux

The default [build profile](#building-only-a-subset-of-the-supported-target-frameworks) on Linux builds FakeItEasy for .NET Standard 2.0, .NET Standard 2.1 and .NET 5.0, and runs the tests on .NET Core 2.1, .NET Core 3.1 and .NET 5.0 (the .NET Framework isn't supported on Linux).

Ensure the following are installed:

1. The .NET Core 2.1 and 3.1 runtimes

2. The .NET 5.0 runtime

3. An up-to-date version of the .NET 6.0 SDK (currently this means 6.0.100 or later)

## Building

Using a command prompt, navigate to your clone root folder and execute:

- `build.cmd` on Windows
- `./build.sh` on Linux

This executes the default build targets to produce .NET Standard and/or .NET Framework artifacts, depending on the selected [build profile](#building-only-a-subset-of-the-supported-target-frameworks).

After the build has completed, the build artifacts will be located in `artifacts`.

## Extras

### Running specific build tasks

`build.cmd` wraps a [Bullseye](https://github.com/adamralph/bullseye) targets project, so you can use all the usual command line arguments that you would use with Bullseye, e.g.:

* View the full list of build targets:

    `build.cmd -T`

* Run a specific target:

    `build.cmd spec`

* Run multiple targets:

    `build.cmd spec pack`

* Run a target without running its dependencies (might fail if the dependencies
  haven't been previously built):

    `build.cmd -s spec`

* View the full list of options:

    `build.cmd -?`

(On Linux, just replace `build.cmd` with `./build.sh`)

### Building only a subset of the supported target frameworks

FakeItEasy targets multiple versions of .NET (.NET Framework 4.5, .NET
Standard 2.0 and 2.1, .NET 5.0), and the tests also run on multiple frameworks (.NET
Framework 4.6.1, .NET Core 2.1 and 3.1, .NET 5.0). A consequence is that a full
build can take a significant amount of time. When working on the code, you might
want a faster feedback loop. To this end, FakeItEasy's build infrastructure has
the concept of "build profiles", which makes it possible to build only a subset
of all the target frameworks supported by FakeItEasy. The following profiles are
available:

* `full`: the default profile, builds all supported target frameworks supported
  on the current platform
* `net45`: builds only the .NET Framework 4.5 target framework
* `netcore2.1`: builds only .NET Core 2.1 / .NET Standard 2.0 target frameworks
* `netcore3.1`: builds only .NET Core 3.1 / .NET Standard 2.1 target frameworks
* `net5.0`: builds only the .NET 5.0 target framework

In order to select a profile, create a `FakeItEasy.user.props` file at the root
of the repository by running

```
build.cmd initialize-user-properties
```

Thereafter
you can switch profiles by replacing the contents of the `BuildProfile` element
either by editing the file by hand or via a build target (which will actually
create the file if it doesn't already exist). For example:

```
build.cmd use-profile-net5.0
```

Note that Visual Studio will not reflect a change of build profile until you
reload the solution. The command line build will reflect the change immediately.
