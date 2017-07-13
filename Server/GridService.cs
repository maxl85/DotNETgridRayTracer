using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Windows;

namespace Server
{
    public class GridService: IGridService
    {
        //static List<Guid> listAgentID = new List<Guid>();

        public static MainWindow mw;

        public void Connect()
        {
            OperationContext context = OperationContext.Current;
            MessageProperties messageProperties = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpointProperty =
                messageProperties [ RemoteEndpointMessageProperty.Name ]
                as RemoteEndpointMessageProperty;

            Clients.AddClient(endpointProperty.Address, endpointProperty.Port);
        }

        public void Disconnect()
        {
            OperationContext context = OperationContext.Current;
            MessageProperties messageProperties = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpointProperty =
                messageProperties [ RemoteEndpointMessageProperty.Name ]
                as RemoteEndpointMessageProperty;

            string s;
            if ( endpointProperty.Address == "::1" ) s = "localhost";
            else s = endpointProperty.Address;
            
            Clients.RemoveClient(s);
            //if ( MainWindow.RangeList != null )//при отсоединении клиента не удаляю его из списка клиентов, 
            //    for ( int i = 0; i < Clients.ListClients.Count; i++ )//а просто перевожу его в неактивное состояние
            //    {
            //        if ( Clients.ListClients [ i ].IP == s )
            //        {
            //            Clients.ListClients [ i ].Done = "Not ready";
            //            MainWindow.RangeList [ Clients.ListClients [ i ].Task ].state = RangeOfCalculation.CurrentState.notComputing;
            //        }
            //    }
        }
    }
}

