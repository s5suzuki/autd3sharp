/*
 * File: PointSequence.cs
 * Project: src
 * Created Date: 28/04/2021
 * Author: Shun Suzuki
 * -----
 * Last Modified: 28/04/2021
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2021 Hapis Lab. All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
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
    public class PointSequence : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal IntPtr SeqPtr => handle;

        public PointSequence(IntPtr seq) : base(true)
        {
            SetHandle(seq);
        }

        protected override bool ReleaseHandle()
        {
            NativeMethods.AUTDDeleteSequence(handle);
            return true;
        }

        public bool AddPoint(Vector3f point)
        {
            AUTD.AdjustVector(ref point);
            return NativeMethods.AUTDSequenceAddPoint(handle, point[0], point[1], point[2]);
        }

        public bool AddPoints(IList<Vector3f> points)
        {
            var pointsArr = new float[points.Count * 3];
            for (var i = 0; i < points.Count; i++)
            {
                var point = points[i];
                AUTD.AdjustVector(ref point);
                pointsArr[3 * i] = point[0];
                pointsArr[3 * i + 1] = point[1];
                pointsArr[3 * i + 2] = point[2];

            }
            unsafe
            {
                fixed (float* pd = pointsArr)
                    return NativeMethods.AUTDSequenceAddPoints(handle, pd, (ulong)points.Count);
            }
        }
        public float SetFrequency(float freq) => NativeMethods.AUTDSequenceSetFreq(handle, freq);
        public float Frequency() => NativeMethods.AUTDSequenceFreq(handle);
        public float SamplingFrequency() => NativeMethods.AUTDSequenceSamplingFreq(handle);
        public ushort SamplingFrequencyDivision() => NativeMethods.AUTDSequenceSamplingFreqDiv(handle);
    }
}
