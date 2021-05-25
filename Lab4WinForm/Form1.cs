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
        private bool isEnd = false;

        public Form1()
        {
            InitializeComponent();
            dataGridView1.Columns.AddRange(new[]
           {
                new DataGridViewTextBoxColumn() { Name = "DeviceID" },
                new DataGridViewTextBoxColumn() { Name = "PNPDeviceID" },
                new DataGridViewTextBoxColumn() { Name = "Description" }
            });
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

            var writeThread = new Thread(new ThreadStart(() => Write(fileName, delay)));
            writeThread.Start();

            var readThread = new Thread(new ThreadStart(() => Read(fileName, delay)));
            readThread.Start();
        }

        private void Write(string fileName, int delay)
        {
            while (true)
            {
                var text = GetUsbDevicesInfo();
                try
                {
                    using (var sw = new StreamWriter(fileName, false, Encoding.Default))
                    {
                        foreach (var row in text)
                        {
                            foreach (var col in row)
                            {
                                sw.Write(col + "\t");
                            }
                            sw.Write("\n");
                        }
                    }
                    Thread.Sleep(delay);
                }
                catch (IOException) { }
            }
        }

        private void Read(string fileName, int delay)
        {
            while (true)
            {
                try
                {
                    string text;

                    using (var sr = new StreamReader(fileName))
                    {
                        text = sr.ReadToEnd();
                    }

                    var rows = text.Split('\n');

                    foreach (var row in rows)
                    {
                        Invoke(new Action(() => dataGridView1.Rows.Add(row.Split('\t'))));
                    }

                    Thread.Sleep(delay);
                    Invoke(new Action(() => dataGridView1.Rows.Clear()));
                }
                catch (IOException) { }
            }
        }

        private IEnumerable<string[]> GetUsbDevicesInfo()
        {
            using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_USBHub"))
            using (var collection = searcher.Get())
            {
                foreach (var device in collection)
                {
                    yield return new string[]
                    {
                            (string)device.GetPropertyValue("DeviceID"),
                            (string)device.GetPropertyValue("PNPDeviceID"),
                            (string)device.GetPropertyValue("Description")
                    };
                }
            }
        }
    }
}
