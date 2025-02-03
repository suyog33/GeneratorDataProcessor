namespace GeneratorDataProcessor.Core.Entities
{
    public abstract class Generator
    {
        public string Name { get; set; }
        public List<DayGeneration> Days { get; set; } = new List<DayGeneration>();
    }
}
