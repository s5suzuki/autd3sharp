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


using AUTD3Sharp;
using AUTD3SharpTest.Test;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AUTD3SharpTest
{
    internal class SOEMTest
    {
        public static string GetIfname()
        {
            IEnumerable<EtherCATAdapter> adapters = AUTD.EnumerateAdapters();
            foreach ((EtherCATAdapter adapter, int index) in adapters.Select((adapter, index) => (adapter, index)))
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

            AUTD autd = new AUTD();
            autd.AddDevice(Vector3d.Zero, Vector3d.Zero);
            //autd.AddDevice(Vector3d.UnitY * AUTD.AUTDHeight, Vector3d.Zero);

            string ifname = GetIfname();
            Link link = AUTD.SOEMLink(ifname, autd.NumDevices);
            autd.OpenWith(link);

            TestRunner.Run(autd);
        }
    }
}
