using System;
using System.ComponentModel;

namespace INTEL
{
    static class Console
    {
        static BackgroundWorker _bgw = new BackgroundWorker();
        static DataVisual _dv;

        public static void Initialize(DataVisual dv)
        {
            _dv = dv;

            _bgw.DoWork += _bgw_DoWork;
            _bgw.RunWorkerAsync();
        }

        private static void _bgw_DoWork(object sender, DoWorkEventArgs e)
        {
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

            Problem p = new Problem();
            int max = 100;

            Algorithm a = new Algorithm();
            a.Run(p, max);
        }
    }
}
