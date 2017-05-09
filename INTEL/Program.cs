using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace INTEL
{
    static class Program
    {
        public static Random R { get; private set; }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            R = new Random();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            DataVisual dv = new DataVisual();
            INTEL.Console.Initialize(dv);
            Application.Run(dv);
        }
    }
}
