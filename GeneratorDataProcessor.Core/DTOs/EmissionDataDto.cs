namespace GeneratorDataProcessor.Core.DTOs
{
    public class EmissionDataDto
        {
            public string GeneratorName { get; set; }
            public DateTimeOffset Date { get; set; }
            public double EmissionValue { get; set; }
        }
    

}
