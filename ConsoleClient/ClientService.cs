using System;
using System.Diagnostics;
using RayTracer;
using System.Drawing;

namespace ConsoleClient
{
    public class ClientService: IClientService
    {
        public Tuple<double, int> Perfomance(Scene scene, Rectangle viewport, RayTracer.RayTracer raytracer, int step, int number)
        {
            Stopwatch time = new Stopwatch();
            time.Start();
            RayTraceRows(scene, viewport, 0, step, raytracer, 0);
            time.Stop();
            
            return Tuple.Create<double, int>(time.ElapsedMilliseconds, number);
        }
        public bool CheckConnection()//нужно для проверки соединения между клиентом и сервером
        {
            return true;
        }
        public Tuple<Bitmap, int, RayTracer.RayTracer, int, DateTime> RayTraceRows(Scene scene, Rectangle viewport, int startRow, int numberOfRowsToTrace, RayTracer.RayTracer raytracer, int numberOfRange)
        {
            Console.SetCursorPosition(10, 2);
            Console.Write("       ");
            int maxsamples = (int) raytracer.AntiAliasing;
            RayTracer.Color [ , ] buffer = new RayTracer.Color [ viewport.Width + 2, numberOfRowsToTrace + 2 ];
            DateTime timestart = DateTime.Now;
            Bitmap image = new Bitmap(viewport.Width, numberOfRowsToTrace);
            int y = 0;
            for ( y = startRow; y < (startRow + numberOfRowsToTrace) + 2; y++ )
            {
                for ( int x = 0; x < viewport.Width + 2; x++ )
                {
                    double yp = y * 1.0f / viewport.Height * 2 - 1;
                    double xp = x * 1.0f / viewport.Width * 2 - 1;

                    Ray ray = scene.Camera.GetRay(xp, yp);

                    // this will trigger the raytracing algorithm
                    buffer [ x, y - startRow ] = raytracer.CalculateColor(ray, scene);

                    // if current line is at least 2 lines into the scan
                    if ( (x > 1) && (y > startRow + 1) )
                    {
                        if ( raytracer.AntiAliasing != AntiAliasing.None )
                        {
                            RayTracer.Color avg = (buffer [ x - 2, y - startRow - 2 ] + buffer [ x - 1, y - startRow - 2 ] + buffer [ x, y - startRow - 2 ] +
                                         buffer [ x - 2, y - startRow - 1 ] + buffer [ x - 1, y - startRow - 1 ] + buffer [ x, y - startRow - 1 ] +
                                         buffer [ x - 2, y - startRow ] + buffer [ x - 1, y - startRow ] + buffer [ x, y - startRow ]) / 9;

                            if ( raytracer.AntiAliasing == AntiAliasing.Quick )
                            {
                                // this basically implements a simple mean filter
                                // it quick but not very accurate
                                buffer [ x - 1, y - startRow - 1 ] = avg;
                            }
                            else
                            {   // use a more accurate antialasing method (MonteCarlo implementation)
                                // this will fire multiple rays per pixel
                                if ( avg.Distance(buffer [ x - 1, y - startRow - 1 ]) > 0.18 ) // 0.18 is a treshold for detailed aliasing
                                {
                                    for ( int i = 0; i < maxsamples; i++ )
                                    {
                                        // get some 'random' samples
                                        double rx = Math.Sign(i % 4 - 1.5) * (raytracer.IntNoise(x + y * viewport.Width * maxsamples * 2 + i) + 1) / 4; // interval <-0.5, 0.5>
                                        double ry = Math.Sign(i % 2 - 0.5) * (raytracer.IntNoise(x + y * viewport.Width * maxsamples * 2 + 1 + i) + 1) / 4; // interval <-0.5, 0.5>

                                        xp = (x - 1 + rx) * 1.0f / viewport.Width * 2 - 1;
                                        yp = (y - 1 + ry) * 1.0f / viewport.Height * 2 - 1;

                                        ray = scene.Camera.GetRay(xp, yp);
                                        // execute even more ray traces, this makes detailed anti-aliasing expensive
                                        buffer [ x - 1, y - startRow - 1 ] += raytracer.CalculateColor(ray, scene);
                                    }
                                    buffer [ x - 1, y - startRow - 1 ] /= (maxsamples + 1);
                                }
                            }
                        }

                        image.SetPixel(x - 2, y - startRow - 2, buffer [ x - 1, y - startRow - 1 ].ToArgb());
                    }
                }
                //if ( ((y - startRow) / numberOfRowsToTrace * 100) % 10 == 0 )
                //{
                //    Console.SetCursorPosition(10, 3);
                //    Console.Write(((y-startRow) / numberOfRowsToTrace * 100) + "%  ");
                //}
            }
            Console.SetCursorPosition(10, 3);
            Console.Write(100 + "%  ");
            return Tuple.Create<Bitmap, int, RayTracer.RayTracer, int, DateTime>(image, numberOfRange, raytracer, startRow, timestart);
        }
    }
}
