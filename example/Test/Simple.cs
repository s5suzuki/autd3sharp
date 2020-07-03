/*
 * File: Simple.cs
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
    internal class SimpleTest
    {
        public static void Test(AUTD autd)
        {
            double x = AUTD.AUTDWidth / 2;
            double y = AUTD.AUTDHeight / 2;
            double z = 150;

            Modulation mod = AUTD.SineModulation(150); // AM sin 150 Hz
            autd.AppendModulationSync(mod);

            Gain gain = AUTD.FocalPointGain(x, y, z); // Focal point @ (x, y, z) [mm]
            autd.AppendGainSync(gain);
        }
    }
}
