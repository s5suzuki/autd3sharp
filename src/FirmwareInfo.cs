/*
 * File: FirmwareInfo.cs
 * Project: src
 * Created Date: 28/04/2021
 * Author: Shun Suzuki
 * -----
 * Last Modified: 23/05/2021
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2021 Hapis Lab. All rights reserved.
 * 
 */

using System;

namespace AUTD3Sharp
{
    public readonly struct FirmwareInfo : IEquatable<FirmwareInfo>
    {
        public string CpuVersion { get; }
        public string FpgaVersion { get; }

        internal FirmwareInfo(string cpu, string fpga)
        {
            CpuVersion = cpu;
            FpgaVersion = fpga;
        }

        public override string ToString() => $"CPU: {CpuVersion}, FPGA: {FpgaVersion}";
        public bool Equals(FirmwareInfo other) => CpuVersion.Equals(other.CpuVersion) && FpgaVersion.Equals(other.FpgaVersion);
        public static bool operator ==(FirmwareInfo left, FirmwareInfo right) => left.Equals(right);
        public static bool operator !=(FirmwareInfo left, FirmwareInfo right) => !left.Equals(right);
        public override bool Equals(object? obj) => obj is FirmwareInfo info && Equals(info);
        public override int GetHashCode() => CpuVersion.GetHashCode() ^ FpgaVersion.GetHashCode();
    }
}
