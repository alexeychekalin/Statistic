using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {

            string appFilePath = Process.GetCurrentProcess().MainModule.FileName;

            // Создаем объект блокировки файла
            using (Mutex mutex = new Mutex(true, $"Global\\{appFilePath.GetHashCode()}"))
            {
                string currentProcessName = Process.GetCurrentProcess().ProcessName;

                // Проверяем, сколько процессов с таким же именем запущено
                Process[] processes = Process.GetProcessesByName(currentProcessName);

                // Если запущено более одного процесса с таким же именем, завершаем выполнение
                if (processes.Length > 1)
                {
                    MessageBox.Show("Приложение уже запущено.");
                    return;
                }               
               
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Karta0209());
            Application.Run(new Form1());
        }
    }
}
