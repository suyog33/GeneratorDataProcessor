using GeneratorDataProcessor.Core.DTOs;
using GeneratorDataProcessor.Core.Entities;
using GeneratorDataProcessor.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Reflection.Emit;
using System.Xml.Linq;

namespace GeneratorDataProcessor.Infrastructure.Repositories
{
    public class XmlGeneratorRepository : IGeneratorRepository
    {
        private readonly string outputFolder;

        public XmlGeneratorRepository(IConfiguration configuration)
        {
            outputFolder = configuration["OutputFolder"];
        }

        public IEnumerable<Generator> GetAllGenerators(string filePath)
        {
            var xmlDoc = XDocument.Load(filePath);
            return ParseGenerators(xmlDoc);
        }

        private IEnumerable<Generator> ParseGenerators(XDocument xmlDoc)
        {
            var generators = new List<Generator>();

            generators.AddRange(xmlDoc.Descendants("WindGenerator")
                .Select(g => new WindGenerator
                {
                    Name = g.Element("Name")?.Value,
                    Location = g.Element("Location")?.Value,
                    Days = ParseDays(g)
                }));

            generators.AddRange(xmlDoc.Descendants("GasGenerator")
                .Select(g => new GasGenerator
                {
                    Name = g.Element("Name")?.Value,
                    EmissionsRating = double.Parse(g.Element("EmissionsRating")?.Value ?? "0"),
                    Days = ParseDays(g)
                }));

            generators.AddRange(xmlDoc.Descendants("CoalGenerator")
                .Select(g => new CoalGenerator
                {
                    Name = g.Element("Name")?.Value,
                    EmissionsRating = double.Parse(g.Element("EmissionsRating")?.Value ?? "0"),
                    TotalHeatInput = double.Parse(g.Element("TotalHeatInput")?.Value ?? "0"),
                    ActualNetGeneration = double.Parse(g.Element("ActualNetGeneration")?.Value ?? "0"),
                    Days = ParseDays(g)
                }));

            return generators;
        }

        private List<DayGeneration> ParseDays(XElement generatorElement)
        {
            return generatorElement.Descendants("Day").Select(d => new DayGeneration
            {
                Date = DateTime.Parse(d.Element("Date")?.Value ?? "0001-01-01"),
                Energy = double.Parse(d.Element("Energy")?.Value ?? "0"),
                Price = double.Parse(d.Element("Price")?.Value ?? "0")
            }).ToList();
        }

        public void SaveResults(string fileName,
            IEnumerable<GeneratorOutputDto> totalGeneration,
            IEnumerable<EmissionDataDto> maxEmissions,
            IEnumerable<HeatRateDto> heatRates)
        {
            var outputXml = new XDocument(new XElement("GenerationOutput",
                new XElement("Totals",
                    totalGeneration.Select(dto => new XElement("Generator",
                        new XElement("Name", dto.Name),
                        new XElement("Total", dto.TotalGenerationValue)))),
                new XElement("MaxEmissionGenerators",
                    maxEmissions.Select(dto => new XElement("Day",
                        new XElement("Name", dto.GeneratorName),
                        new XElement("Date", dto.Date),
                        new XElement("Emission", dto.EmissionValue)))),
                new XElement("ActualHeatRates",
                    heatRates.Select(dto => new XElement("ActualHeatRate",
                        new XElement("Name", dto.GeneratorName),
                        new XElement("HeatRate", dto.HeatRate))))
            ));

            string outputFileName = Path.Combine(outputFolder, $"{fileName}-Results.xml");

            outputXml.Save(outputFileName);
            Console.WriteLine($"Results saved to {outputFileName}");
        }
    }
}
