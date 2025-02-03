using GeneratorDataProcessor.Core.DTOs;
using GeneratorDataProcessor.Core.Entities;

namespace GeneratorDataProcessor.Core.Interfaces
{
    public interface IGeneratorRepository
    {
        IEnumerable<Generator> GetAllGenerators(string filePath);
        void SaveResults(string fileName,
                         IEnumerable<GeneratorOutputDto> totalGeneration,
                         IEnumerable<EmissionDataDto> maxEmissions,
                         IEnumerable<HeatRateDto> heatRates);
    }
}
