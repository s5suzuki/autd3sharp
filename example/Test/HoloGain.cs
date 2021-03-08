/*
 * File: HoloGain.cs
 * Project: Test
 * Created Date: 25/08/2019
 * Author: Shun Suzuki
 * -----
 * Last Modified: 09/03/2021
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
            const float x = AUTD.TransSize * (AUTD.NumTransInX - 1) / 2.0f;
            const float y = AUTD.TransSize * (AUTD.NumTransInY - 1) / 2.0f;
            const float z = 150.0f;

            autd.AppendModulationSync(AUTD.SineModulation(150)); // AM sin 150 HZ

            var center = new Vector3f(x, y, z);
            var focuses = new[] {
                    center + 30.0f * Vector3f.UnitX,
                    center - 30.0f * Vector3f.UnitX
                };
            var amps = new[] {
                    1f,
                    1f
                };

            autd.AppendGainSync(AUTD.HoloGain(focuses, amps));
        }
    }
}
