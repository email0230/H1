using h1;
using h1.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace h1.tests
{
    public class HotelTests
    {
        [Fact]
        public void FindAllPairs_ShouldReturnCorrectPairs()
        {
            List<int> guests = new List<int> { 1, 2, 3, 4 };

            List<Tuple<int, int>> result = SolutionInputBuilder.FindAllPairs(guests);

            Assert.NotNull(result);
            Assert.Equal(6, result.Count); // n * (n - 1) / 2 pairs for n elements
            Assert.Contains(new Tuple<int, int>(1, 2), result);
            Assert.Contains(new Tuple<int, int>(1, 3), result);
            Assert.Contains(new Tuple<int, int>(1, 4), result);
            Assert.Contains(new Tuple<int, int>(2, 3), result);
            Assert.Contains(new Tuple<int, int>(2, 4), result);
            Assert.Contains(new Tuple<int, int>(3, 4), result);
        }

        [Fact]
        public void BuildRooms_ShouldReturnRooms()
        {
            List<Room> rooms = new List<Room>
            {
                new Room { Capacity = 2 },
                new Room { Capacity = 3 },
            };

            string result = SolutionInputBuilder.BuildRooms(rooms);

            Assert.NotNull(result);
            Assert.Contains("room(1,2)", result);
            Assert.Contains("room(2,3)", result);
        }
    }
}