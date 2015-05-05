using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GenericParsing;

namespace Synth.DB
{
  public class TestCSVHansenAccessor : IHansenDBAccessor
  {
    string ditchFileName;
    string sanitaryFileName;
    string stormFileName;
    string addressFileName;
    int maxMapInfoID = 0;
    SynthProcessingDataSet.All_Hansen_SegsDataTable hansenSegments;

    public TestCSVHansenAccessor(string ditchFileName, string sanitaryFileName,
      string stormFileName, string addressFileName)
    {
      this.ditchFileName = ditchFileName;
      this.sanitaryFileName = sanitaryFileName;
      this.stormFileName = stormFileName;
      this.addressFileName = addressFileName;
    }

    private void GetData(SynthProcessingDataSet.USP_RESULTDataTable resultTable,
      string fileName)
    {
      using (GenericParser parser = new GenericParser(fileName))
      {
        parser.ColumnDelimiter = ',';
        parser.FirstRowHasHeader = true;
        parser.MaxBufferSize = 4096;
        parser.TextQualifier = '"';

        while (parser.Read())
        {
          int MAPINFO_ID_Counter = maxMapInfoID;
          int COMPKEY = Convert.ToInt32(parser["COMPKEY"]);
          string UNITID = parser["UNITID"];
          string UNITID2 = parser["UNITID2"];
          string UNITTYPE = parser["UNITTYPE"];
          string COMPCODE = parser["COMPCODE"] == "NULL" ? string.Empty : parser["COMPCODE"];
          double PIPELEN = Convert.ToDouble(parser["PIPELEN"]);
          double PIPEDIAM = Convert.ToDouble(parser["PIPEDIAM"]);
          double PIPEHT = Convert.ToDouble(parser["PIPEHT"]);
          string MHFROM = parser["MHFROM"] == "NULL" ? string.Empty : parser["MHFROM"];
          string MHFROM_TYPE = parser["MHFROM_TYPE"] == "NULL" ? string.Empty : parser["MHFROM_TYPE"];
          string MHTO = parser["MHTO"] == "NULL" ? string.Empty : parser["MHTO"];
          string MHTO_TYPE = parser["MHTO_TYPE"] == "NULL" ? string.Empty : parser["MHTO_TYPE"];
          string STNO = parser["STNO"] == "NULL" ? string.Empty : parser["STNO"];
          string PREDIR = parser["PREDIR"] == "NULL" ? string.Empty : parser["PREDIR"];
          string STNAME = parser["STNAME"] == "NULL" ? string.Empty : parser["STNAME"];
          string SUFFIX = parser["SUFFIX"] == "NULL" ? string.Empty : parser["SUFFIX"];
          string SOURCE = parser["SOURCE"] == "NULL" ? string.Empty : parser["SOURCE"];
          string SERVSTAT = parser["SERVSTAT"] == "NULL" ? string.Empty : parser["SERVSTAT"];
          int ADDRKEY = Convert.ToInt32(parser["ADDRKEY"]);
          string SPECINST = parser["SPECINST"] == "NULL" ? string.Empty : 
            parser["SPECINST"].Substring(0, parser["SPECINST"].Length > 250 ?
            250 : parser["SPECINST"].Length);
          string SYNTH_FLAG = parser["SYNTH_FLAG"];
          string PIPESHP = parser["PIPESHP"] == "NULL" ? string.Empty : parser["PIPESHP"];
          string PIPETYPE = parser["PIPETYPE"] == "NULL" ? string.Empty : parser["PIPETYPE"];
          double UPSDPTH = parser["UPSDPTH"] == "NULL" ? 0.0 : Convert.ToDouble(parser["UPSDPTH"]);
          double UPSELEV = parser["UPSELEV"] == "NULL" ? 0.0 : Convert.ToDouble(parser["UPSELEV"]);
          double DWNDPTH = parser["DWNDPTH"] == "NULL" ? 0.0 : Convert.ToDouble(parser["DWNDPTH"]);
          double DWNELEV = parser["DWNELEV"] == "NULL" ? 0.0 : Convert.ToDouble(parser["DWNELEV"]);
          string ASBLT = parser["ASBLT"] == "NULL" ? string.Empty : parser["ASBLT"];
          DateTime? INSTDATE;
          if (parser["INSTDATE"] == "NULL")
            INSTDATE = null;
          else
            INSTDATE = DateTime.Parse(parser["INSTDATE"]);

          hansenSegments.Rows.Add(MAPINFO_ID_Counter, COMPKEY, null, UNITID,
            UNITID2, UNITTYPE, COMPCODE, PIPELEN, PIPEDIAM, PIPEHT, PIPESHP, PIPETYPE,
            null, UPSDPTH, UPSELEV, null, DWNDPTH, DWNELEV, null, MHFROM, MHFROM_TYPE, MHTO,
            MHTO_TYPE, ASBLT, INSTDATE.Equals(DateTime.MinValue) ? null : INSTDATE, STNO, PREDIR, STNAME, SUFFIX, SYNTH_FLAG,
            null, null, null, null, null, null, null, SOURCE, SERVSTAT, ADDRKEY, SPECINST);

          maxMapInfoID++;
        }
      }
    }

    public SynthProcessingDataSet.USP_RESULTDataTable GetDitches()
    {
      SynthProcessingDataSet.USP_RESULTDataTable resultTable =
        new SynthProcessingDataSet.USP_RESULTDataTable();

      GetData(resultTable, ditchFileName);

      return resultTable;
    }

    public SynthProcessingDataSet.USP_RESULTDataTable GetSanitaryPipes()
    {
      SynthProcessingDataSet.USP_RESULTDataTable resultTable =
        new SynthProcessingDataSet.USP_RESULTDataTable();

      GetData(resultTable, sanitaryFileName);

      return resultTable;
    }

    public SynthProcessingDataSet.USP_RESULTDataTable GetStormPipes()
    {
      SynthProcessingDataSet.USP_RESULTDataTable resultTable =
        new SynthProcessingDataSet.USP_RESULTDataTable();

      GetData(resultTable, stormFileName);

      return resultTable;
    }

    public SynthProcessingDataSet.All_Hansen_SegsDataTable HansenSegments
    {
      get
      {
        if (hansenSegments == null)
        {
          hansenSegments = new SynthProcessingDataSet.All_Hansen_SegsDataTable();
          GetDitches();
          GetSanitaryPipes();
          GetStormPipes();
        }

        return hansenSegments;
      }
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
