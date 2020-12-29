/*
 * File: STM.cs
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

using System;
using AUTD3Sharp;

namespace example.Test
{
    internal static class STMTest
    {
        public static void Test(AUTD autd)
        {
            const float x = AUTD.AUTDWidth / 2;
            const float y = AUTD.AUTDHeight / 2;
            const float z = 150f;

            autd.SetSilentMode(false);

            autd.AppendModulationSync(AUTD.Modulation()); // static

            const float radius = 30.0f;
            const int size = 200;
            var center = new Vector3f(x, y, z);
            for (var i = 0; i < size; i++)
            {
                var theta = 2 * AUTD.Pi * i / size;
                var r = new Vector3f(MathF.Cos(theta), MathF.Sin(theta), 0);
                var f = AUTD.FocalPointGain(center + radius * r);
                autd.AppendSTMGain(f);
            }
            autd.StartSTModulation(1);
        }
    }
}
