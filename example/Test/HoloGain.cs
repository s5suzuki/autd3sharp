/*
 * File: HoloGain.cs
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
    internal static class HoloGainTest
    {
        public static void Test(AUTD autd)
        {
            const float  x = AUTD.AUTDWidth / 2;
            const float y = AUTD.AUTDHeight / 2;
            const float z = 150f;

            autd.AppendModulationSync(AUTD.SineModulation(150)); // AM sin 150 HZ

            var focuses = new[] {
                    new Vector3f(x - 30, y ,z),
                    new Vector3f(x + 30, y ,z)
                };
            var amps = new[] {
                    1.0f,
                    1.0f
                };
            autd.AppendGainSync(AUTD.HoloGain(focuses, amps));
        }
    }
}
