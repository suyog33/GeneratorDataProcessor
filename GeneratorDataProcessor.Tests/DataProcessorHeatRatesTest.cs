using GeneratorDataProcessor.Core.Entities;
using GeneratorDataProcessor.Core.Interfaces;
using GeneratorDataProcessor.Core.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace GeneratorDataProcessor.Tests
{
    public class DataProcessorHeatRatesTest
    {
        private readonly Mock<IGeneratorRepository> mockRepository;
        private readonly DataProcessor dataProcessor;
        public DataProcessorHeatRatesTest()
        {
            mockRepository = new Mock<IGeneratorRepository>();
            var mockConfig = new Mock<IConfiguration>();
            dataProcessor = new DataProcessor(mockRepository.Object, mockConfig.Object);
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
            var result = dataProcessor.CalculateHeatRates(generators);

            // Assert
            Assert.Single(result);
            Assert.Equal(2, result.First().HeatRate);
        }

        [Fact]
        public void CalculateHeatRates_SingleCoalGenerator_ShouldReturnCorrectHeatRate()
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
            var result = dataProcessor.CalculateHeatRates(generators);

            // Assert
            Assert.Single(result);
            Assert.Equal("CoalGen1", result.First().GeneratorName);
            Assert.Equal(2.0, result.First().HeatRate); // 500 / 250 = 2.0
        }

        [Fact]
        public void CalculateHeatRates_MultipleCoalGenerators_ShouldReturnCorrectHeatRates()
        {
            // Arrange
            var generators = new List<Generator>
            {
                new CoalGenerator
                {
                    Name = "CoalGen1",
                    TotalHeatInput = 600,
                    ActualNetGeneration = 300
                },
                new CoalGenerator
                {
                    Name = "CoalGen2",
                    TotalHeatInput = 800,
                    ActualNetGeneration = 400
                }
            };

            // Act
            var result = dataProcessor.CalculateHeatRates(generators);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal(2.0, result.First(g => g.GeneratorName == "CoalGen1").HeatRate); // 600 / 300 = 2.0
            Assert.Equal(2.0, result.First(g => g.GeneratorName == "CoalGen2").HeatRate); // 800 / 400 = 2.0
        }

        [Fact]
        public void CalculateHeatRates_CoalGeneratorWithZeroActualNetGeneration_ShouldReturnZero()
        {
            // Arrange
            var generators = new List<Generator>
            {
                new CoalGenerator
                {
                    Name = "CoalGen1",
                    TotalHeatInput = 500,
                    ActualNetGeneration = 0
                }
            };

            // Act
            var result = dataProcessor.CalculateHeatRates(generators);

            // Assert
            Assert.Single(result);
            Assert.Equal(0, result.First().HeatRate); // Avoids divide by zero
        }

        [Fact]
        public void CalculateHeatRates_NonCoalGenerators_ShouldReturnEmpty()
        {
            // Arrange
            var generators = new List<Generator>
            {
                new GasGenerator
                {
                    Name = "GasGen1"
                },
                new WindGenerator
                {
                    Name = "WindGen1"
                }
            };

            // Act
            var result = dataProcessor.CalculateHeatRates(generators);

            // Assert
            Assert.Empty(result);
        }
    }
}
