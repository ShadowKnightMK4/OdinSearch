using OdinSearchEngine;

namespace VirusTotalDemo
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            TotalDemo testrun = new();
            testrun.AddSearchFolder(new string[] {"C:\\Euphoria\\bin" });
            testrun.AddSearchTargetDefaults();
            testrun.Begin();

            Thread.Sleep(500);
            again:
            if (testrun.IsSearching())
            {
                Thread.Sleep(2000);
                goto again;
            }
            return;
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}