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
    [Serializable]
    public class Lights : List<Light>
    {
    }

    /// <summary>
    /// a point light
    /// </summary>
    [Serializable]
    public class Light
    {
        public Vector Position;
        public Color Color;
        public double strength;

        public Light(Vector pos, Color color)
        {
            Position = pos;
            Color = color;

            strength = 10;
        }

        public double Strength(double distance)
        {
            if (distance >= strength) return 0;

            return Math.Pow((strength - distance) / strength, .2);
        }
        public override string ToString()
        {
            return string.Format("Light ({0},{1},{2})", Position.x, Position.y, Position.z);
        }
    }
}
