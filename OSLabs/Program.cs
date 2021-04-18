using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace OSLabs
{
    class Program
    {
        static void Main(string[] args)
        {
            var filePath = @"../../../Files";
            // Task.Run запускает выполнение задачи в отдельном потоке. Как именно управлять потоками - решает операционная система
            // Потоки могут выполнятся на одном процессоре или на нескольких. Это решает операционная система, мы же используем абстракции в виде Task (задач)
            var fileTasks = Directory.GetFiles(filePath).Select(x => Task.Run(() => GetCheckSum(x))).ToArray();
            Task.WaitAll(fileTasks);
            Console.WriteLine("Check sum calculate is end");
        }

        static void GetCheckSum(string fileName)
        {
            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead(fileName))
            {
                var hash = md5.ComputeHash(stream);
                var hashString = BitConverter.ToString(hash).Replace("-", string.Empty);
                Console.WriteLine($"Check sum of {fileName}: {hashString}");
            }
        }

    }
}
