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
namespace AUTD3SharpTest.Test
{
    internal class HoloGainTest
    {
        public static void Test(AUTD autd)
        {
            double x = AUTD.AUTDWidth / 2;
            double y = AUTD.AUTDHeight / 2;
            double z = 150.0;

            autd.AppendModulationSync(AUTD.SineModulation(150)); // AM sin 150 HZ

            Vector3d[] focuses = new[] {
                    new Vector3d(x - 30, y ,z),
                    new Vector3d(x + 30, y ,z)
                };
            double[] amps = new[] {
                    1.0,
                    1.0
                };
            autd.AppendGainSync(AUTD.HoloGain(focuses, amps));
        }
    }
}
