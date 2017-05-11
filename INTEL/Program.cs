using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;
using INTEL.Network;

namespace INTEL
{
    static class Program
    {
        public static Random R { get { return _r; } }

        private static Random _r = new Random();
        private static BackgroundWorker _bgw = new BackgroundWorker();
        private static ProblemFactory[] _problems = new ProblemFactory[] { new XORProblemFactory(), new IPDProblemFactory() };

        private static Visual.Window _dv;
        private static Algorithm _a;

        const int PROBLEM = 1;      //set to 0 to allow choice..
        const int GENERATIONS = 0;  //set to 0 to allow choice..

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            _dv = new Visual.Window();

            _bgw.WorkerReportsProgress = true;
            _bgw.DoWork += _bgw_DoWork;
            _bgw.ProgressChanged += _bgw_ProgressChanged;
            _bgw.RunWorkerCompleted += _bgw_RunWorkerCompleted;
            _a = GetAlgorithm();
            _a.GenerationCompleted += _a_GenerationCompleted;
            _bgw.RunWorkerAsync();   

            Application.Run(_dv);
        }

        private static void _bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                throw e.Error;
        }

        private static void _a_GenerationCompleted(Algorithm.Info info)
        {
            _bgw.ReportProgress(info.Generation, info);
        }

        private static void _bgw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _dv.AlgorithmUpdated((Algorithm.Info)e.UserState);
        }

        private static void _bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            _a.Run();
        }

        private static Algorithm GetAlgorithm()
        {
            System.Console.Clear();
            Parameter.Initialize();

            DateTime thisDay = DateTime.UtcNow;
            WriteLine(thisDay + " (UTC)");
            WriteLine("INTEL NeuralNetwork for IPD v0.1, by Stan Noordman at Utrecht University");
            for (int i = 0; i < Parameter.List.Count; i++)
            {
                if (Parameter.ListDecorator.ContainsKey(i))
                    WriteLine(Parameter.ListDecorator[i]);
                WriteLine(i + ": " + Parameter.List[i].ToString() + " = " + Parameter.List[i].Value.ToString() + Parameter.List[i].Tooltip);
            }

            WriteLine("\nSelect problem:\n");
            for (int i = 0; i < _problems.Length; i++)
                WriteLine("<" + (i + 1) + "> " + _problems[i].Name);

            WriteLine("\nInput problem ID to start...");
            int p = (PROBLEM <= 0) ? -1 : PROBLEM;
            while (p == -1)
                p = ParseInput(System.Console.ReadLine(), _problems.Length);

            WriteLine("Input max generations...");
            int m = (GENERATIONS <= 0) ? -1 : GENERATIONS;
            while (m == -1)
                m = ParseInput(System.Console.ReadLine(), 9999);

            return new Algorithm(_problems[p - 1], m);
        }

        private static void WriteLine(string text)
        {
            System.Console.WriteLine(text);
        }

        private static int ParseInput(string input, int max)
        {
            int m = Math.Min(input.Length, max.ToString().Length);
            int o = -1;
            for (int i = m; i > 0; i--)
            {
                if (int.TryParse(input.Substring(0, i), out o))
                    break;
            }

            return (o > max) ? -1 : o;
        }
    }
}