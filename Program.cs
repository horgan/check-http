using System;
using System.Net;
using System.Threading;

// Usage: CheckHTTP.exe <arg1> <arg2> <argN>
// Free to use by anyone, anywhere, anytime.
// github.com/horgan | September 2019

namespace CheckHTTP
{
    class Program
    {
        static void Main(string[] args)
        {
            const int TenMinutes = 600000;
            const int OneMinute = 60000;
            int successes = 0;
            int failures = 0;
            bool failureMode = false;

            #region ConsolePrep
            Console.ResetColor();
            Console.Clear();
            Console.Title = "Initializing...";

            if (args == null || args.Length == 0)
            {
                Console.Title = "Error: no arguments specified";
                Console.WriteLine("Usage:\n");
                Console.WriteLine("1) CheckHTTP.exe https://goo.gl");
                Console.WriteLine(@"2) CheckHTTP.exe https://goo.gl https://twitter.com ");
                Console.WriteLine("\nPress any key to exit...");
                Console.Read();
                System.Environment.Exit(1);
            }
            #endregion

            while (true)
            {
                foreach (var arg in args)
                {
                    try
                    {
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(arg);
                        request.Timeout = 5000;
                        request.AllowAutoRedirect = true;
                        using (HttpWebResponse reponse = (HttpWebResponse)request.GetResponse())
                        {
                            if (reponse.StatusCode == HttpStatusCode.OK)
                            {
                                successes += 1;
                                failureMode = false;
                                Console.ForegroundColor = ConsoleColor.DarkGreen;
                                Console.WriteLine($"{DateTime.Now.ToString()} - reachable - {arg}");
                            }
                            else
                            {
                                failures += 1;
                                failureMode = true;
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                Console.WriteLine($"{DateTime.Now.ToString()} - unreachable - {arg}");
                                System.Media.SystemSounds.Asterisk.Play();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        failures += 1;
                        failureMode = true;
                        Console.WriteLine(e.Message + " - " + arg);
                        System.Media.SystemSounds.Asterisk.Play();
                    }
                    finally
                    {
                        if (failureMode)
                        {
                            Console.Title = "Failure mode [Interval: 1 minute, successes: " + successes + ", failures: " + failures + "]";
                        }
                        else
                        {
                            Console.Title = "Normal operation [Interval: 10 minutes, successes: " + successes + ", failures: " + failures + "]";
                        }
                        Console.ResetColor();
                    }
                }
                if (failureMode)
                {
                    Thread.Sleep(OneMinute);
                }
                else
                {
                    Thread.Sleep(TenMinutes);
                }
            }
        }
    }
}
