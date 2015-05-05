using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Synth.DB
{
  public interface IMasterDataDBAccessor
  {
    SynthProcessingDataSet.mst_links_acDataTable MasterLinks { get; }
    SynthProcessingDataSet.mst_nodes_acDataTable MasterNodes { get; }
  }
}
