/*
 * File: TwinCAT.cs
 * Project: example
 * Created Date: 20/05/2020
 * Author: Shun Suzuki
 * -----
 * Last Modified: 23/05/2021
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2020 Hapis Lab. All rights reserved.
 * 
 */


using AUTD3Sharp;
using AUTD3SharpTest.Test;
using System;
using AUTD3Sharp.Utils;

namespace example
{
    internal static class TwinCATTest
    {
        public static void Test()
        {
            Console.WriteLine("Test with TwinCAT");

            var autd = new AUTD();
            autd.AddDevice(Vector3d.Zero, Vector3d.Zero);

            var link = Link.EtherCATLink();
            autd.Open(link);

            TestRunner.Run(autd);
        }
    }
}
