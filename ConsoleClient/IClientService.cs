using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using RayTracer;
using System.Drawing;
using System.IO;
namespace ConsoleClient
{
    [ServiceContract]
    public interface IClientService
    {
        [OperationContract]
        Tuple<double, int> Perfomance(Scene scene, Rectangle viewport, RayTracer.RayTracer raytracer, int step, int number);
        [OperationContract]
        bool CheckConnection();        
        [OperationContract]
        Tuple<Bitmap, int, RayTracer.RayTracer, int, DateTime> RayTraceRows(Scene scene, Rectangle viewport, int startRow, int numberOfRowsToTrace, RayTracer.RayTracer raytracer, int numberOfRange);
    }
}
