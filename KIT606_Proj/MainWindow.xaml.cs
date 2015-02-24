using KIT606_Proj.Controller;
using KIT606_Proj.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace KIT606_Proj
{
    public partial class MainWindow : Window
    {
        bool isFwd;
        CABuilder cb = new CABuilder();
        SLCAEngine se = new SLCAEngine();
        ELCAEngine ee = new ELCAEngine();
        Stopwatch stopWatch = new Stopwatch();
        private static Timer aTimer;
        delegate void SetValueCallback(string value);
        private static DateTime startTime = DateTime.Now;

        public MainWindow()
        {
            InitializeComponent();
            aTimer = new System.Timers.Timer(1000);
            aTimer.Elapsed += OnTimedEvent;
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            SetValueCallback d = new SetValueCallback(updateTimeElapse);
            TimeSpan ts = e.SignalTime - startTime;
            TimeElapsed.Dispatcher.BeginInvoke(d, String.Format("{0:00}:{1:00}:{2:00}.{3:000}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds));
        }

        private void updateTimeElapse(string value)
        {
            TimeElapsed.Content = value;
        }

        private void Task_BuildCA(string path, string query)
        {
            cb.buildLs2(path, query);
        }

        private void Task_SLCA()
        {
            se.getSLCA(isFwd);
        }

        private void Task_ELCA()
        {
            ee.getELCA(isFwd);
        }

        private async void buildCABtn_Click(object sender, RoutedEventArgs e)
        {
            if (checkProcess()) return;
            if (filePath.Text.Equals("")) return;
            if (qryTxt.Text.Equals("")) return;

            string path = filePath.Text;
            string query = qryTxt.Text;
            pBar.IsIndeterminate = true;
            startTime = DateTime.Now;
            aTimer.Enabled = true;
            Task caTask = Task.Factory.StartNew(() => {
                stopWatch.Reset();
                stopWatch.Start();
                Task_BuildCA(path, query);
                stopWatch.Stop();
            });
            await caTask;
            pBar.IsIndeterminate = false;
            aTimer.Enabled = false;
            TimeElapsed.Content = "";
            SLCABtn.IsEnabled = true;
            ELCABtn.IsEnabled = true;
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
            StringBuilder sb = new StringBuilder();
            sb.Append("CAs for keywords {");
            foreach (string keyword in cb.keywords)
            {
                sb.Append(keyword);
                sb.Append(",");
            }
            sb.Replace(',', '}', sb.Length - 1, 1);
            sb.Append("\r\n");
            sb.Append("elapsedTime:  ");
            sb.Append(elapsedTime);
            sb.Append("\r\n");
            sb.Append("# of CAs: \r\n");
            int total = 0;
            for(int i =0 ;i<cb.Ls2.Count; i++){
                sb.Append(cb.keywords[i]);
                sb.Append(":");
                sb.Append(cb.Ls2[i].Count);
                sb.Append("\r\n");
                total += cb.Ls2[i].Count;
            }
            sb.Append("In total:");
            sb.Append(total);
            sb.Append("\r\n");
            sb.Append("--------------------------------------------------------\r\n");
            txt2.Text += sb.ToString();
        }

        private bool checkProcess()
        {
            if (pBar.IsIndeterminate)
            {
                MessageBox.Show("Query is in process", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return true;
            }
            return false;
        }

        private void browse_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Source File";
            dlg.DefaultExt = ".xml";
            dlg.Filter = "XML Documents (.xml)|*.xml";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                filePath.Text = dlg.FileName;
            }
        }

        private void qryTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                buildCABtn_Click(this, null);
            }

        }

        private async void ELCABtn_Click(object sender, RoutedEventArgs e)
        {
            if (checkProcess()) return;
            ee.Ls2 = cb.Ls2;
            ee.keywords = cb.keywords;
            pBar.IsIndeterminate = true;
            startTime = DateTime.Now;
            aTimer.Enabled = true;
            Task elcaTask = Task.Factory.StartNew(() =>
            {
                stopWatch.Reset();
                stopWatch.Start();
                Task_ELCA();
                stopWatch.Stop();
            });
            await elcaTask;
            pBar.IsIndeterminate = false;
            aTimer.Enabled = false;
            TimeElapsed.Content = "";
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
            StringBuilder sb = new StringBuilder();
            sb.Append(isFwd?"Fwd" : "Bwd");
            sb.Append("ELCA for keywords {");
            foreach (string keyword in cb.keywords)
            {
                sb.Append(keyword);
                sb.Append(",");
            }
            sb.Replace(',', '}', sb.Length - 1, 1);
            sb.Append("\r\n");
            sb.Append("elapsedTime:  ");
            sb.Append(elapsedTime);
            sb.Append("\r\n");
            sb.Append("----------------------------\r\n");
            txt2.Text += sb.ToString();
            txt1.Text += isFwd ? "Fwd" : "Bwd";
            txt1.Text += ee.printELCAs(false);
        }

        private async void SLCABtn_Click(object sender, RoutedEventArgs e)
        {
            if (checkProcess()) return;
            se.Ls2 = cb.Ls2;
            se.keywords = cb.keywords;
            pBar.IsIndeterminate = true;
            startTime = DateTime.Now;
            aTimer.Enabled = true;
            Task slcaTask = Task.Factory.StartNew(() =>
            {
                stopWatch.Reset();
                stopWatch.Start();
                Task_SLCA();
                stopWatch.Stop();
            });
            await slcaTask;
            pBar.IsIndeterminate = false;
            aTimer.Enabled = false;
            TimeElapsed.Content = "";
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            StringBuilder sb = new StringBuilder();
            sb.Append(isFwd ? "Fwd" : "Bwd");
            sb.Append("SLCA for keywords {");
            foreach (string keyword in cb.keywords)
            {
                sb.Append(keyword);
                sb.Append(",");
            }
            sb.Replace(',', '}', sb.Length - 1, 1);
            sb.Append("\r\n");
            sb.Append("elapsedTime:  ");
            sb.Append(elapsedTime);
            sb.Append("\r\n");
            sb.Append("----------------------------\r\n");
            txt2.Text += sb.ToString();
            txt1.Text += isFwd ? "Fwd" : "Bwd";
            txt1.Text += se.printSLCAs(false);
        }

        private void fwdRbn_Checked(object sender, RoutedEventArgs e)
        {
            isFwd = true;
        }

        private void bwdRbn_Checked(object sender, RoutedEventArgs e)
        {
            isFwd = false;
        }

        private void CLearRight_Click(object sender, RoutedEventArgs e)
        {
            txt2.Text = "";
        }

        private void CLearLeft_Click(object sender, RoutedEventArgs e)
        {
            txt1.Text = "";
        }
    }
}
