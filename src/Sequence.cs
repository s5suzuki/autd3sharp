/*
 * File: PointSequence.cs
 * Project: src
 * Created Date: 28/04/2021
 * Author: Shun Suzuki
 * -----
 * Last Modified: 17/12/2021
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2021 Hapis Lab. All rights reserved.
 * 
 */

using System;
using System.Runtime.InteropServices;

#if UNITY_2018_3_OR_NEWER
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
#else
using Vector3 = AUTD3Sharp.Utils.Vector3d;
#endif

namespace AUTD3Sharp
{
    [ComVisible(false)]
    public abstract class Sequence : Body
    {
        internal IntPtr SeqPtr => handle;

        internal Sequence(IntPtr seq) : base(seq)
        {
        }

        protected override bool ReleaseHandle()
        {
            NativeMethods.AUTDDeleteSequence(handle);
            return true;
        }

        public double Frequency
        {
            get => NativeMethods.AUTDSequenceFreq(handle);
            set => NativeMethods.AUTDSequenceSetFreq(handle, value);
        }

        public double SamplingFrequency => NativeMethods.AUTDSequenceSamplingFreq(handle);
        public uint SamplingFrequencyDivision
        {
            get => NativeMethods.AUTDSequenceSamplingFreqDiv(handle);
            set => NativeMethods.AUTDSequenceSetSamplingFreqDiv(handle, value);
        }
        public uint SamplingFrequencyPeriod => NativeMethods.AUTDSequenceSamplingPeriod(handle);
        public uint Period => NativeMethods.AUTDSequencePeriod(handle);
    }

    [ComVisible(false)]
    public class PointSequence : Sequence
    {
        internal PointSequence(IntPtr seq) : base(seq)
        {
        }

        public bool AddPoint(Vector3 point, byte duty = 0xFF)
        {
            var (x, y, z) = AUTD.Adjust(point);
            return NativeMethods.AUTDSequenceAddPoint(handle, x, y, z, duty);
        }

        public static PointSequence Create()
        {
            NativeMethods.AUTDSequence(out var p);
            return new PointSequence(p);
        }
    }


    public enum GainMode : ushort
    {
        DutyPhaseFull = 1,
        PhaseFull = 2,
        PhaseHalf = 4
    }

    [ComVisible(false)]
    public class GainSequence : Sequence
    {
        internal GainSequence(IntPtr seq) : base(seq)
        {
        }

        public bool AddGain(Gain gain)
        {
            return NativeMethods.AUTDSequenceAddGain(handle, gain.Ptr);
        }

        public static GainSequence Create(AUTD autd, GainMode gainMode = GainMode.DutyPhaseFull)
        {
            NativeMethods.AUTDGainSequence(out var p, autd.AUTDControllerHandle.CntPtr, (ushort)gainMode);
            return new GainSequence(p);
        }
    }
}
