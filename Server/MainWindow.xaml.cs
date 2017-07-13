using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.ServiceModel;
using Server.ClientServiceReference;
using System.Diagnostics;
using System.Threading;
using RayTracer;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Server
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow: Window
    {
        // Глобальные переменные
        Stopwatch time = new Stopwatch();
        ServiceHost serviceHost;
        ClientServiceClient [ ] clients;
        BasicHttpBinding binding = new BasicHttpBinding();
        public static List<RangeOfCalculation> RangeList;
        public Scene scene = new Scene();
        public AntiAliasing antiAliasing = AntiAliasing.Medium;
        public Bitmap localBitmap;
        public bool isLocalTracing = false;
        Rectangle rect = new Rectangle(0, 0, 400, 400);
        Graphics gr;
        List<ResultRenderRows> Result;
        
        public bool renderDiffuse = true;
        public bool renderHighlights = true;
        public bool renderShadow = true;
        public bool renderReflection = true;
        public bool renderRefraction = true;
        

        Thread thr;
        Thread thr1;
        int countRes = 0;
        int countRange;//количество диапазонов
        double maxPerfCount = 0.00;//максимальное время ожидания
        int Steps;

        public MainWindow()
        {
            InitializeComponent();
            Clients.mw = this;
            DataGridTextColumn col1 = new DataGridTextColumn();
            DataGridTextColumn col2 = new DataGridTextColumn();
            DataGridTextColumn col3 = new DataGridTextColumn();
            DataGridTextColumn col4 = new DataGridTextColumn();
            DataGridTextColumn col5 = new DataGridTextColumn();
            dataGrid1.Columns.Add(col1);
            dataGrid1.Columns.Add(col2);
            dataGrid1.Columns.Add(col3);
            dataGrid1.Columns.Add(col4);
            dataGrid1.Columns.Add(col5);
            dataGrid1.Columns [ 0 ].Width = 40;
            dataGrid1.Columns [ 1 ].Width = 200;
            dataGrid1.Columns [ 2 ].Width = 80;
            dataGrid1.Columns [ 3 ].Width = 70;
            dataGrid1.Columns [ 4 ].Width = 80;
            col1.Binding = new System.Windows.Data.Binding("Num");
            col2.Binding = new System.Windows.Data.Binding("IP");
            col3.Binding = new System.Windows.Data.Binding("Port");
            col4.Binding = new System.Windows.Data.Binding("Done");
            col5.Binding = new System.Windows.Data.Binding("Perfomance");
            col1.Header = "№";
            col2.Header = "IP";
            col3.Header = "Port";
            col4.Header = "Done";
            col5.Header = "Perfomance";

            localBitmap = new Bitmap(400, 400);
            pbSceneLocal.Image = localBitmap;
            pbSceneLocal.SizeMode = PictureBoxSizeMode.StretchImage;
            pbSceneLocal.Height = 400;
            pbSceneLocal.Width=400;

            //binding.MaxBufferSize = int.MaxValue; //попытка увеличить пропускную способность сети
            //binding.MaxReceivedMessageSize = long.MaxValue;
            //binding.MaxBufferPoolSize = long.MaxValue;//теперь вообще не запускается
            SetupScene(Convert.ToInt32(this.tbStepRange.Text));
        }
        private void SetupScene(int numberOfBalls)
        {
            scene = new Scene();
            scene.Background = new Background(new RayTracer.Color(0.0, 0.0, 0.0), 0.1);//new Background(new Color(.2, .3, .4), 0.5);
            RayTracer.Vector campos = new RayTracer.Vector(0, 0, -5);
            scene.Camera = new Camera(campos, campos / -2, new RayTracer.Vector(0, 1, 0).Normalize());

            Random rnd = new Random();
            for ( int i = 0; i < numberOfBalls; i++ )
            {
                // setup a solid reflecting sphere
                scene.Shapes.Add(new SphereShape(new RayTracer.Vector(rnd.Next(-100, 100) / 50.0, rnd.Next(-100, 100) / 50.0, rnd.Next(0, 200) / 50.0), .2,
                                   new SolidMaterial(new RayTracer.Color(rnd.Next(0, 100) / 100.0, rnd.Next(0, 100) / 100.0, rnd.Next(0, 100) / 100.0), 0.4, 0.0, 2.0)));

            }
            // setup the chessboard floor
            scene.Shapes.Add(new PlaneShape(new RayTracer.Vector(0.1, 0.9, -0.5).Normalize(), 1.2,
                               new ChessboardMaterial(new RayTracer.Color(1, 1, 1), new RayTracer.Color(0, 0, 0), 0.2, 0, 1, 0.7)));

            scene.Lights.Add(new Light(new RayTracer.Vector(5, 10, -1), new RayTracer.Color(0.8, 0.8, 0.8)));
            scene.Lights.Add(new Light(new RayTracer.Vector(-3, 5, -15), new RayTracer.Color(0.8, 0.8, 0.8)));
        }
        bool start_stop = false;
        private void bStartServer_Click(object sender, RoutedEventArgs e)
        {
            if ( start_stop == false )
            {
                serviceHost = new ServiceHost(typeof(GridService));
                serviceHost.Open();
                bStartServer.Content = "Stop Server";
                start_stop = true;
                //bGO.IsEnabled = true;
                bTest1.IsEnabled = true;
            }
            else
            {
                serviceHost.Close();
                bStartServer.Content = "Start Server";
                start_stop = false;
                bGO.IsEnabled = false;
                bTest1.IsEnabled = false;
            }
        }

        private void bGO_Click(object sender, RoutedEventArgs e)
        {
            SetupScene(Convert.ToInt32(this.tbStepRange.Text));
            Clients.UpdateTable();
            if ( isLocalTracing )
            {
                DoCancelLocalApp();
                countRes = rect.Height / Steps;                                 //остается в списке процессов в диспетчере задач
               if ( thr1 != null )
                    if ( thr.ThreadState != System.Threading.ThreadState.Aborted && thr1.ThreadState != System.Threading.ThreadState.Unstarted )
                    {
                        thr1.Abort();
                    }
                return;
            }
            tbTime.Text = "";
            time.Reset();
            isLocalTracing = true;
            bGO.Content = "Stop render";
            Steps = Convert.ToInt32(tbRectangles.Text);
            SendClientJob(Steps);
        }

        void SendClientJob(int Steps)
        {
            Graphics g = Graphics.FromImage(pbSceneLocal.Image);
            g.FillRectangle(new SolidBrush(System.Drawing.Color.Black), 0.0F, 0.0F, (float) pbSceneLocal.Image.Width, (float) pbSceneLocal.Image.Height);
            rect = new System.Drawing.Rectangle(0, 0, 400, 400);
            localBitmap = new System.Drawing.Bitmap(rect.Width, rect.Height);
            pbSceneLocal.Image = localBitmap;
            gr = System.Drawing.Graphics.FromImage(pbSceneLocal.Image);

            RangeList = new List<RangeOfCalculation>();
            Result = new List<ResultRenderRows>();
            //создаем клиенты
            binding.SendTimeout = new TimeSpan(0, 0, 0, 0, Convert.ToInt32(maxPerfCount * 10));
            for ( int i = 0; i < Clients.ListClients.Count; i++ )
            {
                EndpointAddress address = new EndpointAddress("http://" + Clients.ListClients [ i ].IP + ":8734/ClientService/");
               clients [ i ] = new ClientServiceClient(binding, address);
               clients [ i ].RayTraceRowsCompleted += MainWindow_RayTraceRowsCompleted;
            }
            // Разбиваем задачу на части
            for ( int i = 0; i < rect.Height / Steps; i++ )
            {
                RangeOfCalculation r = new RangeOfCalculation(i * Steps, i * Steps + Steps, RangeOfCalculation.CurrentState.notComputing);
                RangeList.Add(r);
            }
            countRange = RangeList.Count;//количество диапазонов
            //запускаем поток, в котором части задачи будут распределяться между клиентами
            thr = new Thread(new ThreadStart(Computing));
            thr.Start();
            //запускаем поток в котором будет выполняться отрисовка созданных клиентом кусочков сцены
            thr1 = new Thread(new ThreadStart(DrawingRows));
            thr1.Start();
        }

        public void DrawingRows()//пришлось запускать отрисовку кусочков картинки в отдельном потоке, иначе, если одновременно приходили ответы от
        {                        //нескольких клиентов, то программа пыталась одновременно нарисовать два кусочка картинки и на picturebox появлялись пустые полосы
            countRes = 0;
            int start = 10;
            int c = 0;
            do
            {
                if ( Result != null )
                {                    
                    countRes = Result.Count;
                    for ( int i = start-10; i < countRes; i++ )
                    {
                        try
                        {
                            if ( Result [ i ].state == ResultRenderRows.CurrentState.notDraw )
                            {
                                gr.DrawImage(Result [ i ].image, 0, Result [ i ].startPos);
                                pbSceneLocal.Invalidate(new System.Drawing.Rectangle(0, Result [ i ].startPos, pbSceneLocal.Image.Width, Result [ i ].image.Height));
                                Result [ i ].state = ResultRenderRows.CurrentState.Draw;
                                c += 1;        
                            }
                        }
                        catch ( NullReferenceException )//может возникнуть при обращении к только что созданному элементу списка Result
                        {                               
                            continue;
                        }
                    }
                }                
            } while ( countRes != rect.Height/Steps );
        }
        public void Computing()
        {
            time.Start();
            RayTracer.RayTracer raytracer = new RayTracer.RayTracer(antiAliasing, renderDiffuse, renderHighlights, renderShadow, renderReflection, renderRefraction);
            while ( countRange != 0 )
            {
                for ( int i = 0; i < Clients.ListClients.Count; i++ )//тут появляется проблема при отключении клиента
                {                                                    //т.к. уменьшается количество клиентов в списке
                    if ( Clients.ListClients.Count>0 && (Clients.ListClients [ i ].Done == "Ready" || Clients.ListClients [ i ].Done == "OK") )
                    {
                        for ( int j = 0; j < RangeList.Count; j++ )
                        {
                            if ( RangeList [ j ].state == RangeOfCalculation.CurrentState.notComputing )
                            {
                                RangeList [ j ].state = RangeOfCalculation.CurrentState.computing;
                                clients [ i ].RayTraceRowsAsync(scene, rect, RangeList [ j ].start, Steps, raytracer, j);
                                Clients.ListClients [ i ].Done = "Working";
                                Clients.ListClients [ i ].Task = j;
                                break;
                            }
                        }
                    }
                }
            }
        }
        void raytracer_RenderUpdate(int progress, double duration, double ETA, int scanline)//не нужно
        {
            pbSceneLocal.Invoke((MethodInvoker) delegate ()
            {
                //only invalidate part of the picturebox that needs to be redrawn
                pbSceneLocal.Invalidate(new System.Drawing.Rectangle(0, scanline - 1, pbSceneLocal.Image.Width, 2));
                System.Windows.Forms.Application.DoEvents(); // some time to redraw the screen
            });
        }
        private void DoCancelLocalApp()
        {
            thr.Abort();            
            isLocalTracing = false;            
            bGO.Dispatcher.BeginInvoke(new Action(delegate () { bGO.Content = "Start render"; }));
            pbSceneLocal.Invoke((MethodInvoker) delegate ()
            {
                pbSceneLocal.Refresh();
            });
            
            
        }

        object objLock = new object();
        void MainWindow_RayTraceRowsCompleted(object sender, RayTraceRowsCompletedEventArgs e)
        {
            string host = (sender as ClientServiceClient).Endpoint.Address.Uri.Host;
            try
            {
                lock ( pbSceneLocal )
                {
                    RangeList [ e.Result.Item2 ].state = RangeOfCalculation.CurrentState.completing;
                    dataGrid1.Dispatcher.BeginInvoke(new Action(delegate ()
                    {
                        for ( int i = 0; i < Clients.ListClients.Count; i++ )
                        {
                            if ( Clients.ListClients [ i ].IP == host )
                            {
                                Clients.ListClients [ i ].Done = "OK";
                                Clients.UpdateTable();
                                break;
                            }
                        }
                    }));
                    Result.Add(new ResultRenderRows(e.Result.Item1,  e.Result.Item4, ResultRenderRows.CurrentState.notDraw));
                    tbError.Dispatcher.BeginInvoke(new Action(delegate () { tbError.Text = (e.Result.Item1.Height).ToString()+"  "+ (e.Result.Item4).ToString(); }));

                    //gr.DrawImage(e.Result.Item1, 0, RangeList [ e.Result.Item2 ].start);   
                    //e.Result.Item3.RenderUpdate += new RenderUpdateDelegate(raytracer_RenderUpdate);
                    //e.Result.Item3.RenderUp(RangeList [ e.Result.Item2 ].start, pbSceneLocal, e.Result.Item5);                    
                    countRange--;
                    if ( countRange == 0 )
                    {
                        time.Stop();
                        double d = 1.00 * time.ElapsedMilliseconds / 1000;
                        tbTime.Dispatcher.BeginInvoke(new Action(delegate () { tbTime.Text = (d).ToString(); }));
                        DoCancelLocalApp();
                    }
                }
            }
            catch ( System.Reflection.TargetInvocationException )//возникает при отключении клиента
            {
                dataGrid1.Dispatcher.BeginInvoke(new Action(delegate ()
                {
                    for ( int i = 0; i < Clients.ListClients.Count; i++ )
                    {
                        if ( Clients.ListClients [ i ].IP == host )
                        {
                            Clients.ListClients [ i ].Done = "Not ready";//тут я перевожу клиент в неактивное состояние
                            RangeList [ Clients.ListClients [ i ].Task ].state = RangeOfCalculation.CurrentState.notComputing;
                            Clients.UpdateTable();
                            break;
                        }
                    }
                }));
            }
        }

        #region Test Perfomance
        object objLock1 = new object();
        private void bTest1Click(object sender, RoutedEventArgs e)
        {
            SetupScene(Convert.ToInt32(this.tbStepRange.Text));
            binding.SendTimeout = new TimeSpan(0, 2, 0);

            int n = Clients.ListClients.Count;
            for ( int i = n - 1; i >= 0; i-- )//при повторном запуске задачи проверяет наличие соединения между сервером и клиентом
            {                                 //чтобы убрать из списка клиентов те клиенты, которые выдали ошибку при предыдущем вычислении задачи.
                try                           //Если же соединение было обнаружено, то клиент остается в списке
                {
                    if ( Clients.ListClients [ i ].Done == "Not ready" )
                    {
                        ClientServiceClient client = new ClientServiceClient(binding, new EndpointAddress("http://" + Clients.ListClients [ i ].IP + ":8734/ClientService/"));
                        if ( client.CheckConnection() == true )
                            Clients.ListClients [ i ].Done = " ";
                    }
                }
                catch ( Exception )
                {
                    Clients.RemoveClient(Clients.ListClients [ i ].IP);
                }
            }
            RayTracer.RayTracer raytracer = new RayTracer.RayTracer(antiAliasing, renderDiffuse, renderHighlights, renderShadow, renderReflection, renderRefraction);
            clients = new ClientServiceClient [ Clients.ListClients.Count ];
            Steps = Convert.ToInt32(tbRectangles.Text); // # of rectangles
            for ( int i = 0; i < Clients.ListClients.Count; i++ )
            {
                EndpointAddress address = new EndpointAddress("http://" + Clients.ListClients [ i ].IP + ":8734/ClientService/");
                clients [ i ] = new ClientServiceClient(binding, address);
                clients [ i ].PerfomanceCompleted += MainWindow_PerfomanceCompleted;
                clients [ i ].PerfomanceAsync(scene, rect, raytracer, Steps, i);
            }
            bGO.IsEnabled = true;
        }

        void MainWindow_PerfomanceCompleted(object sender, PerfomanceCompletedEventArgs e)
        {
            try
            {
                Clients.ListClients [ e.Result.Item2 ].Perfomance = Convert.ToString(e.Result.Item1 / 1000);
                Clients.ListClients [ e.Result.Item2 ].Done = "Ready";
                Clients.UpdateTable();
                if ( e.Result.Item1 > maxPerfCount )
                {
                    maxPerfCount = e.Result.Item1;
                }
            }
            catch ( System.Reflection.TargetInvocationException )//возникает, если клиент отсоединяется во время проверки
            {                                                  //производительности
                //long size = 0;                      //для измерения размера передаваемого объекта в битах
                //using ( Stream stream = new MemoryStream() )
                //{
                //    BinaryFormatter formatter = new BinaryFormatter();
                //    formatter.Serialize(stream, scene);                    
                //    size = stream.Length;
                //}
                //tbError.Dispatcher.BeginInvoke(new Action(delegate () { tbError.Text = size.ToString() + "   "; }));
                for ( int i = 0; i < Clients.ListClients.Count; i++ )
                {
                    string host = (sender as ClientServiceClient).Endpoint.Address.Uri.Host;
                    if ( Clients.ListClients [ i ].IP == host )
                    {
                        Clients.ListClients [ i ].Done = "Not ready";
                        Clients.UpdateTable();
                        break;
                    }
                }
            }
        }
        #endregion
        private void CloseWindow(object sender, EventArgs e)//событие, которое возникает при закрытии окна программы. Нужно для остановки потока
        {                                                   //в котором задача распределяется между клиентами, т.к. если его не остановить, то программа
            countRes = rect.Height;                                 //остается в списке процессов в диспетчере задач
            countRange = 0;
            if ( thr != null )
                if ( thr.ThreadState != System.Threading.ThreadState.Aborted && thr.ThreadState != System.Threading.ThreadState.Unstarted )
                {
                    thr.Abort();
                }            
            if ( thr1 != null )
                if ( thr.ThreadState != System.Threading.ThreadState.Aborted && thr1.ThreadState != System.Threading.ThreadState.Unstarted )
                {
                    thr1.Abort();
                }
        }
        }
}
