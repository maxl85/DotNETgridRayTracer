using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Client
{
    [ServiceContract]
    public interface IClientService
    {
        [OperationContract]
        double DoWork(ulong start, ulong stop, double step);
    }
}
