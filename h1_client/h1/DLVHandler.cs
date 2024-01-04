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
        private static string ParseGroupsToString(ObservableCollection<Group> inputGroupCollection, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath, true, Encoding.UTF8)) //might need to use the simpler version to reduce indentation
            {
                foreach (Group group in inputGroupCollection)
                {
                    // Append a line for each group
                    string line = $"placeholder"; // Customize this line as needed
                    writer.WriteLine(line);
                }
            }
            return null;
        }
        private static string ParseRoomsToString()
        {
            Hotel hotel = Hotel.GetInstance();
            List<Room> rooms = hotel.Rooms;

            return "haha";
        }
        public static string GenerateQuery(ObservableCollection<Group> input)
        {
            const string filepath = "dlvStuff/target/groups.txt";

            string groupsString = ParseGroupsToString(input, filepath);
            string roomsString = ParseRoomsToString();
            return "haha";
        }

        public static string GetSolutionFromSolver(string query)
        {
            //call ".\dlv-2.1.2-win64.exe --filter=guest_in_room/2 hotel3.dl" here! (but better)
            return "haha";
        }
    }
}
