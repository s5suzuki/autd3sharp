/*
 * File: ModulationAudioFile.cs
 * Project: src
 * Created Date: 23/05/2021
 * Author: Shun Suzuki
 * -----
 * Last Modified: 02/06/2022
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2022 Shun Suzuki. All rights reserved.
 * 
 */

namespace AUTD3Sharp
{
    public sealed class RawPCM : Modulation
    {
        public RawPCM(string filename, double samplingFreq, uint modSamplingFreqDiv) : base()
        {
            NativeMethods.ModulationAudioFile.AUTDModulationRawPCM(out handle, filename, samplingFreq, modSamplingFreqDiv);
        }
    }
    public sealed class Wav : Modulation
    {
        public Wav(string filename, uint modSamplingFreqDiv) : base()
        {
            NativeMethods.ModulationAudioFile.AUTDModulationWav(out handle, filename, modSamplingFreqDiv);
        }
    }
}
