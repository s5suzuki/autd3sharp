/*
 * File: Modulation.cs
 * Project: src
 * Created Date: 28/04/2021
 * Author: Shun Suzuki
 * -----
 * Last Modified: 14/10/2021
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2021 Hapis Lab. All rights reserved.
 * 
 */

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace AUTD3Sharp
{
    [ComVisible(false)]
    public class Modulation : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal IntPtr ModPtr => handle;

        internal Modulation(IntPtr modulation) : base(true)
        {
            SetHandle(modulation);
        }

        protected override bool ReleaseHandle()
        {
            NativeMethods.AUTDDeleteModulation(handle);
            return true;
        }

        public double SamplingFrequency => NativeMethods.AUTDModulationSamplingFreq(handle);
        public uint SamplingFrequencyDivision
        {
            get => NativeMethods.AUTDModulationSamplingFreqDiv(handle);
            set => NativeMethods.AUTDModulationSetSamplingFreqDiv(handle, value);
        }

        public static Modulation Static(byte amp = 0xFF)
        {
            NativeMethods.AUTDModulationStatic(out var modPtr, amp);
            return new Modulation(modPtr);
        }

        public static Modulation Custom(byte[] data)
        {
            NativeMethods.AUTDModulationCustom(out var modPtr, data, (uint)data.Length);
            return new Modulation(modPtr);
        }

        public static Modulation Square(int freq, byte low = 0x00, byte high = 0xFF, double duty = 0.5)
        {
            NativeMethods.AUTDModulationSquare(out var modPtr, freq, low, high, duty);
            return new Modulation(modPtr);
        }

        public static Modulation Sine(int freq, double amp = 1, double offset = 0.5f)
        {
            NativeMethods.AUTDModulationSine(out var modPtr, freq, amp, offset);
            return new Modulation(modPtr);
        }

        public static Modulation SinePressure(int freq, double amp = 1, double offset = 0.5f)
        {
            NativeMethods.AUTDModulationSinePressure(out var modPtr, freq, amp, offset);
            return new Modulation(modPtr);
        }
        public static Modulation SineLegacy(double freq, double amp = 1, double offset = 0.5f)
        {
            NativeMethods.AUTDModulationSineLegacy(out var modPtr, freq, amp, offset);
            return new Modulation(modPtr);
        }

        public static Modulation RawPCM(string filename, double samplingFreq, ushort modSamplingFreqDiv)
        {
            NativeMethods.AUTDModulationRawPCM(out var mod, filename, samplingFreq, modSamplingFreqDiv);
            return new Modulation(mod);
        }

        public static Modulation Wav(string filename, ushort modSamplingFreqDiv)
        {
            NativeMethods.AUTDModulationWav(out var mod, filename, modSamplingFreqDiv);
            return new Modulation(mod);
        }
    }
}
