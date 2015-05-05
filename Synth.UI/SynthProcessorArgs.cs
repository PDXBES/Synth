using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Synth.UI
{
  internal struct SynthProcessorArgs
  {
    public string HansenServer;
    public string StoredProcsDB;
    public string HansenConnectionString;
    public string GetSanitaryPipesUSP;
    public string GetStormDitchesUSP;
    public string GetStormPipesUSP;
    public string AddressFile;
    public string MasterLinksFile;
    public string MasterNodesFile;
    public string SynthFile;

    internal SynthProcessorArgs(
      string HansenServer, 
      string StoredProcsDB, 
      string HansenConnectionString,
      string GetSanitaryPipesUSP,
      string GetStormDitchesUSP,
      string GetStormPipesUSP,
      string AddressFile,
      string MasterLinksFile,
      string MasterNodesFile,
      string SynthFile)
    {
      this.HansenServer = HansenServer;
      this.StoredProcsDB = StoredProcsDB;
      this.HansenConnectionString = HansenConnectionString;
      this.GetSanitaryPipesUSP = GetSanitaryPipesUSP;
      this.GetStormDitchesUSP = GetStormDitchesUSP;
      this.GetStormPipesUSP = GetStormPipesUSP;
      this.AddressFile = AddressFile;
      this.MasterLinksFile = MasterLinksFile;
      this.MasterNodesFile = MasterNodesFile;
      this.SynthFile = SynthFile;
    }
  }
}
