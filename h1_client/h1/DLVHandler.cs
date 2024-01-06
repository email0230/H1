using h1.Models;
using MongoDB.Bson.IO;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace h1
{
    public static class DLVHandler
    {
        #region Constants
        private static readonly string GROUPS_FILEPATH = @"dlvStuff/assets/groups.txt";
        private static readonly string ROOMS_FILEPATH = @"dlvStuff/assets/rooms.txt";
        private static readonly string MODEL_FILEPATH = @"dlvStuff/assets/model.txt";
        private static readonly string QUERY_FILEPATH = @"dlvStuff/assets/query.dl";
        private static readonly string SOLVER_FILEPATH = @"dlvStuff/solver/dlv-2.1.2-win64.exe";
        private static readonly string SOLUTION_FILEPATH = @"dlvStuff/target/solver_output.txt";
        private static readonly int TOGETHER_RULE_WEIGHT = 100;
        private static readonly int NO_TOGETHER_RULE_WEIGHT = 1;
        #endregion
        private static Dictionary<Guest, int> GuestDict { get; set; }

        private static void CheckIfFileExists(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        #region group handling
        private static void ParseGroupsToString(ObservableCollection<Models.Group> inputGroupCollection, string path)
        {
            CheckIfFileExists(path);

            using (StreamWriter writer = new StreamWriter(path, true, Encoding.UTF8)) //might need to use the simpler version to reduce indentation
            {
                writer.WriteLine("% AUTOMATICALLY GENERATED | GUEST GROUPS\n");

                BuildGroups(inputGroupCollection, writer);
                BuildGroupProps(inputGroupCollection, writer);
                BuildTogetherStatements(inputGroupCollection, writer);

                writer.Close();
            }
        }

        private static void BuildGroups(ObservableCollection<Models.Group> inputGroupCollection, StreamWriter writer)
        {
            writer.WriteLine("% Groups:");

            byte counter = 0;
            byte guestCounter = 0;
            GuestDict = new Dictionary<Guest, int>(); //might need to move up a scope to be visible to the other method...
            foreach (Models.Group group in inputGroupCollection)
            {
                counter++;
                group.GroupName = counter.ToString();

                for (int i = 0; i < group.Guests.Count; i++)
                {
                    guestCounter++;
                    //get i'th guest
                    Guest currentGuest = group.Guests[i];
                    GuestDict.Add(currentGuest, guestCounter);

                    writer.WriteLine($"group({group.GroupName}, {guestCounter}).");
                }
            }
        }

        private static void BuildGroupProps(ObservableCollection<Models.Group> inputGroupCollection, StreamWriter writer)
        {
            writer.WriteLine("% Group Properties:");
            foreach (Models.Group group in inputGroupCollection)
            {
                WriteGroupPropertyIfTrue(group.WantNoiseReduction, $"want_prop1({group.GroupName}).", writer);
                WriteGroupPropertyIfTrue(group.WantSecurityFeatures, $"want_prop2({group.GroupName}).", writer);
                WriteGroupPropertyIfTrue(group.WantSmartLighting, $"want_prop3({group.GroupName}).", writer);
                WriteGroupPropertyIfTrue(group.WantBalcony, $"want_prop4({group.GroupName}).", writer);
                WriteGroupPropertyIfTrue(group.WantModularFurniture, $"want_prop5({group.GroupName}).", writer);
            }
        }

        private static void WriteGroupPropertyIfTrue(bool condition, string property, StreamWriter writer)
        {
            if (condition)
            {
                writer.WriteLine(property);
            }
        }

        private static void BuildTogetherStatements(ObservableCollection<Models.Group> inputGroupCollection, StreamWriter writer)
        {
            writer.WriteLine("% Together statements:");

            foreach (Models.Group group in inputGroupCollection)
            {
                List<int> guestNumbers = GetGuestNumbers(group);

                List<Tuple<int, int>> pairsForThisGroup = FindAllPairs(guestNumbers);

                foreach (Tuple<int, int> pair in pairsForThisGroup)
                {
                    writer.WriteLine($":~ not together({pair.Item1}, {pair.Item2}). [{DetermineWeight(group)}]");
                }
            }
        }

        private static int DetermineWeight(Models.Group group) => group.WantGroupToStayTogether ? TOGETHER_RULE_WEIGHT : NO_TOGETHER_RULE_WEIGHT;

        private static List<int> GetGuestNumbers(Models.Group group)
        {
            List<int> numbers = new List<int>();

            foreach (Guest guest in group.Guests)
            {
                if (GuestDict.TryGetValue(guest, out int guestNumber))
                {
                    numbers.Add(guestNumber);
                }
            }
            return numbers;
        }

        private static List<Tuple<int, int>> FindAllPairs(List<int> guests)
        {
            List<Tuple<int, int>> pairs = new List<Tuple<int, int>>();

            for (int i = 0; i < guests.Count - 1; i++)
            {
                for (int j = i + 1; j < guests.Count; j++)
                {
                    pairs.Add(new Tuple<int, int>(guests[i], guests[j]));
                }
            }
            return pairs;
        }

        #endregion

        #region room handling
        private static void ParseRoomsToString(string path)
        {
            Hotel hotel = Hotel.GetInstance();
            List<Room> rooms = hotel.Rooms;

            CheckIfFileExists(path);

            using (StreamWriter writer = new StreamWriter(path, true, Encoding.UTF8))
            {
                writer.WriteLine("% AUTOMATICALLY GENERATED | ROOMS\n");

                BuildRooms(rooms, writer); //hehe
                BuildRoomProps(rooms, writer);

                writer.Close();
            }
        }

        private static void BuildRoomProps(List<Room> rooms, StreamWriter writer)
        {
            foreach (var room in rooms)
            {
                WriteRoomProperty(writer, room.Id, room.NoiseReduction, "prop1");
                WriteRoomProperty(writer, room.Id, room.SecurityFeatures, "prop2");
                WriteRoomProperty(writer, room.Id, room.SmartLighting, "prop3");
                WriteRoomProperty(writer, room.Id, room.Balcony, "prop4");
                WriteRoomProperty(writer, room.Id, room.ModularFurniture, "prop5");
            }
        }

        private static void WriteRoomProperty(StreamWriter writer, int roomId, bool hasProperty, string propName)
        {
            if (hasProperty)
            {
                writer.WriteLine($"has_{propName}({roomId}).");
            }
        }


        private static void BuildRooms(List<Room> rooms, StreamWriter writer)
        {
            foreach (var room in rooms)
            {
                writer.WriteLine($"room({room.Id},{room.Capacity})."); //roomid might need to get normalized
            }
        } 
        #endregion

        public static string GenerateQuery(ObservableCollection<Models.Group> input)
        {
            ParseGroupsToString(input, GROUPS_FILEPATH);
            ParseRoomsToString(ROOMS_FILEPATH);
            string groupsString = File.ReadAllText(GROUPS_FILEPATH);
            string roomsString = File.ReadAllText(ROOMS_FILEPATH);
            string modelString = File.ReadAllText(MODEL_FILEPATH);
            string query = $"{groupsString}\n\n{roomsString}\n\n{modelString}";
           
            File.WriteAllText(QUERY_FILEPATH, query); //get the file to the solver folder for easy access
            return query;
        }

        public static string GetSolutionFromSolver(string query)
        {
            string cmd_args = $"--filter=guest_in_room/2 {QUERY_FILEPATH}";

            StartProcess(SOLVER_FILEPATH, cmd_args, SOLUTION_FILEPATH);
            List<Tuple<int, int>> assignments = InterpretSolution(SOLUTION_FILEPATH);
            //this list, along with the guest dict can be passed to a different class that can assign these guests as a part of transaction.

            return "xd";
        }

        private static List<Tuple<int, int>> InterpretSolution(string path)
        {
            var pairs = new List<Tuple<int, int>>();

            var input = File.ReadAllText(path);

            //regex: get stuff from between curly braces
            Match match = Regex.Match(input, @"\{([^}]*)\}");

            if (match.Success)
            {
                // Extract the content within standard brackets from the curly braces content
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

        //silly todo: put the model somewhere where git will find it, same with the solver

        static void StartProcess(string executablePath, string args, string outputPath) //can be improved tremenodus
        {
            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = executablePath,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };

                // Create a new process and start it
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

                    // Wait for the process to exit
                    process.WaitForExit();

                    //this one might need to go
                    int exitCode = process.ExitCode;
                    Debug.WriteLine($"Process exited with code {exitCode}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
