using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Client.GridServiceReference;
using System.ServiceModel;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        static Guid agentID = Guid.NewGuid();
        GridServiceClient proxy = new GridServiceClient();

        /*
        //Specify the binding to be used for the client.
        BasicHttpBinding binding = new BasicHttpBinding();

        //Specify the address to be used for the client.
        EndpointAddress address =
           new EndpointAddress("http://localhost/servicemodelsamples/service.svc");

        // Create a client that is configured with this address and binding.
        CalculatorClient client = new CalculatorClient(binding, address);
        */

        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            // Справка "Как асинхронно вызывать операции службы WCF"
            // http://msdn.microsoft.com/ru-ru/library/ms730059(v=vs.110).aspx
            proxy.AddCompleted += new EventHandler<AddCompletedEventArgs>(RegisterCallback);
            proxy.AddAsync(2, 2);


        }

        static void RegisterCallback(object sender, AddCompletedEventArgs e)
        {
            MessageBox.Show(e.Result.ToString());
        }

        bool cond_discon = false;
        private void butConnect_Click(object sender, RoutedEventArgs e)
        {
            if (cond_discon == false)
            {
                proxy.Connect(agentID);
                butConnect.Content = "Disconnect";
                cond_discon = true;
            }
            else
            {
                proxy.Disconnect(agentID);
                butConnect.Content = "Connect";
                cond_discon = false;
            }
            
            
            
        }

    }
}
