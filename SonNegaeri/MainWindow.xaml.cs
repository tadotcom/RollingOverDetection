using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Media;
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

namespace SonNegaeri
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SerialPort serialPort = null;
        private string acceleration = "";
        int count = 0;

        public MainWindow()
        {
            InitializeComponent();
            //disconnct.IsEnabled = false;
        }

        //トワイライトからデータを受信したときの関数
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            try
            {
                //! 受信データを読み込む.
                var data = serialPort.ReadExisting();

                AccelerationAnalysis(data);

                //! 受信したデータをテキストボックスに書き込む.
                Debug.WriteLine(data);

                //connct.IsEnabled = false;
                //disconnct.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //警告音を鳴らす関数
        private void PlayBeep()
        {
            System.Media.SystemSounds.Beep.Play();
            Console.Beep(1000, 1000);
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            //ポート番号取得
            var _port = port.Text;

            //シリアルポートに接続するための情報
            serialPort = new SerialPort();
            serialPort.PortName = _port;
            serialPort.BaudRate = 115200;
            serialPort.DataBits = 8;
            serialPort.Parity = Parity.None;
            serialPort.StopBits = StopBits.One;
            serialPort.Encoding = Encoding.UTF8;
            serialPort.WriteTimeout = 100000;

            // シリアルポートに接続
            try
            {
                //イベントリスナーを追加
                serialPort.DataReceived += SerialPort_DataReceived;

                // ポートオープン
                serialPort.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            serialPort.Close();
        }

        private void AccelerationAnalysis(string data)
        {
            string accelerationData = "";

            if(60 < data.Length)
            {
                accelerationData = data.Substring(54, 1);
            } else
            {
                return;
            }

            if(!accelerationData.Equals("1") && !accelerationData.Equals("6"))
            {
                Debug.WriteLine("1と6以外");
                return;
            }

            Debug.WriteLine(accelerationData);

            if (accelerationData.Equals("1"))
            {
                Debug.WriteLine("問題なし");
                count = 0;
            }

            if (accelerationData.Equals("6"))
            {
                Debug.WriteLine("寝返りを検知");
                count++;
            }

            if(2 == count)
            {
                Alert();
            }
        }

        private void Alert()
        {
            PlayBeep();
            count = 0;
        }
    }
}
