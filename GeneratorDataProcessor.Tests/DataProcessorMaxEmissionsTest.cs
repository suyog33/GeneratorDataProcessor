using GeneratorDataProcessor.Core.Entities;
using GeneratorDataProcessor.Core.Interfaces;
using GeneratorDataProcessor.Core.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace GeneratorDataProcessor.Tests
{
    public class DataProcessorMaxEmissionTests
    {
        private readonly Mock<IGeneratorRepository> mockRepository;
        private readonly DataProcessor dataProcessor;
        private DateTimeOffset date = DateTimeOffset.UtcNow;
        public DataProcessorMaxEmissionTests()
        {
            mockRepository = new Mock<IGeneratorRepository>();
            var mockConfig = new Mock<IConfiguration>();
            dataProcessor = new DataProcessor(mockRepository.Object, mockConfig.Object);
        }

        [Fact]
        public void CalculateMaxEmissions_ShouldReturnHighestPerDay()
        {
            // Arrange
            var generators = new List<Generator>
            {
                new GasGenerator
                {
                    Name = "GasGen1",
                    EmissionsRating = 0.05,
                    Days = new List<DayGeneration>
                    {
                        new DayGeneration { Date = date, Energy = 100 }
                    }
                },
                new CoalGenerator
                {
                    Name = "CoalGen1",
                    EmissionsRating = 0.1,
                    Days = new List<DayGeneration>
                    {
                        new DayGeneration { Date = date, Energy = 200 }
                    }
                }
            };

            // Act
            var result = dataProcessor.CalculateMaxEmissions(generators);

            // Assert
            Assert.Single(result);
            Assert.Equal("CoalGen1", result.First().GeneratorName);
        }

        [Fact]
        public void CalculateMaxEmissions_SingleGeneratorSingleDay_ShouldReturnCorrectMax()
        {
            // Arrange
            var generators = new List<Generator>
            {
                new GasGenerator
                {
                    Name = "GasGen1",
                    EmissionsRating = 0.05,
                    Days = new List<DayGeneration>
                    {
                        new DayGeneration { Date = date, Energy = 100 }
                    }
                }
            };

            // Act
            var result = dataProcessor.CalculateMaxEmissions(generators);

            // Assert
            Assert.Single(result);
            Assert.Equal("GasGen1", result.First().GeneratorName);
            Assert.Equal(5, result.First().EmissionValue); // 100 * 0.05 = 5
        }

        [Fact]
        public void CalculateMaxEmissions_MultipleGeneratorsSameDay_ShouldReturnHighest()
        {
            // Arrange
            var generators = new List<Generator>
            {
                new GasGenerator
                {
                    Name = "GasGen1",
                    EmissionsRating = 0.05,
                    Days = new List<DayGeneration>
                    {
                        new DayGeneration { Date = date, Energy = 100 }
                    }
                },
                new CoalGenerator
                {
                    Name = "CoalGen1",
                    EmissionsRating = 0.1,
                    Days = new List<DayGeneration>
                    {
                        new DayGeneration { Date = date, Energy = 200 }
                    }
                }
            };

            // Act
            var result = dataProcessor.CalculateMaxEmissions(generators);

            // Assert
            Assert.Single(result);
            Assert.Equal("CoalGen1", result.First().GeneratorName);
            Assert.Equal(20, result.First().EmissionValue); // 200 * 0.1 = 20
        }

        [Fact]
        public void CalculateMaxEmissions_MultipleDays_ShouldReturnMaxForEachDay()
        {
            // Arrange
            var date = DateTimeOffset.UtcNow;
            var generators = new List<Generator>
            {
                new GasGenerator
                {
                    Name = "GasGen1",
                    EmissionsRating = 0.05,
                    Days = new List<DayGeneration>
                    {
                        new DayGeneration { Date = date, Energy = 100 },
                        new DayGeneration { Date = date.AddDays(1), Energy = 50 }
                    }
                },
                new CoalGenerator
                {
                    Name = "CoalGen1",
                    EmissionsRating = 0.1,
                    Days = new List<DayGeneration>
                    {
                        new DayGeneration { Date = date, Energy = 200 },
                        new DayGeneration { Date = date.AddDays(1), Energy = 300 }
                    }
                }
            };

            // Act
            var result = dataProcessor.CalculateMaxEmissions(generators);

            // Assert
            Assert.Equal(2, result.Count()); // Two days
            Assert.Equal("CoalGen1", result.First().GeneratorName);
            Assert.Equal(20, result.First().EmissionValue); // 200 * 0.1
            Assert.Equal("CoalGen1", result.Last().GeneratorName);
            Assert.Equal(30, result.Last().EmissionValue); // 300 * 0.1
        }

        [Fact]
        public void CalculateMaxEmissions_NoGeneratorsWithEmissions_ShouldReturnEmpty()
        {
            // Arrange
            var generators = new List<Generator>
            {
                new WindGenerator
                {
                    Name = "WindGen1",
                    Days = new List<DayGeneration>
                    {
                        new DayGeneration { Date = date, Energy = 100 }
                    }
                }
            };

            // Act
            var result = dataProcessor.CalculateMaxEmissions(generators);

            // Assert
            Assert.Empty(result);
        }
    }
}