/*
 * File: SeqTest.cs
 * Project: Test
 * Created Date: 30/04/2021
 * Author: Shun Suzuki
 * -----
 * Last Modified: 23/05/2021
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2021 Hapis Lab. All rights reserved.
 * 
 */

using AUTD3Sharp;
using System;
using AUTD3Sharp.Utils;

namespace example.Test
{
    internal static class SeqTest
    {
        public static void Test(AUTD autd)
        {
            const double x = AUTD.AUTDWidth / 2;
            const double y = AUTD.AUTDHeight / 2;
            const double z = 150;

            autd.SilentMode = false;

            var mod = Modulation.Static();
            autd.Send(mod);

            var center = new Vector3d(x, y, z);
            var dir = Vector3d.UnitZ;
            var seq = PointSequence.CircumferencePointSequence(center, dir, 30.0, 200);
            seq.Frequency = 1;
            Console.WriteLine($"Actual frequency is {seq.Frequency}");
            autd.Send(seq);
        }
    }
}
