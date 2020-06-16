![build](https://github.com/shinolab/autd3sharp/workflows/build/badge.svg)
![Publish to NuGet](https://github.com/shinolab/autd3sharp/workflows/Publish%20to%20NuGet/badge.svg)
[![unitypackage](https://github.com/shinolab/autd3sharp/workflows/unitypackage/badge.svg)](https://github.com/shinolab/autd3sharp/releases)

# autd3sharp

[autd3 library](https://github.com/shinolab/autd3-library-software) wrapper for .Net Core 2.0+

version: 0.4.1

## Install

Please install using NuGet

https://www.nuget.org/packages/autd3sharp

If you are using .Net Framework, please place autd3capi.dll in the output directory by your self.

## Requirements

If you are using Windows, install [Npcap](https://nmap.org/npcap/) with WinPcap API-compatible mode (recomennded) or [WinPcap](https://www.winpcap.org/).

## Example

* Please refer to [example](./example)

```
cd example
dotnet run
```

* If you are using Linux/macOS, you may need to run as root (e.g. `sudo dotnet run`).

* On Ubuntu 20.04, you may need to specify runtime (e.g. `sudo dotnet run -r ubuntu-x64`).

## Unity

Please use unitypackage in [Release page](https://github.com/shinolab/autd3sharp/releases).

# Author

Shun Suzuki 2018-2020
