// This file was automatically generated from header file
using System;
using System.Runtime.InteropServices;
            
namespace AUTD3Sharp.NativeMethods
{
    internal static class LinkTwinCAT
    {
        private const string DLL = "autd3capi-link-twincat";

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDLinkTwinCAT(out IntPtr @out);
    }
}
