/*
 * File: ModulationAudioFile.cs
 * Project: NativeMethods
 * Created Date: 23/05/2022
 * Author: Shun Suzuki
 * -----
 * Last Modified: 23/05/2022
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2022 Hapis Lab. All rights reserved.
 * 
 */

using System;
using System.Runtime.InteropServices;

namespace AUTD3Sharp.NativeMethods
{
    internal static class ModulationAudioFile
    {
        [DllImport("autd3capi-modulation-audio-file", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDModulationRawPCM(out IntPtr mod, string filename, double samplingFreq, uint modSamplingFreqDiv);
        [DllImport("autd3capi-modulation-audio-file", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDModulationWav(out IntPtr mod, string filename, uint modSamplingFreqDiv);
    }
}
