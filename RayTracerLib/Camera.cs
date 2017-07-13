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
    public class Camera
    {
        public Vector Position;
        public Vector LookAt;
        public Vector Equator;
        public Vector Up; // defines the tilt of the camera
        public Vector Screen; // defines the center position of the viewport/screen in 3D space

        public Camera(Vector position, Vector lookat) : this(position, lookat, new Vector(0,1,0))
        {
        }

        public Camera(Vector position, Vector lookat, Vector up)
        {
            Up = up.Normalize();
            Position = position;
            LookAt = lookat;
            Equator = LookAt.Normalize().Cross(Up);
            Screen = Position + LookAt;
        }

        /// <summary>
        /// returns the ray as it passes through the viewport form the camera perspective
        /// it assumes that the viewport is scaled down to (1,1)-(-1,-1)
        /// </summary>
        /// <param name="vx">x position on the viewport must be between [-1,1]</param>
        /// <param name="vy">y position on the viewport must be between [-1,1]</param>
        /// <returns></returns>
        public Ray GetRay(double vx, double vy)
        {
            Vector pos = Screen - Up * vy - Equator * vx;
            Vector dir = pos - Position;

            Ray ray = new Ray(pos, dir.Normalize());
            return ray;
        }

    }
}
