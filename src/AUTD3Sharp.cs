/*
 * File: AUTD3Sharp.cs
 * Project: csharp
 * Created Date: 02/07/2018
 * Author: Shun Suzuki
 * -----
 * Last Modified: 07/09/2021
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2018-2019 Hapis Lab. All rights reserved.
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

[assembly: CLSCompliant(false), ComVisible(false)]
namespace AUTD3Sharp
{
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
    public class STMController : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal STMController(IntPtr ptr) : base(true)
        {
            handle = ptr;
        }

        public bool AddSTMGain(Gain gain)
        {
            if (gain == null) throw new ArgumentNullException(nameof(gain));
            return NativeMethods.AUTDAddSTMGain(handle, gain.GainPtr);
        }

        public bool StartSTM(double freq) => NativeMethods.AUTDStartSTM(handle, freq);

        public bool StopSTM() => NativeMethods.AUTDStopSTM(handle);

        public bool FinishSTM() => NativeMethods.AUTDFinishSTM(handle);

        protected override bool ReleaseHandle()
        {
            return true;
        }
    }

    public sealed class AUTD : IDisposable
    {
        #region const

#if USE_SINGLE
#if DIMENSION_M
        public const float MeterScale = 1000.0f;
#else
        public const float MeterScale = 1;
#endif
        public const float AUTDWidth = 192.0f / MeterScale;
        public const float AUTDHeight = 151.4f / MeterScale;
        public const float TransSize = 10.16f / MeterScale;
        public const float Pi = Math.PI;
#else
#if DIMENSION_M
        public const double MeterScale = 1000.0;
#else
        public const double MeterScale = 1;
#endif
        public const double AUTDWidth = 192.0 / MeterScale;
        public const double AUTDHeight = 151.4 / MeterScale;
        public const double TransSize = 10.16 / MeterScale;
        public const double Pi = Math.PI;
#endif
        public const int NumTransInDevice = 249;
        public const int NumTransInX = 18;
        public const int NumTransInY = 14;

        #endregion

        #region field

        private bool _isDisposed;
        internal readonly AUTDControllerHandle AUTDControllerHandle;

        #endregion

        #region Controller

        public AUTD()
        {
            AUTDControllerHandle = new AUTDControllerHandle(true);
        }

        public bool Open(Link link) => NativeMethods.AUTDOpenController(AUTDControllerHandle.CntPtr, link.LinkPtr);

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
            var size = NativeMethods.AUTDGetFirmwareInfoListPointer(AUTDControllerHandle.CntPtr, out var handle);
            for (var i = 0; i < size; i++)
            {
                var sbCpu = new StringBuilder(128);
                var sbFpga = new StringBuilder(128);
                NativeMethods.AUTDGetFirmwareInfo(handle, i, sbCpu, sbFpga);
                yield return new FirmwareInfo(sbCpu.ToString(), sbFpga.ToString());
            }

            NativeMethods.AUTDFreeFirmwareInfoListPointer(handle);
        }

#if UNITY_2018_3_OR_NEWER
#else
        public int AddDevice(Vector3 position, Vector3 rotation, int groupId = 0)
        {
            var (x, y, z) = Adjust(position);
            var (rx, ry, rz) = Adjust(rotation, false);
            return NativeMethods.AUTDAddDevice(AUTDControllerHandle.CntPtr, x, y, z, rx, ry, rz, groupId);
        }
#endif

        public int AddDevice(Vector3 position, Quaternion quaternion, int groupId = 0)
        {
            var (x, y, z) = Adjust(position);
            var (qw, qx, qy, qz) = Adjust(quaternion);
            return NativeMethods.AUTDAddDeviceQuaternion(AUTDControllerHandle.CntPtr, x, y, z, qw, qx, qy, qz,
                groupId);
        }

        public int DeleteDevice(int idx) => NativeMethods.AUTDDeleteDevice(AUTDControllerHandle.CntPtr, idx);

        public void ClearDevices() => NativeMethods.AUTDClearDevices(AUTDControllerHandle.CntPtr);

        public bool Close() => NativeMethods.AUTDCloseController(AUTDControllerHandle.CntPtr);

        public bool Clear() => NativeMethods.AUTDClear(AUTDControllerHandle.CntPtr);

        public bool Stop() => NativeMethods.AUTDStop(AUTDControllerHandle.CntPtr);
        public bool Pause() => NativeMethods.AUTDPause(AUTDControllerHandle.CntPtr);
        public bool Resume() => NativeMethods.AUTDResume(AUTDControllerHandle.CntPtr);

        public bool SetOutputDelay(byte[,] delays)
        {
            if (delays.GetLength(0) != NumDevices) throw new ArgumentException("The number of devices are incorrect.");
            if (delays.GetLength(1) != NumTransInDevice)
                throw new ArgumentException("The number of transducers are incorrect.");
            unsafe
            {
                fixed (byte* p = delays)
                    return NativeMethods.AUTDSetOutputDelay(AUTDControllerHandle.CntPtr, p);
            }
        }

        public bool SetDutyOffset(byte[,] offset)
        {
            if (offset.GetLength(0) != NumDevices) throw new ArgumentException("The number of devices are incorrect.");
            if (offset.GetLength(1) != NumTransInDevice)
                throw new ArgumentException("The number of transducers are incorrect.");
            unsafe
            {
                fixed (byte* p = offset)
                    return NativeMethods.AUTDSetDutyOffset(AUTDControllerHandle.CntPtr, p);
            }
        }

        public bool SetDelayOffset(byte[,] delay, byte[,] offset)
        {
            if (delay.GetLength(0) != NumDevices || offset.GetLength(0) != NumDevices) throw new ArgumentException("The number of devices are incorrect.");
            if (delay.GetLength(1) != NumTransInDevice || offset.GetLength(1) != NumTransInDevice)
                throw new ArgumentException("The number of transducers are incorrect.");
            unsafe
            {
                fixed (byte* pd = delay)
                fixed (byte* po = offset)
                    return NativeMethods.AUTDSetDelayOffset(AUTDControllerHandle.CntPtr, pd, po);
            }
        }

        public bool UpdateControlFlags() => NativeMethods.AUTDUpdateCtrlFlags(AUTDControllerHandle.CntPtr);

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

        ~AUTD()
        {
            Dispose(false);
        }

        #endregion

        #region Property

        public bool IsOpen => NativeMethods.AUTDIsOpen(AUTDControllerHandle.CntPtr);

        public bool SilentMode
        {
            get => NativeMethods.AUTDIsSilentMode(AUTDControllerHandle.CntPtr);
            set => NativeMethods.AUTDSetSilentMode(AUTDControllerHandle.CntPtr, value);
        }

        public bool ForceFan
        {
            get => NativeMethods.AUTDIsForceFan(AUTDControllerHandle.CntPtr);
            set => NativeMethods.AUTDSetForceFan(AUTDControllerHandle.CntPtr, value);
        }

        public bool ReadsFPGAInfo
        {
            get => NativeMethods.AUTDIsReadsFPGAInfo(AUTDControllerHandle.CntPtr);
            set => NativeMethods.AUTDSetReadsFPGAInfo(AUTDControllerHandle.CntPtr, value);
        }

        public byte[] FPGAInfo
        {
            get
            {
                var infos = new byte[NumDevices];
                unsafe
                {
                    fixed (byte* p = infos)
                        NativeMethods.AUTDGetFPGAInfo(AUTDControllerHandle.CntPtr, p);
                }
                return infos;
            }
        }

        public int NumDevices => NativeMethods.AUTDNumDevices(AUTDControllerHandle.CntPtr);
        public int NumTransducers => NativeMethods.AUTDNumTransducers(AUTDControllerHandle.CntPtr);
        public double Wavelength
        {
            get => NativeMethods.AUTDGetWavelength(AUTDControllerHandle.CntPtr);
            set => NativeMethods.AUTDSetWavelength(AUTDControllerHandle.CntPtr, value);
        }
        public double Attenuation
        {
            get => NativeMethods.AUTDGetAttenuation(AUTDControllerHandle.CntPtr);
            set => NativeMethods.AUTDSetAttenuation(AUTDControllerHandle.CntPtr, value);
        }

        public static string LastError
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

        #region LowLevelInterface
        public bool Send(Gain gain, bool wait_for_msg_processed = true)
        {
            if (gain == null) throw new ArgumentNullException(nameof(gain));
            return NativeMethods.AUTDSendGain(AUTDControllerHandle.CntPtr, gain.GainPtr, wait_for_msg_processed);
        }
        public bool Send(Modulation mod)
        {
            if (mod == null) throw new ArgumentNullException(nameof(mod));
            return NativeMethods.AUTDSendModulation(AUTDControllerHandle.CntPtr, mod.ModPtr);
        }
        public bool Send(Gain gain, Modulation mod, bool wait_for_msg_processed = true)
        {
            if (gain == null) throw new ArgumentNullException(nameof(gain));
            if (mod == null) throw new ArgumentNullException(nameof(mod));
            return NativeMethods.AUTDSendGainModulation(AUTDControllerHandle.CntPtr, gain.GainPtr, mod.ModPtr, wait_for_msg_processed);
        }

        public bool Send(PointSequence seq)
        {
            if (seq == null) throw new ArgumentNullException(nameof(seq));
            return NativeMethods.AUTDSendSequence(AUTDControllerHandle.CntPtr, seq.SeqPtr);
        }

        public bool Send(GainSequence seq)
        {
            if (seq == null) throw new ArgumentNullException(nameof(seq));
            return NativeMethods.AUTDSendGainSequence(AUTDControllerHandle.CntPtr, seq.SeqPtr);
        }

        public STMController STM()
        {
            NativeMethods.AUTDSTMController(out var handle, AUTDControllerHandle.CntPtr);
            return new STMController(handle);
        }

        public int DeviceIdxForTransIdx(int devIdx) => NativeMethods.AUTDDeviceIdxForTransIdx(AUTDControllerHandle.CntPtr, devIdx);

        public Vector3 TransPosition(int transIdxGlobal)
        {
            double x = 0;
            double y = 0;
            double z = 0;
            unsafe
            {
                NativeMethods.AUTDTransPositionByGlobal(AUTDControllerHandle.CntPtr, transIdxGlobal, &x, &y, &z);
            }
            return Adjust(x, y, z);
        }

        public Vector3 TransPosition(int deviceIdx, int transIdxLocal)
        {
            double x = 0;
            double y = 0;
            double z = 0;
            unsafe
            {
                NativeMethods.AUTDTransPositionByLocal(AUTDControllerHandle.CntPtr, deviceIdx, transIdxLocal, &x, &y,
                    &z);
            }

            return Adjust(x, y, z);
        }

        public Vector3 DeviceDirectionX(int deviceIdx)
        {
            double x = 0;
            double y = 0;
            double z = 0;
            unsafe
            {

                NativeMethods.AUTDDeviceXDirection(AUTDControllerHandle.CntPtr, deviceIdx, &x, &y, &z);
            }

            return Adjust(x, y, z, false);
        }

        public Vector3 DeviceDirectionY(int deviceIdx)
        {
            double x = 0;
            double y = 0;
            double z = 0;
            unsafe
            {
                NativeMethods.AUTDDeviceYDirection(AUTDControllerHandle.CntPtr, deviceIdx, &x, &y, &z);
            }

            return Adjust(x, y, z, false);
        }

        public Vector3 DeviceDirectionZ(int deviceIdx)
        {
            double x = 0;
            double y = 0;
            double z = 0;
            unsafe
            {
                NativeMethods.AUTDDeviceZDirection(AUTDControllerHandle.CntPtr, deviceIdx, &x, &y, &z);
            }

            return Adjust(x, y, z, false);
        }
        #endregion

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
}
