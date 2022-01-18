using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace WpfApp9
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Semaphore semaphore { get; set; }
        Thread Cthread;
        static int count = 0;
        public int countsemaphore { get; set; }
        public MainWindow()
        {
            InitializeComponent();

        }

        private void btn_create_Click(object sender, RoutedEventArgs e)
        {
            semaphore = new Semaphore(countsemaphore,countsemaphore, "SEMAPHORE");

            for (int i = 0; i < 4; i++)
            {
                Cthread = new Thread(() => SomeMethod(semaphore));
                ++count;
                Cthread.Name = $"Thread->{count}";
                Dispatcher.Invoke(() => listbox_created.Items.Add(Cthread));
            }


        }

        private void SomeMethod(Semaphore s)
        {
            bool st = false;
            while (!st)
            {
                if (s.WaitOne(1))
                {
                    try
                    {
                        var t = Thread.CurrentThread;
                        Dispatcher.Invoke(() => listbox_working.Items.Add(t));
                        Dispatcher.Invoke(() => listbox_waiting.Items.Remove(t));
                        Thread.Sleep(5000);
                    }
                    finally
                    {
                        st = true;
                        var t = Thread.CurrentThread;
                        Dispatcher.Invoke(() =>
                        {
                            listbox_working.Items.Remove(t);
                            listbox_waiting.Items.Remove(t);
                        });
                        s.Release();
                    }
                }
                else
                {
                    var t = Thread.CurrentThread;
                    Dispatcher.Invoke(() =>
                    {
                        if(!listbox_waiting.Items.Contains(t)) listbox_waiting.Items.Add(t);
                    });
                }

            }

        }

        private void listbox_created_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var t = listbox_created.SelectedItem as Thread;
                t.Start();
                listbox_created.Items.Remove(t);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void nuSemaphore_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            
            if (nuSemaphore.Value <= 0)
            {
                MessageBox.Show("Error");
                nuSemaphore.Value = 1;
            }
            else
            {
                countsemaphore = Convert.ToInt32(nuSemaphore.Value);
            }
        }
    }
}
