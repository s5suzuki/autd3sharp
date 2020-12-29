/*
 * File: Vector3d.cs
 * Project: Util
 * Created Date: 02/07/2018
 * Author: Shun Suzuki
 * -----
 * Last Modified: 20/02/2020
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2018-2019 Hapis Lab. All rights reserved.
 * 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace AUTD3Sharp
{
    public readonly struct Vector3f : IEquatable<Vector3f>, IEnumerable<float>
    {
        #region ctor
        public Vector3f(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3f(params float[] vector)
        {
            if (vector == null)
            {
                throw new ArgumentNullException(nameof(vector));
            }

            if (vector.Length != 3)
            {
                throw new InvalidCastException();
            }

            x = vector[0];
            y = vector[1];
            z = vector[2];
        }
        #endregion

        #region property
        public static Vector3f UnitX => new Vector3f(1, 0, 0);
        public static Vector3f UnitY => new Vector3f(0, 1, 0);
        public static Vector3f UnitZ => new Vector3f(0, 0, 1);
        public static Vector3f Zero => new Vector3f(0, 0, 0);
        public Vector3f Normalized => this / L2Norm;
        public float L2Norm => MathF.Sqrt(L2NormSquared);
        public float L2NormSquared => x * x + y * y + z * z;
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
                    0 => x,
                    1 => y,
                    2 => z,
                    _ => throw new ArgumentOutOfRangeException(nameof(index)),
                };
            }
        }
        #endregion

        #region arithmetic
        public static Vector3f Negate(Vector3f operand)
        {
            return new Vector3f(-operand.x, -operand.y, -operand.z);
        }

        public static Vector3f Add(Vector3f left, Vector3f right)
        {
            var v1 = left.x + right.x;
            var v2 = left.y + right.y;
            var v3 = left.z + right.z;
            return new Vector3f(v1, v2, v3);
        }
        public static Vector3f Subtract(Vector3f left, Vector3f right)
        {
            var v1 = left.x - right.x;
            var v2 = left.y - right.y;
            var v3 = left.z - right.z;
            return new Vector3f(v1, v2, v3);
        }
        public static Vector3f Divide(Vector3f left, float right)
        {
            var v1 = left.x / right;
            var v2 = left.y / right;
            var v3 = left.z / right;

            return new Vector3f(v1, v2, v3);
        }
        public static Vector3f Multiply(Vector3f left, float right)
        {
            var v1 = left.x * right;
            var v2 = left.y * right;
            var v3 = left.z * right;
            return new Vector3f(v1, v2, v3);
        }

        public static Vector3f Multiply(float left, Vector3f right)
        {
            return Multiply(right, left);
        }

        public static Vector3f operator -(Vector3f operand)
        {
            return Negate(operand);
        }

        public static Vector3f operator +(Vector3f left, Vector3f right)
        {
            return Add(left, right);
        }

        public static Vector3f operator -(Vector3f left, Vector3f right)
        {
            return Subtract(left, right);
        }

        public static Vector3f operator *(Vector3f left, float right)
        {
            return Multiply(left, right);
        }

        public static Vector3f operator *(float left, Vector3f right)
        {
            return Multiply(right, left);
        }

        public static Vector3f operator /(Vector3f left, float right)
        {
            return Divide(left, right);
        }

        public static bool operator ==(Vector3f left, Vector3f right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector3f left, Vector3f right)
        {
            return !left.Equals(right);
        }

        public bool Equals(Vector3f other)
        {
            return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z);
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector3f vec)
            {
                return Equals(vec);
            }

            return false;
        }
        #endregion

        #region public methods
        public Vector3f Rectify()
        {
            return new Vector3f(Math.Max(x, 0), Math.Max(y, 0), Math.Max(z, 0));
        }

        public float[] ToArray()
        {
            return new[] { x, y, z };
        }
        #endregion

        #region util
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public string ToString(string format)
        {
            return "3D Column Vector:\n"
+ string.Format(CultureInfo.CurrentCulture, format, x) + "\n"
+ string.Format(CultureInfo.CurrentCulture, format, y) + "\n"
+ string.Format(CultureInfo.CurrentCulture, format, z);
        }

        public IEnumerator<float> GetEnumerator()
        {
            yield return x;
            yield return y;
            yield return z;
        }

        public override string ToString()
        {
            return ToString("{0,-20}");
        }
        #endregion
    }
}
