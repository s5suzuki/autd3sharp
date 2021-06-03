/*
 * File: Modulation.cs
 * Project: src
 * Created Date: 28/04/2021
 * Author: Shun Suzuki
 * -----
 * Last Modified: 03/06/2021
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

        public static Modulation Static(byte amp = 0xFF)
        {
            NativeMethods.AUTDStaticModulation(out var modPtr, amp);
            return new Modulation(modPtr);
        }

        public static Modulation Custom(byte[] data)
        {
            IntPtr modPtr;
            unsafe
            {
                fixed (byte* p = data) NativeMethods.AUTDCustomModulation(out modPtr, p, (uint)data.Length);
            }
            return new Modulation(modPtr);
        }

        public static Modulation Saw(int freq)
        {
            NativeMethods.AUTDSawModulation(out var modPtr, freq);
            return new Modulation(modPtr);
        }

        public static Modulation Square(int freq, byte low = 0x00, byte high = 0xFF)
        {
            NativeMethods.AUTDSquareModulation(out var modPtr, freq, low, high);
            return new Modulation(modPtr);
        }

        public static Modulation Sine(int freq, double amp = 1, double offset = 0.5f)
        {
            NativeMethods.AUTDSineModulation(out var modPtr, freq, amp, offset);
            return new Modulation(modPtr);
        }
    }
}
