/*
 * File: Gain.cs
 * Project: src
 * Created Date: 28/04/2021
 * Author: Shun Suzuki
 * -----
 * Last Modified: 02/05/2021
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2021 Hapis Lab. All rights reserved.
 * 
 */

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AUTD3Sharp.Utils;
using Microsoft.Win32.SafeHandles;

#if UNITY_2018_3_OR_NEWER
using UnityEngine;
using Vector3f = UnityEngine.Vector3;
using Quaternionf = UnityEngine.Quaternion;
using MathF = UnityEngine.Mathf;
#endif

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte AdjustAmp(float amp)
        {
            var d = MathF.Asin(amp) / MathF.PI;
            return (byte)(511.0 * d);
        }

        public static Gain FocalPointGain(Vector3f point, float amp) => FocalPointGain(point, AdjustAmp(amp));
        public static Gain FocalPointGain(Vector3f point, byte duty = 0xff)
        {
            AUTD.AdjustVector(ref point);
            NativeMethods.AUTDFocalPointGain(out var gainPtr, point[0], point[1], point[2], duty);
            return new Gain(gainPtr);
        }

        public static unsafe Gain GroupedGain(GainMap gainMap)
        {
            if (gainMap == null) throw new ArgumentNullException(nameof(gainMap));
            var gainsPtr = gainMap.GainPointer;
            var idPtr = gainMap.IdPointer;
            NativeMethods.AUTDGroupedGain(out var gainPtr, idPtr, gainsPtr, gainMap.Size);
            return new Gain(gainPtr);
        }

        public static Gain GroupedGain(params GainPair[] gainPairs) => GroupedGain(new GainMap(gainPairs));
        public static Gain BesselBeamGain(Vector3f point, Vector3f dir, float thetaZ, float amp) => BesselBeamGain(point, dir, thetaZ, AdjustAmp(amp));
        public static Gain BesselBeamGain(Vector3f point, Vector3f dir, float thetaZ, byte duty = 0xFF)
        {
            AUTD.AdjustVector(ref point);
            AUTD.AdjustVector(ref dir);
            NativeMethods.AUTDBesselBeamGain(out var gainPtr, point[0], point[1], point[2], dir[0], dir[1], dir[2], thetaZ, duty);
            return new Gain(gainPtr);
        }

        public static Gain PlaneWaveGain(Vector3f dir, float amp) => PlaneWaveGain(dir, AdjustAmp(amp));
        public static Gain PlaneWaveGain(Vector3f dir, byte duty = 0xFF)
        {
            AUTD.AdjustVector(ref dir);
            NativeMethods.AUTDPlaneWaveGain(out var gainPtr, dir[0], dir[1], dir[2], duty);
            return new Gain(gainPtr);
        }

        public enum OptMethod
        {
            SDP = 0,
            EVD = 1,
            GS = 2,
            GSPAT = 3,
            NAIVE = 4,
            LM = 5
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SDPParams
        {
            public float Regularization { get; set; }
            public int Repeat { get; set; }
            public float Lambda { get; set; }
            [field: MarshalAs(UnmanagedType.U1)] public bool NormalizeAmp { get; set; }

            public static SDPParams GetDefault()
            {
                return new SDPParams
                {
                    Regularization = -1,
                    Repeat = -1,
                    Lambda = -1,
                    NormalizeAmp = true
                };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct EVDParams
        {
            public float Regularization { get; set; }
            [field: MarshalAs(UnmanagedType.U1)] public bool NormalizeAmp { get; set; }

            public static EVDParams GetDefault()
            {
                return new EVDParams
                {
                    Regularization = -1,
                    NormalizeAmp = true
                };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct NLSParams
        {
            public float Eps1 { get; set; }
            public float Eps2 { get; set; }
            public int KMax { get; set; }
            public float Tau { get; set; }
            public float* Initial { get; set; }

            public static NLSParams GetDefault()
            {
                return new NLSParams
                {
                    Eps1 = -1,
                    Eps2 = -1,
                    KMax = -1,
                    Tau = -1,
                    Initial = null
                };
            }
        }

        public static Gain HoloGain(Vector3f[] focuses, float[] amps) => HoloGainSDP(focuses, amps, null);

        public static unsafe Gain HoloGainSDP(Vector3f[] focuses, float[] amps, SDPParams? param)
        {
            var p = Marshal.AllocHGlobal(sizeof(SDPParams));
            if (param.HasValue)
                Marshal.StructureToPtr(param.Value, p, false);
            else
                p = IntPtr.Zero;

            return HoloGain(focuses, amps, OptMethod.SDP, p);
        }

        public static unsafe Gain HoloGainEVD(Vector3f[] focuses, float[] amps, EVDParams? param)
        {
            var p = Marshal.AllocHGlobal(sizeof(EVDParams));
            if (param.HasValue)
                Marshal.StructureToPtr(param.Value, p, false);
            else
                p = IntPtr.Zero;

            return HoloGain(focuses, amps, OptMethod.EVD, p);
        }

        public static Gain HoloGainGS(Vector3f[] focuses, float[] amps, uint? repeat)
        {
            var p = Marshal.AllocHGlobal(sizeof(uint));
            if (repeat.HasValue)
                Marshal.StructureToPtr(repeat.Value, p, false);
            else
                p = IntPtr.Zero;

            return HoloGain(focuses, amps, OptMethod.GS, p);
        }

        public static Gain HoloGainGSPAT(Vector3f[] focuses, float[] amps, uint? repeat)
        {
            var p = Marshal.AllocHGlobal(sizeof(uint));
            if (repeat.HasValue)
                Marshal.StructureToPtr(repeat.Value, p, false);
            else
                p = IntPtr.Zero;

            return HoloGain(focuses, amps, OptMethod.GSPAT, p);
        }

        public static Gain HoloGainNaive(Vector3f[] focuses, float[] amps) => HoloGain(focuses, amps, OptMethod.NAIVE, IntPtr.Zero);

        public static unsafe Gain HoloGainLM(Vector3f[] focuses, float[] amps, NLSParams? param)
        {
            var p = Marshal.AllocHGlobal(sizeof(NLSParams));
            if (param.HasValue)
                Marshal.StructureToPtr(param.Value, p, false);
            else
                p = IntPtr.Zero;

            return HoloGain(focuses, amps, OptMethod.LM, p);
        }

        public static unsafe Gain HoloGain(Vector3f[] focuses, float[] amps, OptMethod method, IntPtr param)
        {
            if (focuses == null) throw new ArgumentNullException(nameof(focuses));
            if (amps == null) throw new ArgumentNullException(nameof(amps));

            var size = amps.Length;
            var foci = new float[size * 3];
            for (var i = 0; i < size; i++)
            {
                AUTD.AdjustVector(ref focuses[i]);
                foci[3 * i] = focuses[i][0];
                foci[3 * i + 1] = focuses[i][1];
                foci[3 * i + 2] = focuses[i][2];
            }

            IntPtr gainPtr;
            fixed (float* fp = &foci[0])
            fixed (float* ap = &amps[0])
                NativeMethods.AUTDHoloGain(out gainPtr, fp, ap, size, (int)method, param);
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
            if (data == null) throw new ArgumentNullException(nameof(data));

            IntPtr gainPtr;
            var length = data.GetLength(0) * data.GetLength(1);
            fixed (ushort* r = data) NativeMethods.AUTDCustomGain(out gainPtr, r, length);
            return new Gain(gainPtr);
        }
    }
}
