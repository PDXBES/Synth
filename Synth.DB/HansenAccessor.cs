using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Configuration;
using GenericParsing;

namespace Synth.DB
{
  public class HansenAccessor : IHansenDBAccessor, IDisposable
  {
    
    SqlConnection connection;
    string getSanitaryPipesName;
    string getStormDitchesName;
    string getStormPipesName;
    SynthProcessingDataSet.All_Hansen_SegsDataTable hansenSegments = null;
    string addressFileName;

    List<string> readErrors = new List<string>();

    public HansenAccessor(string connectionString, string getSanitaryPipesName, string getStormDitchesName,
      string getStormPipesName, string addressFileName)
    {
      connection = new SqlConnection(connectionString);
      this.getSanitaryPipesName = getSanitaryPipesName;
      this.getStormDitchesName = getStormDitchesName;
      this.getStormPipesName = getStormPipesName;
      this.addressFileName = addressFileName;
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
        if (connection != null)
        {
          connection.Close();
          connection.Dispose();
          connection = null;
        }
    }

    ~HansenAccessor()
    {
      Dispose(false);
    }

    /// All segments retrieved from Hansen
    /// </summary>
    public SynthProcessingDataSet.All_Hansen_SegsDataTable HansenSegments
    {
      get
      {
        if (hansenSegments == null)
        {
          AppendHansenSegments();
        }
        return hansenSegments;
      }
    }

    internal void GetHansenSegments
      (Func<SynthProcessingDataSet.USP_RESULTDataTable> resultFunc)
    {
      using (var resultTable = resultFunc())
      {
        int maxMAPINFO_ID = 0;
        if (hansenSegments != null && hansenSegments.Count > 0)
        {
          maxMAPINFO_ID = (from r in hansenSegments
                           select r.MAPINFO_ID).Max();
        }
        int MAPINFO_ID_Counter = maxMAPINFO_ID++;
        foreach (SynthProcessingDataSet.USP_RESULTRow r in resultTable)
        {
          hansenSegments.Rows.Add(MAPINFO_ID_Counter, r.COMPKEY, null, r.UNITID, r.
            UNITID2, r.UNITTYPE, r.COMPCODE, r.PIPELEN, r.PIPEDIAM, r.PIPEHT, r.PIPESHP, r.PIPETYPE,
            null, r.UPSDPTH, r.UPSELEV, null, r.DWNDPTH, r.DWNELEV, null, r.MHFROM, r.MHFROM_TYPE, r.MHTO,
            r.MHTO_TYPE, r.ASBLT, r.INSTDATE, r.STNO, r.PREDIR, r.STNAME, r.SUFFIX, r.SYNTH_FLAG,
            null, null, null, null, null, null, null, r.SOURCE, r.SERVSTAT, r.ADDRKEY,
            r.SPECINST);
          MAPINFO_ID_Counter++;
        }
      }
    }

    /// <summary>
    /// Appends Hansen Segments into the processor
    /// </summary>
    private void AppendHansenSegments()
    {
      if (hansenSegments == null)
        hansenSegments = new SynthProcessingDataSet.All_Hansen_SegsDataTable();
      GetHansenSegments(GetDitches);
      GetHansenSegments(GetSanitaryPipes);
      GetHansenSegments(GetStormPipes);
    }

    private void ReadStoredProcedureRow(SqlDataReader reader, 
      SynthProcessingDataSet.USP_RESULTDataTable resultTable)
    {
      SynthProcessingDataSet.USP_RESULTRow row = resultTable.NewUSP_RESULTRow();
      try
      {
        row.COMPKEY = reader.SafeGetInt32(reader.GetOrdinal("COMPKEY"));
        row.UNITID = reader.SafeGetString(reader.GetOrdinal("UNITID"));
        row.UNITID2 = reader.SafeGetString(reader.GetOrdinal("UNITID2"));
        row.UNITTYPE = reader.SafeGetString(reader.GetOrdinal("UNITTYPE"));
        row.COMPCODE = reader.SafeGetString(reader.GetOrdinal("COMPCODE"));
        row.PIPELEN = reader.SafeGetDouble(reader.GetOrdinal("PIPELEN"));
        row.PIPEDIAM = reader.SafeGetDouble(reader.GetOrdinal("PIPEDIAM"));
        row.PIPEHT = reader.GetDouble(reader.GetOrdinal("PIPEHT"));
        row.MHFROM = reader.SafeGetString(reader.GetOrdinal("MHFROM"));
        row.MHFROM_TYPE = reader.SafeGetString(reader.GetOrdinal("MHFROM_TYPE"));
        row.MHTO = reader.SafeGetString(reader.GetOrdinal("MHTO"));
        row.MHTO_TYPE = reader.SafeGetString(reader.GetOrdinal("MHTO_TYPE"));
        row.STNO = reader.SafeGetString(reader.GetOrdinal("STNO"));
        row.PREDIR = reader.SafeGetString(reader.GetOrdinal("PREDIR"));
        row.STNAME = reader.SafeGetString(reader.GetOrdinal("STNAME"));
        row.SUFFIX = reader.SafeGetString(reader.GetOrdinal("SUFFIX"));
        row.SOURCE = reader.SafeGetString(reader.GetOrdinal("SOURCE"));
        row.SERVSTAT = reader.SafeGetString(reader.GetOrdinal("SERVSTAT"));
        row.ADDRKEY = reader.SafeGetInt32(reader.GetOrdinal("ADDRKEY"));
        string specInst = reader.SafeGetString(reader.GetOrdinal("SPECINST"));
        row.SPECINST = specInst.Substring(0, specInst.Length > 250 ? 250 : specInst.Length);
        row.SYNTH_FLAG = reader.SafeGetString(reader.GetOrdinal("SYNTH_FLAG"));
        row.PIPESHP = reader.SafeGetString(reader.GetOrdinal("PIPESHP"));
        row.PIPETYPE = reader.SafeGetString(reader.GetOrdinal("PIPETYPE"));
        row.UPSDPTH = reader.SafeGetDouble(reader.GetOrdinal("UPSDPTH"));
        row.UPSELEV = reader.SafeGetDouble(reader.GetOrdinal("UPSELEV"));
        row.DWNDPTH = reader.SafeGetDouble(reader.GetOrdinal("DWNDPTH"));
        row.DWNELEV = reader.SafeGetDouble(reader.GetOrdinal("DWNELEV"));
        row.ASBLT = reader.SafeGetString(reader.GetOrdinal("ASBLT"));
        row.INSTDATE = reader.SafeGetDateTime(reader.GetOrdinal("INSTDATE"));

        resultTable.AddUSP_RESULTRow(row);
      }
      catch (Exception e)
      {
        readErrors.Add(string.Format("error reading compkey {0}", 
          reader.SafeGetInt32(reader.GetOrdinal("COMPKEY"))));
      }
    }

    private SynthProcessingDataSet.USP_RESULTDataTable GetStoredProcedureResult(string procedureName)
    {
      SqlCommand cmd = new SqlCommand();
      SqlDataReader reader;

      cmd.CommandText = procedureName;
      cmd.CommandType = System.Data.CommandType.StoredProcedure;
      cmd.Connection = connection;

      connection.Open();

      reader = cmd.ExecuteReader();

      SynthProcessingDataSet.USP_RESULTDataTable result =
        new SynthProcessingDataSet.USP_RESULTDataTable();
      while (reader.Read())
      {
        ReadStoredProcedureRow(reader, result);
      }

      connection.Close();

      return result;
    }

    public SynthProcessingDataSet.USP_RESULTDataTable GetDitches()
    {
      return GetStoredProcedureResult(getStormDitchesName);
    }

    public SynthProcessingDataSet.USP_RESULTDataTable GetSanitaryPipes()
    {
      return GetStoredProcedureResult(getSanitaryPipesName);
    }

    public SynthProcessingDataSet.USP_RESULTDataTable GetStormPipes()
    {
      return GetStoredProcedureResult(getStormPipesName);
    }

    public SynthProcessingDataSet.ADDRESS_GEOCODEDDataTable GetAddresses()
    {
      using (GenericParser parser = new GenericParser(addressFileName))
      {
        parser.ColumnDelimiter = ',';
        parser.FirstRowHasHeader = true;
        parser.MaxBufferSize = 4096;
        parser.TextQualifier = '"';

        SynthProcessingDataSet.ADDRESS_GEOCODEDDataTable addressTable =
          new SynthProcessingDataSet.ADDRESS_GEOCODEDDataTable();

        while (parser.Read())
        {
          string ADDBY = parser[0];
          DateTime? ADDDTTM = (parser[1] == string.Empty ? DateTime.MinValue : DateTime.Parse(parser[1]));
          int ADDRKEY = Convert.ToInt32(parser[2]);
          string ADDRTYPE = parser[3];
          string AREA = parser[4];
          string BLOCK = parser[5];
          string CITY = parser[6];
          string COMPDIR = parser[7];
          DateTime EXPDATE = (parser[8] == string.Empty ? DateTime.MinValue : DateTime.Parse(parser[8]));
          string LEGALDESC = parser[9].Substring(0, parser[9].Length > 250 ? 250 : parser[9].Length);
          string LEGALOWNER = parser[10];
          string LOT = parser[11];
          string MAPNO = parser[12];
          int MAXACCTS = Convert.ToInt32(parser[13]);
          string MGMTGRPID = parser[14];
          string MODBY = parser[15];
          DateTime MODDTTM = parser[16] == string.Empty ? DateTime.MinValue : DateTime.Parse(parser[16]);
          string POSTDIR = parser[17];
          string PREDIR = parser[18];
          string ST2NAME = parser[19];
          string ST2POSTDIR = parser[20];
          string ST2PREDIR = parser[21];
          string ST2SUFFIX = parser[22];
          string ST3NAME = parser[23];
          string ST3POSTDIR = parser[24];
          string ST3PREDIR = parser[25];
          string ST3SUFFIX = parser[26];
          string STATE = parser[27];
          string STNAME = parser[28];
          string STNO = parser[29];
          string STNOHI = parser[30];
          string STSUB = parser[31];
          string SUBDIVCODE = parser[32];
          string SUBDIVDESC = parser[33];
          string SUFFIX = parser[34];
          string ZIP = parser[35];
          double X = Convert.ToDouble(parser[36]);
          double Y = Convert.ToDouble(parser[37]);
          int MAPINFO_ID = Convert.ToInt32(parser[38]);
          string ADDRESS = parser[39];

          addressTable.Rows.Add(ADDBY, ADDDTTM, ADDRKEY, ADDRTYPE, AREA, BLOCK, CITY, COMPDIR, 
            EXPDATE, LEGALDESC, LEGALOWNER, LOT, MAPNO, MAXACCTS, MGMTGRPID, MODBY, MODDTTM,
            POSTDIR, PREDIR, ST2NAME, ST2POSTDIR, ST2PREDIR, ST2SUFFIX, ST3NAME, ST3POSTDIR, 
            ST3PREDIR, ST3SUFFIX, STATE, STNAME, STNO, STNOHI, STSUB, SUBDIVCODE, SUBDIVDESC,
            SUFFIX, ZIP, X, Y, MAPINFO_ID, ADDRESS);
        }

        return addressTable;
      }
    }
  }
}
