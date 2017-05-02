using System;
using System.ComponentModel;

namespace INTEL
{
    static class Console
    {
        static BackgroundWorker _bgw = new BackgroundWorker();
        static DataVisual _dv;
        static ProblemFactory[] _problems = new ProblemFactory[] { new XORProblemFactory() };

        const int PROBLEM = 1;      //set to 0 to allow choice..
        const int GENERATIONS = 2;  //set to 0 to allow choice..

        public static void Initialize(DataVisual dv)
        {
            _dv = dv;

            _bgw.DoWork += _bgw_DoWork;
            _bgw.RunWorkerAsync();
        }

        private static void _bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            Run();
        }

        private static void Run()
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

            Algorithm a = new Algorithm(_problems[p - 1]);
            a.Run(m);
        }

        public static void WriteLine(string text)
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
