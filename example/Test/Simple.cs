/*
 * File: Simple.cs
 * Project: Test
 * Created Date: 25/08/2019
 * Author: Shun Suzuki
 * -----
 * Last Modified: 06/05/2021
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
            const float x = AUTD.AUTDWidth / 2;
            const float y = AUTD.AUTDHeight / 2;
            const float z = 150f;

            autd.SilentMode = true;

            var mod = Modulation.SineModulation(150); // AM sin 150 Hz
            autd.AppendModulationSync(mod);

            var gain = Gain.FocalPointGain(new Vector3f(x, y, z)); // Focal point @ (x, y, z) [mm]
            autd.AppendGainSync(gain);
        }
    }
}
