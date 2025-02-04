namespace GeneratorDataProcessor.Core.Entities
{
    public class Factors
    {
        public ValueFactor ValueFactor { get; set; }
        public EmissionsFactor EmissionsFactor { get; set; }
    }

    public class ValueFactor
    {
        public double High { get; set; }
        public double Medium { get; set; }
        public double Low { get; set; }
    }

    public class EmissionsFactor
    {
        public double High { get; set; }
        public double Medium { get; set; }
        public double Low { get; set; }
    }
}
