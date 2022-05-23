﻿/*
 * File: Group.cs
 * Project: Test
 * Created Date: 23/05/2021
 * Author: Shun Suzuki
 * -----
 * Last Modified: 23/05/2022
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2022 Hapis Lab. All rights reserved.
 * 
 */


using AUTD3Sharp;
using AUTD3Sharp.Utils;

namespace example.Test
{
    internal static class GroupTest
    {
        public static void Test(Controller autd)
        {
            const double x = Controller.DeviceWidth / 2;
            const double y = Controller.DeviceHeight / 2;
            const double z = 150;

            var center = new Vector3d(x, y, z);

            var g1 = new Focus(center);
            var g2 = new GSPAT();
            g2.Add(center + new Vector3d(30.0, 0.0, 0.0), 1.0);
            g2.Add(center - new Vector3d(30.0, 0.0, 0.0), 1.0);

            var gain = new Grouped(autd);
            gain.Add(0, g1);
            gain.Add(1, g2);
            var mod = new Sine(150); // AM sin 150 Hz
            autd.Send(mod, gain);
        }
    }
}
