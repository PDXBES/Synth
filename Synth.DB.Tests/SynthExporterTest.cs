using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NSubstitute;

namespace Synth.DB
{
  [TestFixture]
  [Category("Unit")]
  public class SynthExporterTest
  {
    ISynthData synthDataMock;
    IMifMidWriter mifMidWriterMock;
    SynthExporter testObject;

    [SetUp]
    public void Setup()
    {
      synthDataMock = Substitute.For<ISynthData>();
      SynthProcessingDataSet.All_Hansen_SegsDataTable hansenSegs =
        new SynthProcessingDataSet.All_Hansen_SegsDataTable();
      hansenSegs.Rows.Add(1, 100000, null, "ABC123",
        "DEF456", "SAML", "SMN", 345, 18, 7.7,
        "CIRC", "CSP", null, 5, 21.5, null, 6, 20.5, null, "1420-001S",
        "MH", "1420-001S", "MH", "E09000", new DateTime(2000,1,1), "15540", "N",
        "LOMBARD", "ST", "3", null, null, 100, 200, 300, 400,
        null, "SAN", "IN", 39020, "0");
      hansenSegs.Rows.Add(1, 100001, null, "GHI789",
        "JKL012", "SAML", "SMN", 445, 19, 8.7,
        "ARCH", "VSP", null, 6, 31.5, null, 7, 30.5, null, "1420-001S",
        "MH", "1420-001S", "MH", "E10000", new DateTime(2001, 1, 1), "15541", "SE",
        "DIVISION", "ST", "4", null, null, 101, 201, 301, 401,
        null, "address", "IN", 39020, "0");
      hansenSegs.Rows.Add(1, 100002, null, "MNO345",
        "PQR678", "SAML", "SMN", 545, 20, 10.72,
        "RECT", "RCP", null, 7, 41.5, null, 8, 40.5, null, "1420-001S",
        "MH", "1420-001S", "MH", "1000", new DateTime(2002, 1, 1), "15542", "SW",
        "JEFFERSON", "ST", "7", null, null, 102, 202, 302, 402,
        null, "update_to", "IN", 39020, "0");
      synthDataMock.HansenSegments.Returns(hansenSegs);

      mifMidWriterMock = Substitute.For<IMifMidWriter>();

      testObject = new SynthExporter(synthDataMock);
    }

    private static void testSynthExportHeaderWriteCalls(
      IMifMidWriter mockMifMidWriter)
    {
      mockMifMidWriter.Received().WriteLine("Version 300");
      mockMifMidWriter.Received().WriteLine("Charset \"WindowsLatin1\"");
      mockMifMidWriter.Received().WriteLine("Delimiter \",\"");
      mockMifMidWriter.Received().WriteLine("CoordSys Earth Projection 3, 74, 3, -120.5, 43.6666666667, 44.3333333333, 46, 8202099.738, 0");
      mockMifMidWriter.Received().WriteLine("Columns 30");
      mockMifMidWriter.Received().WriteLine("  Compkey integer");
      mockMifMidWriter.Received().WriteLine("  CADKey char(14)");
      mockMifMidWriter.Received().WriteLine("  USNode char(16)");
      mockMifMidWriter.Received().WriteLine("  DSNode char(16)");
      mockMifMidWriter.Received().WriteLine("  Shape char(4)");
      mockMifMidWriter.Received().WriteLine("  LinkType char(2)");
      mockMifMidWriter.Received().WriteLine("  Length float");
      mockMifMidWriter.Received().WriteLine("  DiamWidth float");
      mockMifMidWriter.Received().WriteLine("  Height float");
      mockMifMidWriter.Received().WriteLine("  Material char(6)");
      mockMifMidWriter.Received().WriteLine("  Uspdpth float");
      mockMifMidWriter.Received().WriteLine("  DwnDpth float");
      mockMifMidWriter.Received().WriteLine("  USIE float");
      mockMifMidWriter.Received().WriteLine("  DSIE float");
      mockMifMidWriter.Received().WriteLine("  AsBuilt char(14)");
      mockMifMidWriter.Received().WriteLine("  Instdate Date");
      mockMifMidWriter.Received().WriteLine("  FromX float");
      mockMifMidWriter.Received().WriteLine("  FromY float");
      mockMifMidWriter.Received().WriteLine("  ToX float");
      mockMifMidWriter.Received().WriteLine("  ToY float");
      mockMifMidWriter.Received().WriteLine("  DataFlagSynth integer");
      mockMifMidWriter.Received().WriteLine("  HServStat char(4)");
      mockMifMidWriter.Received().WriteLine("  StNo char(6)");
      mockMifMidWriter.Received().WriteLine("  PreDir char(2)");
      mockMifMidWriter.Received().WriteLine("  StName char(40)");
      mockMifMidWriter.Received().WriteLine("  suffix char(5)");
      mockMifMidWriter.Received().WriteLine("  MHFrom_Type char(6)");
      mockMifMidWriter.Received().WriteLine("  MHTo_Type char(6)");
      mockMifMidWriter.Received().WriteLine("  Spec_inst char(255)");
      mockMifMidWriter.Received().WriteLine("  Source char(6)");
    }

    private void TestSynthExportMifDataWriteCalls(IMifMidWriter mockMifMidWriter)
    {
      mockMifMidWriter.Received().WriteLine("!100000 (1)");
      mockMifMidWriter.Received().WriteLine("Data");
      mockMifMidWriter.Received().WriteLine("Pline 2");
      mockMifMidWriter.Received().WriteLine("100 200");
      mockMifMidWriter.Received().WriteLine("300 400");
      mockMifMidWriter.Received().WriteLine(
        string.Format("Pen({0},{1},{2})",
          SynthExporter.SynthSegmentWidth,
          SynthExporter.SynthSegmentStyle_AcrossTwoSegments,
          SynthExporter.SynthSegmentColor_AcrossTwoSegments));
      mockMifMidWriter.Received().WriteLine(string.Empty);
      mockMifMidWriter.Received().WriteLine("!100001 (2)");
      mockMifMidWriter.Received().WriteLine("Pline 2");
      mockMifMidWriter.Received().WriteLine("101 201");
      mockMifMidWriter.Received().WriteLine("301 401");
      mockMifMidWriter.Received().WriteLine(
        string.Format("Pen({0},{1},{2})",
          SynthExporter.SynthSegmentWidth,
          SynthExporter.SynthSegmentStyle_HasSynthesizedNode,
          SynthExporter.SynthSegmentColor_HasSynthesizedNode));
      mockMifMidWriter.Received().WriteLine(string.Empty);
      mockMifMidWriter.Received().WriteLine("!100002 (3)");
      mockMifMidWriter.Received().WriteLine("Pline 2");
      mockMifMidWriter.Received().WriteLine("102 202");
      mockMifMidWriter.Received().WriteLine("302 402");
      mockMifMidWriter.Received().WriteLine(
        string.Format("Pen({0},{1},{2})",
          SynthExporter.SynthSegmentWidth,
          SynthExporter.SynthSegmentStyle_MappedNodes,
          SynthExporter.SynthSegmentColor_MappedNodes));
      mockMifMidWriter.Received().WriteLine(string.Empty);
    }

    private void TestSynthExportMidDataWriteCalls(IMifMidWriter mockMifMidWriter)
    {
      mockMifMidWriter.Received().WriteLine("100000,\"\",\"ABC123\",\"DEF456\","+
        "\"CIRC\",\"SAML\",345,18.00,7.70,\"CSP\",5.00,6.00,21.50,20.50,\"E09000\",1/1/2000,100.00,200.00,300.00,400.00,"+
        "\"3\",\"IN\",\"15540\",\"N\",\"LOMBARD\",\"ST\",\"MH\",\"MH\",\"0\",\"SAN\"");
      mockMifMidWriter.Received().WriteLine("100001,\"\",\"GHI789\",\"JKL012\"," +
        "\"ARCH\",\"SAML\",445,19.00,8.70,\"VSP\",6.00,7.00,31.50,30.50,\"E10000\",1/1/2001,101.00,201.00,301.00,401.00," +
        "\"4\",\"IN\",\"15541\",\"SE\",\"DIVISION\",\"ST\",\"MH\",\"MH\",\"0\",\"address\"");
      mockMifMidWriter.Received().WriteLine("100002,\"\",\"MNO345\",\"PQR678\"," +
        "\"RECT\",\"SAML\",545,20.00,10.72,\"RCP\",7.00,8.00,41.50,40.50,\"1000\",1/1/2002,102.00,202.00,302.00,402.00," +
        "\"7\",\"IN\",\"15542\",\"SW\",\"JEFFERSON\",\"ST\",\"MH\",\"MH\",\"0\",\"update_to\"");
    }

    [Test]
    public void TestSynthExportHeader()
    {
      testObject.WriteSynthSegmentMifMidHeader(mifMidWriterMock);
      testSynthExportHeaderWriteCalls(mifMidWriterMock);
    }

    [Test]
    public void TestSynthExportMifData()
    {
      testObject.WriteSynthSegmentMifMidHeader(mifMidWriterMock);
      testSynthExportHeaderWriteCalls(mifMidWriterMock);

      testObject.WriteSynthSegmentMifMidMifData(mifMidWriterMock);
      TestSynthExportMifDataWriteCalls(mifMidWriterMock);
    }

    [Test]
    public void TestSynthExportMidData()
    {
      testObject.WriteSynthSegmentMifMidHeader(mifMidWriterMock);
      testSynthExportHeaderWriteCalls(mifMidWriterMock);
      testObject.WriteSynthSegmentMifMidMifData(mifMidWriterMock);
      TestSynthExportMifDataWriteCalls(mifMidWriterMock);

      testObject.WriteSynthSegmentMifMidMidData(mifMidWriterMock);
      TestSynthExportMidDataWriteCalls(mifMidWriterMock);
    }
  }
}
