/*
 * File: base.cs
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
    internal static class Base
    {
        [DllImport("autd3capi", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.Cdecl)] public static extern int AUTDGetLastError(StringBuilder? error);

        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDCreateController(out IntPtr @out);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)][return: MarshalAs(UnmanagedType.U1)] public static extern bool AUTDOpenController(IntPtr handle, IntPtr pLink);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern int AUTDAddDevice(IntPtr handle, double x, double y, double z, double rz1, double ry, double rz2);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern int AUTDAddDeviceQuaternion(IntPtr handle, double x, double y, double z, double qw, double qx, double qy, double qz);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern int AUTDClose(IntPtr handle);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern int AUTDClear(IntPtr handle);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern int AUTDSynchronize(IntPtr handle);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDFreeController(IntPtr handle);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)][return: MarshalAs(UnmanagedType.U1)] public static extern bool AUTDIsOpen(IntPtr handle);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)][return: MarshalAs(UnmanagedType.U1)] public static extern bool AUTDGetForceFan(IntPtr handle);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)][return: MarshalAs(UnmanagedType.U1)] public static extern bool AUTDGetReadsFPGAInfo(IntPtr handle);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)][return: MarshalAs(UnmanagedType.U1)] public static extern bool AUTDGetCheckAck(IntPtr handle);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDSetReadsFPGAInfo(IntPtr handle, [MarshalAs(UnmanagedType.U1)] bool readsFpgaInfo);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDSetCheckAck(IntPtr handle, [MarshalAs(UnmanagedType.U1)] bool checkAck);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDSetForceFan(IntPtr handle, [MarshalAs(UnmanagedType.U1)] bool force);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern double AUTDGetSoundSpeed(IntPtr handle);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDSetSoundSpeed(IntPtr handle, double soundSpeed);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern double AUTDGetTransFrequency(IntPtr handle, int devIdx, int transIdx);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDSetTransFrequency(IntPtr handle, int devIdx, int transIdx, double freq);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern ushort AUTDGetTransCycle(IntPtr handle, int devIdx, int transIdx);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDSetTransCycle(IntPtr handle, int devIdx, int transIdx, ushort cycle);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern double AUTDGetWavelength(IntPtr handle, int devIdx, int transIdx, double soundSpeed);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern double AUTDGetAttenuation(IntPtr handle);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDSetAttenuation(IntPtr handle, double attenuation);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)][return: MarshalAs(UnmanagedType.U1)] public static extern bool AUTDGetFPGAInfo(IntPtr handle, byte[] @out);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern int AUTDUpdateFlags(IntPtr handle);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern int AUTDNumDevices(IntPtr handle);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDTransPosition(IntPtr handle, int deviceIdx, int localTransIdx, out double x, out double y, out double z);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDTransXDirection(IntPtr handle, int deviceIdx, int localTransIdx, out double x, out double y, out double z);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDTransYDirection(IntPtr handle, int deviceIdx, int localTransIdx, out double x, out double y, out double z);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDTransZDirection(IntPtr handle, int deviceIdx, int localTransIdx, out double x, out double y, out double z);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern int AUTDGetFirmwareInfoListPointer(IntPtr handle, out IntPtr @out);
        [DllImport("autd3capi", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDGetFirmwareInfo(IntPtr pFirmInfoList, int index, StringBuilder? info);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDFreeFirmwareInfoListPointer(IntPtr pFirmInfoList);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDGainNull(out IntPtr gain);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDGainGrouped(out IntPtr gain, IntPtr handle);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDGainGroupedAdd(IntPtr groupedGain, int deviceId, IntPtr gain);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDGainFocus(out IntPtr gain, double x, double y, double z, double amp);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDGainBesselBeam(out IntPtr gain, double x, double y, double z, double nX, double nY, double nZ, double thetaZ, double amp);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDGainPlaneWave(out IntPtr gain, double nX, double nY, double nZ, double amp);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDGainCustom(out IntPtr gain, double[] amp, double[] pahse, int dataLength);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDDeleteGain(IntPtr gain);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDModulationStatic(out IntPtr mod, double amp);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDModulationSine(out IntPtr mod, int freq, double amp, double offset);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDModulationSineSquared(out IntPtr mod, int freq, double amp, double offset);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDModulationSineLegacy(out IntPtr mod, double freq, double amp, double offset);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDModulationSquare(out IntPtr mod, int freq, double low, double high, double duty);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDModulationCustom(out IntPtr mod, byte[] buf, ulong size, uint freqDiv);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern uint AUTDModulationSamplingFrequencyDivision(IntPtr mod);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDModulationSetSamplingFrequencyDivision(IntPtr mod, uint freqDiv);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern double AUTDModulationSamplingFrequency(IntPtr mod);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDDeleteModulation(IntPtr mod);

        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDPointSTM(out IntPtr @out);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDGainSTM(out IntPtr @out, IntPtr handle);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)][return: MarshalAs(UnmanagedType.U1)] public static extern bool AUTDPointSTMAdd(IntPtr seq, double x, double y, double z, byte shift);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)][return: MarshalAs(UnmanagedType.U1)] public static extern bool AUTDGainSTMAdd(IntPtr seq, IntPtr gain);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern double AUTDSTMSetFrequency(IntPtr seq, double freq);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern double AUTDSTMFrequency(IntPtr seq);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern double AUTDSTMSamplingFrequency(IntPtr seq);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern uint AUTDSTMSamplingFrequencyDivision(IntPtr seq);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDSTMSetSamplingFrequencyDivision(IntPtr seq, uint freqDiv);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDDeleteSTM(IntPtr seq);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern int AUTDStop(IntPtr handle);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern int AUTDCreateSilencer(out IntPtr @out, ushort step, ushort cycle);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDDeleteSilencer(IntPtr config);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern int AUTDSendHeader(IntPtr handle, IntPtr header);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern int AUTDSendBody(IntPtr handle, IntPtr body);
        [DllImport("autd3capi", CallingConvention = CallingConvention.Cdecl)] public static extern int AUTDSendHeaderBody(IntPtr handle, IntPtr header, IntPtr body);
    }
}
