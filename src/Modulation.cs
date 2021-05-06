/*
 * File: Modulation.cs
 * Project: src
 * Created Date: 28/04/2021
 * Author: Shun Suzuki
 * -----
 * Last Modified: 06/05/2021
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2021 Hapis Lab. All rights reserved.
 * 
 */

using System;
using System.Runtime.InteropServices;
using System.Text;
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

        public static Modulation StaticModulation(byte amp = 0xFF)
        {
            NativeMethods.AUTDModulation(out var modPtr, amp);
            return new Modulation(modPtr);
        }

        public static Modulation CustomModulation(byte[] data)
        {
            IntPtr modPtr;
            unsafe
            {
                fixed (byte* p = data) NativeMethods.AUTDCustomModulation(out modPtr, p, (uint)data.Length);
            }
            return new Modulation(modPtr);
        }

        public static Modulation? RawPcmModulation(string fileName, float samplingFreq)
        {
            var sb = new StringBuilder(128);
            var res = NativeMethods.AUTDRawPCMModulation(out var modPtr, fileName, samplingFreq, sb);
            return res ? new Modulation(modPtr) : null;
        }

        public static Modulation SawModulation(int freq)
        {
            NativeMethods.AUTDSawModulation(out var modPtr, freq);
            return new Modulation(modPtr);
        }

        public static Modulation SquareModulation(int freq, byte low = 0x00, byte high = 0xFF)
        {
            NativeMethods.AUTDSquareModulation(out var modPtr, freq, low, high);
            return new Modulation(modPtr);
        }

        public static Modulation SineModulation(int freq, float amp = 1, float offset = 0.5f)
        {
            NativeMethods.AUTDSineModulation(out var modPtr, freq, amp, offset);
            return new Modulation(modPtr);
        }

        public static Modulation? WavModulation(string fileName)
        {
            var sb = new StringBuilder(128);
            var res = NativeMethods.AUTDWavModulation(out var modPtr, fileName, sb);
            return res ? new Modulation(modPtr) : null;
        }
    }
}
