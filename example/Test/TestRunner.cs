/*
 * File: TestRunner.cs
 * Project: Test
 * Created Date: 20/05/2020
 * Author: Shun Suzuki
 * -----
 * Last Modified: 28/04/2021
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2020 Hapis Lab. All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using AUTD3Sharp;

namespace example.Test
{
    internal delegate void TestFn(AUTD autd);

    public static class TestRunner
    {
        public static void Run(AUTD autd)
        {
            var examples = new List<(TestFn, string)> {(SimpleTest.Test, "Single Focal Point Test"),
                 (BesselBeamTest.Test, "BesselBeam Test"),
                 (HoloGainTest.Test, "Multiple Focal Points Test"),
                 (STMTest.Test, "Spatio-Temporal Modulation Test"),
                 (SeqTest.Test, "PointSequence Test (Hardware STM)")
             };

            autd.Clear();
            autd.Synchronize();

            autd.Wavelength = 8.5f; // mm

            var firmInfoList = autd.FirmwareInfoList().ToArray();
            for (var i = 0; i < firmInfoList.Length; i++)
                Console.WriteLine($"AUTD {i}: CPU={firmInfoList[i].CpuVersion}, FPGA={firmInfoList[i].FpgaVersion}");

            while (true)
            {
                for (var i = 0; i < examples.Count; i++) Console.WriteLine($"[{i}]: {examples[i].Item2}");

                Console.WriteLine("[Others]: finish");
                Console.Write("Choose number: ");

                if (!int.TryParse(Console.ReadLine(), out var idx) || idx >= examples.Count) break;

                var fn = examples[idx].Item1;
                fn(autd);

                Console.WriteLine("press any key to finish...");
                Console.ReadKey(true);

                Console.WriteLine("finish.");
                autd.Stop();
            }

            autd.Clear();
            autd.Close();
            autd.Dispose();
        }
    }
}
