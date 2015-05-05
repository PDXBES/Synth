using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Synth.Logging;

namespace Synth.DB
{
  public class SynthExporter : IDisposable
  {
    public const int SynthSegmentWidth = 3;
    public const int SynthSegmentStyle_AcrossTwoSegments = 65;
    public const int SynthSegmentStyle_HasSynthesizedNode = 63;
    public const int SynthSegmentStyle_MappedNodes = 2;
    public const int SynthSegmentColor_AcrossTwoSegments = 9474192;
    public const int SynthSegmentColor_HasSynthesizedNode = 9502608;
    public const int SynthSegmentColor_MappedNodes = 16748688;

    private SynthProcessingDataSet.SynthFlagsDataTable
      synthFlagsTable;
    private ISynthData synthData;
    private List<string> exportErrors = new List<string>();

    Logger logger;

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
        if (synthFlagsTable != null)
        {
          synthFlagsTable.Dispose();
          synthFlagsTable = null;
        }
    }

    ~SynthExporter()
    {
      Dispose(false);
    }

    public SynthExporter(ISynthData synthData)
    {
      SetupSynthFlagsTable();
      this.synthData = synthData;

      logger = new Logger(this.GetType().Name);
    }

    private void WriteStandardMifMidHeader(
IMifMidWriter writer)
    {
      logger.Debug("Enter WriteStandardMifMidHeader");
      writer.WriteLine("Version 300");
      writer.WriteLine("Charset \"WindowsLatin1\"");
      writer.WriteLine("Delimiter \",\"");
      writer.WriteLine("CoordSys Earth Projection 3, 74, 3, -120.5, 43.6666666667, 44.3333333333, 46, 8202099.738, 0");
      logger.Debug("Exit WriteStandardMifMidHeader");
    }

    private void WriteSynthSegmentMifMidColumns(IMifMidWriter writer)
    {
      logger.Debug("Enter WriteMifMidColumns");
      writer.WriteLine("Columns 30");
      writer.WriteLine("  Compkey integer");
      writer.WriteLine("  CADKey char(14)");
      writer.WriteLine("  USNode char(16)");
      writer.WriteLine("  DSNode char(16)");
      writer.WriteLine("  Shape char(4)");
      writer.WriteLine("  LinkType char(2)");
      writer.WriteLine("  Length float");
      writer.WriteLine("  DiamWidth float");
      writer.WriteLine("  Height float");
      writer.WriteLine("  Material char(6)");
      writer.WriteLine("  Uspdpth float");
      writer.WriteLine("  DwnDpth float");
      writer.WriteLine("  USIE float");
      writer.WriteLine("  DSIE float");
      writer.WriteLine("  AsBuilt char(14)");
      writer.WriteLine("  Instdate Date");
      writer.WriteLine("  FromX float");
      writer.WriteLine("  FromY float");
      writer.WriteLine("  ToX float");
      writer.WriteLine("  ToY float");
      writer.WriteLine("  DataFlagSynth integer");
      writer.WriteLine("  HServStat char(4)");
      writer.WriteLine("  StNo char(6)");
      writer.WriteLine("  PreDir char(2)");
      writer.WriteLine("  StName char(40)");
      writer.WriteLine("  suffix char(5)");
      writer.WriteLine("  MHFrom_Type char(6)");
      writer.WriteLine("  MHTo_Type char(6)");
      writer.WriteLine("  Spec_inst char(255)");
      writer.WriteLine("  Source char(6)");
      logger.Debug("Exit WriteMifMidColumns");
    }

    public void WriteSynthSegmentMifMidHeader(IMifMidWriter writer)
    {
      WriteStandardMifMidHeader(writer);
      WriteSynthSegmentMifMidColumns(writer);
    }

    private void SetupSynthFlagsTable()
    {
      synthFlagsTable = new SynthProcessingDataSet.SynthFlagsDataTable();
      synthFlagsTable.Rows.Add("0", "hansen segments with no ability to map", false);
      synthFlagsTable.Rows.Add("1", "a mapped segment with the same hansen nodes", false);
      synthFlagsTable.Rows.Add("3", "match hansen nodes across two mapped segs", true);
      synthFlagsTable.Rows.Add("4", "synthetic pipes found in hansen and mapped in appropriate location", true);
      synthFlagsTable.Rows.Add("7", "two hansen nodes were found in mapped manholes", true);
      synthFlagsTable.Rows.Add("9", "hansen ids not found in the map", false);
    }

    public void WriteSynthSegmentMifMidMifData(IMifMidWriter writer)
    {
      logger.Debug("Enter WriteMifMidMifData");

      writer.WriteLine("Data");
      var mifData = from s in synthData.HansenSegments
                    join f in synthFlagsTable on s.Synth_Flag equals f.Flag
                    where f.mifmidexport == true
                    orderby s.COMPKEY ascending
                    select new
                      {
                        compkey = s.COMPKEY,
                        from_x = s.From_X,
                        from_y = s.From_Y,
                        to_x = s.To_X,
                        to_y = s.To_Y,
                        synthFlag = s.Synth_Flag
                      };

      int i = 1;
      foreach (var d in mifData)
      {
        writer.WriteLine("!" + d.compkey.ToString() + " (" + i.ToString() + ")");
        writer.WriteLine("Pline 2");
        writer.WriteLine(string.Format("{0} {1}", d.from_x, d.from_y));
        writer.WriteLine(string.Format("{0} {1}", d.to_x, d.to_y));
        switch (d.synthFlag)
        {
          case "3":
            writer.WriteLine(String.Format("Pen({0},{1},{2})", SynthSegmentWidth, 
              SynthSegmentStyle_AcrossTwoSegments, SynthSegmentColor_AcrossTwoSegments));
            break;
          case "4":
            writer.WriteLine(String.Format("Pen({0},{1},{2})", SynthSegmentWidth, 
              SynthSegmentStyle_HasSynthesizedNode, SynthSegmentColor_HasSynthesizedNode));
            break;
          case "7":
            writer.WriteLine(String.Format("Pen({0},{1},{2})", SynthSegmentWidth, 
              SynthSegmentStyle_MappedNodes, SynthSegmentColor_MappedNodes));
            break;
        }
        writer.WriteLine();
        i++;
      }
      writer.Flush();
      logger.Debug("Exit WriteMifMidMifData");
    }

    public void WriteSynthSegmentMifMidMidData(IMifMidWriter writer)
    {
      logger.Debug("Enter WriteMifMidMidData");
      try
      {
        StringBuilder line = new StringBuilder();
        var segments = from r in synthData.HansenSegments
                       join f in synthFlagsTable on r.Synth_Flag equals f.Flag
                       where f.mifmidexport == true
                       orderby r.COMPKEY ascending
                       select new
                       {
                         row =
                           "" + r.COMPKEY + "," +
                           (r.IsCADKEYNull() ? "\"\"," : ("\"" + r.CADKEY + ",")) +
                           "\"" + r.UNITID + "\"," +
                           "\"" + r.UNITID2 + "\"," +
                           "\"" + (r.IsPIPESHPNull() ? string.Empty : r.PIPESHP) + "\"," +
                           "\"" + r.UNITTYPE + "\"," +
                           string.Format("{0:F0}", r.PIPELEN) + "," +
                           string.Format("{0:F2}", r.PIPEDIAM) + "," +
                           string.Format("{0:F2}", r.PIPEHT) + "," +
                           "\"" + (r.IsPIPETYPENull() ? string.Empty : r.PIPETYPE) + "\"," +
                           string.Format("{0:F2}", r.IsUPSDPTHNull() ? 0.0 : r.UPSDPTH) + "," +
                           string.Format("{0:F2}", r.IsDWNDPTHNull() ? 0.0 : r.DWNDPTH) + "," +
                           string.Format("{0:F2}", r.IsUPSELEVNull() ? 0.0 : r.UPSELEV) + "," +
                           string.Format("{0:F2}", r.IsDWNELEVNull() ? 0.0 : r.DWNELEV) + "," +
                           "\"" + (r.IsASBLTNull() ? string.Empty : r.ASBLT) + "\"," +
                           (r.IsINSTDATENull() ? string.Empty : r.INSTDATE.ToString("d")) + "," +
                           string.Format("{0:F2}", r.From_X) + "," +
                           string.Format("{0:F2}", r.From_Y) + "," +
                           string.Format("{0:F2}", r.To_X) + "," +
                           string.Format("{0:F2}", r.To_Y) + "," +
                           "\"" + r.Synth_Flag + "\"," +
                           "\"" + (r.IsSERVSTATNull() ? string.Empty : r.SERVSTAT) + "\"," +
                           "\"" + (r.IsSTNONull() ? string.Empty : r.STNO) + "\"," +
                           "\"" + (r.IsPREDIRNull() ? string.Empty : r.PREDIR) + "\"," +
                           "\"" + (r.IsSTNAMENull() ? string.Empty : r.STNAME) + "\"," +
                           "\"" + (r.IsSUFFIXNull() ? string.Empty : r.SUFFIX) + "\"," +
                           "\"" + (r.Ismhfrom_typeNull() ? string.Empty : r.mhfrom_type) + "\"," +
                           "\"" + (r.Ismhto_typeNull() ? string.Empty : r.mhto_type) + "\"," +
                           "\"" + (r.IsSPECINSTNull() ? string.Empty : r.SPECINST) + "\"," +
                           "\"" + (r.IsSourceNull() ? string.Empty : r.Source) + "\""
                       };
        foreach (var s in segments)
        {
          writer.WriteLine(s.row);
        }
      }
      catch (Exception ex)
      {
        exportErrors.Add(string.Format("Export error: {0}", ex.Message));
      }
      writer.Flush();
      logger.Debug("Enter WriteMifMidMidData");
    }    
  }
}
