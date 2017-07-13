// Copyright 2006 Herre Kuijpers - <herre@xs4all.nl>
//
// This source file(s) may be redistributed, altered and customized
// by any means PROVIDING the authors name and all copyright
// notices remain intact.
// THIS SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED. USE IT AT YOUR OWN RISK. THE AUTHOR ACCEPTS NO
// LIABILITY FOR ANY DATA DAMAGE/LOSS THAT THIS PRODUCT MAY CAUSE.
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace RayTracer
{
    /// <summary>
    /// a 3D vector class, used to calculate some basic trigonomitry
    /// </summary>
    [Serializable]
    public class Vector
    {
        public double x;
        public double y;
        public double z;

        static public Vector Null;
        static public Vector Infinate;

        static Vector()
        {
            Null = new Vector(0, 0, 0);
            Infinate = new Vector(double.MaxValue, double.MaxValue, double.MaxValue);
        }

        public Vector(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// copy constructor
        /// </summary>
        /// <param name="v"></param>
        public Vector(Vector v) : this(v.x, v.y, v.z)
        {
        }

        public Vector Normalize()
        {
            double t = (double) this.Magnitude();
            return new Vector(x / t, y / t, z / t);
        }

        static public Vector operator +(Vector v, Vector w)
        {
            return new Vector(w.x + v.x, w.y + v.y, w.z + v.z);
        }

        static public Vector operator -(Vector v, Vector w)
        {
            return new Vector(v.x - w.x, v.y - w.y, v.z - w.z);
        }

        static public Vector operator *(Vector v, Vector w)
        {
            return new Vector(v.x * w.x, v.y * w.y, v.z * w.z);
        }

        static public Vector operator *(Vector v, double f)
        {
            return new Vector(v.x * f, v.y * f, v.z * f);
        }

        static public Vector operator /(Vector v, double f)
        {
            return new Vector(v.x / f, v.y / f, v.z / f);
        }

        public double Dot(Vector w)
        {
            return this.x*w.x + this.y*w.y + this.z * w.z;
        }

        public Vector Cross(Vector w)
        {
            return new Vector(-this.z * w.y + this.y * w.z,
                               this.z * w.x - this.x * w.z, 
                              -this.y * w.x + this.x * w.y);
        }

        public double Magnitude()
        {
            return Math.Sqrt((double)((x * x) + (y * y) + (z * z)));
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", this.x, this.y, this.z);
        }
    }
}
