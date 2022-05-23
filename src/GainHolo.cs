/*
 * File: GainHolo.cs
 * Project: src
 * Created Date: 23/05/2021
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
using Microsoft.Win32.SafeHandles;

#if UNITY_2018_3_OR_NEWER
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
#else
using Vector3 = AUTD3Sharp.Utils.Vector3d;
#endif

namespace AUTD3Sharp
{
    [ComVisible(false)]
    public abstract class BackendNormal : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal IntPtr Ptr => handle;

        internal BackendNormal() : base(true)
        {
            var ptr = new IntPtr();
            SetHandle(ptr);
        }

        protected override bool ReleaseHandle()
        {
            NativeMethods.GainHolo.AUTDDeleteBackend(handle);
            return true;
        }
    }

    public sealed class BackendEigenNormal : BackendNormal
    {
        public BackendEigenNormal() : base()
        {
            NativeMethods.GainHolo.AUTDEigenBackend(out handle);
        }
    }

    public abstract class AmplitudeConstraintNormal : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal IntPtr Ptr => handle;

        internal AmplitudeConstraintNormal() : base(true)
        {
            var ptr = new IntPtr();
            SetHandle(ptr);
        }

        protected override bool ReleaseHandle()
        {
            NativeMethods.GainHolo.AUTDDeleteAmplitudeConstraint(handle);
            return true;
        }
    }

    public sealed class DontCareNormal : AmplitudeConstraintNormal
    {
        public DontCareNormal() : base()
        {
            NativeMethods.GainHolo.AUTDAmplitudeConstraintDontCate(out handle);
        }
    }


    public sealed class NormalizeNormal : AmplitudeConstraintNormal
    {
        public NormalizeNormal() : base()
        {
            NativeMethods.GainHolo.AUTDAmplitudeConstraintNormalize(out handle);
        }
    }

    public sealed class UniformNormal : AmplitudeConstraintNormal
    {
        public UniformNormal(double value) : base()
        {
            NativeMethods.GainHolo.AUTDAmplitudeConstraintUniform(out handle, value);
        }
    }

    public sealed class ClampNormal : AmplitudeConstraintNormal
    {
        public ClampNormal() : base()
        {
            NativeMethods.GainHolo.AUTDAmplitudeConstraintClamp(out handle);
        }
    }

    public class HoloNormal : GainNormal
    {
        public HoloNormal() : base()
        {
            Backend = new BackendEigenNormal();
            Constraint = new NormalizeNormal();
        }

        public BackendNormal Backend { get; set; }
        public AmplitudeConstraintNormal Constraint { get; set; }

        public void Add(Vector3 focus, double amp)
        {
            var (x, y, z) = ControllerNormal.Adjust(focus);
            NativeMethods.GainHolo.AUTDGainHoloAdd(handle, x, y, z, amp);
        }
    }

    public sealed class SDPNormal : HoloNormal
    {
        public SDPNormal(double alpha = 1e-3, double lambda = 0.9, ulong repeat = 100) : base()
        {
            NativeMethods.GainHolo.AUTDGainHoloSDP(out handle, Backend.Ptr, alpha, lambda, repeat, Constraint.Ptr);
        }
    }
    public sealed class EVDNormal : HoloNormal
    {
        public EVDNormal(double gamma = 1.0) : base()
        {
            NativeMethods.GainHolo.AUTDGainHoloEVD(out handle, Backend.Ptr, gamma, Constraint.Ptr);
        }
    }
    public sealed class NaiveNormal : HoloNormal
    {
        public NaiveNormal() : base()
        {
            NativeMethods.GainHolo.AUTDGainHoloNaive(out handle, Backend.Ptr, Constraint.Ptr);
        }
    }

    public sealed class GSNormal : HoloNormal
    {
        public GSNormal(ulong repeat = 100) : base()
        {
            NativeMethods.GainHolo.AUTDGainHoloGS(out handle, Backend.Ptr, repeat, Constraint.Ptr);
        }
    }

    public sealed class GSPATNormal : HoloNormal
    {
        public GSPATNormal(ulong repeat = 100) : base()
        {
            NativeMethods.GainHolo.AUTDGainHoloGSPAT(out handle, Backend.Ptr, repeat, Constraint.Ptr);
        }
    }
    public sealed class LMNormal : HoloNormal
    {
        public LMNormal(double eps1 = 1e-8, double eps2 = 1e-8, double tau = 1e-3, ulong kMax = 5, double[]? initial = null) : base()
        {
            NativeMethods.GainHolo.AUTDGainHoloLM(out handle, Backend.Ptr, eps1, eps2, tau, kMax, initial, initial is null ? 0 : initial.Length, Constraint.Ptr);
        }
    }

    public sealed class GaussNewtonNormal : HoloNormal
    {
        public GaussNewtonNormal(double eps1 = 1e-6, double eps2 = 1e-6, ulong kMax = 500, double[]? initial = null) : base()
        {
            NativeMethods.GainHolo.AUTDGainHoloGaussNewton(out handle, Backend.Ptr, eps1, eps2, kMax, initial, initial is null ? 0 : initial.Length, Constraint.Ptr);
        }
    }
    public sealed class GradientDescentNormal : HoloNormal
    {
        public GradientDescentNormal(double eps = 1e-6, double step = 0.5, ulong kMax = 2000, double[]? initial = null) : base()
        {
            NativeMethods.GainHolo.AUTDGainHoloGradientDescent(out handle, Backend.Ptr, eps, step, kMax, initial, initial is null ? 0 : initial.Length, Constraint.Ptr);
        }
    }

    public sealed class GreedyNormal : HoloNormal
    {
        public GreedyNormal(int phaseDiv = 16) : base()
        {
            NativeMethods.GainHolo.AUTDGainHoloGreedy(out handle, Backend.Ptr, phaseDiv, Constraint.Ptr);
        }
    }
}
