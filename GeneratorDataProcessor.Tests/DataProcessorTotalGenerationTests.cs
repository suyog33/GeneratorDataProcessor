using GeneratorDataProcessor.Core.Entities;
using GeneratorDataProcessor.Core.Interfaces;
using GeneratorDataProcessor.Core.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace GeneratorDataProcessor.Tests
{
    public class DataProcessorTotalGenerationTests
    {
        private readonly Mock<IGeneratorRepository> mockRepository;
        private readonly DataProcessor dataProcessor;
        private DateTimeOffset date = DateTimeOffset.UtcNow;
        public DataProcessorTotalGenerationTests()
        {
            mockRepository = new Mock<IGeneratorRepository>();
            var mockConfig = new Mock<IConfiguration>();
            dataProcessor = new DataProcessor(mockRepository.Object, mockConfig.Object);
        }

        [Fact]
        public void CalculateTotalGeneration_ShouldReturnCorrectValues()
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
                        new DayGeneration { Date = date, Energy = 100, Price = 5 }
                    }
                },
                new CoalGenerator
                {
                    Name = "CoalGen1",
                    EmissionsRating = 0.1,
                    TotalHeatInput = 500,
                    ActualNetGeneration = 250,
                    Days = new List<DayGeneration>
                    {
                        new DayGeneration { Date = date, Energy = 150, Price = 6 }
                    }
                }
            };

            // Act
            var result = dataProcessor.CalculateTotalGeneration(generators);

            // Assert
            Assert.Equal(500, result.First().TotalGenerationValue);
            Assert.Equal(900, result.Last().TotalGenerationValue);
        }

        [Fact]
        public void CalculateTotalGeneration_SingleGeneratorSingleDay_ShouldReturnCorrectTotal()
        {
            // Arrange
            var generators = new List<Generator>
            {
                new GasGenerator
                {
                    Name = "GasGen1",
                    Days = new List<DayGeneration>
                    {
                        new DayGeneration { Date = date, Energy = 100, Price = 5 }
                    }
                }
            };

            // Act
            var result = dataProcessor.CalculateTotalGeneration(generators);

            // Assert
            Assert.Single(result);
            Assert.Equal("GasGen1", result.First().Name);
            Assert.Equal(500, result.First().TotalGenerationValue);
        }

        [Fact]
        public void CalculateTotalGeneration_SingleGeneratorMultipleDays_ShouldSumCorrectly()
        {
            // Arrange
            var generators = new List<Generator>
            {
                new GasGenerator
                {
                    Name = "GasGen1",
                    Days = new List<DayGeneration>
                    {
                        new DayGeneration { Date = date, Energy = 100, Price = 5 },
                        new DayGeneration { Date = date.AddDays(1), Energy = 200, Price = 6 }
                    }
                }
            };

            // Act
            var result = dataProcessor.CalculateTotalGeneration(generators);

            // Assert
            Assert.Single(result);
            Assert.Equal(1700, result.First().TotalGenerationValue);
        }

        [Fact]
        public void CalculateTotalGeneration_MultipleGenerators_ShouldCalculateEachCorrectly()
        {
            // Arrange
            var generators = new List<Generator>
            {
                new GasGenerator
                {
                    Name = "GasGen1",
                    Days = new List<DayGeneration>
                    {
                        new DayGeneration { Date = date, Energy = 100, Price = 5 }
                    }
                },
                new CoalGenerator
                {
                    Name = "CoalGen1",
                    Days = new List<DayGeneration>
                    {
                        new DayGeneration { Date = date, Energy = 150, Price = 6 }
                    }
                }
            };

            // Act
            var result = dataProcessor.CalculateTotalGeneration(generators);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal(500, result.First(g => g.Name == "GasGen1").TotalGenerationValue);
            Assert.Equal(900, result.First(g => g.Name == "CoalGen1").TotalGenerationValue);
        }

        [Fact]
        public void CalculateTotalGeneration_ZeroEnergyOrPrice_ShouldReturnZero()
        {
            // Arrange
            var generators = new List<Generator>
            {
                new GasGenerator
                {
                    Name = "GasGen1",
                    Days = new List<DayGeneration>
                    {
                        new DayGeneration { Date = date, Energy = 0, Price = 5 },
                        new DayGeneration { Date = date.AddDays(1), Energy = 100, Price = 0 }
                    }
                }
            };

            // Act
            var result = dataProcessor.CalculateTotalGeneration(generators);

            // Assert
            Assert.Single(result);
            Assert.Equal(0, result.First().TotalGenerationValue);
        }

        [Fact]
        public void CalculateTotalGeneration_NoDaysForGenerator_ShouldReturnZero()
        {
            // Arrange
            var generators = new List<Generator>
            {
                new GasGenerator
                {
                    Name = "GasGen1",
                    Days = new List<DayGeneration>() // No days recorded
                }
            };

            // Act
            var result = dataProcessor.CalculateTotalGeneration(generators);

            // Assert
            Assert.Single(result);
            Assert.Equal(0, result.First().TotalGenerationValue);
        }
    }
}