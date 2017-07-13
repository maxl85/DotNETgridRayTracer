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
using System.Drawing;
using System.Diagnostics;

namespace RayTracer
{
    public delegate void RenderUpdateDelegate(int progress, double duration, double ETA, int scanline);

    [Serializable]
    public enum AntiAliasing
    {
        None = 0,
        Quick = 1,
        Low = 4,
        Medium = 8,
        High = 16,
        VeryHigh = 32
    }

    //[Serializable]
    public class RayTracer
    {
        public bool RenderDiffuse;
        public bool RenderHighlights;
        public bool RenderShadow;
        public bool RenderReflection;
        public bool RenderRefraction;
        public AntiAliasing AntiAliasing;

        public event RenderUpdateDelegate RenderUpdate;

        public RayTracer() : this(AntiAliasing.Medium, true, true, true, true, true)
        {
        }

        public RayTracer(AntiAliasing antialiasing, bool renderDiffuse, bool renderHighlights, bool renderShadow, bool renderReflection, bool renderRefraction)
        {
            RenderDiffuse = renderDiffuse;
            RenderHighlights = renderHighlights;
            RenderShadow = renderShadow;
            RenderReflection = renderReflection;
            RenderRefraction = renderRefraction;
            AntiAliasing = antialiasing;
        }

        /// <summary>
        /// a helper function to generate a parameterized 'random' noise values
        /// it is used by the monte-carlo anti-aliasing algorithm
        /// </summary>
        /// <param name="x">the parameter to calculate a random value for</param>
        /// <returns>returns a value between [-1,1]</returns>
        public double IntNoise(int x)			 
        {
            x = (x<<13) ^ x;
            return ( 1.0 - ( (x * (x * x * 15731 + 789221) + 1376312589) & 0x7fffffff) / (int.MaxValue / 2.0));
        }

        #region Ray Trace Scene

        /// <summary>
        /// this is the main entrypoint for rendering a scene. this method is responsible for correctly rendering
        /// the graphics device (in this case a bitmap).
        /// Note that apart from the raytracing, painting on a graphics device is rather slow
        /// </summary>
        /// <param name="g">the graphics to render on</param>
        /// <param name="viewport">basically determines the size of the bitmap to render on</param>
        /// <param name="scene">the scene to render.</param>
        public void RayTraceScene(Graphics g, Rectangle viewport, Scene scene)
        {
            int maxsamples = (int)AntiAliasing;
            

            g.FillRectangle(Brushes.Black, viewport);

            //Color[] scanline1;
            //Color[] scanline2 = null;
            //Color[] scanline3 = null;

            Color[,] buffer = new Color[viewport.Width+2, viewport.Height+2];

            for (int y = 0; y < viewport.Height + 2; y++)
            {DateTime timestart = DateTime.Now;
                //// used for anti-aliasing
                //scanline1 = scanline2;
                //scanline2 = scanline3;
                //scanline3 = new Color[viewport.Width + 2];

                for (int x = 0; x < viewport.Width + 2; x++)
                {
                    double yp = y * 1.0f / viewport.Height * 2 - 1;
                    double xp = x * 1.0f / viewport.Width * 2 - 1;

                    Ray ray = scene.Camera.GetRay(xp, yp);

                    // this will trigger the raytracing algorithm
                    buffer[x, y] = CalculateColor(ray, scene);

                    if ((x > 1) && (y > 1))
                    {
                        if (AntiAliasing != AntiAliasing.None)
                        {
                            Color avg = (buffer[x - 2, y - 2] + buffer[x - 1, y - 2] + buffer[x, y - 2] +
                                         buffer[x - 2, y - 1] + buffer[x - 1, y - 1] + buffer[x, y - 1] +
                                         buffer[x - 2, y] + buffer[x - 1, y] + buffer[x, y]) / 9;

                            if (AntiAliasing == AntiAliasing.Quick)
                            {   
                                // this basically implements a simple mean filter
                                // it quick but not very accurate
                                buffer[x - 1, y - 1] = avg; 
                            }
                            else
                            {   // use a more accurate antialasing method (MonteCarlo implementation)
                                // this will fire multiple rays per pixel
                                if (avg.Distance(buffer[x - 1, y - 1]) > 0.18) // 0.18 is a treshold for detailed aliasing
                                {
                                    for (int i = 0; i < maxsamples; i++)
                                    {
                                        // get some 'random' samples
                                        double rx = Math.Sign(i % 4 - 1.5) * (IntNoise(x + y * viewport.Width * maxsamples * 2 + i) + 1) / 4; // interval <-0.5, 0.5>
                                        double ry = Math.Sign(i % 2 - 0.5) * (IntNoise(x + y * viewport.Width * maxsamples * 2 + 1 + i) + 1) / 4; // interval <-0.5, 0.5>

                                        xp = (x - 1 + rx) * 1.0f / viewport.Width * 2 - 1;
                                        yp = (y - 1 + ry) * 1.0f / viewport.Height * 2 - 1;

                                        ray = scene.Camera.GetRay(xp, yp);
                                        // execute even more ray traces, this makes detailed anti-aliasing expensive
                                        buffer[x - 1, y - 1] += CalculateColor(ray, scene);
                                    }
                                    buffer[x - 1, y - 1] /= (maxsamples + 1);
                                }
                            }
                        }

                        // this is the slow part of the painting algorithm, it can be greatly speed up
                        // by directly accessing the bitmap data
                        Brush br = new SolidBrush(buffer[x - 1, y - 1].ToArgb());
                        g.FillRectangle(br, viewport.Left + x - 2, viewport.Top + y - 2, 1, 1);
                        br.Dispose();
                    }
                }

                // update progress after every scanline
                if ( RenderUpdate != null )
                {
                    double progress = (y) / (double) (viewport.Height);
                    double duration = DateTime.Now.Subtract(timestart).TotalMilliseconds;

                    double ETA = duration / progress - duration;
                    RenderUpdate.Invoke((int) progress * 100, duration, ETA, y - 1);
                }
            }
        }
        public void RenderUp(int y , System.Windows.Forms.PictureBox viewport, DateTime  timestart)
        {
            if ( RenderUpdate != null )
            {
                double progress = (y) / (double) (viewport.Height);
                double duration = DateTime.Now.Subtract(timestart).TotalMilliseconds;

                double ETA = duration / progress - duration;
                RenderUpdate.Invoke((int) progress * 100, duration, ETA, y - 1);
            }
        }
        /// <summary>
        /// this is the main entrypoint for rendering a scene. this method is responsible for correctly rendering
        /// the graphics device (in this case a bitmap).
        /// Note that apart from the raytracing, painting on a graphics device is rather slow
        /// </summary>
        /// <param name="g">the graphics to render on</param>
        /// <param name="viewport">basically determines the size of the bitmap to render on</param>
        /// <param name="scene">the scene to render.</param>
        public void ModifiedRayTraceScene(Graphics g, Rectangle viewport, Scene scene)
        {
            int maxsamples = (int)AntiAliasing;
            DateTime timestart = DateTime.Now;

            g.FillRectangle(Brushes.Black, viewport);

            Color[,] buffer = new Color[viewport.Width + 2, viewport.Height + 2];

            for (int y = 299; y < viewport.Height + 2; y++)
            {
                for (int x = 0; x < viewport.Width + 2; x++)
                {
                    double yp = y * 1.0f / viewport.Height * 2 - 1;
                    double xp = x * 1.0f / viewport.Width * 2 - 1;

                    Ray ray = scene.Camera.GetRay(xp, yp);

                    // this will trigger the raytracing algorithm
                    buffer[x, y] = CalculateColor(ray, scene);

                    // if current line is at least 2 lines into the scan
                    if ((x > 1) && (y > 300))
                    {
                        #region Anti Aliasing

                        //if (AntiAliasing != AntiAliasing.None)
                        //{
                        //    Color avg = (buffer[x - 2, y - 2] + buffer[x - 1, y - 2] + buffer[x, y - 2] +
                        //                 buffer[x - 2, y - 1] + buffer[x - 1, y - 1] + buffer[x, y - 1] +
                        //                 buffer[x - 2, y] + buffer[x - 1, y] + buffer[x, y]) / 9;

                        //    if (AntiAliasing == AntiAliasing.Quick)
                        //    {
                        //        // this basically implements a simple mean filter
                        //        // it quick but not very accurate
                        //        buffer[x - 1, y - 1] = avg;
                        //    }
                        //    else
                        //    {   // use a more accurate antialasing method (MonteCarlo implementation)
                        //        // this will fire multiple rays per pixel
                        //        if (avg.Distance(buffer[x - 1, y - 1]) > 0.18) // 0.18 is a treshold for detailed aliasing
                        //        {
                        //            for (int i = 0; i < maxsamples; i++)
                        //            {
                        //                // get some 'random' samples
                        //                double rx = Math.Sign(i % 4 - 1.5) * (IntNoise(x + y * viewport.Width * maxsamples * 2 + i) + 1) / 4; // interval <-0.5, 0.5>
                        //                double ry = Math.Sign(i % 2 - 0.5) * (IntNoise(x + y * viewport.Width * maxsamples * 2 + 1 + i) + 1) / 4; // interval <-0.5, 0.5>

                        //                xp = (x - 1 + rx) * 1.0f / viewport.Width * 2 - 1;
                        //                yp = (y - 1 + ry) * 1.0f / viewport.Height * 2 - 1;

                        //                ray = scene.Camera.GetRay(xp, yp);
                        //                // execute even more ray traces, this makes detailed anti-aliasing expensive
                        //                buffer[x - 1, y - 1] += CalculateColor(ray, scene);
                        //            }
                        //            buffer[x - 1, y - 1] /= (maxsamples + 1);
                        //        }
                        //    }
                        //}

                        #endregion

                        // this is the slow part of the painting algorithm, it can be greatly speed up
                        // by directly accessing the bitmap data
                        Brush br = new SolidBrush(buffer[x - 1, y - 1].ToArgb());
                        g.FillRectangle(br, viewport.Left + x - 2, viewport.Top + y - 2, 1, 1);
                        br.Dispose();
                    }
                }

                // update progress after every scanline
                if (RenderUpdate != null)
                {
                    double progress = (y) / (double)(viewport.Height);
                    double duration = DateTime.Now.Subtract(timestart).TotalMilliseconds;

                    double ETA = duration / progress - duration;
                    RenderUpdate.Invoke((int)progress * 100, duration, ETA, y - 1);
                }
            }
        }
        
        #endregion

        public Bitmap RayTraceRows(Scene scene, Rectangle viewport, int startRow, int numberOfRowsToTrace)
        {
            int maxsamples = (int)AntiAliasing;
            Color[,] buffer = new Color[viewport.Width + 2, numberOfRowsToTrace + 2];
            Bitmap image = new Bitmap(viewport.Width, numberOfRowsToTrace);

            for (int y = startRow; y < (startRow + numberOfRowsToTrace) + 2; y++)
            {
                for (int x = 0; x < viewport.Width + 2; x++)
                {
                    double yp = y * 1.0f / viewport.Height * 2 - 1;
                    double xp = x * 1.0f / viewport.Width * 2 - 1;

                    Ray ray = scene.Camera.GetRay(xp, yp);

                    // this will trigger the raytracing algorithm
                    buffer[x,y - startRow] = CalculateColor(ray, scene);

                    // if current line is at least 2 lines into the scan
                    if ((x > 1) && (y > startRow + 1))
                    {
                        if (AntiAliasing != AntiAliasing.None)
                        {
                            Color avg = (buffer[x - 2, y - startRow - 2] + buffer[x - 1, y - startRow - 2] + buffer[x, y - startRow - 2] +
                                         buffer[x - 2, y - startRow - 1] + buffer[x - 1, y - startRow - 1] + buffer[x, y - startRow - 1] +
                                         buffer[x - 2, y - startRow] + buffer[x - 1, y - startRow] + buffer[x, y - startRow]) / 9;

                            if (AntiAliasing == AntiAliasing.Quick)
                            {
                                // this basically implements a simple mean filter
                                // it quick but not very accurate
                                buffer[x - 1, y - startRow - 1] = avg;
                            }
                            else
                            {   // use a more accurate antialasing method (MonteCarlo implementation)
                                // this will fire multiple rays per pixel
                                if (avg.Distance(buffer[x - 1, y - startRow - 1]) > 0.18) // 0.18 is a treshold for detailed aliasing
                                {
                                    for (int i = 0; i < maxsamples; i++)
                                    {
                                        // get some 'random' samples
                                        double rx = Math.Sign(i % 4 - 1.5) * (IntNoise(x + y * viewport.Width * maxsamples * 2 + i) + 1) / 4; // interval <-0.5, 0.5>
                                        double ry = Math.Sign(i % 2 - 0.5) * (IntNoise(x + y * viewport.Width * maxsamples * 2 + 1 + i) + 1) / 4; // interval <-0.5, 0.5>

                                        xp = (x - 1 + rx) * 1.0f / viewport.Width * 2 - 1;
                                        yp = (y - 1 + ry) * 1.0f / viewport.Height * 2 - 1;

                                        ray = scene.Camera.GetRay(xp, yp);
                                        // execute even more ray traces, this makes detailed anti-aliasing expensive
                                        buffer[x - 1, y - startRow - 1] += CalculateColor(ray, scene);
                                    }
                                    buffer[x - 1, y - startRow - 1] /= (maxsamples + 1);
                                }
                            }
                        }

                        image.SetPixel(x - 2, y - startRow - 2, buffer[x - 1, y - startRow - 1].ToArgb());
                    }
                }
                System.Windows.Forms.Application.DoEvents();
            }

            return image;
        }
        
        /// <summary>
        /// this implementation is used for debugging purposes.
        /// the color is calculated following the normal raytrace procedure
        /// execpt it is calculated for 1 particula ray
        /// </summary>
        /// <param name="ray">the ray for which to calculate the color</param>
        /// <param name="scene">the scene which is raytraced</param>
        /// <returns></returns>
        public Color CalculateColor(Ray ray, Scene scene)
        {
            IntersectInfo info = TestIntersection(ray, scene, null);
            if (info.IsHit)
            {
                // execute the actual raytrace algorithm
                Color c = RayTrace(info, ray, scene, 0);
                return c;
            }

            return scene.Background.Color;
            
        }

        /// <summary>
        /// This is the main RayTrace controller algorithm, the core of the RayTracer
        /// recursive method setup
        /// this does the actual tracing of the ray and determines the color of each pixel
        /// supports:
        /// - ambient lighting
        /// - diffuse lighting
        /// - Gloss lighting
        /// - shadows
        /// - reflections
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ray"></param>
        /// <param name="scene"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        private Color RayTrace(IntersectInfo info, Ray ray, Scene scene, int depth)
        {
            // calculate ambient light
            Color color = info.Color * scene.Background.Ambience;
            double shininess = Math.Pow(10, info.Element.Material.Gloss+1);

            foreach (Light light in scene.Lights)
            {

                // calculate diffuse lighting
                Vector v = (light.Position - info.Position).Normalize();

                if (RenderDiffuse)
                {
                    double L = v.Dot(info.Normal);
                    if (L > 0.0f)
                        color += info.Color * light.Color * L;
                }

                
                // this is the max depth of raytracing.
                // increasing depth will calculate more accurate color, however it will
                // also take longer (exponentially)
                if (depth < 3)
                {
                    
                    // calculate reflection ray
                    if (RenderReflection && info.Element.Material.Reflection > 0)
                    {
                        Ray reflectionray = GetReflectionRay(info.Position, info.Normal, ray.Direction);
                        IntersectInfo refl = TestIntersection(reflectionray, scene, info.Element);
                        if (refl.IsHit && refl.Distance > 0)
                        {
                            // recursive call, this makes reflections expensive
                            refl.Color = RayTrace(refl, reflectionray, scene, depth + 1);
                        }
                        else // does not reflect an object, then reflect background color
                            refl.Color = scene.Background.Color;
                        color = color.Blend(refl.Color, info.Element.Material.Reflection);
                    }

                    //calculate refraction ray
                    if (RenderRefraction && info.Element.Material.Transparency > 0)
                    {
                        Ray refractionray = GetRefractionRay(info.Position, info.Normal, ray.Direction, info.Element.Material.Refraction);
                        IntersectInfo refr = info.Element.Intersect(refractionray);
                        if (refr.IsHit)
                        {
                            //refractionray = new Ray(refr.Position, ray.Direction);
                            refractionray = GetRefractionRay(refr.Position, refr.Normal, refractionray.Direction, refr.Element.Material.Refraction);
                            refr = TestIntersection(refractionray, scene, info.Element);
                            if (refr.IsHit && refr.Distance > 0)
                            {
                                // recursive call, this makes refractions expensive
                                refr.Color = RayTrace(refr, refractionray, scene, depth + 1);
                            }
                            else
                                refr.Color = scene.Background.Color;
                        }
                        else
                            refr.Color = scene.Background.Color;
                        color = color.Blend(refr.Color, info.Element.Material.Transparency);
                    }
                }


                IntersectInfo shadow = new IntersectInfo();
                if (RenderShadow)
                {
                    // calculate shadow, create ray from intersection point to light
                    Ray shadowray = new Ray(info.Position, v);

                    // find any element in between intersection point and light
                    shadow = TestIntersection(shadowray, scene, info.Element);
                    if (shadow.IsHit && shadow.Element != info.Element)
                    {
                        // only cast shadow if the found interesection is another
                        // element than the current element
                        color *= 0.5 + 0.5 * Math.Pow(shadow.Element.Material.Transparency, 0.5); // Math.Pow(.5, shadow.HitCount);
                    }
                }

                // only show highlights if it is not in the shadow of another object
                if (RenderHighlights && !shadow.IsHit && info.Element.Material.Gloss > 0)
                {
                    // only show Gloss light if it is not in a shadow of another element.
                    // calculate Gloss lighting (Phong)
                    Vector Lv = (info.Element.Position - light.Position).Normalize();
                    Vector E = (scene.Camera.Position - info.Element.Position).Normalize();
                    Vector H = (E - Lv).Normalize();

                    double Glossweight = 0.0;
                    Glossweight = Math.Pow(Math.Max(info.Normal.Dot(H), 0), shininess);
                    color += light.Color * (Glossweight);
                }
            }
           
            // normalize the color
            color.Limit();
            return color;
        }

        /// <summary>
        /// this method tests for an intersection. It will try to find the closest
        /// object that intersects with the ray. 
        /// it will inspect every object in the scene. also here there is room for increased performance.
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="scene"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        private IntersectInfo TestIntersection(Ray ray, Scene scene, IShape exclude)
        {
            int hitcount = 0;
            IntersectInfo best = new IntersectInfo();
            best.Distance = double.MaxValue;

            foreach (IShape elt in scene.Shapes)
            {
                if (elt == exclude) 
                    continue;

                IntersectInfo info = elt.Intersect(ray);
                if (info.IsHit && info.Distance < best.Distance && info.Distance >= 0)
                {
                    best = info;
                    hitcount++;
                }
            }
            best.HitCount = hitcount;
            return best;
        }

        /// <summary>
        /// some helper functions to calculate the reflection rays
        /// </summary>
        /// <param name="P"></param>
        /// <param name="N"></param>
        /// <param name="V"></param>
        /// <returns></returns>
        private Ray GetReflectionRay(Vector P, Vector N, Vector V)
        {
            double c1 = -N.Dot(V);
            Vector Rl = V + (N * 2 * c1);
            return new Ray(P, Rl);
        }
    
        /// <summary>
        /// some helper functions to calculate the refraction rays
        /// </summary>
        /// <param name="P"></param>
        /// <param name="N"></param>
        /// <param name="V"></param>
        /// <param name="refraction"></param>
        /// <returns></returns>
        private Ray GetRefractionRay(Vector P, Vector N, Vector V, double refraction)
        {
            //V = V * -1;
            //double n = -0.55; // refraction constant for now
            //if (n < 0 || n > 1) return new Ray(P, V); // no refraction

            double c1 = N.Dot(V);
            double c2 = 1 - refraction * refraction * (1 - c1 * c1);
            if (c2 < 0) 
                

            c2 = Math.Sqrt(c2);
            Vector T = (N * (refraction * c1 - c2) - V * refraction) * -1;
            T.Normalize();

            return new Ray(P, T); // no refraction
        }
    }
}
