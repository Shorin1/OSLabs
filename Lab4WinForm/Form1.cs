using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab4WinForm
{
    public partial class Form1 : Form
    {
        public delegate void InvokeDelegate();

        /// <summary>
        /// Семафор для потока, в котором происходит чтение-запись
        /// </summary>
        private readonly Semaphore buttonSemaphore;

        /// <summary>
        /// Семафор для запуска потоков, чтобы чтение не начиналось раньше записи
        /// </summary>
        private readonly Semaphore startThreadsSemaphore;

        /// <summary>
        /// Семафор на чтение-запись. Пока происходит чтение запись невозможна
        /// </summary>
        private readonly Semaphore readWriteSemaphore;

        /// <summary>
        /// Флаг означающий конец текста для записи
        /// </summary>
        private bool isEnd;

        public Form1()
        {
            isEnd = false;
            buttonSemaphore = new Semaphore(1, 1);
            startThreadsSemaphore = new Semaphore(1, 1);
            readWriteSemaphore = new Semaphore(1, 1);
            InitializeComponent();
        }

        private void runButton_Click(object sender, EventArgs e)
        {
            var readWriteThread = new Thread(new ThreadStart(() => ReadWrite()));
            readWriteThread.Start();
        }

        /// <summary>
        /// Чтение-запись в отдельном потоке (фоне)
        /// </summary>
        private void ReadWrite()
        {
            int.TryParse(speedTextBox.Text, out int delay);

            var fileName = "text.txt";
            var text = inputTextBox.Text;

            // Блокируем чтоб новые операции на чтение-запись ждали пока закончатся предыдущие
            buttonSemaphore.WaitOne();

            var writeThread = new Thread(new ThreadStart(() => Write(fileName, text, delay)));
            writeThread.Start();

            // Блокируем чтоб чтения не начиналось раньше записи
            startThreadsSemaphore.WaitOne();

            var readThread = new Thread(new ThreadStart(() => Read(fileName)));
            readThread.Start();
        }

        private void Write(string fileName, string text, int delay)
        {
            foreach (var letter in text)
            {
                // Блокируем поток чтобы чтение не начиналось пока не закончилась запись
                readWriteSemaphore.WaitOne();
                using (var sw = new StreamWriter(fileName, true))
                {
                    sw.Write(letter);
                }
                Thread.Sleep(delay);
                // Высвобождаем семафор чтения-записи
                readWriteSemaphore.Release();
                // Высвобождаем семафор синхранизации старта задач 
                startThreadsSemaphore.Release();
            }

            isEnd = true;
        }

        private void Read(string fileName)
        {
            while (!isEnd)
            {
                // Ждем окончания блокировки семафора синхронизации старта задач
                startThreadsSemaphore.WaitOne();
                // Ждем окончания блокировки семафора синхронизации чтения-записи
                readWriteSemaphore.WaitOne();
                using (var sr = new StreamReader(fileName))
                {
                    var text = sr.ReadToEnd();
                    // ui поток не позволяет обращаться к нему из других потоков поэтому юзаем такой костыль
                    outputTextBox.BeginInvoke(new InvokeDelegate(() => outputTextBox.Text = text));
                }
                // Высвобождаем семафор чтения-записи
                readWriteSemaphore.Release();
            }

            // Высвобождаем семафор потока в котором начинаются операции чтения-записи
            buttonSemaphore.Release();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.Columns.AddRange(new[]
            {
                new DataGridViewTextBoxColumn() { Name = "DeviceID" },
                new DataGridViewTextBoxColumn() { Name = "PNPDeviceID" },
                new DataGridViewTextBoxColumn() { Name = "Description" }
            });

            var thread = new Thread(new ParameterizedThreadStart(obj => ViewUsbDevices()));
            thread.Start();
        }

        private void ViewUsbDevices()
        {
            while(true)
            {
                using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_USBHub"))
                using (var collection = searcher.Get())
                {
                    foreach (var device in collection)
                    {
                        var row = new string[]
                        {
                            (string)device.GetPropertyValue("DeviceID"),
                            (string)device.GetPropertyValue("PNPDeviceID"),
                            (string)device.GetPropertyValue("Description")
                        };

                        Invoke(new Action(() => dataGridView1.Rows.Add(row)));
                    }
                }

                Thread.Sleep(2000);
                Invoke(new Action(() => dataGridView1.Rows.Clear()));
            }
        }
    }
}
