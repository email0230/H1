using h1.Models;
using MongoDB.Bson.IO;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace h1
{
    public static class DLVHandler
    {
        //TODO:
        /*give groups numbers
         * give guests numbers
         * 
         * 
         * 
         * 
         * 
         * 
         */

        private static Dictionary<Guest,int> GuestDict { get; set; }
        
        private static readonly string GROUPS_FILEPATH = @"dlvStuff/target/groups.txt";
        private static readonly string ROOMS_FILEPATH = @"dlvStuff/target/rooms.txt";
        private static readonly string MODEL_FILEPATH = @"dlvStuff/target/model.txt";
        private static readonly int TOGETHER_RULE_WEIGHT = 100;
        private static readonly int NO_TOGETHER_RULE_WEIGHT = 1;


        #region group handling
        private static string ParseGroupsToString(ObservableCollection<Group> inputGroupCollection, string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            using (StreamWriter writer = new StreamWriter(path, true, Encoding.UTF8)) //might need to use the simpler version to reduce indentation
            {
                writer.WriteLine("% AUTOMATICALLY GENERATED\n");

                BuildGroups(inputGroupCollection, writer);
                BuildGroupProps(inputGroupCollection, writer);
                BuildTogetherStatements(inputGroupCollection, writer);

                writer.Close();
            }
            return null;
        }

        private static void BuildGroups(ObservableCollection<Group> inputGroupCollection, StreamWriter writer)
        {
            writer.WriteLine("% Groups:");

            byte counter = 0;
            byte guestCounter = 0;
            GuestDict = new Dictionary<Guest, int>(); //might need to move up a scope to be visible to the other method...
            foreach (Group group in inputGroupCollection)
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

        private static void BuildGroupProps(ObservableCollection<Group> inputGroupCollection, StreamWriter writer)
        {
            writer.WriteLine("% Group Properties:");
            foreach (Group group in inputGroupCollection)
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

        private static void BuildTogetherStatements(ObservableCollection<Group> inputGroupCollection, StreamWriter writer)
        {
            writer.WriteLine("% Together statements:");

            foreach (Group group in inputGroupCollection)
            {
                List<int> guestNumbers = GetGuestNumbers(group);

                List<Tuple<int, int>> pairsForThisGroup = FindAllPairs(guestNumbers);

                foreach (Tuple<int, int> pair in pairsForThisGroup)
                {
                    writer.WriteLine($":~ not together({pair.Item1}, {pair.Item2}). [{DetermineWeight(group)}]");
                }
            }
        }

        private static int DetermineWeight(Group group) => group.WantGroupToStayTogether ? TOGETHER_RULE_WEIGHT : NO_TOGETHER_RULE_WEIGHT;

        private static List<int> GetGuestNumbers(Group group)
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

        private static string ParseRoomsToString(string path)
        {
            Hotel hotel = Hotel.GetInstance();
            List<Room> rooms = hotel.Rooms;

            using (StreamWriter writer = new StreamWriter(path, true, Encoding.UTF8))
            {
                //INTRO

                //ROOMS

                //ROOM PROPS


                foreach (var room in rooms)
                {
                    writer.WriteLine($"room({room.Id},{room.Capacity})."); //roomid might need to get normalized
                }
            }

            return "haha";
        }
        public static string GenerateQuery(ObservableCollection<Group> input)
        {
            
            string groupsString = ParseGroupsToString(input, GROUPS_FILEPATH);
            string roomsString = ParseRoomsToString(ROOMS_FILEPATH);

            //include the model string here!
            return "haha";
        }

        public static string GetSolutionFromSolver(string query)
        {
            //call ".\dlv-2.1.2-win64.exe --filter=guest_in_room/2 hotel3.dl" here! (but better)
            return "haha";
        }
    }
}
