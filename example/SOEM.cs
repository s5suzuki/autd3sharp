/*
 * File: SOEM.cs
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
using System.Linq;
using AUTD3Sharp;
using example.Test;

namespace example
{
    internal static class SOEMTest
    {
        public static string GetIfname()
        {
            var adapters = AUTD.EnumerateAdapters();
            foreach ((var adapter, var index) in adapters.Select((adapter, index) => (adapter, index)))
            {
                Console.WriteLine($"[{index}]: {adapter}");
            }

            Console.Write("Choose number: ");
            int i;
            while (!int.TryParse(Console.ReadLine(), out i)) { }
            return adapters.ElementAt(i).Name;
        }

        public static void Test()
        {
            Console.WriteLine("Test with SOEM");

            var autd = new AUTD();
            autd.AddDevice(Vector3f.Zero, Vector3f.Zero);
            //autd.AddDevice(Vector3d.UnitY * AUTD.AUTDHeight, Vector3d.Zero);

            var ifname = GetIfname();
            var link = AUTD.SOEMLink(ifname, autd.NumDevices);

            if (!autd.OpenWith(link))
            {
                Console.WriteLine(autd.LastError);
                return;
            }

            TestRunner.Run(autd);
        }
    }
}
