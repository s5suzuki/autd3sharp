/*
 * File: Gain.cs
 * Project: src
 * Created Date: 28/04/2021
 * Author: Shun Suzuki
 * -----
 * Last Modified: 06/05/2021
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

        public static Gain HoloGain(Vector3f[] focuses, float[] amps) => HoloGainSDP(focuses, amps);

        public static Gain HoloGainSDP(Vector3f[] focuses, float[] amps, float alpha = 1e-3f, float lambda = 0.9f, ulong repeat = 100, bool normalize = true)
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
            unsafe
            {
                fixed (float* fp = &foci[0])
                fixed (float* ap = &amps[0])
                    NativeMethods.AUTDHoloGainSDP(out gainPtr, fp, ap, size, alpha, lambda, repeat, normalize);
            }
            return new Gain(gainPtr);
        }

        public static Gain HoloGainEVD(Vector3f[] focuses, float[] amps, float gamma = 1, bool normalize = true)
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
            unsafe
            {
                fixed (float* fp = &foci[0])
                fixed (float* ap = &amps[0])
                    NativeMethods.AUTDHoloGainEVD(out gainPtr, fp, ap, size, gamma, normalize);
            }
            return new Gain(gainPtr);
        }

        public static Gain HoloGainGS(Vector3f[] focuses, float[] amps, ulong repeat = 100)
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
            unsafe
            {
                fixed (float* fp = &foci[0])
                fixed (float* ap = &amps[0])
                    NativeMethods.AUTDHoloGainGS(out gainPtr, fp, ap, size, repeat);
            }
            return new Gain(gainPtr);
        }

        public static Gain HoloGainGSPAT(Vector3f[] focuses, float[] amps, uint repeat = 100)
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
            unsafe
            {
                fixed (float* fp = &foci[0])
                fixed (float* ap = &amps[0])
                    NativeMethods.AUTDHoloGainGSPAT(out gainPtr, fp, ap, size, repeat);
            }
            return new Gain(gainPtr);
        }

        public static Gain HoloGainNaive(Vector3f[] focuses, float[] amps)
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
            unsafe
            {
                fixed (float* fp = &foci[0])
                fixed (float* ap = &amps[0])
                    NativeMethods.AUTDHoloGainNaive(out gainPtr, fp, ap, size);
            }
            return new Gain(gainPtr);
        }

        public static Gain HoloGainLM(Vector3f[] focuses, float[] amps, float eps_1 = 1e-8f, float eps_2 = 1e-8f, float tau = 1e-3f, ulong k_max = 5, float[]? initial = null)
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
            unsafe
            {
                if (initial == null)
                {
                    fixed (float* fp = &foci[0])
                    fixed (float* ap = &amps[0])
                        NativeMethods.AUTDHoloGainLM(out gainPtr, fp, ap, size, eps_1, eps_2, tau, k_max, null, 0);
                }
                else
                {
                    fixed (float* fp = &foci[0])
                    fixed (float* ap = &amps[0])
                    fixed (float* ip = &initial[0])
                        NativeMethods.AUTDHoloGainLM(out gainPtr, fp, ap, size, eps_1, eps_2, tau, k_max, ip, initial.Length);
                }
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
            if (data == null) throw new ArgumentNullException(nameof(data));

            IntPtr gainPtr;
            var length = data.GetLength(0) * data.GetLength(1);
            fixed (ushort* r = data) NativeMethods.AUTDCustomGain(out gainPtr, r, length);
            return new Gain(gainPtr);
        }
    }
}
