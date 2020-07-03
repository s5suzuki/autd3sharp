/*
 * File: AUTD3Sharp.cs
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

#if UNITY_2018_3_OR_NEWER
#define UNITY
#endif

#if UNITY
#define LEFT_HANDED
#define DIMENSION_M
#else
#define RIGHT_HANDED
#define DIMENSION_MM
#endif

using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

#if UNITY
using UnityEngine;
using Vector3d = UnityEngine.Vector3;
using Quaterniond = UnityEngine.Quaternion;
using Float = System.Single;
#else
using Float = System.Double;
#endif

[assembly: CLSCompliant(false), ComVisible(false)]
namespace AUTD3Sharp
{
    [ComVisible(false)]
    public class Gain : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal IntPtr GainPtr => handle;

        public Gain(IntPtr gain) : base(true)
        {
            SetHandle(gain);
        }

        protected override bool ReleaseHandle()
        {
            NativeMethods.AUTDDeleteGain(handle);
            return true;
        }
    }

    [ComVisible(false)]
    public class Modulation : SafeHandleZeroOrMinusOneIsInvalid
    {
        public Modulation(IntPtr modulation) : base(true)
        {
            SetHandle(modulation);
        }
        protected override bool ReleaseHandle()
        {
            NativeMethods.AUTDDeleteModulation(handle);
            return true;
        }
    }

    [ComVisible(false)]
    public class PointSequence : SafeHandleZeroOrMinusOneIsInvalid
    {
        public PointSequence(IntPtr seq) : base(true)
        {
            SetHandle(seq);
        }

        public void AppendPoint(Vector3d point)
        {
            AUTD.AdjustVector(ref point);
            NativeMethods.AUTDSequenceAppnedPoint(handle, point[0], point[1], point[2]);
        }

        public void AppendPoints(IList<Vector3d> points)
        {
            double[] points_d = new double[points.Count * 3];
            for (int i = 0; i < points.Count; i++)
            {
                Vector3d point = points[i];
                AUTD.AdjustVector(ref point);
                points_d[3 * i] = point[0];
                points_d[3 * i + 1] = point[1];
                points_d[3 * i + 2] = point[2];

            }
            unsafe
            {
                fixed (double* pd = points_d)
                {
                    NativeMethods.AUTDSequenceAppnedPoints(handle, pd, (ulong)points.Count);
                }
            }
        }
        public double SetFrequency(double freq)
        {
            return NativeMethods.AUTDSequenceSetFreq(handle, freq);
        }
        public double Frequency()
        {
            return NativeMethods.AUTDSequenceFreq(handle);
        }
        public double SamplingFrequency()
        {
            return NativeMethods.AUTDSequenceSamplingFreq(handle);
        }
        public ushort SamplingFrequencyDivision()
        {
            return NativeMethods.AUTDSequenceSamplingFreqDiv(handle);
        }
        protected override bool ReleaseHandle()
        {
            NativeMethods.AUTDDeleteSequence(handle);
            return true;
        }
    }

    internal class AUTDControllerHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public AUTDControllerHandle(bool ownsHandle) : base(ownsHandle)
        {
            handle = new IntPtr();
            NativeMethods.AUTDCreateController(out handle);
        }

        protected override bool ReleaseHandle()
        {
            NativeMethods.AUTDFreeController(handle);
            return true;
        }
    }

    [ComVisible(false)]
    public class Link : SafeHandleZeroOrMinusOneIsInvalid
    {
        public Link(IntPtr handle) : base(true)
        {
            SetHandle(handle);
        }

        protected override bool ReleaseHandle()
        {
            return true;
        }
    }


    public struct EtherCATAdapter : IEquatable<EtherCATAdapter>
    {
        public string Desc { get; internal set; }
        public string Name { get; internal set; }

        public override string ToString()
        {
            return $"{Desc}, {Name}";
        }

        public bool Equals(EtherCATAdapter other)
        {
            return Desc.Equals(other.Desc) && Name.Equals(other.Name);
        }

        public static bool operator ==(EtherCATAdapter left, EtherCATAdapter right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EtherCATAdapter left, EtherCATAdapter right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is EtherCATAdapter))
            {
                return false;
            }

            return Equals((EtherCATAdapter)obj);
        }

        public override int GetHashCode()
        {
            return Desc.GetHashCode() ^ Name.GetHashCode();
        }
    }

    public struct FirmwareInfo : IEquatable<FirmwareInfo>
    {
        public string CpuVersion { get; internal set; }
        public string FpgaVersion { get; internal set; }

        public override string ToString()
        {
            return $"CPU: {CpuVersion}, FPGA: {FpgaVersion}";
        }

        public bool Equals(FirmwareInfo other)
        {
            return CpuVersion.Equals(other.CpuVersion) && FpgaVersion.Equals(other.FpgaVersion);
        }

        public static bool operator ==(FirmwareInfo left, FirmwareInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(FirmwareInfo left, FirmwareInfo right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FirmwareInfo))
            {
                return false;
            }

            return Equals((FirmwareInfo)obj);
        }

        public override int GetHashCode()
        {
            return CpuVersion.GetHashCode() ^ FpgaVersion.GetHashCode();
        }
    }

    public sealed class AUTD : IDisposable
    {
        #region const

#if DIMENSION_M
        public static readonly float UltrasoundWavelength = 0.0085f;
        public const float AUTDWidth = 0.192f;
        public const float AUTDHeight = 0.1514f;
#else
        public static readonly double UltrasoundWavelength = 8.5;
        public const double AUTDWidth = 192.0;
        public const double AUTDHeight = 151.4;
#endif
        public const double Pi = 3.14159265;
        public const int NumTransInDevice = 249;

#if UNITY
        public readonly static float MeterScale = 1000f;
#endif
        #endregion

        #region field
        private bool _isDisposed;
        private readonly AUTDControllerHandle _autdControllerHandle;
        #endregion

        #region Controller
        public AUTD()
        {
            _autdControllerHandle = new AUTDControllerHandle(true);
        }

        public int OpenWith(Link link)
        {
            return NativeMethods.AUTDOpenControllerWith(_autdControllerHandle, link);
        }

        public static IEnumerable<EtherCATAdapter> EnumerateAdapters()
        {
            int size = NativeMethods.AUTDGetAdapterPointer(out IntPtr handle);
            for (int i = 0; i < size; i++)
            {
                StringBuilder sb_desc = new StringBuilder(128);
                StringBuilder sb_name = new StringBuilder(128);
                NativeMethods.AUTDGetAdapter(handle, i, sb_desc, sb_name);
                yield return new EtherCATAdapter() { Desc = sb_desc.ToString(), Name = sb_name.ToString() };
            }
            NativeMethods.AUTDFreeAdapterPointer(handle);
        }

        public IEnumerable<FirmwareInfo> FirmwareInfoList()
        {
            int size = NativeMethods.AUTDGetFirmwareInfoListPointer(_autdControllerHandle, out IntPtr handle);
            for (int i = 0; i < size; i++)
            {
                StringBuilder sb_cpu = new StringBuilder(128);
                StringBuilder sb_fpga = new StringBuilder(128);
                NativeMethods.AUTDGetFirmwareInfo(handle, i, sb_cpu, sb_fpga);
                yield return new FirmwareInfo() { CpuVersion = sb_cpu.ToString(), FpgaVersion = sb_fpga.ToString() };
            }
            NativeMethods.AUTDFreeFirmwareInfoListPointer(handle);
        }

#if UNITY
#else
        public int AddDevice(double x, double y, double z, double rz1, double ry, double rz2)
        {
            return AddDevice(new Vector3d(x, y, z), new Vector3d(rz1, ry, rz2), 0);
        }

        public int AddDevice(double x, double y, double z, double rz1, double ry, double rz2, int groupId)
        {
            return AddDevice(new Vector3d(x, y, z), new Vector3d(rz1, ry, rz2), groupId);
        }
#endif
        public int AddDevice(Vector3d position, Vector3d rotation)
        {
            return AddDevice(position, rotation, 0);
        }

        public int AddDevice(Vector3d position, Vector3d rotation, int groupId)
        {
            AdjustVector(ref position);
            int res = NativeMethods.AUTDAddDevice(_autdControllerHandle, position[0], position[1], position[2], rotation[0], rotation[1], rotation[2], groupId);
            return res;
        }
        public int AddDevice(Vector3d position, Quaterniond quaternion)
        {
            return AddDevice(position, quaternion, 0);
        }

        public int AddDevice(Vector3d position, Quaterniond quaternion, int groupId)
        {
            AdjustVector(ref position);
            AdjustQuaternion(ref quaternion);
            int res = NativeMethods.AUTDAddDeviceQuaternion(_autdControllerHandle, position[0], position[1], position[2], quaternion[3], quaternion[0], quaternion[1], quaternion[2], groupId);
            return res;
        }

        public void DelDevice(int devId)
        {
            NativeMethods.AUTDDelDevice(_autdControllerHandle, devId);
        }

        public void Calibrate()
        {
            NativeMethods.AUTDCalibrate(_autdControllerHandle);
        }

        public void Close()
        {
            NativeMethods.AUTDCloseController(_autdControllerHandle);
        }

        public void Clear()
        {
            NativeMethods.AUTDClear(_autdControllerHandle);
        }

        public void Stop()
        {
            NativeMethods.AUTDStop(_autdControllerHandle);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (disposing)
            {
                Close();
            }

            _autdControllerHandle.Dispose();

            _isDisposed = true;
        }

        public void SetSilentMode(bool mode)
        {
            NativeMethods.AUTDSetSilentMode(_autdControllerHandle, mode);
        }

        ~AUTD()
        {
            Dispose(false);
        }
        #endregion

        #region Property
        public bool IsOpen
        {
            get
            {
                bool res = NativeMethods.AUTDIsOpen(_autdControllerHandle);
                return res;
            }
        }
        public bool IsSilentMode
        {
            get
            {
                bool res = NativeMethods.AUTDIsSilentMode(_autdControllerHandle);
                return res;
            }
        }
        public int NumDevices
        {
            get
            {
                int res = NativeMethods.AUTDNumDevices(_autdControllerHandle);
                return res;
            }
        }
        public int NumTransducers
        {
            get
            {
                int res = NativeMethods.AUTDNumTransducers(_autdControllerHandle);
                return res;
            }
        }
        public ulong RemainingInBuffer
        {
            get
            {
                ulong res = NativeMethods.AUTDRemainingInBuffer(_autdControllerHandle);
                return res;
            }
        }
        #endregion

        #region Gain
#if UNITY
#else
        public static Gain FocalPointGain(double posX, double posY, double posZ, byte amp)
        {
            NativeMethods.AUTDFocalPointGain(out IntPtr gainPtr, posX, posY, posZ, amp);
            return new Gain(gainPtr);
        }
        public static Gain FocalPointGain(double posX, double posY, double posZ)
        {
            return FocalPointGain(posX, posY, posZ, 255);
        }
#endif
        public static Gain FocalPointGain(Vector3d point, byte amp)
        {
            AdjustVector(ref point);
            NativeMethods.AUTDFocalPointGain(out IntPtr gainPtr, point[0], point[1], point[2], amp);
            return new Gain(gainPtr);
        }

        public static Gain FocalPointGain(Vector3d point)
        {
            return FocalPointGain(point, 0xff);
        }

        public static unsafe Gain GroupedGain(GainMap gainMap)
        {
            if (gainMap == null)
            {
                throw new ArgumentNullException(nameof(gainMap));
            }

            IntPtr* gainsPtr = gainMap.GainPointer;
            int* idPtr = gainMap.IdPointer;
            NativeMethods.AUTDGroupedGain(out IntPtr gainPtr, idPtr, gainsPtr, gainMap.Size);
            return new Gain(gainPtr);
        }
        public static Gain GroupedGain(params GainPair[] gainPairs)
        {
            return GroupedGain(new GainMap(gainPairs));
        }
#if UNITY
#else
        public static Gain BesselBeamGain(double startPosX, double startPosY, double startPosZ, double dirX, double dirY, double dirZ, double thetaZ)
        {
            return BesselBeamGain(new Vector3d(startPosX, startPosY, startPosZ), new Vector3d(dirX, dirY, dirZ), thetaZ);
        }
#endif
        public static Gain BesselBeamGain(Vector3d point, Vector3d dir, double thetaZ)
        {
            AdjustVector(ref point);
            AdjustVector(ref dir);

            NativeMethods.AUTDBesselBeamGain(out IntPtr gainPtr, point[0], point[1], point[2], dir[0], dir[1], dir[2], thetaZ);
            return new Gain(gainPtr);
        }
#if UNITY
#else
        public static Gain PlaneWaveGain(double dirX, double dirY, double dirZ)
        {
            return PlaneWaveGain(new Vector3d(dirX, dirY, dirZ));
        }
#endif
        public static Gain PlaneWaveGain(Vector3d dir)
        {
            AdjustVector(ref dir);

            NativeMethods.AUTDPlaneWaveGain(out IntPtr gainPtr, dir[0], dir[1], dir[2]);
            return new Gain(gainPtr);
        }
        public static unsafe Gain HoloGain(Vector3d[] focuses, double[] amps)
        {
            if (focuses == null)
            {
                throw new ArgumentNullException(nameof(focuses));
            }

            if (amps == null)
            {
                throw new ArgumentNullException(nameof(amps));
            }

            int size = amps.Length;
            double[] foci = new double[size * 3];
            for (int i = 0; i < size; i++)
            {
                AdjustVector(ref focuses[i]);

                foci[3 * i] = focuses[i][0];
                foci[3 * i + 1] = focuses[i][1];
                foci[3 * i + 2] = focuses[i][2];
            }

            IntPtr gainPtr;
            fixed (double* fp = &foci[0])
            fixed (double* ap = &amps[0])
            {
                NativeMethods.AUTDHoloGain(out gainPtr, fp, ap, size);
            }
            return new Gain(gainPtr);
        }

        public static Gain TransducerTestGain(int index, int amp, int phase)
        {
            NativeMethods.AUTDTransducerTestGain(out IntPtr gainPtr, index, amp, phase);
            return new Gain(gainPtr);
        }

        public static Gain NullGain()
        {
            NativeMethods.AUTDNullGain(out IntPtr gainPtr);
            return new Gain(gainPtr);
        }

        [SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "0#")]
        public unsafe Gain CustomGain(ushort[,] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            int numDev = NumDevices;

            if (data.GetLength(0) != numDev)
            {
                throw new ArgumentOutOfRangeException("Invalid data length. " + numDev + " AUTDs was added.");
            }

            if (data.GetLength(1) != NumTransInDevice)
            {
                throw new ArgumentOutOfRangeException("Some Device have wrong Data length. A device must have " + NumTransInDevice + " data.");
            }

            IntPtr gainPtr;
            int length = data.GetLength(0) * data.GetLength(1);
            fixed (ushort* r = data)
            {
                NativeMethods.AUTDCustomGain(out gainPtr, r, length);
            }

            return new Gain(gainPtr);
        }
        #endregion

        #region Modulation
        public static Modulation Modulation()
        {
            return Modulation(255);
        }

        public static Modulation Modulation(byte amp)
        {
            NativeMethods.AUTDModulation(out IntPtr modPtr, amp);
            return new Modulation(modPtr);
        }
        public static Modulation RawPcmModulation(string fileName, double samplingFreq)
        {
            NativeMethods.AUTDRawPCMModulation(out IntPtr modPtr, fileName, samplingFreq);
            return new Modulation(modPtr);
        }
        public static Modulation SawModulation(int freq)
        {
            NativeMethods.AUTDSawModulation(out IntPtr modPtr, freq);
            return new Modulation(modPtr);
        }
        public static Modulation SineModulation(int freq)
        {
            return SineModulation(freq, 1, 0.5);
        }

        public static Modulation SineModulation(int freq, double amp)
        {
            return SineModulation(freq, amp, 0.5);
        }
        public static Modulation WavModulation(string fileName)
        {
            NativeMethods.AUTDWavModulation(out IntPtr modPtr, fileName);
            return new Modulation(modPtr);
        }
        public static Modulation SineModulation(int freq, double amp, double offset)
        {
            NativeMethods.AUTDSineModulation(out IntPtr modPtr, freq, amp, offset);
            return new Modulation(modPtr);
        }
        #endregion

        #region Sequence
        public static PointSequence PointSequence()
        {
            NativeMethods.AUTDSequence(out IntPtr p);
            return new PointSequence(p);
        }
        public static PointSequence CircumferencePointSequence(Vector3d center, Vector3d normal, double radius, ulong n)
        {
            AdjustVector(ref center);
            AdjustVector(ref normal);
            NativeMethods.AUTDCircumSequence(out IntPtr p, center[0], center[1], center[2], normal[0], normal[1], normal[2], radius, n);
            return new PointSequence(p);
        }
        #endregion

        #region Link
        public static Link SOEMLink(string ifname, int device_num)
        {
            NativeMethods.AUTDSOEMLink(out IntPtr plink, ifname, device_num);
            return new Link(plink);
        }
        public static Link EtherCATLink(string ip4Addr, string amsNetId)
        {
            NativeMethods.AUTDTwinCATLink(out IntPtr plink, ip4Addr, amsNetId);
            return new Link(plink);
        }
        public static Link LocalEtherCATLink()
        {
            NativeMethods.AUTDLocalTwinCATLink(out IntPtr plink);
            return new Link(plink);
        }
        public static Link EmulatorLink(string addr, int port, AUTD autd)
        {
            NativeMethods.AUTDEmulatorLink(out IntPtr plink, addr, port, autd._autdControllerHandle);
            return new Link(plink);
        }
        #endregion

        #region LowLevelInterface
        public void AppendGain(Gain gain)
        {
            if (gain == null)
            {
                throw new ArgumentNullException(nameof(gain));
            }

            NativeMethods.AUTDAppendGain(_autdControllerHandle, gain);
        }
        public void AppendGainSync(Gain gain, bool waitForSend = false)
        {
            if (gain == null)
            {
                throw new ArgumentNullException(nameof(gain));
            }

            NativeMethods.AUTDAppendGainSync(_autdControllerHandle, gain, waitForSend);
        }
        public void AppendModulation(Modulation mod)
        {
            if (mod == null)
            {
                throw new ArgumentNullException(nameof(mod));
            }

            NativeMethods.AUTDAppendModulation(_autdControllerHandle, mod);
        }
        public void AppendModulationSync(Modulation mod)
        {
            if (mod == null)
            {
                throw new ArgumentNullException(nameof(mod));
            }

            NativeMethods.AUTDAppendModulationSync(_autdControllerHandle, mod);
        }
        public void AppendSTMGain(Gain gain)
        {
            if (gain == null)
            {
                throw new ArgumentNullException(nameof(gain));
            }

            NativeMethods.AUTDAppendSTMGain(_autdControllerHandle, gain);
        }
        public void AppendSTMGain(IList<Gain> gains)
        {
            if (gains == null)
            {
                throw new ArgumentNullException(nameof(gains));
            }

            foreach (Gain gain in gains)
            {
                AppendSTMGain(gain);
            }
        }
        public void AppendSTMGain(params Gain[] gainList)
        {
            if (gainList == null)
            {
                throw new ArgumentNullException(nameof(gainList));
            }

            foreach (Gain gain in gainList)
            {
                AppendSTMGain(gain);
            }
        }
        public void StartSTModulation(double freq)
        {
            NativeMethods.AUTDStartSTModulation(_autdControllerHandle, freq);
        }
        public void StopSTModulation()
        {
            NativeMethods.AUTDStopSTModulation(_autdControllerHandle);
        }
        public void FinishSTModulation()
        {
            NativeMethods.AUTDFinishSTModulation(_autdControllerHandle);
        }
        public void AppendSequence(PointSequence seq)
        {
            if (seq == null)
            {
                throw new ArgumentNullException(nameof(seq));
            }

            NativeMethods.AUTDAppendSequence(_autdControllerHandle, seq);
        }
        public void Flush()
        {
            NativeMethods.AUTDFlush(_autdControllerHandle);
        }
        public int DeviceIdForDeviceIndex(int devIdx)
        {
            int res = NativeMethods.AUTDDevIdForDeviceIdx(_autdControllerHandle, devIdx);
            return res;
        }
        public int DeviceIdForDTransducerIndex(int transIdx)
        {
            int res = NativeMethods.AUTDDevIdForTransIdx(_autdControllerHandle, transIdx);
            return res;
        }
        public unsafe Vector3d TransPosition(int transIdx)
        {
            double* fp = NativeMethods.AUTDTransPosition(_autdControllerHandle, transIdx);
            return CreateVector(fp[0], fp[1], fp[2]);
        }
        public unsafe Vector3d TransDirection(int transIdx)
        {
            double* fp = NativeMethods.AUTDTransDirection(_autdControllerHandle, transIdx);
            return CreateVector(fp[0], fp[1], fp[2]);
        }
        #endregion

        #region DEBUG
#if DEBUG
        public static void SetDebugLogFunc(Action<string> debugLogFunc)
        {
            NativeMethods.DebugLogDelegate callback = new NativeMethods.DebugLogDelegate(debugLogFunc);
            IntPtr funcPtr = Marshal.GetFunctionPointerForDelegate(callback);
            NativeMethods.SetDebugLog(funcPtr);
        }

        public static void DebugLog(string msg)
        {
            NativeMethods.DebugLog(msg);
        }
#endif
        #endregion

        #region GeometryAdjust
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        internal static void AdjustVector(ref Vector3d vector)
        {
#if LEFT_HANDED
            vector[2] = -vector[2];
#endif
#if DIMENSION_M
            vector[0] *= MeterScale;
            vector[1] *= MeterScale;
            vector[2] *= MeterScale;
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Vector3d CreateVector(double x, double y, double z)
        {
            return new Vector3d((Float)x, (Float)y, (Float)z);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        internal static void AdjustQuaternion(ref Quaterniond quaternion)
        {
#if LEFT_HANDED
            quaternion[2] = -quaternion[2];
            quaternion[3] = -quaternion[3];
#endif
        }
        #endregion
    }
}
