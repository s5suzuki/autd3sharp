/*
 * File: SOEM.cs
 * Project: example
 * Created Date: 30/04/2021
 * Author: Shun Suzuki
 * -----
 * Last Modified: 23/05/2022
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2022 Hapis Lab. All rights reserved.
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
            var adapters = SOEM.EnumerateAdapters();
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

            var autd = new Controller();
            autd.AddDevice(Vector3d.Zero, Vector3d.Zero);
            //autd.AddDevice(Vector3d.Zero, Vector3d.Zero, 1);

            var ifname = GetIfname();
            var link = new SOEM(ifname, autd.NumDevices, 2, x =>
            {
                Console.WriteLine($"Unrecoverable error occurred: {x}");
                Environment.Exit(-1);
            });
            if (!autd.Open(link))
            {
                Console.WriteLine(Controller.LastError);
                return;
            }

            TestRunner.Run(autd);
        }
    }
}
