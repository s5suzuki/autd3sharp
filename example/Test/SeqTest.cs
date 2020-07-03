using AUTD3Sharp;
using System;

namespace AUTD3SharpTest.Test
{
    internal class SeqTest
    {
        public static void Test(AUTD autd)
        {
            double x = AUTD.AUTDWidth / 2;
            double y = AUTD.AUTDHeight / 2;
            double z = 150;

            Modulation mod = AUTD.Modulation(255);
            autd.AppendModulationSync(mod);

            Vector3d center = new Vector3d(x, y, z);
            Vector3d dir = Vector3d.UnitZ;
            PointSequence seq = AUTD.CircumferencePointSequence(center, dir, 30.0, 200);
            double f = seq.SetFrequency(200);
            Console.WriteLine($"Actual frequency is {f}");
            autd.SetSilentMode(false);
            autd.AppendSequence(seq);
        }
    }
}
