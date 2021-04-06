/*
 * File: TwinCAT.cs
 * Project: example
 * Created Date: 20/05/2020
 * Author: Shun Suzuki
 * -----
 * Last Modified: 06/04/2021
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2020 Hapis Lab. All rights reserved.
 * 
 */


using System;
using AUTD3Sharp;
using example.Test;

namespace example
{
    internal class TwinCATTest
    {
        public static void Test()
        {
            Console.WriteLine("Test with TwinCAT");

            var autd = new AUTD();
            autd.AddDevice(Vector3f.Zero, Vector3f.Zero);
            //autd.AddDevice(Vector3d.UnitY * AUTD.AUTDHeight, Vector3d.Zero);

            var link = AUTD.LocalEtherCATLink();
            if (!autd.OpenWith(link))
            {
                Console.WriteLine(autd.LastError);
                return;
            }

            TestRunner.Run(autd);
        }
    }
}
