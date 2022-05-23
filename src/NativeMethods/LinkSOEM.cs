/*
 * File: LinkSOEM.cs
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
using System.Text;

namespace AUTD3Sharp.NativeMethods
{
    internal static class LinkSOEM
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)] public delegate void OnLostCallbackDelegate(string str);

        [DllImport("autd3capi-link-soem", CallingConvention = CallingConvention.Cdecl)] public static extern int AUTDGetAdapterPointer(out IntPtr @out);
        [DllImport("autd3capi-link-soem", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDGetAdapter(IntPtr pAdapter, int index, StringBuilder? desc, StringBuilder? name);
        [DllImport("autd3capi-link-soem", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDFreeAdapterPointer(IntPtr pAdapter);
        [DllImport("autd3capi-link-soem", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDLinkSOEM(out IntPtr @out, string ifname, int deviceNum, uint cycleTicks, IntPtr onLost, bool highPresicion);
    }
}