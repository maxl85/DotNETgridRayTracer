using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using ConsoleClient.GridServiceReference;
using System.Runtime.InteropServices;

namespace ConsoleClient
{
    internal delegate void SignalHandler(ConsoleSignal consoleSignal);

        internal enum ConsoleSignal
        {
            CtrlC = 0,
            CtrlBreak = 1,
            Close = 2,
            LogOff = 5,
            Shutdown = 6
        }

        internal static class ConsoleHelper
        {
            [DllImport("Kernel32", EntryPoint = "SetConsoleCtrlHandler")]
            public static extern bool SetSignalHandler(SignalHandler handler, bool add);
        }
    class Program
    {
        private static GridServiceClient proxy = new GridServiceClient();
        private static SignalHandler signalHandler;
        private static void HandleConsoleSignal(ConsoleSignal consoleSignal)
        {
            proxy.Disconnect();
        }
        static void Main(string[] args)
        {

            // Запуск WCF-сервиса на клиенте
            signalHandler += HandleConsoleSignal;
            ConsoleHelper.SetSignalHandler(signalHandler, true);
            ServiceHost serviceHost;
            serviceHost = new ServiceHost(typeof(ClientService));
            try
            {
                serviceHost.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка запуска WCF-сервиса: {0}", ex.Message);
                Console.ReadKey();
            }
            
            // Подключаемся к серверу
            
            try
            {
                proxy.Connect();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка подключения к серверу: {0}", ex.Message);
                Console.ReadKey();
            }
            
            Console.WriteLine("Calculating PI on CPU single-threaded ...\n\n");
            Console.Write("Complite: ");
            Console.WriteLine("\n\n");
            Console.WriteLine("Press <ENTER> to stop ...");

            Console.ReadKey();
            proxy.Disconnect();
            serviceHost.Close();
        }
    }
}
