using GeneratorDataProcessor.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorDataProcessor.Core.Interfaces
{
    public interface IFactors
    {
        Factors LoadReferenceData();
    }
}
