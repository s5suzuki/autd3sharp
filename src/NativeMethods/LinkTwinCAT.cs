/*
 * File: LinkTwinCAT.cs
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
    internal static class LinkTwinCAT
    {
        [DllImport("autd3capi-link-twincat", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDLinkTwinCAT(out IntPtr @out, ushort cycleTicks);
    }
}
