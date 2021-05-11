using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab4WinForm
{
    public partial class Form1 : Form
    {
        public delegate void InvokeDelegate();
        private readonly object syncObj;

        public Form1()
        {
            syncObj = new object();
            InitializeComponent();
        }

        private void runButton_Click(object sender, EventArgs e)
        {
            int.TryParse(speedTextBox.Text, out int delay);
            ReadWriteAsync(delay);
        }

        private void ReadWriteAsync(int delay)
        {
            var thread = new Thread(new ThreadStart(() => ReadWrite(delay)));
            thread.Start();
        }

        private void ReadWrite(int delay)
        {
            lock (syncObj)
            {
                var fileName = "file.txt";
                var text = inputTextBox.Text;

                foreach (var letter in text)
                {
                    using (var sw = new StreamWriter(fileName, true))
                    {
                        sw.Write(letter);
                    }

                    Thread.Sleep(delay);

                    using (var sr = new StreamReader(fileName))
                    {
                        var lines = sr.ReadToEnd();
                        outputTextBox.BeginInvoke(new InvokeDelegate(() => outputTextBox.Text = lines));

                    }
                }
            }
        }
    }
}
