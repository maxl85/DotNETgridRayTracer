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
    /// this is a data class that stores all relevant information about a ray-shape intersection
    /// this information is to be filled in by every custom implemented shape type in the Intersect method.
    /// this information is used to determine the color at the intersection point
    /// </summary>
    [Serializable]
    public class IntersectInfo
    {
        public bool IsHit; // indicates if the shape was hit
        public int HitCount; // counts the number of shapes that were hit
        public IShape Element; // the closest shape that was intersected
        public Vector Position; // position of intersection
        public Vector Normal; // normal vector on intesection point 
        public Color Color; // color at intersection
        public double Distance; // distance from point to screen


        public IntersectInfo()
        {
        }
    }

    /// <summary>
    /// a virtual ray that is casted from a begin Position into a certain Direction.
    /// </summary>
    [Serializable]
    public class Ray
    {
        public Vector Position;
        public Vector Direction;

        public Ray(Vector position, Vector direction)
        {
            Position = position;
            Direction = direction;
        }
    }
}
