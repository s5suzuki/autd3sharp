/*
 * File: BesselBeam.cs
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
    internal static class BesselBeamTest
    {
        public static void Test(AUTD autd)
        {
            var x = AUTD.AUTDWidth / 2;
            var y = AUTD.AUTDHeight / 2;

            var mod = Modulation.Sine(150); // AM sin 150 Hz

            var start = new Vector3d(x, y, 0);
            var dir = Vector3d.UnitZ;
            var gain = Gain.BesselBeam(start, dir, 13.0f / 180f * AUTD.Pi); // BesselBeam from (x, y, 0), theta = 13 deg
            autd.Send(gain, mod);
        }
    }
}
