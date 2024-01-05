using h1.Models;
using MongoDB.Bson.IO;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        #region group handling
        private static string ParseGroupsToString(ObservableCollection<Group> inputGroupCollection, string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            using (StreamWriter writer = new StreamWriter(path, true, Encoding.UTF8)) //might need to use the simpler version to reduce indentation
            {
                BuildGroups(inputGroupCollection, writer);
                BuildGroupProps(inputGroupCollection, writer);
                //BuildTogetherStatements(inputGroupCollection, writer);
                writer.Close();
            }
            return null;
        }

        private static void BuildGroups(ObservableCollection<Group> inputGroupCollection, StreamWriter writer)
        {

            uint a;
            byte counter = 0;
            byte guestCounter = 0;
            foreach (Group group in inputGroupCollection)
            {
                counter++;
                group.GroupName = counter.ToString();

                for (int i = 0; i < group.Guests.Count; i++)
                {
                    writer.WriteLine($"group({group.GroupName}, {++guestCounter}).");
                }
            }
        }

        private static void BuildGroupProps(ObservableCollection<Group> inputGroupCollection, StreamWriter writer)
        {
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
            foreach (Group group in inputGroupCollection)
            {
                //List<Tuple<string, string>> pairs = FindAllPairs(group.Guests);
            }
        }

        #endregion

        private static string ParseRoomsToString(string path)
        {
            Hotel hotel = Hotel.GetInstance();
            List<Room> rooms = hotel.Rooms;

            using (StreamWriter writer = new StreamWriter(path, true, Encoding.UTF8))
            {
                foreach (var room in rooms)
                {
                    writer.WriteLine($"room({room.Id},{room.Capacity})."); //roomid might need to get normalized
                }
            }

            return "haha";
        }
        public static string GenerateQuery(ObservableCollection<Group> input)
        {
            const string filepath = @"dlvStuff/target/groups.txt"; //this one might still be broken
            const string filepath2 = @"dlvStuff/target/rooms.txt";
            const string filepath3 = @"dlvStuff/target/model.txt"; //as of 05/01 doesnt exist yet

            string groupsString = ParseGroupsToString(input, filepath);
            string roomsString = ParseRoomsToString();

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
