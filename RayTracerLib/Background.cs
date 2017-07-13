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
    /// defines the background of the scene
    /// for now it only supports a background color and an ambience value, for ambient lighting
    /// </summary>
    [Serializable]
    public class Background
    {

        public Color Color;
        public double Ambience;

        /// <summary>
        /// specifies the background of the scene
        /// </summary>
        /// <param name="color">the color of the background</param>
        /// <param name="ambience">the ambient lighting used [0,1]</param>
        public Background(Color color, double ambience)
        {
            Color = color;
            Ambience = ambience;
        }

    }
}
