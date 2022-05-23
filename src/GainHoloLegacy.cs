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
    public abstract class Backend : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal IntPtr Ptr => handle;

        internal Backend() : base(true)
        {
            var ptr = new IntPtr();
            SetHandle(ptr);
        }

        protected override bool ReleaseHandle()
        {
            NativeMethods.GainHoloLegacy.AUTDDeleteBackend(handle);
            return true;
        }
    }

    public sealed class BackendEigen : Backend
    {
        public BackendEigen() : base()
        {
            NativeMethods.GainHoloLegacy.AUTDEigenBackend(out handle);
        }
    }

    public abstract class AmplitudeConstraint : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal IntPtr Ptr => handle;

        internal AmplitudeConstraint() : base(true)
        {
            var ptr = new IntPtr();
            SetHandle(ptr);
        }

        protected override bool ReleaseHandle()
        {
            NativeMethods.GainHoloLegacy.AUTDDeleteAmplitudeConstraint(handle);
            return true;
        }
    }

    public sealed class DontCare : AmplitudeConstraint
    {
        public DontCare() : base()
        {
            NativeMethods.GainHoloLegacy.AUTDAmplitudeConstraintDontCate(out handle);
        }
    }


    public sealed class Normalize : AmplitudeConstraint
    {
        public Normalize() : base()
        {
            NativeMethods.GainHoloLegacy.AUTDAmplitudeConstraintNormalize(out handle);
        }
    }

    public sealed class Uniform : AmplitudeConstraint
    {
        public Uniform(double value) : base()
        {
            NativeMethods.GainHoloLegacy.AUTDAmplitudeConstraintUniform(out handle, value);
        }
    }

    public sealed class Clamp : AmplitudeConstraint
    {
        public Clamp() : base()
        {
            NativeMethods.GainHoloLegacy.AUTDAmplitudeConstraintClamp(out handle);
        }
    }

    public class Holo : Gain
    {
        public Holo() : base()
        {
            Backend = new BackendEigen();
            Constraint = new Normalize();
        }

        public Backend Backend { get; set; }
        public AmplitudeConstraint Constraint { get; set; }

        public void Add(Vector3 focus, double amp)
        {
            var (x, y, z) = Controller.Adjust(focus);
            NativeMethods.GainHoloLegacy.AUTDGainHoloAdd(handle, x, y, z, amp);
        }
    }

    public sealed class SDP : Holo
    {
        public SDP(double alpha = 1e-3, double lambda = 0.9, ulong repeat = 100) : base()
        {
            NativeMethods.GainHoloLegacy.AUTDGainHoloSDP(out handle, Backend.Ptr, alpha, lambda, repeat, Constraint.Ptr);
        }
    }
    public sealed class EVD : Holo
    {
        public EVD(double gamma = 1.0) : base()
        {
            NativeMethods.GainHoloLegacy.AUTDGainHoloEVD(out handle, Backend.Ptr, gamma, Constraint.Ptr);
        }
    }
    public sealed class Naive : Holo
    {
        public Naive() : base()
        {
            NativeMethods.GainHoloLegacy.AUTDGainHoloNaive(out handle, Backend.Ptr, Constraint.Ptr);
        }
    }

    public sealed class GS : Holo
    {
        public GS(ulong repeat = 100) : base()
        {
            NativeMethods.GainHoloLegacy.AUTDGainHoloGS(out handle, Backend.Ptr, repeat, Constraint.Ptr);
        }
    }

    public sealed class GSPAT : Holo
    {
        public GSPAT(ulong repeat = 100) : base()
        {
            NativeMethods.GainHoloLegacy.AUTDGainHoloGSPAT(out handle, Backend.Ptr, repeat, Constraint.Ptr);
        }
    }
    public sealed class LM : Holo
    {
        public LM(double eps1 = 1e-8, double eps2 = 1e-8, double tau = 1e-3, ulong kMax = 5, double[]? initial = null) : base()
        {
            NativeMethods.GainHoloLegacy.AUTDGainHoloLM(out handle, Backend.Ptr, eps1, eps2, tau, kMax, initial, initial is null ? 0 : initial.Length, Constraint.Ptr);
        }
    }

    public sealed class GaussNewton : Holo
    {
        public GaussNewton(double eps1 = 1e-6, double eps2 = 1e-6, ulong kMax = 500, double[]? initial = null) : base()
        {
            NativeMethods.GainHoloLegacy.AUTDGainHoloGaussNewton(out handle, Backend.Ptr, eps1, eps2, kMax, initial, initial is null ? 0 : initial.Length, Constraint.Ptr);
        }
    }
    public sealed class GradientDescent : Holo
    {
        public GradientDescent(double eps = 1e-6, double step = 0.5, ulong kMax = 2000, double[]? initial = null) : base()
        {
            NativeMethods.GainHoloLegacy.AUTDGainHoloGradientDescent(out handle, Backend.Ptr, eps, step, kMax, initial, initial is null ? 0 : initial.Length, Constraint.Ptr);
        }
    }

    public sealed class Greedy : Holo
    {
        public Greedy(int phaseDiv = 16) : base()
        {
            NativeMethods.GainHoloLegacy.AUTDGainHoloGreedy(out handle, Backend.Ptr, phaseDiv, Constraint.Ptr);
        }
    }
}
