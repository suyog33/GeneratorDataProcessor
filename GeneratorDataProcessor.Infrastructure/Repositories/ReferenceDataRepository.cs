using GeneratorDataProcessor.Core.Entities;
using GeneratorDataProcessor.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GeneratorDataProcessor.Infrastructure.Repositories
{
    public class FactorsRepository : IFactors
    {
        private readonly string referenceFilePath;

        public FactorsRepository(IConfiguration configuration)
        {
            referenceFilePath = configuration["ReferenceDataFilePath"];
        }

        public Factors LoadReferenceData()
        {
            if (!File.Exists(referenceFilePath))
                throw new FileNotFoundException($"The XML file at {referenceFilePath} was not found.");

            try
            {
                XDocument doc = XDocument.Load(referenceFilePath);

                var factors = new Factors
                {
                    ValueFactor = new ValueFactor
                    {
                        High = double.Parse(doc.Descendants("ValueFactor").Elements("High").FirstOrDefault()?.Value ?? "0"),
                        Medium = double.Parse(doc.Descendants("ValueFactor").Elements("Medium").FirstOrDefault()?.Value ?? "0"),
                        Low = double.Parse(doc.Descendants("ValueFactor").Elements("Low").FirstOrDefault()?.Value ?? "0")
                    },
                    EmissionsFactor = new EmissionsFactor
                    {
                        High = double.Parse(doc.Descendants("EmissionsFactor").Elements("High").FirstOrDefault()?.Value ?? "0"),
                        Medium = double.Parse(doc.Descendants("EmissionsFactor").Elements("Medium").FirstOrDefault()?.Value ?? "0"),
                        Low = double.Parse(doc.Descendants("EmissionsFactor").Elements("Low").FirstOrDefault()?.Value ?? "0")
                    }
                };

                return factors;
            }
            catch (Exception ex)
            {
                throw new Exception("Error parsing XML data using LINQ", ex);
            }

        }
    }
}
