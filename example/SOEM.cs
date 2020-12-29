/*
 * File: SOEM.cs
 * Project: example
 * Created Date: 20/05/2020
 * Author: Shun Suzuki
 * -----
 * Last Modified: 03/07/2020
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
        private static string GetIfname()
        {
            var adapters = AUTD.EnumerateAdapters();
            var etherCATAdapters = adapters as EtherCATAdapter[] ?? adapters.ToArray();
            foreach (var (adapter, index) in etherCATAdapters.Select((adapter, index) => (adapter, index)))
            {
                Console.WriteLine($"[{index}]: {adapter}");
            }

            Console.Write("Choose number: ");
            int i;
            while (!int.TryParse(Console.ReadLine(), out i)) { }
            return etherCATAdapters[i].Name;
        }

        public static void Test()
        {
            Console.WriteLine("Test with SOEM");

            var autd = new AUTD();
            autd.AddDevice(Vector3f.Zero, Vector3f.Zero);
            //autd.AddDevice(Vector3d.UnitY * AUTD.AUTDHeight, Vector3d.Zero);

            var ifname = GetIfname();
            var link = AUTD.SOEMLink(ifname, autd.NumDevices);
            autd.OpenWith(link);

            TestRunner.Run(autd);
        }
    }
}
