/*
 * File: NativeMethods.cs
 * Project: csharp
 * Created Date: 02/07/2018
 * Author: Shun Suzuki
 * -----
 * Last Modified: 03/07/2020
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2018-2019 Hapis Lab. All rights reserved.
 * 
 */

using System;
using System.Runtime.InteropServices;
using System.Text;
using AUTDGainPtr = System.IntPtr;
using AUTDLinkPtr = System.IntPtr;
using AUTDModulationPtr = System.IntPtr;
using AUTDSequencePtr = System.IntPtr;

#if DEBUG
using DebugLogFunc = System.IntPtr;
#endif

namespace AUTD3Sharp
{
    internal static unsafe class NativeMethods
    {
        private const string DllName = "autd3capi";

        #region Controller
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDCreateController(out IntPtr handle, int version);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern int AUTDOpenControllerWith(AUTDControllerHandle handle, Link plink);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern int AUTDAddDevice(AUTDControllerHandle handle, double x, double y, double z, double rz1, double ry, double rz2, int groupId);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern int AUTDAddDeviceQuaternion(AUTDControllerHandle handle, double x, double y, double z, double quaW, double quaX, double quaY, double quaZ, int groupId);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDDelDevice(AUTDControllerHandle handle, int devId);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern bool AUTDCalibrate(AUTDControllerHandle handle, int modSmplFreq, int modBufSize);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDCloseController(AUTDControllerHandle handle);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDClear(AUTDControllerHandle handle);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDFreeController(IntPtr handle);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDSetSilentMode(AUTDControllerHandle handle, [MarshalAs(UnmanagedType.U1)] bool mode);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDStop(AUTDControllerHandle handle);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern int AUTDGetAdapterPointer(out IntPtr p_adapter);

        [DllImport(DllName, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.StdCall)]
        public static extern void AUTDGetAdapter(IntPtr p_adapter, int index, StringBuilder desc, StringBuilder name);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDFreeAdapterPointer(IntPtr p_adapter);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern int AUTDGetFirmwareInfoListPointer(AUTDControllerHandle handle, out IntPtr plist);
        [DllImport(DllName, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDGetFirmwareInfo(IntPtr plist, int index, StringBuilder cpu_ver, StringBuilder fpga_ver);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDFreeFirmwareInfoListPointer(IntPtr pfirminfolist);
        #endregion

        #region Property
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] [return: MarshalAs(UnmanagedType.U1)] public static extern bool AUTDIsOpen(AUTDControllerHandle handle);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] [return: MarshalAs(UnmanagedType.U1)] public static extern bool AUTDIsSilentMode(AUTDControllerHandle handle);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern int AUTDNumDevices(AUTDControllerHandle handle);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern int AUTDNumTransducers(AUTDControllerHandle handle);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern ulong AUTDRemainingInBuffer(AUTDControllerHandle handle);
        #endregion region

        #region Gain
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDFocalPointGain(out AUTDGainPtr gain, double x, double y, double z, byte duty);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDGroupedGain(out AUTDGainPtr gain, int* groupIDs, AUTDGainPtr* gains, int size);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDBesselBeamGain(out AUTDGainPtr gain, double x, double y, double z, double nX, double nY, double nZ, double thetaZ, byte duty);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDPlaneWaveGain(out AUTDGainPtr gain, double nX, double nY, double nZ, byte duty);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDCustomGain(out AUTDGainPtr gain, ushort* data, int dataLength);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDHoloGain(out AUTDGainPtr gain, double* points, double* amps, int size, int method, IntPtr param);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDTransducerTestGain(out AUTDGainPtr gain, int idx, byte duty, byte phase);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDNullGain(out AUTDGainPtr gain);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDDeleteGain(AUTDGainPtr gain);
        #endregion

        #region Modulation
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDModulation(out AUTDModulationPtr mod, byte amp);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDCustomModulation(out AUTDModulationPtr mod, byte* data, uint size);
        [DllImport(DllName, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.StdCall)]
        public static extern void AUTDRawPCMModulation(out AUTDModulationPtr mod, string filename, double samplingFrequency);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDSawModulation(out AUTDModulationPtr mod, int freq);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDSquareModulation(out AUTDModulationPtr mod, int freq, byte low, byte high);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDSineModulation(out AUTDModulationPtr mod, int freq, double amp, double offset);
        [DllImport(DllName, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.StdCall)]
        public static extern void AUTDWavModulation(out AUTDModulationPtr mod, string filename);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDDeleteModulation(AUTDModulationPtr mod);
        #endregion

        #region Sequence
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDSequence(out AUTDSequencePtr seq);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDSequenceAppnedPoint(AUTDSequencePtr seq, double x, double y, double z);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDSequenceAppnedPoints(AUTDSequencePtr seq, double* points, ulong size);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern double AUTDSequenceSetFreq(AUTDSequencePtr seq, double freq);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern double AUTDSequenceFreq(AUTDSequencePtr seq);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern double AUTDSequenceSamplingFreq(AUTDSequencePtr seq);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern ushort AUTDSequenceSamplingFreqDiv(AUTDSequencePtr seq);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDCircumSequence(out AUTDSequencePtr seq, double x, double y, double z, double nx, double ny, double nz, double radius, ulong n);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDDeleteSequence(AUTDSequencePtr seq);
        #endregion

        #region Link
        [DllImport(DllName, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.StdCall)]
        public static extern void AUTDSOEMLink(out AUTDLinkPtr link, string ifname, int device_num);
        [DllImport(DllName, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.StdCall)]
        public static extern void AUTDTwinCATLink(out AUTDLinkPtr link, string ipv4addr, string ams_net_id);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDLocalTwinCATLink(out AUTDLinkPtr link);
        [DllImport(DllName, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.StdCall)]
        public static extern void AUTDEmulatorLink(out AUTDLinkPtr link, string addr, int port, AUTDControllerHandle handle);
        #endregion

        #region LowLevelInterface
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDAppendGain(AUTDControllerHandle handle, Gain gain);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDAppendGainSync(AUTDControllerHandle handle, Gain gain, bool waitForSend);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDAppendModulation(AUTDControllerHandle handle, Modulation mod);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDAppendModulationSync(AUTDControllerHandle handle, Modulation mod);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDAppendSTMGain(AUTDControllerHandle handle, Gain gainHandle);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDStartSTModulation(AUTDControllerHandle handle, double freq);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDStopSTModulation(AUTDControllerHandle handle);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDFinishSTModulation(AUTDControllerHandle handle);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDAppendSequence(AUTDControllerHandle handle, PointSequence seq);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern void AUTDFlush(AUTDControllerHandle handle);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern int AUTDDevIdForDeviceIdx(AUTDControllerHandle handle, int devIdx);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern int AUTDDevIdForTransIdx(AUTDControllerHandle handle, int transIdx);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern double* AUTDTransPosition(AUTDControllerHandle handle, int transIdx);
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern double* AUTDTransDirection(AUTDControllerHandle handle, int transIdx);
        #endregion

        #region Debug
#if DEBUG
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] public delegate void DebugLogDelegate(string str);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)] public static extern void SetDebugLog(DebugLogFunc func);
        [DllImport(DllName, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.StdCall)]
        public static extern void DebugLog(string msg);
#endif
        #endregion
    }
}