using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Clients
    {
        public static MainWindow mw;

        public static List<DataRow> ListClients = new List<DataRow>();

        public static void AddClient(string ip, int port)
        {
            string _ip;
            if (ip == "::1") _ip = "localhost";
            else _ip = ip;

            DataRow client = new DataRow { Num = ListClients.Count + 1, IP = _ip, Port = port, Done = "" ,Perfomance = ""};
            ListClients.Add(client);
            //mw.dataGrid1.Items.Add(client);
            mw.tbNumClients.Text = ListClients.Count.ToString();
            UpdateTable();
        }

        public static void RemoveClient(string ip)
        {
            int i;
            for (i = 0; i < ListClients.Count; i++)
            {
                if (ListClients[i].IP == ip)
                {
                    ListClients.RemoveAt(i);
                    break;
                }
            }

            //mw.dataGrid1.Items.RemoveAt(i);
            mw.tbNumClients.Text = ListClients.Count.ToString();
            UpdateTable();
        }

        public static void UpdateTable()
        {
            mw.dataGrid1.Items.Clear();
            for ( int i = 0; i < ListClients.Count; i++ )
            {
                mw.dataGrid1.Items.Add(ListClients [ i ]);
            }
        }
        
    }

    class DataRow
    {
        public int Num { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
        public string Done { get; set; }
        public string Perfomance { get; set; }
        public int Task { get; set; }
    }
}
