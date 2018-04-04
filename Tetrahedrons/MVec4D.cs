using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Runtime.Remoting.Messaging;
using System.Text;
using OpenTK;
using OpenTK.Graphics.ES30;

namespace Tetrahedrons
{
   public struct MVec4D
   {
      #region Units

      public static readonly MVec4D Zero = new MVec4D(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
      public static readonly MVec4D One = new MVec4D(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);

      public static readonly MVec4D Unit = new MVec4D(1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
      public static readonly MVec4D UnitX = new MVec4D(0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
      public static readonly MVec4D UnitY = new MVec4D(0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
      public static readonly MVec4D UnitZ = new MVec4D(0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
      public static readonly MVec4D UnitW = new MVec4D(0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
      public static readonly MVec4D UnitXy = new MVec4D(0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
      public static readonly MVec4D UnitXz = new MVec4D(0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0);
      public static readonly MVec4D UnitXw = new MVec4D(0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0);
      public static readonly MVec4D UnitYz = new MVec4D(0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0);
      public static readonly MVec4D UnitYw = new MVec4D(0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0);
      public static readonly MVec4D UnitZw = new MVec4D(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
      public static readonly MVec4D UnitXyz = new MVec4D(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
      public static readonly MVec4D UnitXyw = new MVec4D(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
      public static readonly MVec4D UnitXzw = new MVec4D(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
      public static readonly MVec4D UnitYzw = new MVec4D(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0);
      public static readonly MVec4D UnitXyzw = new MVec4D(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1);

      #endregion

      public double S, X, Y, Z, W, Xy, Xz, Xw, Yz, Yw, Zw, Xyz, Xyw, Xzw, Yzw, Xyzw;

      public double Norm2 => S * S + X * X + Y * Y + Z * Z + W * W + Xy * Xy + Xz * Xz + Xw * Xw + Yz * Yz + Yw * Yw + Zw * Zw + Xyz * Xyz +
                             Xyw * Xyw + Xzw * Xzw + Yzw * Yzw + Xyzw * Xyzw;

      public double Norm => Math.Sqrt(Norm2);

      public MVec4D Grade(int k)
      {
         switch (k)
         {
            case 0:
               return Vec0(S);
            case 1:
               return Vec1(X, Y, Z, W);
            case 2:
               return Vec2(Xy, Xz, Xw, Yz, Yw, Zw);
            case 3:
               return Vec3(Xyz, Xyw, Xzw, Yzw);
            case 4:
               return Vec4(Xyzw);
            default:
               return Zero;
         }
      }

      public Vector4d V4 => new Vector4d(X, Y, Z, W);
      public Vector3d V3 => new Vector3d(X, Y, Z);

      #region Swizzle

      public double Yx
      {
         get => -Xy;
         set => Xy = -value;
      }

      public double Zx
      {
         get => -Xz;
         set => Xz = -value;
      }

      public double Wx
      {
         get => -Xw;
         set => Xw = -value;
      }

      public double Zy
      {
         get => -Yz;
         set => Yz = -value;
      }

      public double Wy
      {
         get => -Yw;
         set => Yw = -value;
      }

      public double Wz
      {
         get => -Zw;
         set => Zw = -value;
      }

      public double Xzy
      {
         get => -Xyz;
         set => Xyz = -value;
      }

      public double Yxz
      {
         get => -Xyz;
         set => Xyz = -value;
      }

      public double Yzx
      {
         get => Xyz;
         set => Xyz = value;
      }

      public double Zxy
      {
         get => Xyz;
         set => Xyz = value;
      }

      public double Zyx
      {
         get => -Xyz;
         set => Xyz = -value;
      }

      public double Xwy
      {
         get => -Xyw;
         set => Xyw = -value;
      }

      public double Yxw
      {
         get => -Xyw;
         set => Xyw = -value;
      }

      public double Ywx
      {
         get => Xyw;
         set => Xyw = value;
      }

      public double Wxy
      {
         get => Xyw;
         set => Xyw = value;
      }

      public double Wyx
      {
         get => -Xyw;
         set => Xyw = -value;
      }

      public double Xwz
      {
         get => -Xzw;
         set => Xzw = -value;
      }

      public double Zxw
      {
         get => -Xzw;
         set => Xzw = -value;
      }

      public double Zwx
      {
         get => Xzw;
         set => Xzw = value;
      }

      public double Wxz
      {
         get => Xzw;
         set => Xzw = value;
      }

      public double Wzx
      {
         get => -Xzw;
         set => Xzw = -value;
      }

      public double Ywz
      {
         get => -Yzw;
         set => Yzw = -value;
      }

      public double Zyw
      {
         get => -Yzw;
         set => Yzw = -value;
      }

      public double Zwy
      {
         get => Yzw;
         set => Yzw = value;
      }

      public double Wyz
      {
         get => Yzw;
         set => Yzw = value;
      }

      public double Wzy
      {
         get => -Yzw;
         set => Yzw = -value;
      }

      public double Xywz
      {
         get => -Xyzw;
         set => Xyzw = -value;
      }

      public double Xzyw
      {
         get => -Xyzw;
         set => Xyzw = -value;
      }

      public double Xzwy
      {
         get => Xyzw;
         set => Xyzw = value;
      }

      public double Xwyz
      {
         get => Xyzw;
         set => Xyzw = value;
      }

      public double Xwzy
      {
         get => -Xyzw;
         set => Xyzw = -value;
      }

      public double Yxzw
      {
         get => -Xyzw;
         set => Xyzw = -value;
      }

      public double Yxwz
      {
         get => Xyzw;
         set => Xyzw = value;
      }

      public double Yzxw
      {
         get => Xyzw;
         set => Xyzw = value;
      }

      public double Yzwx
      {
         get => -Xyzw;
         set => Xyzw = -value;
      }

      public double Ywxz
      {
         get => -Xyzw;
         set => Xyzw = -value;
      }

      public double Ywzx
      {
         get => Xyzw;
         set => Xyzw = value;
      }

      public double Zxyw
      {
         get => Xyzw;
         set => Xyzw = value;
      }

      public double Zxwy
      {
         get => -Xyzw;
         set => Xyzw = -value;
      }

      public double Zyxw
      {
         get => -Xyzw;
         set => Xyzw = -value;
      }

      public double Zywx
      {
         get => Xyzw;
         set => Xyzw = value;
      }

      public double Zwxy
      {
         get => Xyzw;
         set => Xyzw = value;
      }

      public double Zwyx
      {
         get => -Xyzw;
         set => Xyzw = -value;
      }

      public double Wxyz
      {
         get => -Xyzw;
         set => Xyzw = -value;
      }

      public double Wxzy
      {
         get => Xyzw;
         set => Xyzw = value;
      }

      public double Wyxz
      {
         get => Xyzw;
         set => Xyzw = value;
      }

      public double Wyzx
      {
         get => -Xyzw;
         set => Xyzw = -value;
      }

      public double Wzxy
      {
         get => -Xyzw;
         set => Xyzw = -value;
      }

      public double Wzyx
      {
         get => Xyzw;
         set => Xyzw = value;
      }

      #endregion

      #region Inverses

      public MVec4D Neg => new MVec4D(-S, -X, -Y, -Z, -W, -Xy, -Xz, -Xw, -Yz, -Yw, -Zw, -Xyz, -Xyw, -Xzw, -Yzw, -Xyzw);

      public MVec4D? Inv
      {
         get
         {
            var v = Mul(this, Rev);
            if (v == Vec0(v.S))
               return Mul(1d / v.S, Rev);

            return null;
         }
      }

      public MVec4D Rev => new MVec4D(S, X, Y, Z, W, Yx, Zx, Wx, Zy, Wy, Wz, Zyx, Wyx, Wzx, Wzy, Wzyx);

      public MVec4D Conj => new MVec4D(Xyzw, -Yzw, Xzw, -Xyw, -Xyz, Zw, Yw, Yz, Xw, Xz, Xy, -W, Z, Y, X, S);

      public MVec4D Dual => new MVec4D(Xyzw, -Yzw, -Xzw, -Xyw, Xyz, -Zw, Yw, -Yz, -Xw, Xz, -Xy, W, -Z, Y, -Xz, S);

      #endregion

      #region Constructor

      public MVec4D(double s, double x, double y, double z, double w, double xy, double xz, double xw, double yz, double yw, double zw, double xyz,
         double xyw, double xzw, double yzw, double xyzw)
      {
         S = s;
         X = x;
         Y = y;
         Z = z;
         W = w;
         Xy = xy;
         Xz = xz;
         Xw = xw;
         Yz = yz;
         Yw = yw;
         Zw = zw;
         Xyz = xyz;
         Xyw = xyw;
         Xzw = xzw;
         Yzw = yzw;
         Xyzw = xyzw;
      }

      public MVec4D(MVec4D s, MVec4D v, MVec4D b, MVec4D t, MVec4D q)
      {
         S = s.S;
         X = v.X;
         Y = v.Y;
         Z = v.Z;
         W = v.W;
         Xy = b.Xy;
         Xz = b.Xz;
         Xw = b.Xw;
         Yz = b.Yz;
         Yw = b.Yw;
         Zw = b.Zw;
         Xyz = t.Xyz;
         Xyw = t.Xyw;
         Xzw = t.Xzw;
         Yzw = t.Yzw;
         Xyzw = q.Xyzw;
      }

      #endregion

      #region Factory Methods

      public static MVec4D Vec0(double s)
      {
         return new MVec4D(s, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
      }

      public static MVec4D Vec1(double x, double y, double z, double w)
      {
         return new MVec4D(0, x, y, z, w, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
      }

      public static MVec4D Vec2(double xy, double xz, double xw, double yz, double yw, double zw)
      {
         return new MVec4D(0, 0, 0, 0, 0, xy, xz, xw, yz, yw, zw, 0, 0, 0, 0, 0);
      }

      public static MVec4D Vec3(double xyz, double xyw, double xzw, double yzw)
      {
         return new MVec4D(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, xyz, xyw, xzw, yzw, 0);
      }

      public static MVec4D Vec4(double xyzw)
      {
         return new MVec4D(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, xyzw);
      }

      public static MVec4D Rotor(double angle, MVec4D plane)
      {
         return Add(Math.Cos(angle), Mul(Math.Sin(angle), plane));
      }

      #endregion

      #region Operations

      public static MVec4D Add(MVec4D m, MVec4D n)
      {
         return new MVec4D(
            m.S + n.S,
            m.X + n.X,
            m.Y + n.Y,
            m.Z + n.Z,
            m.W + n.W,
            m.Xy + n.Xy,
            m.Xz + n.Xz,
            m.Xw + n.Xw,
            m.Yz + n.Yz,
            m.Yw + n.Yw,
            m.Zw + n.Zw,
            m.Xyz + n.Xyz,
            m.Xyw + n.Xyw,
            m.Xzw + n.Xzw,
            m.Yzw + n.Yzw,
            m.Xyzw + n.Xyzw
         );
      }

      public static MVec4D Add(MVec4D m, double c) => Add(c, m);

      public static MVec4D Add(double c, MVec4D m)
      {
         return new MVec4D(
            m.S + c,
            m.X,
            m.Y,
            m.Z,
            m.W,
            m.Xy,
            m.Xz,
            m.Xw,
            m.Yz,
            m.Yw,
            m.Zw,
            m.Xyz,
            m.Xyw,
            m.Xzw,
            m.Yzw,
            m.Xyzw
         );
      }

      public static MVec4D Sub(MVec4D m, MVec4D n)
      {
         return new MVec4D(
            m.S - n.S,
            m.X - n.X,
            m.Y - n.Y,
            m.Z - n.Z,
            m.W - n.W,
            m.Xy - n.Xy,
            m.Xz - n.Xz,
            m.Xw - n.Xw,
            m.Yz - n.Yz,
            m.Yw - n.Yw,
            m.Zw - n.Zw,
            m.Xyz - n.Xyz,
            m.Xyw - n.Xyw,
            m.Xzw - n.Xzw,
            m.Yzw - n.Yzw,
            m.Xyzw - n.Xyzw
         );
      }

      public static MVec4D Sub(MVec4D m, double c)
      {
         return new MVec4D(
            m.S - c,
            m.X,
            m.Y,
            m.Z,
            m.W,
            m.Xy,
            m.Xz,
            m.Xw,
            m.Yz,
            m.Yw,
            m.Zw,
            m.Xyz,
            m.Xyw,
            m.Xzw,
            m.Yzw,
            m.Xyzw
         );
      }

      public static MVec4D Sub(double c, MVec4D m)
      {
         return new MVec4D(
            c - m.S,
            -m.X,
            -m.Y,
            -m.Z,
            -m.W,
            -m.Xy,
            -m.Xz,
            -m.Xw,
            -m.Yz,
            -m.Yw,
            -m.Zw,
            -m.Xyz,
            -m.Xyw,
            -m.Xzw,
            -m.Yzw,
            -m.Xyzw
         );
      }

      public static MVec4D Mul(MVec4D m, MVec4D n)
      {
         var r = new MVec4D();
         if (Math.Abs(m.S) > 1e-10)
         {
            r.S += m.S * n.S;
            r.X += m.S * n.X;
            r.Y += m.S * n.Y;
            r.Z += m.S * n.Z;
            r.W += m.S * n.W;
            r.Xy += m.S * n.Xy;
            r.Xz += m.S * n.Xz;
            r.Xw += m.S * n.Xw;
            r.Yz += m.S * n.Yz;
            r.Yw += m.S * n.Yw;
            r.Zw += m.S * n.Zw;
            r.Xyz += m.S * n.Xyz;
            r.Xyw += m.S * n.Xyw;
            r.Xzw += m.S * n.Xzw;
            r.Yzw += m.S * n.Yzw;
            r.Xyzw += m.S * n.Xyzw;
         }

         if (Math.Abs(m.X) > 1e-10)
         {
            r.S += m.X * n.X;
            r.X += m.X * n.S;
            r.Y += m.X * n.Xy;
            r.Z += m.X * n.Xz;
            r.W += m.X * n.Xw;
            r.Xy += m.X * n.Y;
            r.Xz += m.X * n.Z;
            r.Xw += m.X * n.W;
            r.Yz += m.X * n.Xyz;
            r.Yw += m.X * n.Xyw;
            r.Zw += m.X * n.Xzw;
            r.Xyz += m.X * n.Yz;
            r.Xyw += m.X * n.Yw;
            r.Xzw += m.X * n.Zw;
            r.Yzw += m.X * n.Xyzw;
            r.Xyzw += m.X * n.Yzw;
         }

         if (Math.Abs(m.Y) > 1e-10)
         {
            r.S += m.Y * n.Y;
            r.X += m.Y * n.Yx;
            r.Y += m.Y * n.S;
            r.Z += m.Y * n.Yz;
            r.W += m.Y * n.Yw;
            r.Yx += m.Y * n.X;
            r.Xz += m.Y * n.Yxz;
            r.Xw += m.Y * n.Yxw;
            r.Yz += m.Y * n.Z;
            r.Yw += m.Y * n.W;
            r.Zw += m.Y * n.Yzw;
            r.Yxz += m.Y * n.Xz;
            r.Yxw += m.Y * n.Xw;
            r.Xzw += m.Y * n.Yxzw;
            r.Yzw += m.Y * n.Zw;
            r.Yxzw += m.Y * n.Xzw;
         }

         if (Math.Abs(m.Z) > 1e-10)
         {
            r.S += m.Z * n.Z;
            r.X += m.Z * n.Zx;
            r.Y += m.Z * n.Zy;
            r.Z += m.Z * n.S;
            r.W += m.Z * n.Zw;
            r.Xy += m.Z * n.Zxy;
            r.Zx += m.Z * n.X;
            r.Xw += m.Z * n.Zxw;
            r.Zy += m.Z * n.Y;
            r.Yw += m.Z * n.Zyw;
            r.Zw += m.Z * n.W;
            r.Zxy += m.Z * n.Xy;
            r.Xyw += m.Z * n.Zxyw;
            r.Zxw += m.Z * n.Xw;
            r.Zyw += m.Z * n.Yw;
            r.Zxyw += m.Z * n.Xyw;
         }

         if (Math.Abs(m.W) > 1e-10)
         {
            r.S += m.W * n.W;
            r.X += m.W * n.Wx;
            r.Y += m.W * n.Wy;
            r.Z += m.W * n.Wz;
            r.W += m.W * n.S;
            r.Xy += m.W * n.Wxy;
            r.Xz += m.W * n.Wxz;
            r.Wx += m.W * n.X;
            r.Yz += m.W * n.Wyz;
            r.Wy += m.W * n.Y;
            r.Wz += m.W * n.Z;
            r.Xyz += m.W * n.Wxyz;
            r.Wxy += m.W * n.Xy;
            r.Wxz += m.W * n.Xz;
            r.Wyz += m.W * n.Yz;
            r.Wxyz += m.W * n.Xyz;
         }

         if (Math.Abs(m.Xy) > 1e-10)
         {
            r.S += m.Xy * n.Yx;
            r.X += m.Xy * n.Y;
            r.Y += m.Yx * n.X;
            r.Z += m.Xy * n.Yxz;
            r.W += m.Xy * n.Yxw;
            r.Xy += m.Xy * n.S;
            r.Xz += m.Xy * n.Yz;
            r.Xw += m.Xy * n.Yw;
            r.Yz += m.Yx * n.Xz;
            r.Yw += m.Yx * n.Xw;
            r.Zw += m.Xy * n.Yxzw;
            r.Xyz += m.Xy * n.Z;
            r.Xyw += m.Xy * n.W;
            r.Xzw += m.Xy * n.Yzw;
            r.Yzw += m.Yx * n.Xzw;
            r.Xyzw += m.Xy * n.Zw;
         }

         if (Math.Abs(m.Xz) > 1e-10)
         {
            r.S += m.Xz * n.Zx;
            r.X += m.Xz * n.Z;
            r.Y += m.Xz * n.Zxy;
            r.Z += m.Zx * n.X;
            r.W += m.Xz * n.Zxw;
            r.Xy += m.Xz * n.Zy;
            r.Xz += m.Xz * n.S;
            r.Xw += m.Xz * n.Zw;
            r.Zy += m.Zx * n.Xy;
            r.Yw += m.Xz * n.Zxyw;
            r.Zw += m.Zx * n.Xw;
            r.Xzy += m.Xz * n.Y;
            r.Xyw += m.Xz * n.Zyw;
            r.Xzw += m.Xz * n.W;
            r.Zyw += m.Zx * n.Xyw;
            r.Xzyw += m.Xz * n.Yw;
         }

         if (Math.Abs(m.Xw) > 1e-10)
         {
            r.S += m.Xw * n.Wx;
            r.X += m.Xw * n.W;
            r.Y += m.Xw * n.Wxy;
            r.Z += m.Xw * n.Wxz;
            r.W += m.Wx * n.X;
            r.Xy += m.Xw * n.Wy;
            r.Xz += m.Xw * n.Wz;
            r.Xw += m.Xw * n.S;
            r.Yz += m.Xw * n.Wxyz;
            r.Wy += m.Wx * n.Xy;
            r.Wz += m.Wx * n.Xz;
            r.Xyz += m.Xw * n.Wyz;
            r.Xwy += m.Xw * n.Y;
            r.Xwz += m.Xw * n.Z;
            r.Wyz += m.Wx * n.Xyz;
            r.Xwyz += m.Xw * n.Yz;
         }

         if (Math.Abs(m.Yz) > 1e-10)
         {
            r.S += m.Yz * n.Zy;
            r.X += m.Yz * n.Zyx;
            r.Y += m.Yz * n.Z;
            r.Z += m.Zy * n.Y;
            r.W += m.Yz * n.Zyw;
            r.Yx += m.Yz * n.Zx;
            r.Zx += m.Zy * n.Yx;
            r.Xw += m.Yz * n.Zyxw;
            r.Yz += m.Yz * n.S;
            r.Yw += m.Yz * n.Zw;
            r.Zw += m.Zy * n.Yw;
            r.Yzx += m.Yz * n.X;
            r.Yxw += m.Yz * n.Zxw;
            r.Zxw += m.Zy * n.Yxw;
            r.Yzw += m.Yz * n.W;
            r.Yzxw += m.Yz * n.Xw;
         }

         if (Math.Abs(m.Yw) > 1e-10)
         {
            r.S += m.Yw * n.Wy;
            r.X += m.Yw * n.Wyx;
            r.Y += m.Yw * n.W;
            r.Z += m.Yw * n.Wyz;
            r.W += m.Wy * n.Y;
            r.Yx += m.Yw * n.Wx;
            r.Xz += m.Yw * n.Wyxz;
            r.Wx += m.Wy * n.Yx;
            r.Yz += m.Yw * n.Wz;
            r.Yw += m.Yw * n.S;
            r.Wz += m.Wy * n.Yz;
            r.Yxz += m.Yw * n.Wxz;
            r.Ywx += m.Yw * n.X;
            r.Wxz += m.Wy * n.Yxz;
            r.Ywz += m.Yw * n.Z;
            r.Ywxz += m.Yw * n.Xz;
         }

         if (Math.Abs(m.Zw) > 1e-10)
         {
            r.S += m.Zw * n.Wz;
            r.X += m.Zw * n.Wzx;
            r.Y += m.Zw * n.Wzy;
            r.Z += m.Zw * n.W;
            r.W += m.Wz * n.Z;
            r.Xy += m.Zw * n.Wzxy;
            r.Zx += m.Zw * n.Wx;
            r.Wx += m.Wz * n.Zx;
            r.Zy += m.Zw * n.Wy;
            r.Wy += m.Wz * n.Zy;
            r.Zw += m.Zw * n.S;
            r.Zxy += m.Zw * n.Wxy;
            r.Wxy += m.Wz * n.Zxy;
            r.Zwx += m.Zw * n.X;
            r.Zwy += m.Zw * n.Y;
            r.Zwxy += m.Zw * n.Xy;
         }

         if (Math.Abs(m.Xyz) > 1e-10)
         {
            r.S += m.Xyz * n.Zyx;
            r.X += m.Xyz * n.Zy;
            r.Y += m.Yxz * n.Zx;
            r.Z += m.Zxy * n.Yx;
            r.W += m.Xyz * n.Zyxw;
            r.Xy += m.Xyz * n.Z;
            r.Xz += m.Xzy * n.Y;
            r.Xw += m.Xyz * n.Zyw;
            r.Yz += m.Yzx * n.X;
            r.Yw += m.Yxz * n.Zxw;
            r.Zw += m.Zxy * n.Yxw;
            r.Xyz += m.Xyz * n.S;
            r.Xyw += m.Xyz * n.Zw;
            r.Xzw += m.Xzy * n.Yw;
            r.Yzw += m.Yzx * n.Xw;
            r.Xyzw += m.Xyz * n.W;
         }

         if (Math.Abs(m.Xyw) > 1e-10)
         {
            r.S += m.Xyw * n.Wyx;
            r.X += m.Xyw * n.Wy;
            r.Y += m.Yxw * n.Wx;
            r.Z += m.Xyw * n.Wyxz;
            r.W += m.Wxy * n.Yx;
            r.Xy += m.Xyw * n.W;
            r.Xz += m.Xyw * n.Wyz;
            r.Xw += m.Xwy * n.Y;
            r.Yz += m.Yxw * n.Wxz;
            r.Yw += m.Ywx * n.X;
            r.Wz += m.Wxy * n.Yxz;
            r.Xyz += m.Xyw * n.Wz;
            r.Xyw += m.Xyw * n.S;
            r.Xwz += m.Xwy * n.Yz;
            r.Ywz += m.Ywx * n.Xz;
            r.Xywz += m.Xyw * n.Z;
         }

         if (Math.Abs(m.Xzw) > 1e-10)
         {
            r.S += m.Xzw * n.Wzx;
            r.X += m.Xzw * n.Wz;
            r.Y += m.Xzw * n.Wzxy;
            r.Z += m.Zxw * n.Wx;
            r.W += m.Wxz * n.Zx;
            r.Xy += m.Xzw * n.Wzy;
            r.Xz += m.Xzw * n.W;
            r.Xw += m.Xwz * n.Z;
            r.Zy += m.Zxw * n.Wxy;
            r.Wy += m.Wxz * n.Zxy;
            r.Zw += m.Zwx * n.X;
            r.Xzy += m.Xzw * n.Wy;
            r.Xwy += m.Xwz * n.Zy;
            r.Xzw += m.Xzw * n.S;
            r.Zwy += m.Zwx * n.Xy;
            r.Xzwy += m.Xzw * n.Y;
         }

         if (Math.Abs(m.Yzw) > 1e-10)
         {
            r.S += m.Yzw * n.Wzy;
            r.X += m.Yzw * n.Wzyx;
            r.Y += m.Yzw * n.Wz;
            r.Z += m.Zyw * n.Wy;
            r.W += m.Wyz * n.Zy;
            r.Yx += m.Yzw * n.Wzx;
            r.Zx += m.Zyw * n.Wyx;
            r.Wx += m.Wyz * n.Zyx;
            r.Yz += m.Yzw * n.W;
            r.Yw += m.Ywz * n.Z;
            r.Zw += m.Zwy * n.Y;
            r.Yzx += m.Yzw * n.Wx;
            r.Ywx += m.Ywz * n.Zx;
            r.Zwx += m.Zwy * n.Yx;
            r.Yzw += m.Yzw * n.S;
            r.Yzwx += m.Yzw * n.X;
         }

         if (Math.Abs(m.Xyzw) > 1e-10)
         {
            r.S += m.Xyzw * n.Wzyx;
            r.X += m.Xyzw * n.Wzy;
            r.Y += m.Yxzw * n.Wzx;
            r.Z += m.Zxyw * n.Wyx;
            r.W += m.Wxyz * n.Zyx;
            r.Xy += m.Xyzw * n.Wz;
            r.Xz += m.Xzyw * n.Wy;
            r.Xw += m.Xwyz * n.Zy;
            r.Yz += m.Yzxw * n.Wx;
            r.Yw += m.Ywxz * n.Zx;
            r.Zw += m.Zwxy * n.Yx;
            r.Xyz += m.Xyzw * n.W;
            r.Xyw += m.Xywz * n.Z;
            r.Xzw += m.Xzwy * n.Y;
            r.Yzw += m.Yzwx * n.X;
            r.Xyzw += m.Xyzw * n.S;
         }

         return r;
      }

      public static MVec4D Mul(MVec4D m, double c) => Mul(c, m);

      public static MVec4D Mul(double c, MVec4D m)
      {
         return new MVec4D(
            c * m.S,
            c * m.X,
            c * m.Y,
            c * m.Z,
            c * m.W,
            c * m.Xy,
            c * m.Xz,
            c * m.Xw,
            c * m.Yz,
            c * m.Yw,
            c * m.Zw,
            c * m.Xyz,
            c * m.Xyw,
            c * m.Xzw,
            c * m.Yzw,
            c * m.Xyzw
         );
      }

      public static MVec4D? Div(MVec4D m, MVec4D n) => Mul(m, n.Inv);

      public static MVec4D Div(MVec4D m, double c) => Mul(m, 1 / c);

      public static MVec4D? Div(double c, MVec4D n) => Mul(c, n.Inv);

      public static MVec4D Inner(MVec4D m, MVec4D n)
      {
         var m0 = m.Grade(0);
         var m1 = m.Grade(1);
         var m2 = m.Grade(2);
         var m3 = m.Grade(3);
         var m4 = m.Grade(4);

         var n0 = n.Grade(0);
         var n1 = n.Grade(1);
         var n2 = n.Grade(2);
         var n3 = n.Grade(3);
         var n4 = n.Grade(4);

         return new MVec4D(
            m0 * n0 + m1 * n1 + m2 * n2 + m3 * n3 + n4 * n4,
            m0 * n1 + m1 * n2 + m2 * n3 + m3 * n4,
            m0 * n2 + m1 * n3 + m2 * n4,
            m0 * n3 + m1 * n4,
            m0 * n4
         );
      }

      public static MVec4D Inner(MVec4D m, double c) => Mul(m, c);

      public static MVec4D Inner(double c, MVec4D n) => Mul(c, n);

      public static MVec4D Outer(MVec4D m, MVec4D n)
      {
         var m0 = m.Grade(0);
         var m1 = m.Grade(1);
         var m2 = m.Grade(2);
         var m3 = m.Grade(3);
         var m4 = m.Grade(4);

         var n0 = n.Grade(0);
         var n1 = n.Grade(1);
         var n2 = n.Grade(2);
         var n3 = n.Grade(3);
         var n4 = n.Grade(4);

         return new MVec4D(
            m0 * n0,
            m1 * n0 + m0 * n1,
            m2 * n0 + m1 * n1 + m0 * n2,
            m3 * n0 + m2 * n1 + m1 * n2 + m0 * n3,
            m4 * n0 + m3 * n1 + m2 * n2 + m1 * n3 + m0 * n4
         );
      }

      public static MVec4D Outer(MVec4D m, double c) => Mul(m, c);

      public static MVec4D Outer(double c, MVec4D n) => Mul(c, n);


      public static MVec4D? Add(MVec4D? m, MVec4D? n) => m.HasValue && n.HasValue ? (MVec4D?) Add(m.Value, n.Value) : null;
      public static MVec4D? Add(MVec4D? m, double? n) => m.HasValue && n.HasValue ? (MVec4D?) Add(m.Value, n.Value) : null;
      public static MVec4D? Add(double? m, MVec4D? n) => n.HasValue && m.HasValue ? (MVec4D?) Add(m.Value, n.Value) : null;

      public static MVec4D? Sub(MVec4D? m, MVec4D? n) => m.HasValue && n.HasValue ? (MVec4D?) Sub(m.Value, n.Value) : null;
      public static MVec4D? Sub(MVec4D? m, double? n) => m.HasValue && n.HasValue ? (MVec4D?) Sub(m.Value, n.Value) : null;
      public static MVec4D? Sub(double? m, MVec4D? n) => n.HasValue && m.HasValue ? (MVec4D?) Sub(m.Value, n.Value) : null;

      public static MVec4D? Mul(MVec4D? m, MVec4D? n) => m.HasValue && n.HasValue ? (MVec4D?) Mul(m.Value, n.Value) : null;
      public static MVec4D? Mul(MVec4D? m, double? n) => m.HasValue && n.HasValue ? (MVec4D?) Mul(m.Value, n.Value) : null;
      public static MVec4D? Mul(double? m, MVec4D? n) => n.HasValue && m.HasValue ? (MVec4D?) Mul(m.Value, n.Value) : null;

      public static MVec4D? Div(MVec4D? m, MVec4D? n) => m.HasValue && n.HasValue ? (MVec4D?) Div(m.Value, n.Value) : null;
      public static MVec4D? Div(MVec4D? m, double? n) => m.HasValue && n.HasValue ? (MVec4D?) Div(m.Value, n.Value) : null;
      public static MVec4D? Div(double? m, MVec4D? n) => n.HasValue && m.HasValue ? (MVec4D?) Div(m.Value, n.Value) : null;

      public static MVec4D? Inner(MVec4D? m, MVec4D? n) => m.HasValue && n.HasValue ? (MVec4D?) Inner(m.Value, n.Value) : null;
      public static MVec4D? Inner(MVec4D? m, double? n) => m.HasValue && n.HasValue ? (MVec4D?) Inner(m.Value, n.Value) : null;
      public static MVec4D? Inner(double? m, MVec4D? n) => n.HasValue && m.HasValue ? (MVec4D?) Inner(m.Value, n.Value) : null;

      public static MVec4D? Outer(MVec4D? m, MVec4D? n) => m.HasValue && n.HasValue ? (MVec4D?) Outer(m.Value, n.Value) : null;
      public static MVec4D? Outer(MVec4D? m, double? n) => m.HasValue && n.HasValue ? (MVec4D?) Outer(m.Value, n.Value) : null;
      public static MVec4D? Outer(double? m, MVec4D? n) => n.HasValue && m.HasValue ? (MVec4D?) Outer(m.Value, n.Value) : null;

      #endregion

      #region Operators

      public static MVec4D operator +(MVec4D m, MVec4D n) => Add(m, n);
      public static MVec4D operator +(double c, MVec4D n) => Add(c, n);
      public static MVec4D operator +(MVec4D m, double c) => Add(m, c);

      public static MVec4D operator -(MVec4D m, MVec4D n) => Sub(m, n);
      public static MVec4D operator -(double c, MVec4D n) => Sub(c, n);
      public static MVec4D operator -(MVec4D m, double c) => Sub(m, c);

      public static MVec4D operator *(MVec4D m, MVec4D n) => Mul(m, n);
      public static MVec4D operator *(double c, MVec4D n) => Mul(c, n);
      public static MVec4D operator *(MVec4D m, double c) => Mul(m, c);

      public static MVec4D? operator /(MVec4D m, MVec4D n) => Div(m, n);
      public static MVec4D? operator /(double c, MVec4D n) => Div(c, n);
      public static MVec4D operator /(MVec4D m, double c) => Div(m, c);

      public static MVec4D operator &(MVec4D m, MVec4D n) => Inner(m, n);
      public static MVec4D operator &(MVec4D m, double c) => Inner(m, c);
      public static MVec4D operator &(double c, MVec4D n) => Inner(c, n);

      public static MVec4D operator ^(MVec4D m, MVec4D n) => Outer(m, n);
      public static MVec4D operator ^(MVec4D m, double c) => Outer(m, c);
      public static MVec4D operator ^(double c, MVec4D n) => Outer(c, n);

      public static MVec4D? operator +(MVec4D? m, MVec4D? n) => Add(m, n);
      public static MVec4D? operator +(double? c, MVec4D? n) => Add(c, n);
      public static MVec4D? operator +(MVec4D? m, double? c) => Add(m, c);

      public static MVec4D? operator -(MVec4D? m, MVec4D? n) => Sub(m, n);
      public static MVec4D? operator -(double? c, MVec4D? n) => Sub(c, n);
      public static MVec4D? operator -(MVec4D? m, double? c) => Sub(m, c);

      public static MVec4D? operator *(MVec4D? m, MVec4D? n) => Mul(m, n);
      public static MVec4D? operator *(double? c, MVec4D? n) => Mul(c, n);
      public static MVec4D? operator *(MVec4D? m, double? c) => Mul(m, c);

      public static MVec4D? operator /(MVec4D? m, MVec4D? n) => Div(m, n);
      public static MVec4D? operator /(double? c, MVec4D? n) => Div(c, n);
      public static MVec4D? operator /(MVec4D? m, double? c) => Div(m, c);

      public static MVec4D? operator &(MVec4D? m, MVec4D? n) => Inner(m, n);
      public static MVec4D? operator &(MVec4D? m, double? c) => Inner(m, c);
      public static MVec4D? operator &(double? c, MVec4D? n) => Inner(c, n);

      public static MVec4D? operator ^(MVec4D? m, MVec4D? n) => Outer(m, n);
      public static MVec4D? operator ^(MVec4D? m, double? c) => Outer(m, c);
      public static MVec4D? operator ^(double? c, MVec4D? n) => Outer(c, n);


      public static MVec4D operator -(MVec4D m) => m.Neg;
      public static MVec4D? operator ~(MVec4D m) => m.Inv;
      public static MVec4D operator !(MVec4D m) => m.Rev;

      public static implicit operator MVec4D(double v) => Vec0(v);
      public static implicit operator MVec4D(Vector3d v) => Vec1(v.X, v.Y, v.Z, 0);
      public static implicit operator MVec4D(Vector4d v) => Vec1(v.X, v.Y, v.Z, v.W);

      #endregion

      #region Equality

      public static bool operator ==(MVec4D m, MVec4D n)
      {
         var diff = Sub(m, n).Norm2;
         return Math.Abs(diff) <= 10e-8;
      }

      public static bool operator !=(MVec4D m, MVec4D n)
      {
         var diff = Sub(m, n).Norm2;
         return Math.Abs(diff) > 10e-8;
      }

      #endregion

      #region Formatting

      public override string ToString()
      {
         var sb = new List<string>();
         if (Math.Abs(S) > 10e-2) sb.Add($"{S:F2}");
         if (Math.Abs(X) > 10e-2) sb.Add($"{X:F2}e1");
         if (Math.Abs(Y) > 10e-2) sb.Add($"{Y:F2}e2");
         if (Math.Abs(Z) > 10e-2) sb.Add($"{Z:F2}e3");
         if (Math.Abs(W) > 10e-2) sb.Add($"{W:F2}e4");
         if (Math.Abs(Xy) > 10e-2) sb.Add($"{Xy:F2}e12");
         if (Math.Abs(Yz) > 10e-2) sb.Add($"{Yz:F2}e23");
         if (Math.Abs(Zw) > 10e-2) sb.Add($"{Zw:F2}e34");
         if (Math.Abs(Wx) > 10e-2) sb.Add($"{Wx:F2}e41");
         if (Math.Abs(Xz) > 10e-2) sb.Add($"{Xz:F2}e13");
         if (Math.Abs(Yw) > 10e-2) sb.Add($"{Yw:F2}e24");
         if (Math.Abs(Xyz) > 10e-2) sb.Add($"{Xyz:F2}e123");
         if (Math.Abs(Yzw) > 10e-2) sb.Add($"{Yzw:F2}e234");
         if (Math.Abs(Zwx) > 10e-2) sb.Add($"{Zwx:F2}e341");
         if (Math.Abs(Wxy) > 10e-2) sb.Add($"{Wxy:F2}e412");
         if (Math.Abs(Xyzw) > 10e-2) sb.Add($"{Xyzw:F2}e1234");
         return $"({string.Join(" + ", sb)})";
      }

      #endregion
   }
}