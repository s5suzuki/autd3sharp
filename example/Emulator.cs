/*
 * File: Emulator.cs
 * Project: example
 * Created Date: 10/07/2021
 * Author: Shun Suzuki
 * -----
 * Last Modified: 10/07/2021
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2021 Hapis Lab. All rights reserved.
 * 
 */


using AUTD3Sharp;
using System;
using AUTD3Sharp.Utils;
using example.Test;

namespace example
{
    internal static class EmulatorTest
    {
        public static void Test()
        {
            Console.WriteLine("Test with Emulator");

            var autd = new AUTD();
            autd.AddDevice(Vector3d.Zero, Vector3d.Zero);

            var link = Link.Emulator(50632, autd);
            if (!autd.Open(link))
            {
                Console.WriteLine(AUTD.LastError);
                return;
            }

            TestRunner.Run(autd);
        }
    }
}
