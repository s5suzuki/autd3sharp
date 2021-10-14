/*
 * File: Simple.cs
 * Project: Test
 * Created Date: 25/08/2019
 * Author: Shun Suzuki
 * -----
 * Last Modified: 23/05/2021
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2019 Hapis Lab. All rights reserved.
 * 
 */

using AUTD3Sharp;
using AUTD3Sharp.Utils;

namespace example.Test
{
    internal static class SimpleTest
    {
        public static void Test(AUTD autd)
        {
            const double x = AUTD.DeviceWidth / 2;
            const double y = AUTD.DeviceHeight / 2;
            const double z = 150;

            var mod = Modulation.Sine(150); // AM sin 150 Hz
            var gain = Gain.FocalPoint(new Vector3d(x, y, z)); // Focal point @ (x, y, z) [mm]
            autd.Send(gain, mod);
        }
    }
}
