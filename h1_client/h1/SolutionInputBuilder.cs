using h1.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace h1
{
    internal class SolutionInputBuilder
    {
        #region Constants
        private static readonly string PROJECT_ROOT = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..");
        private static readonly string MODEL_FILEPATH = Path.Combine(PROJECT_ROOT, "Assets", "Text", "dlvModel.txt");
        private static readonly int TOGETHER_RULE_WEIGHT = 100;
        private static readonly int NO_TOGETHER_RULE_WEIGHT = 1;
        #endregion

        private Dictionary<Guest, int> GuestDict;

        public Dictionary<Guest, int> GetGuestDict()
        {
            return GuestDict;
        }

        #region group handling
        public string ParseGroupsToString(ObservableCollection<Group> inputGroupCollection)
        {
            StringBuilder output = new StringBuilder();
            output.AppendLine("% AUTOMATICALLY GENERATED | GUEST GROUPS\n");

            // Append the outputs directly to the StringBuilder
            output.AppendLine(BuildGroups(inputGroupCollection));
            output.AppendLine(BuildGroupProps(inputGroupCollection));
            output.AppendLine(BuildTogetherStatements(inputGroupCollection));

            return output.ToString();
        }

        public string BuildGroups(ObservableCollection<Group> inputGroupCollection)
        {
            StringBuilder output = new StringBuilder();
            output.AppendLine("% Groups:");

            byte counter = 0;
            byte guestCounter = 0;
            GuestDict = new Dictionary<Guest, int>();

            foreach (Group group in inputGroupCollection)
            {
                counter++;
                group.GroupName = counter.ToString();

                for (int i = 0; i < group.Guests.Count; i++)
                {
                    guestCounter++;
                    Guest currentGuest = group.Guests[i];
                    GuestDict.Add(currentGuest, guestCounter);

                    output.AppendLine($"group({group.GroupName}, {guestCounter}).");
                }
            }

            return output.ToString();
        }

        public static string BuildGroupProps(ObservableCollection<Group> inputGroupCollection)
        {
            StringBuilder output = new StringBuilder();
            output.AppendLine("% Group Properties:");

            foreach (Group group in inputGroupCollection)
            {
                AppendGroupPropertyIfTrue(group.WantNoiseReduction, $"want_prop1({group.GroupName}).", output);
                AppendGroupPropertyIfTrue(group.WantSecurityFeatures, $"want_prop2({group.GroupName}).", output);
                AppendGroupPropertyIfTrue(group.WantSmartLighting, $"want_prop3({group.GroupName}).", output);
                AppendGroupPropertyIfTrue(group.WantBalcony, $"want_prop4({group.GroupName}).", output);
                AppendGroupPropertyIfTrue(group.WantModularFurniture, $"want_prop5({group.GroupName}).", output);
            }

            return output.ToString();
        }

        public static void AppendGroupPropertyIfTrue(bool condition, string property, StringBuilder output)
        {
            if (condition)
            {
                output.AppendLine(property);
            }
        }

        public string BuildTogetherStatements(ObservableCollection<Group> inputGroupCollection)
        {
            StringBuilder output = new StringBuilder();
            output.AppendLine("% Together statements:");

            foreach ( Group group in inputGroupCollection)
            {
                List<int> guestNumbers = GetGuestNumbers(group);

                List<Tuple<int, int>> pairsForThisGroup = FindAllPairs(guestNumbers);

                foreach (Tuple<int, int> pair in pairsForThisGroup)
                {
                    output.AppendLine($":~ not together({pair.Item1}, {pair.Item2}). [{DetermineWeight(group)}]");
                }
            }
            return output.ToString();
        }

        public List<int> GetGuestNumbers(Group group)
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

        public static List<Tuple<int, int>> FindAllPairs(List<int> guests)
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

        public static int DetermineWeight(Group group) => group.WantGroupToStayTogether ? TOGETHER_RULE_WEIGHT : NO_TOGETHER_RULE_WEIGHT;

        #endregion

        #region room handling
        public static string ParseRoomsToString(List<Room> rooms)
        {

            //RemoveOccupiedRooms();

            StringBuilder output = new StringBuilder();
            output.AppendLine("% AUTOMATICALLY GENERATED | ROOMS\n");

            output.AppendLine(BuildRooms(rooms));
            output.AppendLine(BuildRoomProps(rooms));

            return output.ToString();
        }

        public static void RemoveOccupiedRooms()
        {
            throw new NotImplementedException();
        }

        public static string BuildRoomProps(List<Room> rooms)
        {
            StringBuilder output = new StringBuilder();
            foreach (var room in rooms)
            {
                if (room.IsRoomOccupied())
                {
                    continue;
                }

                AppendRoomProperty(output, room.Id, room.NoiseReduction, "prop1");
                AppendRoomProperty(output, room.Id, room.SecurityFeatures, "prop2");
                AppendRoomProperty(output, room.Id, room.SmartLighting, "prop3");
                AppendRoomProperty(output, room.Id, room.Balcony, "prop4");
                AppendRoomProperty(output, room.Id, room.ModularFurniture, "prop5");
            }

            return output.ToString();
        }

        public static void AppendRoomProperty(StringBuilder output, int roomId, bool hasProperty, string propName)
        {
            if (hasProperty)
            {
                output.AppendLine($"has_{propName}({roomId}).");
            }
        }

        public static string BuildRooms(List<Room> rooms)
        {
            StringBuilder output = new StringBuilder();
            foreach (var room in rooms)
            {
                if (!room.IsRoomOccupied())
                {
                    output.AppendLine($"room({room.Id},{room.Capacity})."); //roomid might need to get normalized
                }
               
            }

            return output.ToString();
        }

        //private static bool RoomIsOccupied(Room room)
        //{
        //    if (room.Guests.Count != 0)
        //    {
        //        return false;
        //    }
        //    return true;
        //}
        #endregion

        public string GenerateQuery(ObservableCollection<Group> input, List<Room> rooms)
        {
            string groupsString = ParseGroupsToString(input);
            string roomsString = ParseRoomsToString(rooms);
            string modelString = File.ReadAllText(MODEL_FILEPATH);

            return $"{groupsString}\n\n\n{roomsString}\n\n\n{modelString}";
        }
    }
}
