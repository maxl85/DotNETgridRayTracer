using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Client
{
    public class ClientService : IClientService
    {
        public double DoWork(ulong start, ulong stop, double step)
        {
            double sum = 0.0;
            double x;
            for (ulong i = start; i < stop; i++)
            {
                x = (i + 0.5) * step;
                sum = sum + 4.0 / (1.0 + x * x);
            }
            return step * sum;
            
        }
    }
}
