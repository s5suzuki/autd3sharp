/*
 * File: GainHolo.cs
 * Project: src
 * Created Date: 23/05/2021
 * Author: Shun Suzuki
 * -----
 * Last Modified: 02/06/2022
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2022 Shun Suzuki. All rights reserved.
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
            NativeMethods.GainHolo.AUTDDeleteBackend(handle);
            return true;
        }
    }

    public sealed class BackendEigen : Backend
    {
        public BackendEigen() : base()
        {
            NativeMethods.GainHolo.AUTDEigenBackend(out handle);
        }
    }

    public abstract class AmplitudeConstraint
    {
        internal int Id { get; }

        internal abstract IntPtr Ptr();

        internal AmplitudeConstraint(int id)
        {
            Id = id;
        }
    }

    public sealed class DontCare : AmplitudeConstraint
    {
        public DontCare() : base(0)
        {
        }

        internal override IntPtr Ptr()
        {
            return IntPtr.Zero;
        }
    }


    public sealed class Normalize : AmplitudeConstraint
    {
        public Normalize() : base(1)
        {
        }

        internal override IntPtr Ptr()
        {
            return IntPtr.Zero;
        }
    }

    public sealed class Uniform : AmplitudeConstraint
    {

        private readonly double _value;

        public Uniform(double value) : base(2)
        {
            _value = value;
        }


        internal override IntPtr Ptr()
        {
            unsafe
            {
                fixed (double* vp = &_value)
                    return new IntPtr(vp);
            }
        }
    }

    public sealed class Clamp : AmplitudeConstraint
    {
        public Clamp() : base(3)
        {
        }


        internal override IntPtr Ptr()
        {
            return IntPtr.Zero;
        }
    }

    public class Holo : Gain
    {
        public Holo() : base()
        {
            Backend = new BackendEigen();
        }

        public Backend Backend { get; set; }
        public AmplitudeConstraint Constraint
        {
            set
            {
                NativeMethods.GainHolo.AUTDSetConstraint(handle, value.Id, value.Ptr());
            }
        }

        public void Add(Vector3 focus, double amp)
        {
            var (x, y, z) = Controller.Adjust(focus);
            NativeMethods.GainHolo.AUTDGainHoloAdd(handle, x, y, z, amp);
        }
    }

    public sealed class SDP : Holo
    {
        public SDP(double alpha = 1e-3, double lambda = 0.9, ulong repeat = 100) : base()
        {
            NativeMethods.GainHolo.AUTDGainHoloSDP(out handle, Backend.Ptr, alpha, lambda, repeat);
        }
    }
    public sealed class EVD : Holo
    {
        public EVD(double gamma = 1.0) : base()
        {
            NativeMethods.GainHolo.AUTDGainHoloEVD(out handle, Backend.Ptr, gamma);
        }
    }
    public sealed class Naive : Holo
    {
        public Naive() : base()
        {
            NativeMethods.GainHolo.AUTDGainHoloNaive(out handle, Backend.Ptr);
        }
    }

    public sealed class GS : Holo
    {
        public GS(ulong repeat = 100) : base()
        {
            NativeMethods.GainHolo.AUTDGainHoloGS(out handle, Backend.Ptr, repeat);
        }
    }

    public sealed class GSPAT : Holo
    {
        public GSPAT(ulong repeat = 100) : base()
        {
            NativeMethods.GainHolo.AUTDGainHoloGSPAT(out handle, Backend.Ptr, repeat);
        }
    }
    public sealed class LM : Holo
    {
        public LM(double eps1 = 1e-8, double eps2 = 1e-8, double tau = 1e-3, ulong kMax = 5, double[]? initial = null) : base()
        {
            NativeMethods.GainHolo.AUTDGainHoloLM(out handle, Backend.Ptr, eps1, eps2, tau, kMax, initial, initial is null ? 0 : initial.Length);
        }
    }

    public sealed class GaussNewton : Holo
    {
        public GaussNewton(double eps1 = 1e-6, double eps2 = 1e-6, ulong kMax = 500, double[]? initial = null) : base()
        {
            NativeMethods.GainHolo.AUTDGainHoloGaussNewton(out handle, Backend.Ptr, eps1, eps2, kMax, initial, initial is null ? 0 : initial.Length);
        }
    }
    public sealed class GradientDescent : Holo
    {
        public GradientDescent(double eps = 1e-6, double step = 0.5, ulong kMax = 2000, double[]? initial = null) : base()
        {
            NativeMethods.GainHolo.AUTDGainHoloGradientDescent(out handle, Backend.Ptr, eps, step, kMax, initial, initial is null ? 0 : initial.Length);
        }
    }

    public sealed class Greedy : Holo
    {
        public Greedy(int phaseDiv = 16) : base()
        {
            NativeMethods.GainHolo.AUTDGainHoloGreedy(out handle, Backend.Ptr, phaseDiv);
        }
    }
}
