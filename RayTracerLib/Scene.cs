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
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace RayTracer
{
    /// <summary>
    /// a scene is defined by:
    /// - lights
    /// - a camera, of viewpoint from which the scene is observed
    /// - a background
    /// - the objects in the scene, called the shapes.
    /// </summary>
    
    [Serializable]
    public class Scene
    {
        public Background Background;
        public Camera Camera;
        public Shapes Shapes;
        public Lights Lights;
        

        public Scene()
        {
            Camera = new Camera(new Vector(0,0,-5), new Vector(0,0,1));
            Shapes = new Shapes();
            Lights = new Lights();
            Background = new Background(new Color(0, 0, .5), 0.2);
        }
    }
    
}
