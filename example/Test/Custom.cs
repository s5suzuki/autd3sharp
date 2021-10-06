/*
 * File: Custom.cs
 * Project: Test
 * Created Date: 06/10/2021
 * Author: Shun Suzuki
 * -----
 * Last Modified: 06/10/2021
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2021 Hapis Lab. All rights reserved.
 * 
 */

using System;
using AUTD3Sharp;
using AUTD3Sharp.Utils;

namespace example.Test
{
    internal static class CustomTest
    {
        private static byte ToPhase(double phase)
        {
            return (byte)((int)(Math.Round((phase / (2.0 * Math.PI) + 0.5) * 256.0)) & 0xFF);
        }

        private static Gain Focus(AUTD autd, Vector3d point)
        {
            var devNum = autd.NumDevices;

            var data = new ushort[devNum, AUTD.NumTransInDevice];

            var wavenumber = 2.0 * Math.PI / autd.Wavelength;
            for (var dev = 0; dev < devNum; dev++)
            {
                for (var i = 0; i < AUTD.NumTransInDevice; i++)
                {
                    var tp = autd.TransPosition(dev, i);
                    var dist = (tp - point).L2Norm;
                    var phase = ToPhase(wavenumber * dist);
                    const ushort duty = 0xFF;
                    data[dev, i] = (ushort)((duty << 8) | phase);
                }
            }

            return Gain.Custom(data);
        }

        public static void Test(AUTD autd)
        {
            const double x = AUTD.AUTDWidth / 2;
            const double y = AUTD.AUTDHeight / 2;
            const double z = 150;

            var mod = Modulation.Sine(150);
            var gain = Focus(autd, new Vector3d(x, y, z));
            autd.Send(gain, mod);
        }
    }
}
