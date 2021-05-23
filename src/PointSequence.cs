/*
 * File: PointSequence.cs
 * Project: src
 * Created Date: 28/04/2021
 * Author: Shun Suzuki
 * -----
 * Last Modified: 23/05/2021
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2021 Hapis Lab. All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
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
    public class PointSequence : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal IntPtr SeqPtr => handle;

        internal PointSequence(IntPtr seq) : base(true)
        {
            SetHandle(seq);
        }

        protected override bool ReleaseHandle()
        {
            NativeMethods.AUTDDeleteSequence(handle);
            return true;
        }

        public bool AddPoint(Vector3 point)
        {
            var (x, y, z) = AUTD.Adjust(point);
            return NativeMethods.AUTDSequenceAddPoint(handle, x, y, z);
        }

        public bool AddPoints(IList<Vector3> points)
        {
            var pointsArr = new double[points.Count * 3];
            for (var i = 0; i < points.Count; i++)
            {
                var (x, y, z) = AUTD.Adjust(points[i]);
                pointsArr[3 * i] = x;
                pointsArr[3 * i + 1] = y;
                pointsArr[3 * i + 2] = z;
            }
            unsafe
            {
                fixed (double* pd = pointsArr)
                    return NativeMethods.AUTDSequenceAddPoints(handle, pd, (ulong)points.Count);
            }
        }

        public double Frequency
        {
            get => NativeMethods.AUTDSequenceFreq(handle);
            set => NativeMethods.AUTDSequenceSetFreq(handle, value);
        }

        public double SamplingFrequency => NativeMethods.AUTDSequenceSamplingFreq(handle);
        public ushort SamplingFrequencyDivision => NativeMethods.AUTDSequenceSamplingFreqDiv(handle);

        public uint SamplingFrequencyPeriod => NativeMethods.AUTDSequenceSamplingPeriod(handle);
        public uint Period => NativeMethods.AUTDSequencePeriod(handle);

        public static PointSequence Create()
        {
            NativeMethods.AUTDSequence(out var p);
            return new PointSequence(p);
        }

        public static PointSequence CircumferencePointSequence(Vector3 center, Vector3 normal, double radius, ulong n)
        {
            var (x, y, z) = AUTD.Adjust(center);
            var (nx, ny, nz) = AUTD.Adjust(normal, false);
            NativeMethods.AUTDCircumSequence(out var p, x, y, z, nx, ny, nz, radius, n);
            return new PointSequence(p);
        }
    }
}
