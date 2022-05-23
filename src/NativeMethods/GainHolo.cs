/*
 * File: GainHolo.cs
 * Project: NativeMethods
 * Created Date: 23/05/2022
 * Author: Shun Suzuki
 * -----
 * Last Modified: 23/05/2022
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2022 Hapis Lab. All rights reserved.
 * 
 */


using System;
using System.Runtime.InteropServices;

namespace AUTD3Sharp.NativeMethods
{
    internal static class GainHolo
    {
        [DllImport("autd3capi-gain-holo", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDEigenBackend(out IntPtr @out);
        [DllImport("autd3capi-gain-holo", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDDeleteBackend(IntPtr backend);
        [DllImport("autd3capi-gain-holo", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDAmplitudeConstraintDontCate(out IntPtr @out);
        [DllImport("autd3capi-gain-holo", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDAmplitudeConstraintNormalize(out IntPtr @out);
        [DllImport("autd3capi-gain-holo", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDAmplitudeConstraintUniform(out IntPtr @out, double value);
        [DllImport("autd3capi-gain-holo", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDAmplitudeConstraintClamp(out IntPtr @out);
        [DllImport("autd3capi-gain-holo", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDDeleteAmplitudeConstraint(IntPtr constraint);
        [DllImport("autd3capi-gain-holo", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDGainHoloSDP(out IntPtr gain, IntPtr backend, double alpha, double lambda, ulong repeat, IntPtr constraint);
        [DllImport("autd3capi-gain-holo", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDGainHoloEVD(out IntPtr gain, IntPtr backend, double gamma, IntPtr constraint);
        [DllImport("autd3capi-gain-holo", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDGainHoloNaive(out IntPtr gain, IntPtr backend, IntPtr constraint);
        [DllImport("autd3capi-gain-holo", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDGainHoloGS(out IntPtr gain, IntPtr backend, ulong repeat, IntPtr constraint);
        [DllImport("autd3capi-gain-holo", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDGainHoloGSPAT(out IntPtr gain, IntPtr backend, ulong repeat, IntPtr constraint);
        [DllImport("autd3capi-gain-holo", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDGainHoloLM(out IntPtr gain, IntPtr backend, double eps1, double eps2, double tau, ulong kMax, double[]? initial, int initialSize, IntPtr constraint);
        [DllImport("autd3capi-gain-holo", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDGainHoloGaussNewton(out IntPtr gain, IntPtr backend, double eps1, double eps2, ulong kMax, double[]? initial, int initialSize, IntPtr constraint);
        [DllImport("autd3capi-gain-holo", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDGainHoloGradientDescent(out IntPtr gain, IntPtr backend, double eps, double step, ulong kMax, double[]? initial, int initialSize, IntPtr constraint);
        [DllImport("autd3capi-gain-holo", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDGainHoloGreedy(out IntPtr gain, IntPtr backend, int phaseDiv, IntPtr constraint);
        [DllImport("autd3capi-gain-holo", CallingConvention = CallingConvention.Cdecl)] public static extern void AUTDGainHoloAdd(IntPtr gain, double x, double y, double z, double amp);
    }
}