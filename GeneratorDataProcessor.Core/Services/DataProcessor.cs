using GeneratorDataProcessor.Core.DTOs;
using GeneratorDataProcessor.Core.Entities;
using GeneratorDataProcessor.Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace GeneratorDataProcessor.Core.Services
{
    public class DataProcessor : IDataProcessor
    {
        private readonly IGeneratorRepository generatorRepository;
        private readonly Factors factors;
        private readonly IConfiguration configuration;

        public DataProcessor(IGeneratorRepository generatorRepository,
                             Factors factors,
                             IConfiguration configuration)
        {
            this.generatorRepository = generatorRepository;
            this.factors = factors;
            this.configuration = configuration;
        }

        public void ProcessFile(string inputFilePath, string inputFileName)
        {
            Console.WriteLine($"Processing file: {inputFilePath}");

            var generators = generatorRepository.GetAllGenerators(inputFilePath).ToList();
            var totalGeneration = CalculateTotalGeneration(generators);
            var maxEmissions = CalculateMaxEmissions(generators);
            var heatRates = CalculateHeatRates(generators);

            generatorRepository.SaveResults(inputFileName, totalGeneration, maxEmissions, heatRates);
        }

        internal IEnumerable<GeneratorOutputDto> CalculateTotalGeneration(IEnumerable<Generator> generators)
        {
            return generators.Select(g => new GeneratorOutputDto
            {
                Name = g.Name,
                TotalGenerationValue = g.Days.Sum(d => d.Energy * d.Price * GetValueFactor(g))
            });
        }

        private double GetValueFactor(Generator generator)
        {
            return generator switch
            {
                WindGenerator wind => wind.Location == "Offshore"
                    ? factors.ValueFactor.Low
                    : factors.ValueFactor.High,
                GasGenerator => factors.ValueFactor.Medium,
                CoalGenerator => factors.ValueFactor.Medium,
                _ => 1
            };
        }

        internal IEnumerable<EmissionDataDto> CalculateMaxEmissions(IEnumerable<Generator> generators)
        {
            return generators
                .SelectMany(g => g.Days, (g, d) => new
                {
                    Generator = g,
                    Date = d.Date,
                    Emission = (g is GasGenerator gas ? gas.EmissionsRating * factors.EmissionsFactor.Medium
                               : (g is CoalGenerator coal ? coal.EmissionsRating * factors.EmissionsFactor.High
                               : 0)) * d.Energy
                })
                .GroupBy(gd => gd.Date)
                .Select(gdGroup => gdGroup.MaxBy(gd => gd.Emission))
                .Where(e => e.Emission > 0)
                .Select(maxEmissionGen => new EmissionDataDto
                {
                    GeneratorName = maxEmissionGen.Generator.Name,
                    Date = maxEmissionGen.Date,
                    EmissionValue = maxEmissionGen.Emission
                });
        }

        internal IEnumerable<HeatRateDto> CalculateHeatRates(IEnumerable<Generator> generators)
        {
            return generators.OfType<CoalGenerator>()
                .Select(g => new HeatRateDto
                {
                    GeneratorName = g.Name,
                    HeatRate = g.ActualNetGeneration != 0 ? g.TotalHeatInput / g.ActualNetGeneration : 0
                });
        }


    }
}