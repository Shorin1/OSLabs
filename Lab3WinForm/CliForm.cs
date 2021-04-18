using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab3WinForm
{
    public partial class CliForm : Form
    {
        private string currentDir;
        private Dictionary<string, Action<Command>> commandDict;
        private int maxFileName = 25;

        public CliForm()
        {
            commandDict = new Dictionary<string, Action<Command>>
            {
                { "maxname", SetMaxName }, // Задать макс длину файла
                { "cd", Cd }, // Переход по каталогам
                { "ls", Ls }, // Показ содержимого текущего каталога
                { "dir", Dir }, // Содержимое каталога
                { "rd", Rd }, // Удаление каталога
                { "md", Md }, // Создание каталога
                { "dcopy", DCopy }, // Копирование каталога
                { "dmove", DMove }, // Перемещение/переименование каталога

                { "create", Create }, // создание файла
                { "del", Del }, // удаление файла
                { "ren", Ren }, // переименование файла
                { "fcopy", FCopy }, // копирование файла
                { "fmove", FMove }, // перемещение файла 
                { "fc", Fc } // сравнение файлов
            };

            currentDir = "C:";
            InitializeComponent();
            currentDirTextBox.Text = currentDir;
        }

        private void executeBtn_Click(object sender, EventArgs e)
        {
            string command = string.Empty;
            try
            {
                command = commandTextBox.Text;
                Execute(command);
                commandTextBox.Clear();
                currentDirTextBox.Text = currentDir;
            }
            catch (Exception exc)
            {
                logTextBox.AppendText($"Ошибка выполнения команды {command}: {exc.Message}\r\n");
            }
        }


        private void Execute(string commandStr)
        {
            var command = GetCommand(commandStr);

            if (!commandDict.ContainsKey(command.CommandName))
            {
                throw new Exception("Не удалось определить команду");
            }

            var action = commandDict[command.CommandName];
            action.Invoke(command);
        }

        private Command GetCommand(string command)
        {
            var commandSplit = command.Split(' ');
            var commandName = commandSplit[0].ToLower();
            var commandParams = commandSplit.Skip(1).ToArray();

            return new Command(commandName, commandParams);
        }

        private void Cd(Command command)
        {
            if (command.Params == null || command.Params.Length < 1)
            {
                throw new Exception("Не указан директорий");
            }

            var newCurrentDirPath = Path.Combine(currentDir, command.Params[0]);
            var newDir = new DirectoryInfo(newCurrentDirPath);

            if (!newDir.Exists)
            {
                throw new Exception("Директорий не найден");
            }

            currentDir = newDir.FullName;
        }

        private void Ls(Command command)
        {
            ShowDirInfo(currentDir);
        }

        private void SetMaxName(Command command)
        {
            if (command.Params == null || command.Params.Length < 1)
            {
                throw new Exception("Длина имени файла не задана");
            }

            if (int.TryParse(command.Params[0], out int maxLength))
            {
                throw new Exception("Длина файла должно быть числом");
            }

            maxFileName = maxLength;
        }

        // Работы с директориями
        private void Dir(Command command)
        {
            if (command.Params == null || command.Params.Length < 1)
            {
                throw new Exception("Не указано имя каталога");
            }

            var dir = command.Params[0];
            if (!Directory.Exists(dir))
            {
                throw new Exception("Директорий не найден");
            }

            ShowDirInfo(command.Params[0]);
        }

        private void ShowDirInfo(string dirName)
        {
            var dir = new DirectoryInfo(dirName);

            foreach (var d in dir.GetDirectories())
            {
                logTextBox.AppendText($"Directory: {d.Name}\r\n");
            }

            foreach (var f in dir.GetFiles())
            {
                logTextBox.AppendText($"File: {f.Name}\r\n");
            }
        }

        private void Rd(Command command)
        {
            if (command.Params == null || command.Params.Length < 1)
            {
                throw new Exception("Не указано имя каталога");
            }

            var dirName = command.Params[0];

            if (!Directory.Exists(dirName))
            {
                throw new Exception("Директорий не найден");
            }

            Directory.Delete(dirName);
        }

        private void Md(Command command)
        {
            if (command.Params == null || command.Params.Length < 1)
            {
                throw new Exception("Не указано имя каталога");
            }

            Directory.CreateDirectory(command.Params[0]);
        }

        private void DCopy(Command command)
        {
            if (command.Params == null || command.Params.Length < 2)
            {
                throw new Exception("Не указано имя каталога");
            }

            var dirPath = command.Params[0];
            var newDirPath = command.Params[1];

            if (!Directory.Exists(dirPath))
            {
                throw new Exception("Директорий не найден");
            }

            DirectoryCopy(dirPath, newDirPath);
        }

        private void DirectoryCopy(string dirName, string newDirName)
        {
            DirectoryInfo dir = new DirectoryInfo(dirName);

            DirectoryInfo[] dirs = dir.GetDirectories();

            Directory.CreateDirectory(newDirName);

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(newDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(newDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, tempPath);
            }
        }

        private void DMove(Command command)
        {
            if (command.Params == null || command.Params.Length < 2)
            {
                throw new Exception("Не указано имя каталога");
            }

            var dirPath = command.Params[0];
            var newDirPath = command.Params[1];

            if (!Directory.Exists(dirPath))
            {
                throw new Exception("Директорий не найден");
            }

            Directory.Move(dirPath, newDirPath);
        }

        //Работа с файлами
        private void Create(Command command)
        {
            if (command.Params == null || command.Params.Length < 1)
            {
                throw new Exception("Не указано имя файла");
            }

            var fileName = command.Params[0];
            var file = new FileInfo(fileName);

            if (file.Name.Length > maxFileName)
            {
                throw new Exception("Длина имя файла больше макс значения");
            }

            File.Create(fileName);
        }

        private void Del(Command command)
        {
            if (command.Params == null || command.Params.Length < 1)
            {
                throw new Exception("Не указано имя файла");
            }

            var fileName = command.Params[0];

            if (!File.Exists(fileName))
            {
                throw new Exception("Файл не найден");
            }

            File.Delete(fileName);
        }

        private void Ren(Command command)
        {
            if (command.Params == null || command.Params.Length < 2)
            {
                throw new Exception("Не указано имя файла");
            }

            var fileName = command.Params[0];
            var newFileName = command.Params[1];

            if (!File.Exists(fileName))
            {
                throw new Exception("Файл не найден");
            }

            var newFile = new FileInfo(newFileName);

            if (newFile.Name.Length > maxFileName)
            {
                throw new Exception("Длина имя файла больше макс значения");
            }

            File.Move(fileName, newFileName);
        }

        private void FMove(Command command)
        {
            if (command.Params == null || command.Params.Length < 2)
            {
                throw new Exception("Не указано имя файла");
            }

            var fileName = command.Params[0];
            var newFileName = command.Params[1];

            if (!File.Exists(fileName))
            {
                throw new Exception("Файл не найден");
            }

            var newFile = new FileInfo(newFileName);

            if (newFile.Name.Length > maxFileName)
            {
                throw new Exception("Длина имя файла больше макс значения");
            }

            File.Move(fileName, newFileName);
        }

        private void FCopy(Command command)
        {
            if (command.Params == null || command.Params.Length < 2)
            {
                throw new Exception("Не указано имя файла");
            }

            var fileName = command.Params[0];
            var newFileName = command.Params[1];

            if (!File.Exists(fileName))
            {
                throw new Exception("Файл не найден");
            }

            var newFile = new FileInfo(newFileName);

            if (newFile.Name.Length > maxFileName)
            {
                throw new Exception("Длина имя файла больше макс значения");
            }

            var file = new FileInfo(fileName);
            file.CopyTo(newFileName);
        }

        private void Fc(Command command)
        {
            if (command.Params == null || command.Params.Length < 2)
            {
                throw new Exception("Не указано имя файла");
            }

            var file1Name = command.Params[0];
            var file2Name = command.Params[1];

            if (!File.Exists(file1Name) || !File.Exists(file2Name))
            {
                throw new Exception("Один из файлов не найден");
            }

            using (var md5 = MD5.Create())
            using (var file1Stream = File.OpenRead(file1Name))
            using (var file2Stream = File.OpenRead(file2Name))
            {
                var hashFile1 = md5.ComputeHash(file1Stream);
                var hashFile2 = md5.ComputeHash(file2Stream);

                if (hashFile1 == hashFile2)
                {
                    logTextBox.AppendText("0\r\n");
                }
                else
                {
                    logTextBox.AppendText("1\r\n");
                }
            }
        }
    }

    public class Command
    {
        public string CommandName { get; private set; }

        public string[] Params { get; private set; }

        public Command(string commandName, params string[] commandParams)
        {
            CommandName = commandName;
            Params = commandParams;
        }
    }
}
