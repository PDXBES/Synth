using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.OleDb;

namespace Synth.DB
{
  public class MasterDataAccessor : IMasterDataDBAccessor, IDisposable
  {
    OleDbConnection masterLinksConnection;
    SynthProcessingDataSet.mst_links_acDataTable masterLinks;
    OleDbConnection masterNodesConnection;
    SynthProcessingDataSet.mst_nodes_acDataTable masterNodes;

    public MasterDataAccessor(string masterLinksConnectionString, string masterNodesConnectionString)
    {
      this.masterLinksConnection = new OleDbConnection(masterLinksConnectionString);
      this.masterNodesConnection = new OleDbConnection(masterNodesConnectionString);
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (masterLinksConnection != null)
        {
          masterLinksConnection.Dispose();
          masterLinksConnection = null;
        }
        if (masterNodesConnection != null)
        {
          masterNodesConnection.Dispose();
          masterNodesConnection = null;
        }
      }
    }

    ~MasterDataAccessor()
    {
      Dispose(false);
    }

    public SynthProcessingDataSet.mst_links_acDataTable MasterLinks
    {
      get
      {
        if (masterLinks == null)
        {
          OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT * FROM mst_links_ac", 
            masterLinksConnection);
          masterLinks = new SynthProcessingDataSet.mst_links_acDataTable();
          adapter.Fill(masterLinks);
        }
        return masterLinks;
      }
    }

    public SynthProcessingDataSet.mst_nodes_acDataTable MasterNodes
    {
      get
      {
        if (masterNodes == null)
        {
          OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT * FROM mst_nodes_ac", 
            masterNodesConnection);
          masterNodes = new SynthProcessingDataSet.mst_nodes_acDataTable();
          adapter.Fill(masterNodes);
        }
        return masterNodes;
      }
    }
  }
}
