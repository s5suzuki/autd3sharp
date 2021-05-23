/*
 * File: Holo.cs
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
    internal static class HoloGainTest
    {
        public static void Test(AUTD autd)
        {
            const double x = AUTD.TransSize * (AUTD.NumTransInX - 1) / 2.0;
            const double y = AUTD.TransSize * (AUTD.NumTransInY - 1) / 2.0;
            const double z = 150.0;

            var mod = Modulation.Sine(150);

            var center = new Vector3d(x, y, z);
            var focuses = new[] {
                center + 20.0 * Vector3d.UnitX,
                center - 20.0 * Vector3d.UnitX
                };
            var amps = new[] {
                    1.0,
                    1.0
                };

            var gain = Gain.Holo(focuses, amps);

            autd.Send(gain, mod);
        }
    }
}
