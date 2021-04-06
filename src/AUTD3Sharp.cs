/*
 * File: AUTD3Sharp.cs
 * Project: csharp
 * Created Date: 02/07/2018
 * Author: Shun Suzuki
 * -----
 * Last Modified: 06/04/2021
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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

#if UNITY
using UnityEngine;
using Vector3f = UnityEngine.Vector3;
using Quaternionf = UnityEngine.Quaternion;
using MathF = UnityEngine.Mathf;
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
        internal IntPtr ModPtr => handle;

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
        internal IntPtr SeqPtr => handle;

        public PointSequence(IntPtr seq) : base(true)
        {
            SetHandle(seq);
        }

        public bool AddPoint(Vector3f point)
        {
            AUTD.AdjustVector(ref point);
            return NativeMethods.AUTDSequenceAddPoint(handle, point[0], point[1], point[2]);
        }

        public bool AddPoints(IList<Vector3f> points)
        {
            var pointsArr = new float[points.Count * 3];
            for (var i = 0; i < points.Count; i++)
            {
                var point = points[i];
                AUTD.AdjustVector(ref point);
                pointsArr[3 * i] = point[0];
                pointsArr[3 * i + 1] = point[1];
                pointsArr[3 * i + 2] = point[2];

            }
            unsafe
            {
                fixed (float* pd = pointsArr)
                {
                    return NativeMethods.AUTDSequenceAddPoints(handle, pd, (ulong)points.Count);
                }
            }
        }
        public float SetFrequency(float freq)
        {
            return NativeMethods.AUTDSequenceSetFreq(handle, freq);
        }
        public float Frequency()
        {
            return NativeMethods.AUTDSequenceFreq(handle);
        }
        public float SamplingFrequency()
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
        internal IntPtr CntPtr => handle;

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
        internal IntPtr LinkPtr => handle;

        public Link(IntPtr handle) : base(false)
        {
            SetHandle(handle);
        }

        protected override bool ReleaseHandle()
        {
            return true;
        }
    }


    public readonly struct EtherCATAdapter : IEquatable<EtherCATAdapter>
    {
        public string Desc { get; }
        public string Name { get; }

        internal EtherCATAdapter(string desc, string name)
        {
            Desc = desc;
            Name = name;
        }

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

        public override bool Equals(object? obj)
        {
            return obj is EtherCATAdapter adapter && Equals(adapter);
        }

        public override int GetHashCode()
        {
            return Desc.GetHashCode() ^ Name.GetHashCode();
        }
    }

    public readonly struct FirmwareInfo : IEquatable<FirmwareInfo>
    {
        public string CpuVersion { get; }
        public string FpgaVersion { get; }

        internal FirmwareInfo(string cpu, string fpga)
        {
            CpuVersion = cpu;
            FpgaVersion = fpga;
        }


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

        public override bool Equals(object? obj)
        {
            return obj is FirmwareInfo info && Equals(info);
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
        public const float AUTDWidth = 0.192f;
        public const float AUTDHeight = 0.1514f;
        public const float TransSize = 0.01016f;
#else
        public const float AUTDWidth = 192.0f;
        public const float AUTDHeight = 151.4f;
        public const float TransSize = 10.16f;
#endif
        public const float Pi = MathF.PI;
        public const int NumTransInDevice = 249;
        public const int NumTransInX = 18;
        public const int NumTransInY = 14;
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

        public bool OpenWith(Link link)
        {
            return NativeMethods.AUTDOpenControllerWith(_autdControllerHandle.CntPtr, link.LinkPtr);
        }

        public static IEnumerable<EtherCATAdapter> EnumerateAdapters()
        {
            var size = NativeMethods.AUTDGetAdapterPointer(out var handle);
            for (var i = 0; i < size; i++)
            {
                var sbDesc = new StringBuilder(128);
                var sbName = new StringBuilder(128);
                NativeMethods.AUTDGetAdapter(handle, i, sbDesc, sbName);
                yield return new EtherCATAdapter(sbDesc.ToString(), sbName.ToString());
            }
            NativeMethods.AUTDFreeAdapterPointer(handle);
        }

        public IEnumerable<FirmwareInfo> FirmwareInfoList()
        {
            var size = NativeMethods.AUTDGetFirmwareInfoListPointer(_autdControllerHandle.CntPtr, out var handle);
            for (var i = 0; i < size; i++)
            {
                var sbCpu = new StringBuilder(128);
                var sbFpga = new StringBuilder(128);
                NativeMethods.AUTDGetFirmwareInfo(handle, i, sbCpu, sbFpga);
                yield return new FirmwareInfo(sbCpu.ToString(), sbFpga.ToString());
            }
            NativeMethods.AUTDFreeFirmwareInfoListPointer(handle);
        }

        public int AddDevice(Vector3f position, Vector3f rotation)
        {
            return AddDevice(position, rotation, 0);
        }

        public int AddDevice(Vector3f position, Vector3f rotation, int groupId)
        {
            AdjustVector(ref position);
            return NativeMethods.AUTDAddDevice(_autdControllerHandle.CntPtr, position[0], position[1], position[2], rotation[0], rotation[1], rotation[2], groupId);
        }

        public int AddDevice(Vector3f position, Quaternionf quaternion)
        {
            return AddDevice(position, quaternion, 0);
        }

        public int AddDevice(Vector3f position, Quaternionf quaternion, int groupId)
        {
            AdjustVector(ref position);
            AdjustQuaternion(ref quaternion);
            return NativeMethods.AUTDAddDeviceQuaternion(_autdControllerHandle.CntPtr, position[0], position[1], position[2], quaternion[3], quaternion[0], quaternion[1], quaternion[2], groupId);
        }

        public int DeleteDevice(int idx)
        {
            return NativeMethods.AUTDDeleteDevice(_autdControllerHandle.CntPtr, idx);
        }

        public void ClearDevices()
        {
            NativeMethods.AUTDClearDevices(_autdControllerHandle.CntPtr);
        }

        public enum ModSamplingFreq
        {
            Smpl125Hz = 125,
            Smpl250Hz = 250,
            Smpl500Hz = 500,
            Smpl1Khz = 1000,
            Smpl2Khz = 2000,
            Smpl4Khz = 4000,
            Smpl8Khz = 8000,
        };

        public enum ModBufSize
        {
            Buf125 = 125,
            Buf250 = 250,
            Buf500 = 500,
            Buf1000 = 1000,
            Buf2000 = 2000,
            Buf4000 = 4000,
            Buf8000 = 8000,
            Buf16000 = 16000,
            Buf32000 = 32000,
        };

        public class Configuration
        {
            public ModSamplingFreq ModSamplingFrequency { get; }
            public ModBufSize ModBufferSize { get; }

            public Configuration()
            {
                ModSamplingFrequency = ModSamplingFreq.Smpl4Khz;
                ModBufferSize = ModBufSize.Buf4000;
            }

            public Configuration(ModSamplingFreq modSamplingFrequency, ModBufSize modBufferSize)
            {
                ModSamplingFrequency = modSamplingFrequency;
                ModBufferSize = modBufferSize;
            }
        }

        public bool Synchronize()
        {
            return Synchronize(new Configuration());
        }

        public bool Synchronize(Configuration config)
        {
            return NativeMethods.AUTDSynchronize(_autdControllerHandle.CntPtr, (int)config.ModSamplingFrequency, (int)config.ModBufferSize);
        }

        public bool Close()
        {
            return NativeMethods.AUTDCloseController(_autdControllerHandle.CntPtr);
        }

        public bool Clear()
        {
            return NativeMethods.AUTDClear(_autdControllerHandle.CntPtr);
        }

        public bool Stop()
        {
            return NativeMethods.AUTDStop(_autdControllerHandle.CntPtr);
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
            NativeMethods.AUTDSetSilentMode(_autdControllerHandle.CntPtr, mode);
        }

        ~AUTD()
        {
            Dispose(false);
        }
        #endregion

        #region Property
        public bool IsOpen => NativeMethods.AUTDIsOpen(_autdControllerHandle.CntPtr);
        public bool SilentMode
        {
            get => NativeMethods.AUTDIsSilentMode(_autdControllerHandle.CntPtr);
            set => NativeMethods.AUTDSetSilentMode(_autdControllerHandle.CntPtr, value);
        }
        public int NumDevices => NativeMethods.AUTDNumDevices(_autdControllerHandle.CntPtr);
        public int NumTransducers => NativeMethods.AUTDNumTransducers(_autdControllerHandle.CntPtr);
        public float Wavelength
        {
            get => NativeMethods.AUTDWavelength(_autdControllerHandle.CntPtr);
            set => NativeMethods.AUTDSetWavelength(_autdControllerHandle.CntPtr, value);
        }
        public ulong RemainingInBuffer => NativeMethods.AUTDRemainingInBuffer(_autdControllerHandle.CntPtr);

        public string LastError
        {
            get
            {
                var size = NativeMethods.AUTDGetLastError(null);
                var sb = new StringBuilder(size);
                NativeMethods.AUTDGetLastError(sb);
                return sb.ToString();
            }
        }
        #endregion

        #region Gain

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte AdjustAmp(float amp)
        {
            var d = MathF.Asin(amp) / MathF.PI;
            return (byte)(511.0 * d);
        }

        public static Gain FocalPointGain(Vector3f point, float amp)
        {
            return FocalPointGain(point, AdjustAmp(amp));
        }

        public static Gain FocalPointGain(Vector3f point, byte duty = 0xff)
        {
            AdjustVector(ref point);
            NativeMethods.AUTDFocalPointGain(out var gainPtr, point[0], point[1], point[2], duty);
            return new Gain(gainPtr);
        }

        public static unsafe Gain GroupedGain(GainMap gainMap)
        {
            if (gainMap == null)
            {
                throw new ArgumentNullException(nameof(gainMap));
            }

            var gainsPtr = gainMap.GainPointer;
            var idPtr = gainMap.IdPointer;
            NativeMethods.AUTDGroupedGain(out var gainPtr, idPtr, gainsPtr, gainMap.Size);
            return new Gain(gainPtr);
        }

        public static Gain GroupedGain(params GainPair[] gainPairs)
        {
            return GroupedGain(new GainMap(gainPairs));
        }

        public static Gain BesselBeamGain(Vector3f point, Vector3f dir, float thetaZ, float amp)
        {
            return BesselBeamGain(point, dir, thetaZ, AdjustAmp(amp));
        }

        public static Gain BesselBeamGain(Vector3f point, Vector3f dir, float thetaZ, byte duty = 0xFF)
        {
            AdjustVector(ref point);
            AdjustVector(ref dir);

            NativeMethods.AUTDBesselBeamGain(out var gainPtr, point[0], point[1], point[2], dir[0], dir[1], dir[2], thetaZ, duty);
            return new Gain(gainPtr);
        }

        public static Gain PlaneWaveGain(Vector3f dir, float amp)
        {
            return PlaneWaveGain(dir, AdjustAmp(amp));
        }

        public static Gain PlaneWaveGain(Vector3f dir, byte duty = 0xFF)
        {
            AdjustVector(ref dir);

            NativeMethods.AUTDPlaneWaveGain(out var gainPtr, dir[0], dir[1], dir[2], duty);
            return new Gain(gainPtr);
        }

        private enum OptMethod
        {
            SDP = 0,
            EVD = 1,
            GS = 2,
            GS_PAT = 3,
            NAIVE = 4,
            LM = 5
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SDPParams
        {
            private float _regularization;
            private int _repeat;
            private float _lambda;
            [MarshalAs(UnmanagedType.U1)]
            private bool _normalize;

            public float Regularization { get => _regularization; set => _regularization = value; }
            public int Repeat { get => _repeat; set => _repeat = value; }
            public float Lambda { get => _lambda; set => _lambda = value; }
            public bool NormalizeAmp { get => _normalize; set => _normalize = value; }

            public static SDPParams GetDefault()
            {
                return new SDPParams
                {
                    Regularization = -1,
                    Repeat = -1,
                    Lambda = -1,
                    NormalizeAmp = true,
                };
            }
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct EVDParams
        {
            private float _regularization;
            [MarshalAs(UnmanagedType.U1)]
            private bool _normalize;

            public float Regularization { get => _regularization; set => _regularization = value; }
            public bool NormalizeAmp { get => _normalize; set => _normalize = value; }


            public static EVDParams GetDefault()
            {
                return new EVDParams
                {
                    Regularization = -1,
                    NormalizeAmp = true,
                };
            }
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct NLSParams
        {
            private float _eps1;
            private float _eps2;
            private int _k_max;
            private float _tau;

            public float Eps1 { get => _eps1; set => _eps1 = value; }
            public float Eps2 { get => _eps2; set => _eps2 = value; }
            public int KMax { get => _k_max; set => _k_max = value; }
            public float Tau { get => _tau; set => _tau = value; }

            public static NLSParams GetDefault()
            {
                return new NLSParams
                {
                    Eps1 = -1,
                    Eps2 = -1,
                    KMax = -1,
                    Tau = -1,
                };
            }
        };

        public static Gain HoloGain(Vector3f[] focuses, float[] amps)
        {
            return HoloGainSDP(focuses, amps, null);
        }

        public static unsafe Gain HoloGainSDP(Vector3f[] focuses, float[] amps, SDPParams? param)
        {
            var p = Marshal.AllocHGlobal(sizeof(SDPParams));
            if (param.HasValue)
            {
                Marshal.StructureToPtr(param.Value, p, false);
            }
            else
            {
                p = IntPtr.Zero;
            }

            return HoloGain(focuses, amps, OptMethod.SDP, p);
        }

        public static unsafe Gain HoloGainEVD(Vector3f[] focuses, float[] amps, EVDParams? param)
        {
            var p = Marshal.AllocHGlobal(sizeof(EVDParams));
            if (param.HasValue)
            {
                Marshal.StructureToPtr(param.Value, p, false);
            }
            else
            {
                p = IntPtr.Zero;
            }

            return HoloGain(focuses, amps, OptMethod.EVD, p);
        }

        public static Gain HoloGainGS(Vector3f[] focuses, float[] amps, uint? repeat)
        {
            var p = Marshal.AllocHGlobal(sizeof(uint));
            if (repeat.HasValue)
            {
                Marshal.StructureToPtr(repeat.Value, p, false);
            }
            else
            {
                p = IntPtr.Zero;
            }

            return HoloGain(focuses, amps, OptMethod.GS, p);
        }

        public static Gain HoloGainGSPAT(Vector3f[] focuses, float[] amps, uint? repeat)
        {
            var p = Marshal.AllocHGlobal(sizeof(uint));
            if (repeat.HasValue)
            {
                Marshal.StructureToPtr(repeat.Value, p, false);
            }
            else
            {
                p = IntPtr.Zero;
            }

            return HoloGain(focuses, amps, OptMethod.GS_PAT, p);
        }

        public static Gain HoloGainNaive(Vector3f[] focuses, float[] amps)
        {
            return HoloGain(focuses, amps, OptMethod.NAIVE, IntPtr.Zero);
        }

        public static unsafe Gain HoloGainLM(Vector3f[] focuses, float[] amps, NLSParams? param)
        {
            var p = Marshal.AllocHGlobal(sizeof(NLSParams));
            if (param.HasValue)
            {
                Marshal.StructureToPtr(param.Value, p, false);
            }
            else
            {
                p = IntPtr.Zero;
            }

            return HoloGain(focuses, amps, OptMethod.LM, p);
        }

        private static unsafe Gain HoloGain(Vector3f[] focuses, float[] amps, OptMethod method, IntPtr param)
        {
            if (focuses == null)
            {
                throw new ArgumentNullException(nameof(focuses));
            }

            if (amps == null)
            {
                throw new ArgumentNullException(nameof(amps));
            }

            var size = amps.Length;
            var foci = new float[size * 3];
            for (var i = 0; i < size; i++)
            {
                AdjustVector(ref focuses[i]);

                foci[3 * i] = focuses[i][0];
                foci[3 * i + 1] = focuses[i][1];
                foci[3 * i + 2] = focuses[i][2];
            }

            IntPtr gainPtr;
            fixed (float* fp = &foci[0])
            fixed (float* ap = &amps[0])
            {
                NativeMethods.AUTDHoloGain(out gainPtr, fp, ap, size, (int)method, param);
            }
            return new Gain(gainPtr);
        }

        public static Gain TransducerTestGain(int index, byte duty, byte phase)
        {
            NativeMethods.AUTDTransducerTestGain(out var gainPtr, index, duty, phase);
            return new Gain(gainPtr);
        }

        public static Gain NullGain()
        {
            NativeMethods.AUTDNullGain(out var gainPtr);
            return new Gain(gainPtr);
        }

        public unsafe Gain CustomGain(ushort[,] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var numDev = NumDevices;

            if (data.GetLength(0) != numDev)
            {
                throw new ArgumentOutOfRangeException("Invalid data length. " + numDev + " AUTDs was added.");
            }

            if (data.GetLength(1) != NumTransInDevice)
            {
                throw new ArgumentOutOfRangeException("Some Device have wrong Data length. A device must have " + NumTransInDevice + " data.");
            }

            IntPtr gainPtr;
            var length = data.GetLength(0) * data.GetLength(1);
            fixed (ushort* r = data)
            {
                NativeMethods.AUTDCustomGain(out gainPtr, r, length);
            }

            return new Gain(gainPtr);
        }
        #endregion

        #region Modulation

        public static Modulation Modulation(byte amp = 0xFF)
        {
            NativeMethods.AUTDModulation(out var modPtr, amp);
            return new Modulation(modPtr);
        }
        public static Modulation CustomModulation(byte[] data)
        {
            IntPtr modPtr;
            unsafe
            {
                fixed (byte* p = data)
                {
                    NativeMethods.AUTDCustomModulation(out modPtr, p, (uint)data.Length);
                }
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
        #endregion

        #region Sequence
        public static PointSequence PointSequence()
        {
            NativeMethods.AUTDSequence(out var p);
            return new PointSequence(p);
        }
        public static PointSequence CircumferencePointSequence(Vector3f center, Vector3f normal, float radius, ulong n)
        {
            AdjustVector(ref center);
            AdjustVector(ref normal);
            NativeMethods.AUTDCircumSequence(out var p, center[0], center[1], center[2], normal[0], normal[1], normal[2], radius, n);
            return new PointSequence(p);
        }
        #endregion

        #region Link
        public static Link SOEMLink(string ifname, int deviceNum)
        {
            NativeMethods.AUTDSOEMLink(out var link, ifname, deviceNum);
            return new Link(link);
        }
        public static Link EtherCATLink(string ip4Addr, string amsNetId)
        {
            NativeMethods.AUTDTwinCATLink(out var link, ip4Addr, amsNetId);
            return new Link(link);
        }
        public static Link LocalEtherCATLink()
        {
            NativeMethods.AUTDLocalTwinCATLink(out var link);
            return new Link(link);
        }
        #endregion

        #region LowLevelInterface
        public bool AppendGain(Gain gain)
        {
            if (gain == null)
            {
                throw new ArgumentNullException(nameof(gain));
            }

            return NativeMethods.AUTDAppendGain(_autdControllerHandle.CntPtr, gain.GainPtr);
        }
        public bool AppendGainSync(Gain gain, bool waitForSend = false)
        {
            if (gain == null)
            {
                throw new ArgumentNullException(nameof(gain));
            }

            return NativeMethods.AUTDAppendGainSync(_autdControllerHandle.CntPtr, gain.GainPtr, waitForSend);
        }
        public bool AppendModulation(Modulation mod)
        {
            if (mod == null)
            {
                throw new ArgumentNullException(nameof(mod));
            }

            return NativeMethods.AUTDAppendModulation(_autdControllerHandle.CntPtr, mod.ModPtr);
        }
        public bool AppendModulationSync(Modulation mod)
        {
            if (mod == null)
            {
                throw new ArgumentNullException(nameof(mod));
            }

            return NativeMethods.AUTDAppendModulationSync(_autdControllerHandle.CntPtr, mod.ModPtr);
        }
        public void AddSTMGain(Gain gain)
        {
            if (gain == null)
            {
                throw new ArgumentNullException(nameof(gain));
            }

            NativeMethods.AUTDAddSTMGain(_autdControllerHandle.CntPtr, gain.GainPtr);
        }
        public void AddSTMGain(IList<Gain> gains)
        {
            if (gains == null)
            {
                throw new ArgumentNullException(nameof(gains));
            }

            foreach (var gain in gains)
            {
                AddSTMGain(gain);
            }
        }
        public void AddSTMGain(params Gain[] gainList)
        {
            if (gainList == null)
            {
                throw new ArgumentNullException(nameof(gainList));
            }

            foreach (var gain in gainList)
            {
                AddSTMGain(gain);
            }
        }
        public bool StartSTModulation(float freq)
        {
            return NativeMethods.AUTDStartSTModulation(_autdControllerHandle.CntPtr, freq);
        }
        public bool StopSTModulation()
        {
            return NativeMethods.AUTDStopSTModulation(_autdControllerHandle.CntPtr);
        }
        public bool FinishSTModulation()
        {
            return NativeMethods.AUTDFinishSTModulation(_autdControllerHandle.CntPtr);
        }
        public bool AppendSequence(PointSequence seq)
        {
            if (seq == null)
            {
                throw new ArgumentNullException(nameof(seq));
            }

            return NativeMethods.AUTDAppendSequence(_autdControllerHandle.CntPtr, seq.SeqPtr);
        }
        public void Flush()
        {
            NativeMethods.AUTDFlush(_autdControllerHandle.CntPtr);
        }
        public int DeviceIdxForTransIdx(int devIdx)
        {
            var res = NativeMethods.AUTDDeviceIdxForTransIdx(_autdControllerHandle.CntPtr, devIdx);
            return res;
        }
        public unsafe Vector3f TransPosition(int transIdxGlobal)
        {
            var fp = NativeMethods.AUTDTransPositionByGlobal(_autdControllerHandle.CntPtr, transIdxGlobal);
            return new Vector3f(fp[0], fp[1], fp[2]);
        }
        public unsafe Vector3f TransPosition(int deviceIdx, int transIdxLocal)
        {
            var fp = NativeMethods.AUTDTransPositionByLocal(_autdControllerHandle.CntPtr, deviceIdx, transIdxLocal);
            return new Vector3f(fp[0], fp[1], fp[2]);
        }
        public unsafe Vector3f DeviceDirection(int deviceIdx)
        {
            var fp = NativeMethods.AUTDDeviceDirection(_autdControllerHandle.CntPtr, deviceIdx);
            return new Vector3f(fp[0], fp[1], fp[2]);
        }
        #endregion

        #region GeometryAdjust
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void AdjustVector(ref Vector3f vector)
        {
#if LEFT_HANDED
            vector.z = -vector.z;
#endif
#if DIMENSION_M
            vector[0] *= MeterScale;
            vector[1] *= MeterScale;
            vector[2] *= MeterScale;
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AdjustQuaternion(ref Quaternionf quaternion)
        {
#if LEFT_HANDED
            quaternion.z = -quaternion.z;
            quaternion.w = -quaternion.w;
#endif
        }
        #endregion
    }
}
