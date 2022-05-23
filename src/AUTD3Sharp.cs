﻿/*
 * File: AUTD3Sharp.cs
 * Project: src
 * Created Date: 23/05/2022
 * Author: Shun Suzuki
 * -----
 * Last Modified: 23/05/2022
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2022 Hapis Lab. All rights reserved.
 * 
 */


#if UNITY_2018_3_OR_NEWER
#define LEFT_HANDED
#define DIMENSION_M
#define USE_SINGLE
#else
#define RIGHT_HANDED
#define DIMENSION_MM
#define USE_DOUBLE
#endif

using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

#if UNITY_2018_3_OR_NEWER
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using Math = UnityEngine.Mathf;
#else
using Vector3 = AUTD3Sharp.Utils.Vector3d;
using Quaternion = AUTD3Sharp.Utils.Quaterniond;
#endif


namespace AUTD3Sharp
{
    using Base = NativeMethods.Base;

    internal class AUTDControllerHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal IntPtr CntPtr => handle;

        public AUTDControllerHandle(bool ownsHandle) : base(ownsHandle)
        {
            handle = new IntPtr();
            Base.AUTDCreateController(out handle);
        }

        protected override bool ReleaseHandle()
        {
            Base.AUTDFreeController(handle);
            return true;
        }
    }

    public sealed class ControllerNormal : IDisposable
    {
        #region const
#if USE_SINGLE
#if DIMENSION_M
        public const float MeterScale = 1000.0f;
#else
        public const float MeterScale = 1;
#endif
        public const float DeviceWidth = 192.0f / MeterScale;
        public const float DeviceHeight = 151.4f / MeterScale;
        public const float TransSpacing = 10.16f / MeterScale;
        public const float Pi = Math.PI;
#else
#if DIMENSION_M
        public const double MeterScale = 1000.0;
#else
        public const double MeterScale = 1;
#endif
        public const double DeviceWidth = 192.0 / MeterScale;
        public const double DeviceHeight = 151.4 / MeterScale;
        public const double TransSpacing = 10.16 / MeterScale;
        public const double Pi = Math.PI;
#endif
        public const int NumTransInDevice = 249;
        public const int NumTransInX = 18;
        public const int NumTransInY = 14;

        #endregion

        #region field

        private bool _isDisposed;
        internal readonly AUTDControllerHandleLegacy AUTDControllerHandle;

        #endregion

        #region Controller

        public ControllerNormal()
        {
            AUTDControllerHandle = new AUTDControllerHandleLegacy(true);
        }

        public bool Open(Link link) => Base.AUTDOpenController(AUTDControllerHandle.CntPtr, link.LinkPtr);

        public IEnumerable<FirmwareInfo> FirmwareInfoList()
        {
            var size = Base.AUTDGetFirmwareInfoListPointer(AUTDControllerHandle.CntPtr, out var handle);
            for (var i = 0; i < size; i++)
            {
                var info = new StringBuilder(256);
                Base.AUTDGetFirmwareInfo(handle, i, info);
                yield return new FirmwareInfo(info.ToString());
            }

            Base.AUTDFreeFirmwareInfoListPointer(handle);
        }

        public int AddDevice(Vector3 position, Vector3 rotation)
        {
            var (x, y, z) = Adjust(position);
            var (rx, ry, rz) = Adjust(rotation, false);
            return Base.AUTDAddDevice(AUTDControllerHandle.CntPtr, x, y, z, rx, ry, rz);
        }

        public int AddDevice(Vector3 position, Quaternion quaternion)
        {
            var (x, y, z) = Adjust(position);
            var (qw, qx, qy, qz) = Adjust(quaternion);
            return Base.AUTDAddDeviceQuaternion(AUTDControllerHandle.CntPtr, x, y, z, qw, qx, qy, qz);
        }

        public int Close() => Base.AUTDClose(AUTDControllerHandle.CntPtr);

        public int Clear() => Base.AUTDClear(AUTDControllerHandle.CntPtr);
        public int Synchronize() => Base.AUTDSynchronize(AUTDControllerHandle.CntPtr);

        public int Stop() => Base.AUTDStop(AUTDControllerHandle.CntPtr);

        public int UpdateFlags() => Base.AUTDUpdateFlags(AUTDControllerHandle.CntPtr);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_isDisposed) return;

            if (disposing) Close();

            AUTDControllerHandle.Dispose();

            _isDisposed = true;
        }

        ~ControllerNormal()
        {
            Dispose(false);
        }

        #endregion

        #region Property

        public bool IsOpen => Base.AUTDIsOpen(AUTDControllerHandle.CntPtr);

        public bool ForceFan
        {
            get => Base.AUTDGetForceFan(AUTDControllerHandle.CntPtr);
            set => Base.AUTDSetForceFan(AUTDControllerHandle.CntPtr, value);
        }

        public bool ReadsFPGAInfo
        {
            get => Base.AUTDGetReadsFPGAInfo(AUTDControllerHandle.CntPtr);
            set => Base.AUTDSetReadsFPGAInfo(AUTDControllerHandle.CntPtr, value);
        }

        public bool CheckAck
        {
            get => Base.AUTDGetCheckAck(AUTDControllerHandle.CntPtr);
            set => Base.AUTDSetCheckAck(AUTDControllerHandle.CntPtr, value);
        }

        public byte[] FPGAInfo
        {
            get
            {
                var infos = new byte[NumDevices];
                Base.AUTDGetFPGAInfo(AUTDControllerHandle.CntPtr, infos);
                return infos;
            }
        }

        public int NumDevices => Base.AUTDNumDevices(AUTDControllerHandle.CntPtr);
        public int NumTransducers => NumDevices * NumTransInDevice;
        public double SoundSpeed
        {
            get => Base.AUTDGetSoundSpeed(AUTDControllerHandle.CntPtr);
            set => Base.AUTDSetSoundSpeed(AUTDControllerHandle.CntPtr, value);
        }
        public double Attenuation
        {
            get => Base.AUTDGetAttenuation(AUTDControllerHandle.CntPtr);
            set => Base.AUTDSetAttenuation(AUTDControllerHandle.CntPtr, value);
        }

        public static string LastError
        {
            get
            {
                var size = Base.AUTDGetLastError(null);
                var sb = new StringBuilder(size);
                Base.AUTDGetLastError(sb);
                return sb.ToString();
            }
        }
        #endregion

        public int Send(Header header)
        {
            if (header == null) throw new ArgumentNullException(nameof(header));
            return Base.AUTDSendHeader(AUTDControllerHandle.CntPtr, header.Ptr);
        }

        public int Send(Body body)
        {
            if (body == null) throw new ArgumentNullException(nameof(body));
            return Base.AUTDSendBody(AUTDControllerHandle.CntPtr, body.Ptr);
        }

        public int Send(Header header, Body body)
        {
            if (header == null) throw new ArgumentNullException(nameof(header));
            if (body == null) throw new ArgumentNullException(nameof(body));
            return Base.AUTDSendHeaderBody(AUTDControllerHandle.CntPtr, header.Ptr, body.Ptr);
        }

        public int Send(Body body, Header header)
        {
            if (header == null) throw new ArgumentNullException(nameof(header));
            if (body == null) throw new ArgumentNullException(nameof(body));
            return Base.AUTDSendHeaderBody(AUTDControllerHandle.CntPtr, header.Ptr, body.Ptr);
        }

        public Vector3 TransPosition(int deviceIdx, int transIdxLocal)
        {

            Base.AUTDTransPosition(AUTDControllerHandle.CntPtr, deviceIdx, transIdxLocal, out var x, out var y, out var z);

            return Adjust(x, y, z);
        }

        public double Wavelength(int deviceIdx, int transIdxLocal, double soundSpeed)
        {
            return Base.AUTDGetWavelength(AUTDControllerHandle.CntPtr, deviceIdx, transIdxLocal, soundSpeed);
        }

        public double TransFrequency(int deviceIdx, int transIdxLocal)
        {
            return Base.AUTDGetTransFrequency(AUTDControllerHandle.CntPtr, deviceIdx, transIdxLocal);
        }

        public void SetTransFrequency(int deviceIdx, int transIdxLocal, double freq)
        {
            Base.AUTDSetTransFrequency(AUTDControllerHandle.CntPtr, deviceIdx, transIdxLocal, freq);
        }
        public ushort TransCycle(int deviceIdx, int transIdxLocal)
        {
            return Base.AUTDGetTransCycle(AUTDControllerHandle.CntPtr, deviceIdx, transIdxLocal);
        }

        public void SetTransCycle(int deviceIdx, int transIdxLocal, ushort cycle)
        {
            Base.AUTDSetTransCycle(AUTDControllerHandle.CntPtr, deviceIdx, transIdxLocal, cycle);
        }

        public Vector3 TransDirectionX(int deviceIdx, int transIdxLocal)
        {
            Base.AUTDTransXDirection(AUTDControllerHandle.CntPtr, deviceIdx, transIdxLocal, out var x, out var y, out var z);
            return Adjust(x, y, z, false);
        }

        public Vector3 TransDirectionY(int deviceIdx, int transIdxLocal)
        {
            Base.AUTDTransYDirection(AUTDControllerHandle.CntPtr, deviceIdx, transIdxLocal, out var x, out var y, out var z);
            return Adjust(x, y, z, false);
        }

        public Vector3 TransDirectionZ(int deviceIdx, int transIdxLocal)
        {
            Base.AUTDTransZDirection(AUTDControllerHandle.CntPtr, deviceIdx, transIdxLocal, out var x, out var y, out var z);
            return Adjust(x, y, z, false);
        }

        #region GeometryAdjust
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static (double, double, double) Adjust(Vector3 vector, bool scaling = true)
        {
#if LEFT_HANDED
            vector.z = -vector.z;
#endif
#if DIMENSION_M
            if (scaling) vector = vector * MeterScale;
#endif
#if USE_SINGLE
            return ((double)vector.x, (double)vector.y, (double)vector.z);
#else
            return (vector.x, vector.y, vector.z);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Vector3 Adjust(double x, double y, double z, bool scaling = true)
        {
#if USE_SINGLE
            var vector = new Vector3((float)x, (float)y, (float)z);
#else
            var vector = new Vector3(x, y, z);
#endif
#if LEFT_HANDED
            vector.z = -vector.z;
#endif
#if DIMENSION_M
            if (scaling) vector /= MeterScale;
#endif
            return vector;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static (double, double, double, double) Adjust(Quaternion quaternion)
        {
#if LEFT_HANDED
            quaternion.z = -quaternion.z;
            quaternion.w = -quaternion.w;
#endif
#if USE_SINGLE
            return ((double)quaternion.w, (double)quaternion.x, (double)quaternion.y, (double)quaternion.z);
#else
            return (quaternion.w, quaternion.x, quaternion.y, quaternion.z);
#endif
        }
        #endregion
    }



    [ComVisible(false)]
    public abstract class GainNormal : Body
    {
        internal GainNormal() : base()
        {
        }

        protected override bool ReleaseHandle()
        {
            Base.AUTDDeleteGain(handle);
            return true;
        }
    }

    public sealed class FocusNormal : Gain
    {
        public FocusNormal(Vector3 point, double amp = 1.0) : base()
        {
            var (x, y, z) = Controller.Adjust(point);
            Base.AUTDGainFocus(out handle, x, y, z, amp);
        }
    }

    public sealed class GroupedNormal : Gain
    {
        public GroupedNormal(Controller cnt) : base()
        {
            Base.AUTDGainGrouped(out handle, cnt.AUTDControllerHandle.CntPtr);
        }

        public void Add(int deviceIdx, Gain gain)
        {
            Base.AUTDGainGroupedAdd(handle, deviceIdx, gain.Ptr);
        }
    }

    public sealed class BesselBeamNormal : Gain
    {
        public BesselBeamNormal(Vector3 point, Vector3 dir, double thetaZ, double amp = 1.0) : base()
        {
            var (x, y, z) = Controller.Adjust(point);
            var (dx, dy, dz) = Controller.Adjust(dir, false);
            Base.AUTDGainBesselBeam(out handle, x, y, z, dx, dy, dz, thetaZ, amp);
        }
    }

    public sealed class PlaneWaveNormal : Gain
    {
        public PlaneWaveNormal(Vector3 dir, double amp = 1.0) : base()
        {
            var (dx, dy, dz) = Controller.Adjust(dir, false);
            Base.AUTDGainPlaneWave(out handle, dx, dy, dz, amp);
        }
    }

    public sealed class CustomGainNormal : Gain
    {
        public CustomGainNormal(double[] amp, double[] phase) : base()
        {
            if (amp.Length != phase.Length) throw new ArgumentException();
            var length = amp.Length;
            Base.AUTDGainCustom(out handle, amp, phase, length);
        }
    }

    public sealed class NullNormal : Gain
    {
        public NullNormal() : base()
        {
            Base.AUTDGainNull(out handle);
        }
    }

    [ComVisible(false)]
    public abstract class ModulationNormal : Header
    {
        internal ModulationNormal() : base()
        {
        }

        protected override bool ReleaseHandle()
        {
            Base.AUTDDeleteModulation(handle);
            return true;
        }

        public double SamplingFrequency => Base.AUTDModulationSamplingFrequency(handle);
        public uint SamplingFrequencyDivision
        {
            get => Base.AUTDModulationSamplingFrequencyDivision(handle);
            set => Base.AUTDModulationSetSamplingFrequencyDivision(handle, value);
        }
    }

    public sealed class StaticNormal : Modulation
    {
        public StaticNormal(double amp = 1.0) : base()
        {
            Base.AUTDModulationStatic(out handle, amp);

        }
    }

    public sealed class SineNormal : Modulation
    {
        public SineNormal(int freq, double amp = 1.0, double offset = 0.5) : base()
        {
            Base.AUTDModulationSine(out handle, freq, amp, offset);
        }
    }

    public sealed class SineSquaredNormal : Modulation
    {
        public SineSquaredNormal(int freq, double amp = 1.0, double offset = 0.5) : base()
        {
            Base.AUTDModulationSineSquared(out handle, freq, amp, offset);
        }
    }

    public sealed class SineLegacyNormal : Modulation
    {
        public SineLegacyNormal(int freq, double amp = 1.0, double offset = 0.5) : base()
        {
            Base.AUTDModulationSineLegacy(out handle, freq, amp, offset);
        }
    }


    public sealed class SquareNormal : Modulation
    {
        public SquareNormal(int freq, double low = 0.0, double high = 1.0, double duty = 0.5) : base()
        {
            Base.AUTDModulationSquare(out handle, freq, low, high, duty);
        }
    }

    public sealed class CustomModulationNormal : Modulation
    {
        public CustomModulationNormal(byte[] data, uint freqDiv) : base()
        {
            Base.AUTDModulationCustom(out handle, data, (ulong)data.Length, freqDiv);
        }
    }

    public abstract class STMNormal : Body
    {
        public STMNormal() : base()
        {
        }

        protected override bool ReleaseHandle()
        {
            Base.AUTDDeleteSTM(handle);
            return true;
        }

        public double Frequency
        {
            get => Base.AUTDSTMFrequency(handle);
            set => Base.AUTDSTMSetFrequency(handle, value);
        }

        public double SamplingFrequency => Base.AUTDSTMSamplingFrequency(handle);
        public uint SamplingFrequencyDivision
        {
            get => Base.AUTDSTMSamplingFrequencyDivision(handle);
            set => Base.AUTDSTMSetSamplingFrequencyDivision(handle, value);
        }
    }

    public sealed class PointSTMNormal : STM
    {
        public PointSTMNormal() : base()
        {
            Base.AUTDPointSTM(out handle);
        }

        public bool Add(Vector3 point, byte shift = 0)
        {
            var (x, y, z) = Controller.Adjust(point);
            return Base.AUTDPointSTMAdd(handle, x, y, z, shift);
        }
    }

    public sealed class GainSTMNormal : STM
    {
        public GainSTMNormal(Controller cnt) : base()
        {
            Base.AUTDGainSTM(out handle, cnt.AUTDControllerHandle.CntPtr);
        }

        public bool Add(Gain gain)
        {
            return Base.AUTDGainSTMAdd(handle, gain.Ptr);
        }
    }

    public sealed class SilencerConfigNormal : Header
    {
        public SilencerConfigNormal(ushort step = 10, ushort cycle = 4096) : base()
        {
            Base.AUTDCreateSilencer(out handle, step, cycle);
        }

        protected override bool ReleaseHandle()
        {
            Base.AUTDDeleteSilencer(handle);
            return true;
        }

        public static SilencerConfig None()
        {
            return new SilencerConfig(0xFFFF, 4096);
        }
    }
}
