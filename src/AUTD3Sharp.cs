/*
 * File: AUTD3Sharp.cs
 * Project: csharp
 * Created Date: 02/07/2018
 * Author: Shun Suzuki
 * -----
 * Last Modified: 01/05/2021
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2018-2019 Hapis Lab. All rights reserved.
 * 
 */


#if UNITY_2018_3_OR_NEWER
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
using AUTD3Sharp.Utils;

#if UNITY_2018_3_OR_NEWER
using UnityEngine;
using Vector3f = UnityEngine.Vector3;
using Quaternionf = UnityEngine.Quaternion;
using MathF = UnityEngine.Mathf;
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

    public sealed class AUTD : IDisposable
    {
        #region const

#if DIMENSION_M
        public const float AUTDWidth = 0.192f;
        public const float AUTDHeight = 0.1514f;
        public const float TransSize = 0.01016f;
        public readonly static float MeterScale = 1000f;
#else
        public const float AUTDWidth = 192.0f;
        public const float AUTDHeight = 151.4f;
        public const float TransSize = 10.16f;
#endif
        public const float Pi = MathF.PI;
        public const int NumTransInDevice = 249;
        public const int NumTransInX = 18;
        public const int NumTransInY = 14;
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

        public bool OpenWith(Link link) => NativeMethods.AUTDOpenControllerWith(_autdControllerHandle.CntPtr, link.LinkPtr);

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

        public int AddDevice(Vector3f position, Vector3f rotation, int groupId = 0)
        {
            AdjustVector(ref position);
            return NativeMethods.AUTDAddDevice(_autdControllerHandle.CntPtr, position[0], position[1], position[2], rotation[0], rotation[1], rotation[2], groupId);
        }

        public int AddDevice(Vector3f position, Quaternionf quaternion, int groupId = 0)
        {
            AdjustVector(ref position);
            AdjustQuaternion(ref quaternion);
            return NativeMethods.AUTDAddDeviceQuaternion(_autdControllerHandle.CntPtr, position[0], position[1], position[2], quaternion[3], quaternion[0], quaternion[1], quaternion[2], groupId);
        }

        public int DeleteDevice(int idx) => NativeMethods.AUTDDeleteDevice(_autdControllerHandle.CntPtr, idx);

        public void ClearDevices() => NativeMethods.AUTDClearDevices(_autdControllerHandle.CntPtr);

        public bool Synchronize() => Synchronize(new Configuration());

        public bool Synchronize(Configuration config) => NativeMethods.AUTDSynchronize(_autdControllerHandle.CntPtr, (int)config.ModSamplingFrequency, (int)config.ModBufferSize);

        public bool Close() => NativeMethods.AUTDCloseController(_autdControllerHandle.CntPtr);

        public bool Clear() => NativeMethods.AUTDClear(_autdControllerHandle.CntPtr);

        public bool Stop() => NativeMethods.AUTDStop(_autdControllerHandle.CntPtr);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_isDisposed) return;

            if (disposing) Close();

            _autdControllerHandle.Dispose();

            _isDisposed = true;
        }

        public void SetSilentMode(bool mode) => NativeMethods.AUTDSetSilentMode(_autdControllerHandle.CntPtr, mode);

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

        #region LowLevelInterface
        public bool AppendGain(Gain gain)
        {
            if (gain == null) throw new ArgumentNullException(nameof(gain));
            return NativeMethods.AUTDAppendGain(_autdControllerHandle.CntPtr, gain.GainPtr);
        }
        public bool AppendGainSync(Gain gain, bool waitForSend = false)
        {
            if (gain == null) throw new ArgumentNullException(nameof(gain));
            return NativeMethods.AUTDAppendGainSync(_autdControllerHandle.CntPtr, gain.GainPtr, waitForSend);
        }
        public bool AppendModulation(Modulation mod)
        {
            if (mod == null) throw new ArgumentNullException(nameof(mod));
            return NativeMethods.AUTDAppendModulation(_autdControllerHandle.CntPtr, mod.ModPtr);
        }

        public bool AppendModulationSync(Modulation mod)
        {
            if (mod == null) throw new ArgumentNullException(nameof(mod));
            return NativeMethods.AUTDAppendModulationSync(_autdControllerHandle.CntPtr, mod.ModPtr);
        }

        public void AddSTMGain(Gain gain)
        {
            if (gain == null) throw new ArgumentNullException(nameof(gain));
            NativeMethods.AUTDAddSTMGain(_autdControllerHandle.CntPtr, gain.GainPtr);
        }

        public void AddSTMGain(IList<Gain> gains)
        {
            if (gains == null) throw new ArgumentNullException(nameof(gains));
            foreach (var gain in gains) AddSTMGain(gain);
        }

        public void AddSTMGain(params Gain[] gainList)
        {
            if (gainList == null) throw new ArgumentNullException(nameof(gainList));
            foreach (var gain in gainList) AddSTMGain(gain);
        }

        public bool StartSTM(float freq) => NativeMethods.AUTDStartSTModulation(_autdControllerHandle.CntPtr, freq);

        public bool StopSTM() => NativeMethods.AUTDStopSTModulation(_autdControllerHandle.CntPtr);

        public bool FinishSTM() => NativeMethods.AUTDFinishSTModulation(_autdControllerHandle.CntPtr);

        public bool AppendSequence(PointSequence seq)
        {
            if (seq == null) throw new ArgumentNullException(nameof(seq));
            return NativeMethods.AUTDAppendSequence(_autdControllerHandle.CntPtr, seq.SeqPtr);
        }

        public void Flush() => NativeMethods.AUTDFlush(_autdControllerHandle.CntPtr);

        public int DeviceIdxForTransIdx(int devIdx) => NativeMethods.AUTDDeviceIdxForTransIdx(_autdControllerHandle.CntPtr, devIdx);

        public unsafe Vector3f TransPosition(int transIdxGlobal)
        {
            NativeMethods.AUTDTransPositionByGlobal(_autdControllerHandle.CntPtr, transIdxGlobal, out var x, out var y, out var z);
            return new Vector3f(x, y, z);
        }

        public unsafe Vector3f TransPosition(int deviceIdx, int transIdxLocal)
        {
            NativeMethods.AUTDTransPositionByLocal(_autdControllerHandle.CntPtr, deviceIdx, transIdxLocal, out var x, out var y, out var z);
            return new Vector3f(x, y, z);
        }

        public unsafe Vector3f DeviceDirection(int deviceIdx)
        {
            NativeMethods.AUTDDeviceDirection(_autdControllerHandle.CntPtr, deviceIdx, out var x, out var y, out var z);
            return new Vector3f(x, y, z);
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
