using System;
using OpenTK;

namespace Tetrahedrons
{
    // ReSharper disable once InconsistentNaming
    public struct MVec3d
    {
        public static readonly MVec3d Zero = new MVec3d(0, 0, 0, 0, 0, 0, 0, 0);
        public static readonly MVec3d One = new MVec3d(1, 1, 1, 1, 1, 1, 1, 1);

        public static readonly MVec3d Unit = new MVec3d(1, 0, 0, 0, 0, 0, 0, 0);
        public static readonly MVec3d Unit1 = new MVec3d(0, 1, 0, 0, 0, 0, 0, 0);
        public static readonly MVec3d Unit2 = new MVec3d(0, 0, 1, 0, 0, 0, 0, 0);
        public static readonly MVec3d Unit3 = new MVec3d(0, 0, 0, 1, 0, 0, 0, 0);
        public static readonly MVec3d Unit23 = new MVec3d(0, 0, 0, 0, 1, 0, 0, 0);
        public static readonly MVec3d Unit31 = new MVec3d(0, 0, 0, 0, 0, 1, 0, 0);
        public static readonly MVec3d Unit12 = new MVec3d(0, 0, 0, 0, 0, 0, 1, 0);
        public static readonly MVec3d Unit123 = new MVec3d(0, 0, 0, 0, 0, 0, 0, 1);

        public double E;
        public double E1;
        public double E2;
        public double E3;
        public double E23;
        public double E31;
        public double E12;
        public double E123;

        public MVec3d(double e, double e1, double e2, double e3, double e23, double e31, double e12, double e123)
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

        public MVec3d(MVec3d s, MVec3d v, MVec3d b, MVec3d t)
            : this(s.E, v.E1, v.E2, v.E3, b.E23, b.E31, b.E12, t.E123)
        {
        }

        public static MVec3d Scalar(double e) => new MVec3d(e, 0, 0, 0, 0, 0, 0, 0);
        public static MVec3d Vector(double e1, double e2, double e3) => new MVec3d(0, e1, e2, e3, 0, 0, 0, 0);
        public static MVec3d Vector(Vector3d v) => new MVec3d(0, v.X, v.Y, v.Z, 0, 0, 0, 0);
        public static MVec3d Bivector(double e23, double e31, double e12) => new MVec3d(0, 0, 0, 0, e23, e31, e12, 0);
        public static MVec3d Trivector(double e123) => new MVec3d(0, 0, 0, 0, 0, 0, 0, e123);
        public static MVec3d Complex(double real, double imag) => new MVec3d(real, 0, 0, 0, 0, 0, 0, imag);
        public static MVec3d Complex(double w, double i, double j, double k) => new MVec3d(w, 0, 0, 0, i, j, k, 0);
        public static MVec3d Complex(double angle, MVec3d plane) => Math.Cos(angle) + Math.Sin(angle) * plane;

        public static MVec3d Add(MVec3d m, MVec3d n)
        {
            return new MVec3d(m.E + n.E,
                m.E1 + n.E1, m.E2 + n.E2, m.E3 + n.E3,
                m.E23 + n.E23, m.E31 + n.E31, m.E12 + n.E12,
                m.E123 + n.E123);
        }

        public static MVec3d Sub(MVec3d m, MVec3d n)
        {
            return new MVec3d(m.E - n.E,
                m.E1 - n.E1, m.E2 - n.E2, m.E3 - n.E3,
                m.E23 - n.E23, m.E31 - n.E31, m.E12 - n.E12,
                m.E123 - n.E123);
        }

        public static MVec3d Mult(MVec3d m, MVec3d n)
        {
            var r = new MVec3d();

            if (Math.Abs(m.E) > 1e-8)
            {
                r.E += m.E * n.E;
                r.E1 += m.E * n.E1;
                r.E2 += m.E * n.E2;
                r.E3 += m.E * n.E3;
                r.E23 += m.E * n.E23;
                r.E31 += m.E * n.E31;
                r.E12 += m.E * n.E12;
                r.E123 += m.E * n.E123;
            }

            if (Math.Abs(m.E1) > 1e-8)
            {
                r.E += +m.E1 * n.E1;
                r.E1 += +m.E1 * n.E;
                r.E2 += +m.E1 * n.E12;
                r.E3 += -m.E1 * n.E31;
                r.E23 += +m.E1 * n.E123;
                r.E31 += -m.E1 * n.E3;
                r.E12 += +m.E1 * n.E2;
                r.E123 += +m.E1 * n.E23;
            }

            if (Math.Abs(m.E2) > 1e-8)
            {
                r.E += +m.E2 * n.E2;
                r.E1 += -m.E2 * n.E12;
                r.E2 += +m.E2 * n.E;
                r.E3 += +m.E2 * n.E23;
                r.E23 += +m.E2 * n.E3;
                r.E31 += +m.E2 * n.E123;
                r.E12 += -m.E2 * n.E1;
                r.E123 += +m.E2 * n.E31;
            }

            if (Math.Abs(m.E3) > 1e-8)
            {
                r.E += +m.E3 * n.E3;
                r.E1 += +m.E3 * n.E31;
                r.E2 += -m.E3 * n.E23;
                r.E3 += +m.E3 * n.E;
                r.E23 += -m.E3 * n.E2;
                r.E31 += +m.E3 * n.E1;
                r.E12 += +m.E3 * n.E123;
                r.E123 += +m.E3 * n.E12;
            }

            if (Math.Abs(m.E23) > 1e-8)
            {
                r.E += -m.E23 * n.E23;
                r.E1 += -m.E23 * n.E123;
                r.E2 += +m.E23 * n.E3;
                r.E3 += -m.E23 * n.E2;
                r.E23 += +m.E23 * n.E;
                r.E31 += +m.E23 * n.E12;
                r.E12 += -m.E23 * n.E31;
                r.E123 += +m.E23 * n.E1;
            }

            if (Math.Abs(m.E31) > 1e-8)
            {
                r.E += -m.E31 * n.E31;
                r.E1 += -m.E31 * n.E3;
                r.E2 += -m.E31 * n.E123;
                r.E3 += +m.E31 * n.E1;
                r.E23 += -m.E31 * n.E12;
                r.E31 += +m.E31 * n.E;
                r.E12 += +m.E31 * n.E23;
                r.E123 += +m.E31 * n.E2;
            }

            if (Math.Abs(m.E12) > 1e-8)
            {
                r.E += -m.E12 * n.E12;
                r.E1 += +m.E12 * n.E2;
                r.E2 += -m.E12 * n.E1;
                r.E3 += -m.E12 * n.E123;
                r.E23 += +m.E12 * n.E31;
                r.E31 += -m.E12 * n.E23;
                r.E12 += +m.E12 * n.E;
                r.E123 += +m.E12 * n.E3;
            }

            if (Math.Abs(m.E123) > 1e-8)
            {
                r.E += -m.E123 * n.E123;
                r.E1 += -m.E123 * n.E23;
                r.E2 += -m.E123 * n.E31;
                r.E3 += -m.E123 * n.E12;
                r.E23 += +m.E123 * n.E1;
                r.E31 += +m.E123 * n.E2;
                r.E12 += +m.E123 * n.E3;
                r.E123 += +m.E123 * n.E;
            }

            return r;
        }

        public double Norm2 => E * E + E1 * E1 + E2 * E2 + E3 * E3 + E23 * E23 + E31 * E31 + E12 * E12 + E123 * E123;
        public double Norm => Math.Sqrt(Norm2);

        public MVec3d Inv
        {
            get
            {
                var n2 = Norm2;
                return new MVec3d(E / n2, -E1 / n2, -E2 / n2, -E3 / n2, -E23 / n2, -E31 / n2, -E12 / n2, E123 / n2);
//                return new MVec3d(E / n2, E1 / n2, E2 / n2, E3 / n2, E23 / n2, E31 / n2, E12 / n2, E123 / n2);
            }
        }

        public MVec3d Grade(int k)
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
                    return new MVec3d();
            }
        }

        public static MVec3d Inner(MVec3d m, MVec3d n)
        {
            var m0 = m.Grade(0);
            var m1 = m.Grade(1);
            var m2 = m.Grade(2);
            var m3 = m.Grade(3);

            var n0 = n.Grade(0);
            var n1 = n.Grade(1);
            var n2 = n.Grade(2);
            var n3 = n.Grade(3);

            return new MVec3d(
                m0 * n0 + m1 * n1 + m2 * n2 + m3 * n3,
                m0 * n1 + m1 * n2 + m2 * n3,
                m0 * n2 + m1 * n3,
                m0 * n3
            );
        }

        public static MVec3d Outer(MVec3d m, MVec3d n)
        {
            var m0 = m.Grade(0);
            var m1 = m.Grade(1);
            var m2 = m.Grade(2);
            var m3 = m.Grade(3);

            var n0 = n.Grade(0);
            var n1 = n.Grade(1);
            var n2 = n.Grade(2);
            var n3 = n.Grade(3);

            return new MVec3d(
                m0 * n0,
                m1 * n0 + m0 * n1,
                m2 * n0 + m1 * n1 + m0 * n2,
                m3 * n0 + m2 * n1 + m1 * n2 + m0 * m3
            );
        }

        public static MVec3d operator ~(MVec3d m) => m.Inv;
        public static MVec3d operator *(MVec3d m, MVec3d n) => Mult(m, n);

        public static MVec3d operator +(MVec3d m, MVec3d n) => Add(m, n);
        public static MVec3d operator -(MVec3d m, MVec3d n) => Sub(m, n);

        public static MVec3d operator &(MVec3d m, MVec3d n) => Inner(m, n);
        public static MVec3d operator ^(MVec3d m, MVec3d n) => Outer(m, n);

        public MVec3d ScalarPart => Scalar(E);
        public MVec3d VectorPart => Vector(E1, E2, E3);
        public MVec3d BivectorPart => Bivector(E23, E31, E12);
        public MVec3d TrivectorPart => Trivector(E123);

        public static implicit operator MVec3d(Vector3 v) => new MVec3d(0, v.X, v.Y, v.Z, 0, 0, 0, 0);
        public static implicit operator MVec3d(Vector3d v) => new MVec3d(0, v.X, v.Y, v.Z, 0, 0, 0, 0);
        public static implicit operator MVec3d(double s) => new MVec3d(s, 0, 0, 0, 0, 0, 0, 0);

        public static explicit operator double(MVec3d m) => m.E;
        public static explicit operator Vector3d(MVec3d m) => new Vector3d(m.E1, m.E2, m.E3);

        public override string ToString()
        {
            return $"({E:0.00} {E1:0.00}e1 {E2:0.00}e2 {E3:0.00}e3 {E23:0.00}e23 {E31:0.00}e31 {E12:0.00}e12 {E123:0.00}e123)";
        }
    }
}