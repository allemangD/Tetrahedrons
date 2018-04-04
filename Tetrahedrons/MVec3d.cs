using System;
using OpenTK;

namespace Tetrahedrons
{
   public struct MVec3D
   {
      public static readonly MVec3D Zero = new MVec3D(0, 0, 0, 0, 0, 0, 0, 0);
      public static readonly MVec3D One = new MVec3D(1, 1, 1, 1, 1, 1, 1, 1);

      public static readonly MVec3D Unit = new MVec3D(1, 0, 0, 0, 0, 0, 0, 0);
      public static readonly MVec3D Unit1 = new MVec3D(0, 1, 0, 0, 0, 0, 0, 0);
      public static readonly MVec3D Unit2 = new MVec3D(0, 0, 1, 0, 0, 0, 0, 0);
      public static readonly MVec3D Unit3 = new MVec3D(0, 0, 0, 1, 0, 0, 0, 0);
      public static readonly MVec3D Unit23 = new MVec3D(0, 0, 0, 0, 1, 0, 0, 0);
      public static readonly MVec3D Unit31 = new MVec3D(0, 0, 0, 0, 0, 1, 0, 0);
      public static readonly MVec3D Unit12 = new MVec3D(0, 0, 0, 0, 0, 0, 1, 0);
      public static readonly MVec3D Unit123 = new MVec3D(0, 0, 0, 0, 0, 0, 0, 1);

      public double E;
      public double E1;
      public double E2;
      public double E3;
      public double E23;
      public double E31;
      public double E12;
      public double E123;

      public MVec3D(double e, double e1, double e2, double e3, double e23, double e31, double e12, double e123)
      {
         E = e;
         E1 = e1;
         E2 = e2;
         E3 = e3;
         E23 = e23;
         E31 = e31;
         E12 = e12;
         E123 = e123;
      }

      public MVec3D(MVec3D s, MVec3D v, MVec3D b, MVec3D t)
         : this(s.E, v.E1, v.E2, v.E3, b.E23, b.E31, b.E12, t.E123)
      {
      }

      public static MVec3D Scalar(double e)
      {
         return new MVec3D(e, 0, 0, 0, 0, 0, 0, 0);
      }

      public static MVec3D Vector(double e1, double e2, double e3)
      {
         return new MVec3D(0, e1, e2, e3, 0, 0, 0, 0);
      }

      public static MVec3D Vector(Vector3d v)
      {
         return new MVec3D(0, v.X, v.Y, v.Z, 0, 0, 0, 0);
      }

      public static MVec3D Bivector(double e23, double e31, double e12)
      {
         return new MVec3D(0, 0, 0, 0, e23, e31, e12, 0);
      }

      public static MVec3D Trivector(double e123)
      {
         return new MVec3D(0, 0, 0, 0, 0, 0, 0, e123);
      }

      public static MVec3D Rotor(double angle, MVec3D plane)
      {
         return Math.Cos(angle) + Math.Sin(angle) * plane;
      }

      public static MVec3D Add(MVec3D m, MVec3D n)
      {
         return new MVec3D(m.E + n.E,
            m.E1 + n.E1, m.E2 + n.E2, m.E3 + n.E3,
            m.E23 + n.E23, m.E31 + n.E31, m.E12 + n.E12,
            m.E123 + n.E123);
      }

      public static MVec3D Sub(MVec3D m, MVec3D n)
      {
         return new MVec3D(m.E - n.E,
            m.E1 - n.E1, m.E2 - n.E2, m.E3 - n.E3,
            m.E23 - n.E23, m.E31 - n.E31, m.E12 - n.E12,
            m.E123 - n.E123);
      }

      public static MVec3D Mult(MVec3D m, MVec3D n)
      {
         return new MVec3D(
            m.E * n.E + m.E1 * n.E1 + m.E2 * n.E2 + m.E3 * n.E3 - m.E23 * n.E23 - m.E31 * n.E31 - m.E12 * n.E12 - m.E123 * n.E123,
            m.E * n.E1 + m.E1 * n.E - m.E2 * n.E12 + m.E3 * n.E31 - m.E23 * n.E123 - m.E31 * n.E3 + m.E12 * n.E2 - m.E123 * n.E23,
            m.E * n.E2 + m.E1 * n.E12 + m.E2 * n.E - m.E3 * n.E23 + m.E23 * n.E3 - m.E31 * n.E123 - m.E12 * n.E1 - m.E123 * n.E31,
            m.E * n.E3 - m.E1 * n.E31 + m.E2 * n.E23 + m.E3 * n.E - m.E23 * n.E2 + m.E31 * n.E1 - m.E12 * n.E123 - m.E123 * n.E12,
            m.E * n.E23 + m.E1 * n.E123 + m.E2 * n.E3 - m.E3 * n.E2 + m.E23 * n.E - m.E31 * n.E12 + m.E12 * n.E31 + m.E123 * n.E1,
            m.E * n.E31 - m.E1 * n.E3 + m.E2 * n.E123 + m.E3 * n.E1 + m.E23 * n.E12 + m.E31 * n.E - m.E12 * n.E23 + m.E123 * n.E2,
            m.E * n.E12 + m.E1 * n.E2 - m.E2 * n.E1 + m.E3 * n.E123 - m.E23 * n.E31 + m.E31 * n.E23 + m.E12 * n.E + m.E123 * n.E3,
            m.E * n.E123 + m.E1 * n.E23 + m.E2 * n.E31 + m.E3 * n.E12 + m.E23 * n.E1 + m.E31 * n.E2 + m.E12 * n.E3 + m.E123 * n.E
         );
      }

      public double Norm2 => E * E + E1 * E1 + E2 * E2 + E3 * E3 + E23 * E23 + E31 * E31 + E12 * E12 + E123 * E123;
      public double Norm => Math.Sqrt(Norm2);

      public MVec3D Inv
      {
         get
         {
            var n2 = Norm2;
            return new MVec3D(E / n2, -E1 / n2, -E2 / n2, -E3 / n2, -E23 / n2, -E31 / n2, -E12 / n2, E123 / n2);
         }
      }

      public MVec3D Grade(int k)
      {
         switch (k)
         {
            case 0:
               return Scalar(E);
            case 1:
               return Vector(E1, E2, E3);
            case 2:
               return Bivector(E23, E31, E12);
            case 3:
               return Trivector(E123);
            default:
               return new MVec3D();
         }
      }

      public static MVec3D Inner(MVec3D m, MVec3D n)
      {
         var m0 = m.Grade(0);
         var m1 = m.Grade(1);
         var m2 = m.Grade(2);
         var m3 = m.Grade(3);

         var n0 = n.Grade(0);
         var n1 = n.Grade(1);
         var n2 = n.Grade(2);
         var n3 = n.Grade(3);

         return new MVec3D(
            m0 * n0 + m1 * n1 + m2 * n2 + m3 * n3,
            m0 * n1 + m1 * n2 + m2 * n3,
            m0 * n2 + m1 * n3,
            m0 * n3
         );
      }

      public static MVec3D Outer(MVec3D m, MVec3D n)
      {
         var m0 = m.Grade(0);
         var m1 = m.Grade(1);
         var m2 = m.Grade(2);
         var m3 = m.Grade(3);

         var n0 = n.Grade(0);
         var n1 = n.Grade(1);
         var n2 = n.Grade(2);
         var n3 = n.Grade(3);

         return new MVec3D(
            m0 * n0,
            m1 * n0 + m0 * n1,
            m2 * n0 + m1 * n1 + m0 * n2,
            m3 * n0 + m2 * n1 + m1 * n2 + m0 * m3
         );
      }

      public static MVec3D operator ~(MVec3D m) => m.Inv;
      public static MVec3D operator *(MVec3D m, MVec3D n) => Mult(m, n);

      public static MVec3D operator +(MVec3D m, MVec3D n) => Add(m, n);
      public static MVec3D operator -(MVec3D m, MVec3D n) => Sub(m, n);

      public static MVec3D operator &(MVec3D m, MVec3D n) => Inner(m, n);
      public static MVec3D operator ^(MVec3D m, MVec3D n) => Outer(m, n);

      public Vector3d VectorPart => new Vector3d(E1, E2, E3);

      public static implicit operator MVec3D(Vector3 v) => new MVec3D(0, v.X, v.Y, v.Z, 0, 0, 0, 0);
      public static implicit operator MVec3D(Vector3d v) => new MVec3D(0, v.X, v.Y, v.Z, 0, 0, 0, 0);
      public static implicit operator MVec3D(double s) => new MVec3D(s, 0, 0, 0, 0, 0, 0, 0);

      public override string ToString()
      {
         return $"({E:0.00} {E1:0.00}e1 {E2:0.00}e2 {E3:0.00}e3 {E23:0.00}e23 {E31:0.00}e31 {E12:0.00}e12 {E123:0.00}e123)";
      }
   }
}