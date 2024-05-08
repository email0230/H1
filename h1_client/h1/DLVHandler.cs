
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace h1
{
    public static class DLVHandler
    {
        #region Constants
        private static readonly string QUERY_FILEPATH = @"dlvStuff/assets/query.dl";
        private static readonly string SOLVER_FILEPATH = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"../", @"../", @"../", @"Assets\", "Solver", "dlv-2.1.2-win64.exe"));
        private static readonly string FULL_SOLVER_FILEPATH = @"C:\Users\email0230\Documents\GitHub\h1\H1\h1_client\h1\Assets\Solver\dlv-2.1.2-win64.exe"; //todo: consider removing this one
        private static readonly string SOLUTION_FILEPATH = @"dlvStuff/target/solver_output.txt";
        #endregion

        private static void CheckIfFileExists(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        public static List<Tuple<int, int>> GetSolutionFromSolver(string query)
        {
            CheckIfFileExists(QUERY_FILEPATH);

            //save query to disk so dlv can read it
            File.WriteAllText(QUERY_FILEPATH, query);

            string cmd_args = $"--filter=guest_in_room/2 {QUERY_FILEPATH}";
            StartProcess(SOLVER_FILEPATH, cmd_args, SOLUTION_FILEPATH);

            return InterpretSolution(SOLUTION_FILEPATH);
        }

        private static List<Tuple<int, int>> InterpretSolution(string path)
        {
            var pairs = new List<Tuple<int, int>>();
            var input = File.ReadAllText(path);

            //regex: get stuff from between curly braces
            Match match = Regex.Match(input, @"\{([^}]*)\}");

            if (match.Success)
            {
                //use the first regex expression
                string contentWithinBraces = match.Groups[1].Value;

                //regex: get stuff from between brackets
                MatchCollection numberMatches = Regex.Matches(contentWithinBraces, @"\((\d+),(\d+)\)");

                foreach (Match numberMatch in numberMatches)
                {
                    int firstNumber = int.Parse(numberMatch.Groups[1].Value);
                    int secondNumber = int.Parse(numberMatch.Groups[2].Value);
                    pairs.Add(new Tuple<int, int>(firstNumber, secondNumber));
                }
            }
            else
            {
                Debug.WriteLine("No match found within curly braces.");
            }

            return pairs;
        }

        static void StartProcess(string executablePath, string args, string outputPath) //can be improved tremenodus
        {
            try
            {

                DebugPaths(executablePath, outputPath);
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = executablePath,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };

                using (Process process = new Process())
                {
                    process.StartInfo = processStartInfo;
                    process.Start();

                    using (StreamWriter writer = new StreamWriter(outputPath))
                    {
                        while (!process.StandardOutput.EndOfStream)
                        {
                            string line = process.StandardOutput.ReadLine();
                            writer.WriteLine(line);
                        }
                        writer.Close();
                    }

                    process.WaitForExit();

                    //for debugging
                    int exitCode = process.ExitCode;
                    Debug.WriteLine($"Process exited with code {exitCode}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

        private static void DebugPaths(string executablePath, string outputPath)
        {
            Debug.WriteLine("PATHS AS SEEN IN THE PROCESS RUNNING DLV:");
            Debug.Indent();
            Debug.WriteLine($"exe: {executablePath}");
            Debug.WriteLine($"output: {outputPath}");
        }
    }
}
