/*
 * File: GainPair.cs
 * Project: Utils
 * Created Date: 30/04/2021
 * Author: Shun Suzuki
 * -----
 * Last Modified: 23/05/2021
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2021 Hapis Lab. All rights reserved.
 * 
 */

using System;

namespace AUTD3Sharp.Utils
{
    public readonly struct GainPair : IEquatable<GainPair>
    {
        public int Id { get; }
        public Gain Gain { get; }
        public GainPair(int id, Gain gain)
        {
            Id = id;
            Gain = gain;
        }

        public static bool operator ==(GainPair left, GainPair right) => left.Equals(right);
        public static bool operator !=(GainPair left, GainPair right) => !left.Equals(right);
        public bool Equals(GainPair other) => Id == other.Id && Gain == other.Gain;
        public override bool Equals(object? obj)
        {
            if (obj is GainPair pair) return Equals(pair);
            return false;
        }
        public override int GetHashCode() => Id ^ Gain.GetHashCode();
    }
}
