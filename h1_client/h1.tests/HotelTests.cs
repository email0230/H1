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
            // Arrange
            List<int> guests = new List<int> { 1, 2, 3, 4 };

            // Act
            List<Tuple<int, int>> result = SolutionInputBuilder.FindAllPairs(guests);

            // Assert
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
        public void tt()
        {
            // Arrange
            List<Room> rooms = new List<Room>
            {
                new Room { Capacity = 2 },
                new Room { Capacity = 3 },
                // Add more rooms as needed
            };

            // Act
            string result = SolutionInputBuilder.BuildRooms(rooms);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("room(1,2)", result);
            Assert.Contains("room(2,3)", result);
            // Add more assertions based on your expected output
        }


    }
}