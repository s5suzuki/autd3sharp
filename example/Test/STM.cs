/*
 * File: STM.cs
 * Project: Test
 * Created Date: 25/08/2019
 * Author: Shun Suzuki
 * -----
 * Last Modified: 03/06/2021
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2019 Hapis Lab. All rights reserved.
 * 
 */

using AUTD3Sharp;
using System;
using AUTD3Sharp.Utils;

namespace example.Test
{
    internal static class STMTest
    {
        public static void Test(AUTD autd)
        {
            const double x = AUTD.AUTDWidth / 2;
            const double y = AUTD.AUTDHeight / 2;
            const double z = 150.0;

            autd.SilentMode = false;

            var mod = Modulation.Static();
            autd.Send(mod);

            const double radius = 30.0;
            const int size = 100;
            var center = new Vector3d(x, y, z);
            var stm = autd.STM();
            for (var i = 0; i < size; i++)
            {
                var theta = 2 * AUTD.Pi * i / size;
                var r = new Vector3d(Math.Cos(theta), Math.Sin(theta), 0);
                var f = Gain.FocalPoint(center + radius * r);
                stm.AddSTMGain(f);
            }
            stm.StartSTM(0.5);

            Console.WriteLine("press any key to stop...");
            Console.ReadKey(true);

            stm.StopSTM();
            stm.FinishSTM();
        }
    }
}
