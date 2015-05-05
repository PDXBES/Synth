using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Synth.DB
{
  public interface IHansenDBAccessor
  {
    SynthProcessingDataSet.USP_RESULTDataTable GetDitches();
    SynthProcessingDataSet.USP_RESULTDataTable GetSanitaryPipes();
    SynthProcessingDataSet.USP_RESULTDataTable GetStormPipes();
    SynthProcessingDataSet.All_Hansen_SegsDataTable HansenSegments { get; }
    SynthProcessingDataSet.ADDRESS_GEOCODEDDataTable GetAddresses();
  }
}
