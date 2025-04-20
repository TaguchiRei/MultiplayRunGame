using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GamesKeystoneFramework.PolarCoordinates
{
    public struct PolarCoordinates : IEquatable<PolarCoordinates>, IFormattable
    {
        public float Radius;
        public float Angle;

        //以下は近似値を保存してある
        private static readonly PolarCoordinates UpCoordinates = new(1, 1.5707964f);
        private static readonly PolarCoordinates DownCoordinates = new(1, -1.5707964f);
        private static readonly PolarCoordinates LeftCoordinates = new(1, 3.1415927f);
        private static readonly PolarCoordinates RightCoordinates = new(1, 0);

        /// <summary>
        /// 初期化等を可能にする
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="angle"></param>
        public PolarCoordinates(float radius, float angle)
        {
            this.Radius = radius;
            this.Angle = angle;
        }

        /// <summary>
        /// 個別の初期化を作る
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public float this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return index switch
                {
                    0 => Radius,
                    1 => Angle,
                    _ => throw new IndexOutOfRangeException("Invalid PolarCoordinates index!"),
                };
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                switch (index)
                {
                    case 0:
                        Radius = value;
                        break;
                    case 1:
                        Angle = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid PolarCoordinates index!");
                }
            }

        }



        /// <summary>
        /// Vector2に変換を行う
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2 ToVector2()
        {
            return new Vector2(Radius * Mathf.Cos(Angle), Radius * Mathf.Sin(Angle));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return Radius.GetHashCode() ^ (Angle.GetHashCode() << 2);
        }

        public static PolarCoordinates Up
        {
            get { return UpCoordinates; }
        }
        public static PolarCoordinates Down
        {
            get { return DownCoordinates; }
        }
        public static PolarCoordinates Left
        {
            get { return LeftCoordinates; }
        }
        public static PolarCoordinates Right
        {
            get { return RightCoordinates; }
        }

        //汎用メソッドなどを書く
        /// <summary>
        /// 0から2πの間にまとめる
        /// </summary>
        /// <param name="polarCoordinates"></param>
        /// <returns></returns>
        public PolarCoordinates AngleNormalizeZeroBase(PolarCoordinates polarCoordinates)
        {
            Angle = polarCoordinates.Angle % (2 * Mathf.PI); // まずは0から2πに収める
            if (Angle > Mathf.PI)
                Angle -= 2 * Mathf.PI; // 2πを引いて-πからπに収める
            else if (Angle <= -Mathf.PI)
                Angle += 2 * Mathf.PI;
            return new PolarCoordinates(polarCoordinates.Radius, Angle);
        }

        /// <summary>
        /// -πからπの間に丸める
        /// </summary>
        /// <param name="polarCoordinates"></param>
        /// <returns></returns>
        public PolarCoordinates AngleNormalizePIMax(PolarCoordinates polarCoordinates)
        {
            Angle = polarCoordinates.Angle % (2 * Mathf.PI); // まずは0から2πに収める
            if (Angle < 0)
                Angle += 2 * Mathf.PI; // 負の角度を2πを加えて正の範囲に
            return new PolarCoordinates(polarCoordinates.Radius, Angle);
        }


        /// <summary>
        /// 要素が等しいかを確認する
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            if (obj is Vector2 other2)
                return Equals(obj);
            return false;
        }

        /// <summary>
        /// 要素が等しいかを確認する
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(PolarCoordinates other)
        {
            return Radius == other.Radius && Angle == other.Angle;
        }

        /// <summary>
        /// string形式で出力する
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return ToString(null, null);
        }

        /// <summary>
        /// String形式で出力する
        /// </summary>
        /// <param name="format"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                format = "F2";
            formatProvider ??= CultureInfo.InvariantCulture.NumberFormat;

            return $"({Radius.ToString(format, formatProvider)}, {Angle.ToString(format, formatProvider)})";
        }

        //
        //以下は算術演算子のオーバーロード。
        //通常のVector2等と同じように計算しても意味がなく効率が悪いので、計算のルールを特殊なものに
        //
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PolarCoordinates operator +(PolarCoordinates a, PolarCoordinates b)
        {
            return new PolarCoordinates(a.Radius + b.Radius, a.Angle + b.Angle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PolarCoordinates operator -(PolarCoordinates a, PolarCoordinates b)
        {
            return new PolarCoordinates(a.Radius - b.Radius, a.Angle - b.Angle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PolarCoordinates operator *(PolarCoordinates a, PolarCoordinates b)
        {
            return new PolarCoordinates(a.Radius * b.Radius, a.Angle * b.Angle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PolarCoordinates operator /(PolarCoordinates a, PolarCoordinates b)
        {
            return new PolarCoordinates(a.Radius / b.Radius, a.Angle / b.Angle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PolarCoordinates operator -(PolarCoordinates a)
        {
            return new PolarCoordinates(0f - a.Radius, 0f - a.Angle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PolarCoordinates operator *(PolarCoordinates a, float b)
        {
            return new PolarCoordinates(a.Radius * b, a.Angle * b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PolarCoordinates operator *(float a, PolarCoordinates b)
        {
            return new PolarCoordinates(b.Radius * a, b.Angle * a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PolarCoordinates operator /(PolarCoordinates a, float b)
        {
            return new PolarCoordinates(a.Radius * b, a.Angle / b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PolarCoordinates operator /(float a, PolarCoordinates b)
        {
            return new PolarCoordinates(b.Radius * a, b.Angle * a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(PolarCoordinates a, PolarCoordinates b)
        {
            float num = a.Radius - b.Radius;
            float num2 = a.Angle - b.Angle;
            return num * num + num2 * num2 < 9.99999944E-11f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(PolarCoordinates a, PolarCoordinates b)
        {
            return !(a == b);
        }
        //暗黙的な型変換を後で作る(三次元極座標)
    }
    public class PolarCoordinatesSupport
    {
        public PolarCoordinates ToPolarCoordinates(float x, float y)
        {
            return new PolarCoordinates((float)Math.Sqrt(x * x + y * y), (float)Math.Atan(y / x));
        }
        public PolarCoordinates ToPolarCoordinates(Vector2 vector2)
        {
            return new PolarCoordinates((float)Math.Sqrt(vector2.x * vector2.x + vector2.y * vector2.y), (float)Math.Atan(vector2.y / vector2.x));
        }
        public float GetAngle(float x, float y)
        {
            return (float)Math.Sqrt(x * x + y * y);
        }
        public float GetAngle(Vector2 vector2)
        {
            return (float)Math.Sqrt(vector2.x * vector2.x + vector2.y * vector2.y);
        }
        public float GetRadius(float x, float y)
        {
            return (float)(Math.Atan(y / x));
        }
        public float GetRadius(Vector2 vector2)
        {
            return (float)(Math.Atan(vector2.y / vector2.x));
        }
    }
}