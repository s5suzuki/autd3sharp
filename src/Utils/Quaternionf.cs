/*
 * File: Quaterniond.cs
 * Project: Util
 * Created Date: 02/07/2018
 * Author: Shun Suzuki
 * -----
 * Last Modified: 07/04/2021
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2019 Hapis Lab. All rights reserved.
 * 
 */

using System;

namespace AUTD3Sharp
{
    public readonly struct Quaternionf : IEquatable<Quaternionf>
    {
        #region ctor
        public Quaternionf(float x, float y, float z, float w)
        {
            this.w = w;
            this.x = x;
            this.y = y;
            this.z = z;
        }
        #endregion

        #region property
        public float w { get; }
        public float x { get; }
        public float y { get; }
        public float z { get; }
        #endregion

        #region indexcer
        public float this[int index]
        {
            get
            {
                return index switch
                {
                    3 => w,
                    0 => x,
                    1 => y,
                    2 => z,
                    _ => throw new ArgumentOutOfRangeException(nameof(index))
                };
            }
        }
        #endregion

        #region arithmetic
        public static bool operator ==(Quaternionf left, Quaternionf right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Quaternionf left, Quaternionf right)
        {
            return !left.Equals(right);
        }

        public bool Equals(Quaternionf other)
        {
            return w.Equals(other.w) && x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z);
        }

        public override bool Equals(object? obj)
        {
            if (obj is Quaternionf qua)
            {
                return Equals(qua);
            }

            return false;
        }
        #endregion

        #region util
        public override int GetHashCode()
        {
            return w.GetHashCode() ^ x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
        }
        #endregion
    }
}
