/*
 * File: Configuration.cs
 * Project: src
 * Created Date: 28/04/2021
 * Author: Shun Suzuki
 * -----
 * Last Modified: 28/04/2021
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2021 Hapis Lab. All rights reserved.
 * 
 */

namespace AUTD3Sharp
{
    public enum ModSamplingFreq
    {
        Smpl125Hz = 125,
        Smpl250Hz = 250,
        Smpl500Hz = 500,
        Smpl1Khz = 1000,
        Smpl2Khz = 2000,
        Smpl4Khz = 4000,
        Smpl8Khz = 8000
    }

    public enum ModBufSize
    {
        Buf125 = 125,
        Buf250 = 250,
        Buf500 = 500,
        Buf1000 = 1000,
        Buf2000 = 2000,
        Buf4000 = 4000,
        Buf8000 = 8000,
        Buf16000 = 16000,
        Buf32000 = 32000
    }

    public class Configuration
    {
        public ModSamplingFreq ModSamplingFrequency { get; }
        public ModBufSize ModBufferSize { get; }

        public Configuration()
        {
            ModSamplingFrequency = ModSamplingFreq.Smpl4Khz;
            ModBufferSize = ModBufSize.Buf4000;
        }

        public Configuration(ModSamplingFreq modSamplingFrequency, ModBufSize modBufferSize)
        {
            ModSamplingFrequency = modSamplingFrequency;
            ModBufferSize = modBufferSize;
        }
    }
}
