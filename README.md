![build](https://github.com/shinolab/autd3sharp/workflows/build/badge.svg)
![Publish to NuGet](https://github.com/shinolab/autd3sharp/workflows/Publish%20to%20NuGet/badge.svg)
[![unitypackage](https://github.com/shinolab/autd3sharp/workflows/unitypackage/badge.svg)](https://github.com/shinolab/autd3sharp/releases)

# autd3sharp

[autd3 library](https://github.com/shinolab/autd3-library-software) wrapper for .Net Standard 2.1

version: 1.7.1.1

## :hammer_and_wrench: Install

Please install using NuGet

https://www.nuget.org/packages/autd3sharp

## :ballot_box_with_check: Requirements

If you use Windows and `Link.SOEM`, install [Npcap](https://nmap.org/npcap/) with WinPcap API-compatible mode (recomennded) or [WinPcap](https://www.winpcap.org/).

## :beginner: Example

* Please refer to [example](./example)

```
cd example
dotnet run
```

* If you are using Linux/macOS, you may need to run as root (e.g. `sudo dotnet run`).
    * On Ubuntu 20.04, you may need to specify runtime (e.g. `sudo dotnet run -r ubuntu-x64`).

## :video_game: Unity

Please use unitypackage in [Release page](https://github.com/shinolab/autd3sharp/releases).

* After importing the package, please enable `Allow 'unsafe' code` in `Project Settings > Player`
    * Also, add `-nullable:enable` in `Additional Compiler Arguments` to suppress warnings

## :mortar_board: Citing

If you use this SDK in your research please consider to include the following citation in your publications:

* [S. Suzuki, S. Inoue, M. Fujiwara, Y. Makino and H. Shinoda, "AUTD3: Scalable Airborne Ultrasound Tactile Display," in IEEE Transactions on Haptics, doi: 10.1109/TOH.2021.3069976.](https://ieeexplore.ieee.org/document/9392322)
* S. Inoue, Y. Makino and H. Shinoda "Scalable Architecture for Airborne Ultrasound Tactile Display", Asia Haptics 2016

## :copyright: LICENSE

See [LICENSE](./LICENSE) and [NOTICE](./NOTICE).

# Author

Shun Suzuki 2018-2021
