/*
 * File: Gain.cs
 * Project: src
 * Created Date: 28/04/2021
 * Author: Shun Suzuki
 * -----
 * Last Modified: 03/06/2021
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
using Vector3 = UnityEngine.Vector3;
using Quaternionf = UnityEngine.Quaternion;
using MathF = UnityEngine.Mathf;
#else
using Vector3 = AUTD3Sharp.Utils.Vector3d;
#endif

namespace AUTD3Sharp
{
    [ComVisible(false)]
    public class Gain : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal IntPtr GainPtr => handle;

        internal Gain(IntPtr gain) : base(true)
        {
            SetHandle(gain);
        }

        protected override bool ReleaseHandle()
        {
            NativeMethods.AUTDDeleteGain(handle);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte ToDuty(double amp)
        {
            var d = Math.Asin(amp) / AUTD.Pi;
            return (byte)(511.0 * d);
        }

        public static Gain FocalPoint(Vector3 point, double amp) => FocalPoint(point, ToDuty(amp));
        public static Gain FocalPoint(Vector3 point, byte duty = 0xff)
        {
            var (x, y, z) = AUTD.Adjust(point);
            NativeMethods.AUTDGainFocalPoint(out var gainPtr, x, y, z, duty);
            return new Gain(gainPtr);
        }

        public static Gain Grouped(params GainPair[] gainPairs)
        {
            if (gainPairs == null) throw new ArgumentNullException(nameof(gainPairs));
            NativeMethods.AUTDGainGrouped(out var gainPtr);
            foreach (var gainPair in gainPairs)
                NativeMethods.AUTDGainGroupedAdd(gainPtr, gainPair.Id, gainPair.Gain.GainPtr);
            return new Gain(gainPtr);
        }

        public static Gain BesselBeam(Vector3 point, Vector3 dir, double thetaZ, double amp) => BesselBeam(point, dir, thetaZ, ToDuty(amp));
        public static Gain BesselBeam(Vector3 point, Vector3 dir, double thetaZ, byte duty = 0xFF)
        {
            var (x, y, z) = AUTD.Adjust(point);
            var (dx, dy, dz) = AUTD.Adjust(dir, false);
            NativeMethods.AUTDGainBesselBeam(out var gainPtr, x, y, z, dx, dy, dz, thetaZ, duty);
            return new Gain(gainPtr);
        }

        public static Gain PlaneWave(Vector3 dir, double amp) => PlaneWave(dir, ToDuty(amp));
        public static Gain PlaneWave(Vector3 dir, byte duty = 0xFF)
        {
            var (dx, dy, dz) = AUTD.Adjust(dir, false);
            NativeMethods.AUTDGainPlaneWave(out var gainPtr, dx, dy, dz, duty);
            return new Gain(gainPtr);
        }

        public static Gain Holo(Vector3[] focuses, double[] amps) => HoloSDP(focuses, amps);

        private static double[] PackFoci(Vector3[] focuses)
        {
            var size = focuses.Length;
            var foci = new double[size * 3];
            for (var i = 0; i < size; i++)
            {
                var (x, y, z) = AUTD.Adjust(focuses[i]);
                foci[3 * i] = x;
                foci[3 * i + 1] = y;
                foci[3 * i + 2] = z;
            }
            return foci;
        }

        private static void CheckFociAmps(Vector3[] focuses, double[] amps)
        {
            if (focuses == null) throw new ArgumentNullException(nameof(focuses));
            if (amps == null) throw new ArgumentNullException(nameof(amps));
            if (focuses.Length != amps.Length) throw new ArgumentException("The number of foci and amplitudes must be the same.");
        }

        private static IntPtr GetEigen3Backend()
        {
            NativeMethods.AUTDEigen3Backend(out var backend);
            return backend;
        }

        public static Gain HoloSDP(Vector3[] focuses, double[] amps, double alpha = 1e-3,
            double lambda = 0.9, ulong repeat = 100, bool normalize = false)
        {
            return HoloSDP(focuses, amps, GetEigen3Backend(), alpha, lambda, repeat, normalize);
        }

        public static Gain HoloSDP(Vector3[] focuses, double[] amps, IntPtr backend, double alpha = 1e-3, double lambda = 0.9, ulong repeat = 100, bool normalize = false)
        {
            CheckFociAmps(focuses, amps);

            var size = amps.Length;
            var foci = PackFoci(focuses);
            IntPtr gainPtr;
            unsafe
            {
                fixed (double* fp = &foci[0])
                fixed (double* ap = &amps[0])
                    NativeMethods.AUTDGainHoloSDP(out gainPtr, backend, fp, ap, size, alpha, lambda, repeat, normalize);
            }
            NativeMethods.AUTDDeleteBackend(backend);
            return new Gain(gainPtr);
        }

        public static Gain HoloEVD(Vector3[] focuses, double[] amps, double gamma = 1,
            bool normalize = true)
        {
            return HoloEVD(focuses, amps, GetEigen3Backend(), gamma,
                normalize);
        }

        public static Gain HoloEVD(Vector3[] focuses, double[] amps, IntPtr backend, double gamma = 1, bool normalize = true)
        {
            CheckFociAmps(focuses, amps);

            var size = amps.Length;
            var foci = PackFoci(focuses);
            IntPtr gainPtr;
            unsafe
            {
                fixed (double* fp = &foci[0])
                fixed (double* ap = &amps[0])
                    NativeMethods.AUTDGainHoloEVD(out gainPtr, backend, fp, ap, size, gamma, normalize);
            }
            return new Gain(gainPtr);
        }

        public static Gain HoloGS(Vector3[] focuses, double[] amps, ulong repeat = 100)
        {
            return HoloGS(focuses, amps, GetEigen3Backend(), repeat);
        }

        public static Gain HoloGS(Vector3[] focuses, double[] amps, IntPtr backend, ulong repeat = 100)
        {
            CheckFociAmps(focuses, amps);

            var size = amps.Length;
            var foci = PackFoci(focuses);
            IntPtr gainPtr;
            unsafe
            {
                fixed (double* fp = &foci[0])
                fixed (double* ap = &amps[0])
                    NativeMethods.AUTDGainHoloGS(out gainPtr, backend, fp, ap, size, repeat);
            }
            return new Gain(gainPtr);
        }

        public static Gain HoloGSPAT(Vector3[] focuses, double[] amps, uint repeat = 100)
        {
            return HoloGSPAT(focuses, amps, GetEigen3Backend(), repeat);
        }

        public static Gain HoloGSPAT(Vector3[] focuses, double[] amps, IntPtr backend, uint repeat = 100)
        {
            CheckFociAmps(focuses, amps);

            var size = amps.Length;
            var foci = PackFoci(focuses);
            IntPtr gainPtr;
            unsafe
            {
                fixed (double* fp = &foci[0])
                fixed (double* ap = &amps[0])
                    NativeMethods.AUTDGainHoloGSPAT(out gainPtr, backend, fp, ap, size, repeat);
            }
            return new Gain(gainPtr);
        }

        public static Gain HoloNaive(Vector3[] focuses, double[] amps)
        {
            return HoloNaive(focuses, amps, GetEigen3Backend());
        }

        public static Gain HoloNaive(Vector3[] focuses, double[] amps, IntPtr backend)
        {
            CheckFociAmps(focuses, amps);

            var size = amps.Length;
            var foci = PackFoci(focuses);
            IntPtr gainPtr;
            unsafe
            {
                fixed (double* fp = &foci[0])
                fixed (double* ap = &amps[0])
                    NativeMethods.AUTDGainHoloNaive(out gainPtr, backend, fp, ap, size);
            }
            return new Gain(gainPtr);
        }

        public static Gain HoloLM(Vector3[] focuses, double[] amps, double eps1 = 1e-8, double eps2 = 1e-8,
            double tau = 1e-3, ulong kMax = 5, double[]? initial = null)
        {
            return HoloLM(focuses, amps, GetEigen3Backend(), eps1, eps2, tau, kMax, initial);
        }

        public static Gain HoloLM(Vector3[] focuses, double[] amps, IntPtr backend, double eps1 = 1e-8, double eps2 = 1e-8, double tau = 1e-3, ulong kMax = 5, double[]? initial = null)
        {
            CheckFociAmps(focuses, amps);

            var size = amps.Length;
            var foci = PackFoci(focuses);
            IntPtr gainPtr;
            unsafe
            {
                if (initial == null)
                {
                    fixed (double* fp = &foci[0])
                    fixed (double* ap = &amps[0])
                        NativeMethods.AUTDGainHoloLM(out gainPtr, backend, fp, ap, size, eps1, eps2, tau, kMax, null, 0);
                }
                else
                {
                    fixed (double* fp = &foci[0])
                    fixed (double* ap = &amps[0])
                    fixed (double* ip = &initial[0])
                        NativeMethods.AUTDGainHoloLM(out gainPtr, backend, fp, ap, size, eps1, eps2, tau, kMax, ip, initial.Length);
                }
            }
            return new Gain(gainPtr);
        }

        public static Gain HoloGreedy(Vector3[] focuses, double[] amps, int phaseDiv = 16)
        {
            CheckFociAmps(focuses, amps);

            var size = amps.Length;
            var foci = PackFoci(focuses);
            IntPtr gainPtr;
            unsafe
            {
                fixed (double* fp = &foci[0])
                fixed (double* ap = &amps[0])
                    NativeMethods.AUTDGainHoloGreedy(out gainPtr, fp, ap, size, phaseDiv);
            }
            return new Gain(gainPtr);
        }

        public static Gain TransducerTest(int index, byte duty, byte phase)
        {
            NativeMethods.AUTDGainTransducerTest(out var gainPtr, index, duty, phase);
            return new Gain(gainPtr);
        }

        public static Gain Null()
        {
            NativeMethods.AUTDGainNull(out var gainPtr);
            return new Gain(gainPtr);
        }

        public static Gain Custom(ushort[,] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            IntPtr gainPtr;
            var length = data.GetLength(0) * data.GetLength(1);
            unsafe
            {
                fixed (ushort* r = data) NativeMethods.AUTDGainCustom(out gainPtr, r, length);
            }

            return new Gain(gainPtr);
        }
    }
}
