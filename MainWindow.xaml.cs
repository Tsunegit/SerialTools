using System;
using System.IO.Ports;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;

namespace SericalTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private SerialPort? port;
        public MainWindow()
        {
            InitializeComponent();
            InitializeSerical();
        }

        private void InitializeSerical()
        {
            //初始化串口
            var portList = SerialPort.GetPortNames();
            Cbport.ItemsSource = portList;
            Cbport.SelectedIndex = 0;
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (port != null && port.IsOpen)
            {
                if(CloseSerialPort())
                {
                    button.Content = "打开串口";
                }
            }else
            {
                if (OpenSerialPort())
                {
                    button.Content = "关闭串口";
                }

            }
        }

        private bool OpenSerialPort()
        {
            bool flag = false;

            try
            {
                port = new();
                port.PortName = Cbport.Text;
                port.BaudRate = int.Parse(CbBaud.Text);
                port.Parity = (Parity)Enum.Parse(typeof(Parity),CbBits.Text);
                port.DataBits = int.Parse(CbData.Text);
                port.StopBits = (StopBits)Enum.Parse(typeof(StopBits), CbStop.Text);

                port.Open();

                port.DataReceived += serialPort_DataReceived;
                if (port.IsOpen)
                {
                    flag = true;
                    Cbport.IsEnabled = false;
                    CbBaud.IsEnabled = false;
                    CbBits.IsEnabled = false;
                    CbData.IsEnabled = false;
                    CbStop.IsEnabled = false;
                }
            }
            catch(Exception)
            {
                MessageBox.Show("串口打开失败");
            }

            return flag;
        }

        private bool CloseSerialPort()
        {
            bool flag = false;

            try
            {
                port.Close();
                flag = true;
                Cbport.IsEnabled = true;
                CbBaud.IsEnabled = true;
                CbBits.IsEnabled = true;
                CbData.IsEnabled = true;
                CbStop.IsEnabled = true;

            }
            catch(Exception)
            {
                MessageBox.Show("关闭失败");
            }

            return flag;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (port == null)
            {
                MessageBox.Show("请先连接串口");
                return;
            }
            try
            {
                var datas = Encoding.UTF8.GetBytes(SendTb.Text);
                port!.Write(datas,0,datas.Length);
                RecevTb.Content += "我: " + SendTb.Text + "\n";
                RecevTb.ScrollToEnd();
            }
            catch(Exception) 
            { 
            }

        }

        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int count = port!.BytesToRead;
            byte[] buffer = new byte[count];
            port.Read(buffer, 0,count);
            string msg = Encoding.UTF8.GetString(buffer);

            Dispatcher.Invoke(() =>
            {
                RecevTb.Content +=  "对方: "+msg + "\n";
                RecevTb.ScrollToEnd();
            });
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("确认退出？", "确认", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                Application.Current.Shutdown();
            }
        }

        // 捕获鼠标按下事件
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // 如果按下的是左键，则开始拖动窗口
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
