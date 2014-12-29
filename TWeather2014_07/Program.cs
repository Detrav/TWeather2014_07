using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TWeather2014_07
{
    class Program
    {
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        static void Main(string[] args)
        {
            ShowWindow(GetConsoleWindow(), 0);
            int size = 2000;
            using (var form = new Settings())
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    size = (int)form.numericUpDown1.Value;
                else return;
            using (var tWeatherSnow = new TWeatherSnow(size))
                tWeatherSnow.Run();
        }
    }
}
