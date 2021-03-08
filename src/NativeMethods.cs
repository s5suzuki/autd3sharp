/*
 * File: NativeMethods.cs
 * Project: src
 * Created Date: 13/06/2020
 * Author: Shun Suzuki
 * -----
 * Last Modified: 08/03/2021
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2021 Hapis Lab. All rights reserved.
 * 
 */


using System;
using System.Runtime.InteropServices;
using System.Text;

namespace AUTD3Sharp
{
    internal static unsafe class NativeMethods
    {
        private const string MainDllName = "autd3capi";
        private const string HoloGainDllName = "autd3capi-gain-holo";
        private const string SOEMLinkDllName = "autd3capi-link-soem";
        private const string TwincatLinkDllName = "autd3capi-link-twincat";
        private const string ModulationFromFileDllName = "autd3capi-modulation-from-file";

        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDCreateController(out IntPtr @out);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern int AUTDOpenControllerWith(IntPtr handle, IntPtr pLink);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern int AUTDAddDevice(IntPtr handle, float x, float y, float z, float rz1, float ry, float rz2, int groupId);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern int AUTDAddDeviceQuaternion(IntPtr handle, float x, float y, float z, float quaW, float quaX, float quaY, float quaZ, int groupId);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] [return: MarshalAs(UnmanagedType.U1)] public static extern bool AUTDCalibrate(IntPtr handle, int smplFreq, int bufSize);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDCloseController(IntPtr handle);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDClear(IntPtr handle);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDFreeController(IntPtr handle);
        [DllImport(MainDllName, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDSetSilentMode(IntPtr handle, [MarshalAs(UnmanagedType.U1)] bool mode);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDStop(IntPtr handle);
        [DllImport(SOEMLinkDllName, CallingConvention = CallingConvention.StdCall)] public static extern int AUTDGetAdapterPointer(out IntPtr @out);
        [DllImport(SOEMLinkDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDGetAdapter(IntPtr pAdapter, int index, StringBuilder desc, StringBuilder name);
        [DllImport(SOEMLinkDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDFreeAdapterPointer(IntPtr pAdapter);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern int AUTDGetFirmwareInfoListPointer(IntPtr handle, out IntPtr @out);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDGetFirmwareInfo(IntPtr pFirmInfoList, int index, StringBuilder cpuVer, StringBuilder fpgaVer);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDFreeFirmwareInfoListPointer(IntPtr pFirmInfoList);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] [return: MarshalAs(UnmanagedType.U1)] public static extern bool AUTDIsOpen(IntPtr handle);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] [return: MarshalAs(UnmanagedType.U1)] public static extern bool AUTDIsSilentMode(IntPtr handle);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern float AUTDWavelength(IntPtr handle);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDSetWavelength(IntPtr handle, float wavelength);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern int AUTDNumDevices(IntPtr handle);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern int AUTDNumTransducers(IntPtr handle);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern ulong AUTDRemainingInBuffer(IntPtr handle);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDFocalPointGain(out IntPtr gain, float x, float y, float z, byte duty);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDGroupedGain(out IntPtr gain, int* groupIds, IntPtr* inGains, int size);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDBesselBeamGain(out IntPtr gain, float x, float y, float z, float nX, float nY, float nZ, float thetaZ, byte duty);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDPlaneWaveGain(out IntPtr gain, float nX, float nY, float nZ, byte duty);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDCustomGain(out IntPtr gain, ushort* data, int dataLength);
        [DllImport(HoloGainDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDHoloGain(out IntPtr gain, float* points, float* amps, int size, int method, IntPtr @params);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDTransducerTestGain(out IntPtr gain, int idx, byte duty, byte phase);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDNullGain(out IntPtr gain);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDDeleteGain(IntPtr gain);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDModulation(out IntPtr mod, byte amp);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDCustomModulation(out IntPtr mod, byte* buf, uint size);
        [DllImport(ModulationFromFileDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDRawPCMModulation(out IntPtr mod, string filename, float samplingFreq);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDSawModulation(out IntPtr mod, int freq);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDSineModulation(out IntPtr mod, int freq, float amp, float offset);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDSquareModulation(out IntPtr mod, int freq, byte low, byte high);
        [DllImport(ModulationFromFileDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDWavModulation(out IntPtr mod, string filename);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDDeleteModulation(IntPtr mod);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDSequence(out IntPtr @out);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDSequenceAppendPoint(IntPtr seq, float x, float y, float z);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDSequenceAppendPoints(IntPtr seq, float* points, ulong size);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern float AUTDSequenceSetFreq(IntPtr seq, float freq);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern float AUTDSequenceFreq(IntPtr seq);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern float AUTDSequenceSamplingFreq(IntPtr seq);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern ushort AUTDSequenceSamplingFreqDiv(IntPtr seq);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDCircumSequence(out IntPtr @out, float x, float y, float z, float nx, float ny, float nz, float radius, ulong n);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDDeleteSequence(IntPtr seq);
        [DllImport(SOEMLinkDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDSOEMLink(out IntPtr @out, string ifname, int deviceNum);
        [DllImport(TwincatLinkDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDTwinCATLink(out IntPtr @out, string ipv4Addr, string amsNetId);
        [DllImport(TwincatLinkDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDLocalTwinCATLink(out IntPtr @out);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDAppendGain(IntPtr handle, IntPtr gain);
        [DllImport(MainDllName, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDAppendGainSync(IntPtr handle, IntPtr gain, [MarshalAs(UnmanagedType.U1)] bool waitForSend);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDAppendModulation(IntPtr handle, IntPtr mod);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDAppendModulationSync(IntPtr handle, IntPtr mod);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDAppendSTMGain(IntPtr handle, IntPtr gain);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDStartSTModulation(IntPtr handle, float freq);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDStopSTModulation(IntPtr handle);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDFinishSTModulation(IntPtr handle);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDAppendSequence(IntPtr handle, IntPtr seq);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDFlush(IntPtr handle);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern int AUTDDeviceIdxForTransIdx(IntPtr handle, int globalTransIdx);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern float* AUTDTransPositionByGlobal(IntPtr handle, int globalTransIdx);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern float* AUTDTransPositionByLocal(IntPtr handle, int deviceIdx, int localTransIdx);
        [DllImport(MainDllName, CallingConvention = CallingConvention.StdCall)] public static extern float* AUTDDeviceDirection(IntPtr handle, int deviceIdx);
    }
}
