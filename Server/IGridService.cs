﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Server
{
    [ServiceContract]
    public interface IGridService
    {
        [OperationContract]
        void Connect();

        [OperationContract]
        void Disconnect();     
    }
}
