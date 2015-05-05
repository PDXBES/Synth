using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Synth.DB
{
  public interface ISynthData
  {
    SynthProcessingDataSet.All_Hansen_SegsDataTable HansenSegments { get; }
  }
}
