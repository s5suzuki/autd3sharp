/*
 * File: Advanced.cs
 * Project: Test
 * Created Date: 23/05/2021
 * Author: Shun Suzuki
 * -----
 * Last Modified: 18/06/2021
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2021 Hapis Lab. All rights reserved.
 * 
 */

using AUTD3Sharp;

namespace example.Test
{
    public class AdvancedTest
    {
        public static void Test(AUTD autd)
        {
            autd.SilentMode = false;

            var delays = new byte[autd.NumDevices, AUTD.NumTransInDevice];
            delays[0, 0] = 4;
            autd.SetOutputDelay(delays);

            var uniform = new ushort[autd.NumDevices, AUTD.NumTransInDevice];
            for (var i = 0; i < autd.NumDevices; i++)
                for (var j = 0; j < AUTD.NumTransInDevice; j++)
                    uniform[i, j] = 0xFFFF;

            var burst = new byte[4000];
            burst[0] = 0xFF;

            var g = Gain.Custom(uniform);
            var m = Modulation.Custom(burst);

            autd.Send(g, m);
        }
    }
}
