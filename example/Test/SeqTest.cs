using System;
using AUTD3Sharp;
using AUTD3Sharp.Utils;

namespace example.Test
{
    internal static class SeqTest
    {
        public static void Test(AUTD autd)
        {
            const float x = AUTD.AUTDWidth / 2;
            const float y = AUTD.AUTDHeight / 2;
            const float z = 150;

            autd.SilentMode = false;

            var mod = Modulation.StaticModulation();
            autd.AppendModulationSync(mod);

            var center = new Vector3f(x, y, z);
            var dir = Vector3f.UnitZ;
            var seq = AUTD.CircumferencePointSequence(center, dir, 30.0f, 200);

            var f = seq.SetFrequency(200);
            Console.WriteLine($"Actual frequency is {f}");
            autd.AppendSequence(seq);
        }
    }
}
