/*
 * File: SOEM.cs
 * Project: example
 * Created Date: 20/05/2020
 * Author: Shun Suzuki
 * -----
 * Last Modified: 21/07/2021
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2020 Hapis Lab. All rights reserved.
 * 
 */


using AUTD3Sharp;
using System;
using System.Linq;
using AUTD3Sharp.Utils;
using example.Test;

namespace example
{
    internal static class SOEMTest
    {
        public static string GetIfname()
        {
            var adapters = AUTD.EnumerateAdapters();
            var etherCATAdapters = adapters as EtherCATAdapter[] ?? adapters.ToArray();
            foreach (var (adapter, index) in etherCATAdapters.Select((adapter, index) => (adapter, index)))
                Console.WriteLine($"[{index}]: {adapter}");

            Console.Write("Choose number: ");
            int i;
            while (!int.TryParse(Console.ReadLine(), out i)) { }
            return etherCATAdapters.ElementAt(i).Name;
        }

        public static void Test()
        {
            Console.WriteLine("Test with SOEM");

            var autd = new AUTD();
            autd.AddDevice(Vector3d.Zero, Vector3d.Zero);
            //autd.AddDevice(Vector3d.Zero, Vector3d.Zero, 1);

            var ifname = GetIfname();
            var link = Link.SOEM(ifname, autd.NumDevices, 1, x => Console.WriteLine($"Unrecoverable error occurred: {x}"));
            if (!autd.Open(link))
            {
                Console.WriteLine(AUTD.LastError);
                return;
            }

            TestRunner.Run(autd);
        }
    }
}
