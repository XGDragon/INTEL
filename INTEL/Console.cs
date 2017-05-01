using System;
using System.ComponentModel;

namespace INTEL
{
    static class Console
    {
        static BackgroundWorker _bgw = new BackgroundWorker();
        static DataVisual _dv;
        static Problem[] _problems = new Problem[] { new XORProblem() };

        const int PROBLEM = 1;
        const int GENERATIONS = 2;

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
            System.Console.WriteLine(thisDay + " (UTC)");
            System.Console.WriteLine("INTEL NeuralNetwork for IPD v0.1, by Stan Noordman at Utrecht University");
            for (int i = 0; i < Parameter.List.Count; i++)
            {
                if (Parameter.ListDecorator.ContainsKey(i))
                    System.Console.WriteLine(Parameter.ListDecorator[i]);
                System.Console.WriteLine(i + ": " + Parameter.List[i].ToString() + " = " + Parameter.List[i].Value.ToString() + Parameter.List[i].Tooltip);
            }

            System.Console.WriteLine("\nSelect problem:\n");
            for (int i = 0; i < _problems.Length; i++)
                System.Console.WriteLine("<" + (i + 1) + "> " + _problems[i].Name);

            System.Console.WriteLine("Input problem ID to start...");
            int p = (PROBLEM == 0) ? -1 : PROBLEM;
            while (p == -1)
                p = ParseInput(System.Console.ReadLine(), _problems.Length);
            _problems[p - 1].Initialize();

            System.Console.WriteLine("Input max generations...");
            int m = (GENERATIONS == 0) ? -1 : GENERATIONS;
            while (m == -1)
                m = ParseInput(System.Console.ReadLine(), 9999);

            Algorithm a = new Algorithm();
            a.Run(_problems[p - 1], m);
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
