/*
 * File: Simple.cs
 * Project: Test
 * Created Date: 25/08/2019
 * Author: Shun Suzuki
 * -----
 * Last Modified: 05/10/2021
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2019 Hapis Lab. All rights reserved.
 * 
 */

using AUTD3Sharp;
using AUTD3Sharp.Utils;

namespace example.Test
{
    internal static class GroupTest
    {
        public static void Test(AUTD autd)
        {
            const double x = AUTD.DeviceWidth / 2;
            const double y = AUTD.DeviceHeight / 2;
            const double z = 150;

            var center = new Vector3d(x, y, z);

            var g1 = Gain.FocalPoint(center);

            var focuses = new[] {
                center + 30.0 * Vector3d.UnitX,
                center - 30.0 * Vector3d.UnitX
            };
            var amps = new[] {
                1.0,
                1.0
            };
            var backend = Gain.Eigen3Backend();
            var g2 = Gain.HoloGSPAT(focuses, amps, backend);

            var gain = Gain.Grouped(new GainPair(0, g1), new GainPair(1, g2));
            var mod = Modulation.Sine(150); // AM sin 150 Hz
            autd.Send(gain, mod);

            Gain.DeleteBackend(backend);
        }
    }
}
