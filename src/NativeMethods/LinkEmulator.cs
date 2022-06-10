// This file was automatically generated from header file
using System;
using System.Runtime.InteropServices;
            
namespace AUTD3Sharp.NativeMethods
{
    internal static class LinkEmulator
    {
        const string DLL = "autd3capi-link-emulator";

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDLinkEmulator(out IntPtr @out, ushort port, IntPtr cnt);
    }
}
