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

namespace AUTD3SharpTest.Test
{
    internal class BesselBeamTest
    {
        public static void Test(AUTD autd)
        {
            double x = AUTD.AUTDWidth / 2;
            double y = AUTD.AUTDHeight / 2;

            Modulation mod = AUTD.SineModulation(150); // AM sin 150 Hz
            autd.AppendModulationSync(mod);

            Vector3d start = new Vector3d(x, y, 0);
            Vector3d dir = Vector3d.UnitZ;
            autd.AppendGainSync(AUTD.BesselBeamGain(start, dir, 13.0 / 180 * AUTD.Pi)); // BesselBeam from (x, y, 0), theta = 13 deg
        }
    }
}
