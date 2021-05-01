/*
 * File: HoloGain.cs
 * Project: Test
 * Created Date: 25/08/2019
 * Author: Shun Suzuki
 * -----
 * Last Modified: 28/04/2021
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2019 Hapis Lab. All rights reserved.
 * 
 */

using System;
using AUTD3Sharp;
using AUTD3Sharp.Utils;

namespace example.Test
{
    internal static class HoloGainTest
    {
        private static Gain SelectOpt(Vector3f[] foci, float[] amps)
        {
            var opts = new[] { "SDP", "EVD", "GS", "GSPAT", "Naive", "LM" };
            for (var j = 0; j < opts.Length; j++)
                Console.WriteLine($"[{j}]: {opts[j]}");
            Console.Write("Choose number: ");
            int i;
            while (!int.TryParse(Console.ReadLine(), out i)) { }
            return i switch
            {
                0 => Gain.HoloGainSDP(foci, amps, null),
                1 => Gain.HoloGainEVD(foci, amps, null),
                2 => Gain.HoloGainGS(foci, amps, null),
                3 => Gain.HoloGainGSPAT(foci, amps, null),
                4 => Gain.HoloGainNaive(foci, amps),
                5 => Gain.HoloGainLM(foci, amps, null),
                _ => Gain.HoloGainSDP(foci, amps, null),
            };
        }

        public static void Test(AUTD autd)
        {
            const float x = AUTD.TransSize * (AUTD.NumTransInX - 1) / 2.0f;
            const float y = AUTD.TransSize * (AUTD.NumTransInY - 1) / 2.0f;
            const float z = 150.0f;

            autd.AppendModulationSync(Modulation.SineModulation(150)); // AM sin 150 HZ

            var center = new Vector3f(x, y, z);
            var foci = new[] {
                    center + 30.0f * Vector3f.UnitX,
                    center - 30.0f * Vector3f.UnitX
                };
            var amps = new[] {
                    1f,
                    1f
                };

            autd.AppendGainSync(SelectOpt(foci, amps));
        }
    }
}
