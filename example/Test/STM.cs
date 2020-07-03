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

using AUTD3Sharp;
using System;

namespace AUTD3SharpTest.Test
{
    internal class STMTest
    {
        public static void Test(AUTD autd)
        {
            double x = AUTD.AUTDWidth / 2;
            double y = AUTD.AUTDHeight / 2;
            double z = 150.0;

            autd.SetSilentMode(false);

            autd.AppendModulationSync(AUTD.Modulation(255)); // static

            double radius = 30.0;
            int size = 200;
            Vector3d center = new Vector3d(x, y, z);
            for (int i = 0; i < size; i++)
            {
                double theta = 2 * Math.PI * i / size;
                Vector3d r = new Vector3d(Math.Cos(theta), Math.Sin(theta), 0);
                Gain f = AUTD.FocalPointGain(center + radius * r);
                autd.AppendSTMGain(f);
            }
            autd.StartSTModulation(1);
        }
    }
}
