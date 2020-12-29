/*
 * File: BesselBeam.cs
 * Project: Test
 * Created Date: 25/08/2019
 * Author: Shun Suzuki
 * -----
 * Last Modified: 03/07/2020
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2019 Hapis Lab. All rights reserved.
 * 
 */

using AUTD3Sharp;

namespace example.Test
{
    internal static class BesselBeamTest
    {
        public static void Test(AUTD autd)
        {
            const float x = AUTD.AUTDWidth / 2;
            const float y = AUTD.AUTDHeight / 2;

            var mod = AUTD.SineModulation(150); // AM sin 150 Hz
            autd.AppendModulationSync(mod);

            var start = new Vector3f(x, y, 0f);
            var dir = Vector3f.UnitZ;
            autd.AppendGainSync(AUTD.BesselBeamGain(start, dir, 13.0f / 180 * AUTD.Pi)); // BesselBeam from (x, y, 0), theta = 13 deg
        }
    }
}
