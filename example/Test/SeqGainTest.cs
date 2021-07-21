/*
 * File: SeqGainTest.cs
 * Project: Test
 * Created Date: 21/07/2021
 * Author: Shun Suzuki
 * -----
 * Last Modified: 21/07/2021
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
    internal static class SeqGainTest
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
            var seq = GainSequence.Create();
            var pointNum = 200;
            for (int i = 0; i < pointNum; i++)
            {
                var radius = 30.0;
                var theta = 2.0 * Math.PI * i / pointNum;
                var p = radius * new Vector3d(Math.Cos(theta), Math.Sin(theta), 0);
                var focuses = new[] { center + p, center - p };
                var amps = new[] { 1.0, 1.0 };
                var gain = Gain.HoloGSPAT(focuses, amps);
                seq.AddGain(gain);
            }

            seq.Frequency = 1;
            Console.WriteLine($"Actual frequency is {seq.Frequency}");
            autd.Send(seq);
        }
    }
}
