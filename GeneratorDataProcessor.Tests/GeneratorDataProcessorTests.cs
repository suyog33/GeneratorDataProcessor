using GeneratorDataProcessor.Core.Entities;
using GeneratorDataProcessor.Core.Interfaces;
using GeneratorDataProcessor.Core.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace GeneratorDataProcessor.Tests
{
    public class GeneratorDataProcessorTests
    {
        private readonly Mock<IGeneratorRepository> _mockRepository;
        private readonly DataProcessor _dataProcessor;

        public GeneratorDataProcessorTests()
        {
            _mockRepository = new Mock<IGeneratorRepository>();
            var mockConfig = new Mock<IConfiguration>();
            _dataProcessor = new DataProcessor(_mockRepository.Object, mockConfig.Object);
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
                        new DayGeneration { Date = DateTime.UtcNow, Energy = 100, Price = 5 }
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
                        new DayGeneration { Date = DateTime.UtcNow, Energy = 150, Price = 6 }
                    }
                }
            };

            // Act
            var result = _dataProcessor.CalculateTotalGeneration(generators);

            // Assert
            Assert.Equal(500, result.First().TotalGenerationValue);
            Assert.Equal(900, result.Last().TotalGenerationValue);
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
                        new DayGeneration { Date = DateTime.UtcNow, Energy = 100 }
                    }
                },
                new CoalGenerator
                {
                    Name = "CoalGen1",
                    EmissionsRating = 0.1,
                    Days = new List<DayGeneration>
                    {
                        new DayGeneration { Date = DateTime.UtcNow, Energy = 200 }
                    }
                }
            };

            // Act
            var result = _dataProcessor.CalculateMaxEmissions(generators);

            // Assert
            Assert.Single(result);
            Assert.Equal("CoalGen1", result.First().GeneratorName);
        }

        [Fact]
        public void CalculateHeatRates_ShouldReturnCorrectValues()
        {
            // Arrange
            var generators = new List<Generator>
            {
                new CoalGenerator
                {
                    Name = "CoalGen1",
                    TotalHeatInput = 500,
                    ActualNetGeneration = 250
                }
            };

            // Act
            var result = _dataProcessor.CalculateHeatRates(generators);

            // Assert
            Assert.Single(result);
            Assert.Equal(2, result.First().HeatRate);
        }
    }
}
