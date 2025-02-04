namespace GeneratorDataProcessor.Core.Entities
{
    public class DayGeneration
    {
        public DateTimeOffset Date { get; set; }
        public double Energy { get; set; }
        public double Price { get; set; }
    }
}
