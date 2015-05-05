using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NSubstitute;

namespace Synth.DB
{
  [TestFixture]
  [Category("Unit")]
  public class SynthDataProcessorTest
  {
    // Mock data checks
    IMasterDataDBAccessor masterDataMock;
    IHansenDBAccessor hansenMock;

    internal static void SetupMasterNodesTable(IMasterDataDBAccessor localDB)
    {
      SynthProcessingDataSet.mst_nodes_acDataTable aNodesTable = new SynthProcessingDataSet.mst_nodes_acDataTable();
      aNodesTable.Rows.Add(1, "ABC123", 123456.0, 234567.0, "MH", 1.0, "F", "F", 1001, 1);
      aNodesTable.Rows.Add(2, "Aban23", 345678.0, 456789.0, "MH", 2.0, "F", "F", 1001, 2);
      aNodesTable.Rows.Add(3, "CWS123", 567890.0, 678901.0, "MH", 3.0, "F", "F", 1001, 3);
      aNodesTable.Rows.Add(4, "AAA197", 789012.0, 890123.0, "MH", 4.0, "F", "F", 1001, 4);
      aNodesTable.Rows.Add(5, "AAA206", 901234.0, 123456.0, "MH", 5.0, "F", "F", 1001, 5);
      aNodesTable.Rows.Add(6, "AAA199", 234567.0, 345678.0, "MH", 6.0, "F", "F", 1001, 6);
      aNodesTable.Rows.Add(7, "AAA200", 456789.0, 567890.0, "MH", 7.0, "F", "F", 1001, 7);
      aNodesTable.Rows.Add(8, "AAA341", 200.0, 201.0, "MH", 8.0, "F", "F", 1001, 8);
      aNodesTable.Rows.Add(9, "AAA342", 201.0, 202.0, "MH", 9.0, "F", "F", 1001, 9);
      aNodesTable.Rows.Add(10, "APC635", 202.0, 203.0, "MH", 10.0, "F", "F", 1001, 10);
      aNodesTable.Rows.Add(11, "APC639", 203.0, 204.0, "MH", 11.0, "F", "F", 1001, 11);
      aNodesTable.Rows.Add(12, "AAA002", 100.0, 101.0, "MH", 12.0, "F", "F", 1001, 12);
      aNodesTable.Rows.Add(13, "AAA081", 101.0, 102.0, "MH", 13.0, "F", "F", 1001, 13);
      aNodesTable.Rows.Add(14, "ABH414", 102.0, 103.0, "MH", 14.0, "F", "F", 1001, 14);
      aNodesTable.Rows.Add(15, "ADA357", 103.0, 104.0, "MH", 15.0, "F", "F", 1001, 15);
      aNodesTable.Rows.Add(16, "ADA356", 104.0, 105.0, "MH", 16.0, "F", "F", 1001, 16);
      aNodesTable.Rows.Add(17, "AAA003", 105.0, 106.0, "MH", 17.0, "F", "F", 1001, 17);
      aNodesTable.Rows.Add(18, "AAA006", 106.0, 107.0, "MH", 18.0, "F", "F", 1001, 18);
      aNodesTable.Rows.Add(19, "AAA007", 107.0, 108.0, "MH", 19.0, "F", "F", 1001, 19);
      aNodesTable.Rows.Add(20, "AAA123", 108.0, 109.0, "MH", 12.0, "F", "F", 1001, 20);
      localDB.MasterNodes.Returns(aNodesTable);
    }

    internal static void SetupRemoteDitches(IHansenDBAccessor remoteDB)
    {
      using (SynthProcessingDataSet.USP_RESULTDataTable remoteDitchesTable = new SynthProcessingDataSet.USP_RESULTDataTable())
      {
        remoteDitchesTable.Rows.Add(100196, "AAA197", "AAA206", "CHDTC", "STCH", 120, 0, 4.8, "1619-039D", "OPNEND", "1619-039D", "OPNEND", "13605", "N", "RIVERGATE", "BLVD", "DTCH", "IN", 82068, null, "0");
        remoteDitchesTable.Rows.Add(100198, "AAA199", "AAA200", "CHDTC", "STCH", 130, 0, 4.8, "1619-060D", "WWE", "1619-060D", "WWE", "14005", "N", "RIVERGATE", "BLVD", "DTCH", "IN", 82280, null, "0");
        remoteDitchesTable.Rows.Add(100342, "AAA341", "AAA342", "CHNAT", "STCH", 515, 0, 0.2, "1717-014D", "WWE", "1717-014D", "WWE", "12800", "NW", "MARINA", "WAY", "DTCH", "IN", 81669, null, "0");
        remoteDitchesTable.Rows.Add(440596, "APC635", "APC639", "CHNAT", "STCH", 0, 0, 0, null, "WWJ", null, "WWJ", "11523", "SW", "27TH", "AVE", "DTCH", "IN", 218469, null, "0");
        remoteDB.GetDitches().Returns(remoteDitchesTable);
      }
    }

    internal static void SetupRemoteSanitaryPipes(IHansenDBAccessor remoteDB)
    {
      using (SynthProcessingDataSet.USP_RESULTDataTable remoteSanitaryPipesTable = new SynthProcessingDataSet.USP_RESULTDataTable())
      {
        remoteSanitaryPipesTable.Rows.Add(100000, "AAA002", "AAA081", "SAML", "SMN", 345, 18, 7.7, "1420-001S", "MH", "1420-001S", "MH", "15540", "N", "LOMBARD", "ST", "SAN", "IN", 39020, null, "0");
        remoteSanitaryPipesTable.Rows.Add(131746, "ABH414", "ABH425", "CSML", "SMN", 103, 8, 8.8, "2935-023C", "MH", "2935-023C", "MH", "1321", "NE", "49TH", "AVE", "SAN", "IN", 56540, null, "0");
        remoteSanitaryPipesTable.Rows.Add(173067, "ADA357", "ADA356", "SAML", "SMN", 152, 8, 6.4, "3842-003S", "MH", "3842-003S", "MH", "11336", "SE", "FLAVEL", "ST", "SAN", "IN", 29705, null, "0");
        remoteSanitaryPipesTable.Rows.Add(900000, "XYZ123", "ABH414", "SAML", "SMN", 152, 8, 6.4, "3842-003S", "MH", "3842-003S", "MH", "11336", "SE", "FLAVEL", "ST", "SAN", "IN", 29705, null, "0");
        remoteSanitaryPipesTable.Rows.Add(900001, "XYZ123", "XYZ124", "SAML", "SMN", 152, 8, 6.4, "3842-003S", "MH", "3842-003S", "MH", "11336", "SE", "FLAVEL", "ST", "SAN", "IN", 29705, null, "0");
        remoteSanitaryPipesTable.Rows.Add(900002, "AAA002", "XYZ123", "SAML", "SMN", 152, 8, 6.4, "3842-003S", "MH", "3842-003S", "MH", "11336", "SE", "FLAVEL", "ST", "SAN", "IN", 29705, null, "0");
        remoteSanitaryPipesTable.Rows.Add(900003, "XYZ126", "XYZ125", "SAML", "SMN", 152, 8, 6.4, "3842-003S", "MH", "3842-003S", "MH", "11336", "SE", "FLAVEL", "ST", "SAN", "IN", 29706, null, "0");
        remoteSanitaryPipesTable.Rows.Add(900004, "XXXXXX", "XXXXXX", "SAML", "SMN", 152, 8, 6.4, "3842-003S", "MH", "3842-003S", "MH", "11336", "SE", "FLAVEL", "ST", "SAN", "IN", 29707, null, "0");
        remoteSanitaryPipesTable.Rows.Add(900005, "XYZ127", "XYZ123", "SAML", "SMN", 152, 8, 6.4, "3842-003S", "MH", "3842-003S", "MH", "11336", "SE", "FLAVEL", "ST", "SAN", "IN", 29707, null, "0");
        remoteDB.GetSanitaryPipes().Returns(remoteSanitaryPipesTable);
      }
    }

    internal static void SetupRemoteStormPipes(IHansenDBAccessor remoteDB)
    {
      using (SynthProcessingDataSet.USP_RESULTDataTable remoteStormPipesTable = new SynthProcessingDataSet.USP_RESULTDataTable())
      {
        remoteStormPipesTable.Rows.Add(100001, "AAA003", "AAA006", "STML", "STMN", 115, 30, 10.3, "1420-002D", "MH", "1420-002D", "MH", "16200", "N", "SIMMONS", "RD", "STM", "IN", 40242, null, "0");
        remoteStormPipesTable.Rows.Add(100004, "AAA006", "AAA007", "STML", "STMN", 326, 30, 9.8, "1420-005D", "MH", "1420-005D", "MH", "15840", "N", "SIMMONS", "RD", "STM", "IN", 39597, null, "0");
        remoteDB.GetStormPipes().Returns(remoteStormPipesTable);
      }
    }

    internal static void SetupMasterLinksTable(IMasterDataDBAccessor localDB)
    {
      using (SynthProcessingDataSet.mst_links_acDataTable mstLinksAcTable = new SynthProcessingDataSet.mst_links_acDataTable())
      {
        mstLinksAcTable.Rows.Add(1, 2, 100026, "AAA028", "AAA027", "CIRC", "S", "S", 59, 12, 0, "VARIES", 0, 0, 42.96, 28.05, "5912", new DateTime(1997, 1, 1), 7621044.7884, 726477.988078, 7621087.60746, 726437.309974, 0.013, "EX", 1, "???G????", "IN", "18000101", "21001231", "", "", "", "", 0, "", 5.499, 286535);
        mstLinksAcTable.Rows.Add(2, 3, 100000, "AAA002", "AAA081", "CIRC", "S", "S", 343.846213342, 18, 0, "RCP", 0, 0, 25.36, 24.84, "5912", new DateTime(1997, 1, 1), 7621614.9955, 725838.913664, 7621361.29259, 725606.263458, 0.013, "EX", 0, "", "", "", "", "", "", "", "", 0, "", 4.094, 644624);
        mstLinksAcTable.Rows.Add(3, 4, 131746, "ABH414", "ABH428", "CIRC", "S", "S", 275, 12, 0, "CSP", 0, 0, 15.80, 15.50, "5912", new DateTime(1997, 1, 1), 123456.0, 234567.0, 345678.0, 456789.0, 0.013, "EX", 0, "", "", "", "", "", "", "", "", 0, "", 4.094, 644624);
        mstLinksAcTable.Rows.Add(4, 5, 200000, "AAA003", "AAA007", "CIRC", "S", "S", 275, 12, 0, "CSP", 0, 0, 15.80, 15.50, "5912", new DateTime(1997, 1, 1), 123456.0, 234567.0, 345678.0, 456789.0, 0.013, "EX", 0, "", "", "", "", "", "", "", "", 0, "", 4.094, 644624);
        mstLinksAcTable.Rows.Add(5, 6, 300000, "ADA357", "AAA123", "CIRC", "S", "S", 275, 12, 0, "CSP", 0, 0, 15.80, 15.50, "5912", new DateTime(1997, 1, 1), 123456.0, 234567.0, 345678.0, 456789.0, 0.013, "EX", 0, "", "", "", "", "", "", "", "", 0, "", 4.094, 644624);
        mstLinksAcTable.Rows.Add(6, 7, 300001, "AAA123", "ADA356", "CIRC", "S", "S", 275, 12, 0, "CSP", 0, 0, 15.80, 15.50, "5912", new DateTime(1997, 1, 1), 123456.0, 234567.0, 345678.0, 456789.0, 0.013, "EX", 0, "", "", "", "", "", "", "", "", 0, "", 4.094, 644624);
        localDB.MasterLinks.Returns(mstLinksAcTable);
      }
    }

    internal static void SetupHansenAddresses(IHansenDBAccessor remoteDB)
    {
      using (SynthProcessingDataSet.ADDRESS_GEOCODEDDataTable addressTable = new SynthProcessingDataSet.ADDRESS_GEOCODEDDataTable())
      {
        addressTable.Rows.Add("CONVERSION", new DateTime(2000,6,6), 29706, "A", null, null, "PORTLAND",
          null, null, null, null, null, null, 0, null, "TOOL", new DateTime(2001,10,19),
          null, "SE", null, null, null, null, null, null, null, null, "OR", "13TH", " 7614",
          null, null, null, null, "AVE", null, 7649853.09259, 664765.3799933, 2, " 7614 SE 13TH AVE");
        remoteDB.GetAddresses().Returns(addressTable);
      }
    }

    internal void CreateHansenSegments(SynthProcessingDataSet.All_Hansen_SegsDataTable hansenSegments,
      Func<SynthProcessingDataSet.USP_RESULTDataTable> resultFunc)
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
          UNITID2, r.UNITTYPE, r.COMPCODE, r.PIPELEN, r.PIPEDIAM, r.PIPEHT, null, null
          , null, null, null, null, null, null, null, r.MHFROM, r.MHFROM_TYPE, r.MHTO,
          r.MHTO_TYPE, null, null, r.STNO, r.PREDIR, r.STNAME, r.SUFFIX, r.SYNTH_FLAG,
          null, null, null, null, null, null, null, r.SOURCE, r.SERVSTAT, r.ADDRKEY, r
          .SPECINST);
          MAPINFO_ID_Counter++;
        }
      }
    }

    private void SetupHansenData()
    {
      hansenMock = Substitute.For<IHansenDBAccessor>();

      SetupRemoteDitches(hansenMock);
      SetupRemoteSanitaryPipes(hansenMock);
      SetupRemoteStormPipes(hansenMock);
      SetupHansenAddresses(hansenMock);
      SynthProcessingDataSet.All_Hansen_SegsDataTable hansenSegments =
        new SynthProcessingDataSet.All_Hansen_SegsDataTable();
      CreateHansenSegments(hansenSegments, hansenMock.GetDitches);
      CreateHansenSegments(hansenSegments, hansenMock.GetSanitaryPipes);
      CreateHansenSegments(hansenSegments, hansenMock.GetStormPipes);
      hansenMock.HansenSegments.Returns(hansenSegments);
    }

    private void SetupMasterData()
    {
      masterDataMock = Substitute.For<IMasterDataDBAccessor>();
      SetupMasterNodesTable(masterDataMock);
      SetupMasterLinksTable(masterDataMock);
    }

    [SetUp]
    public void Setup()
    {
      SetupHansenData();
      SetupMasterData();
    }

    [Test]
    public void TestMapNodesFilter()
    {
      SynthDataProcessor localData = new SynthDataProcessor(masterDataMock, hansenMock);
      const int numFilterRows = 2;
      int numMasterNodes = masterDataMock.MasterNodes.Count;
      localData.ClearMapNodes();
      localData.AppendMasterNodesToMapNodes();

      Assert.That(localData.MapNodes.Rows.Count, Is.EqualTo(numMasterNodes),
        string.Format("Number of nodes = {0}", numMasterNodes));

      var filterQuery = from m in localData.MapNodes
                        where m.Node.StartsWith("Aban") || m.Node.StartsWith("CWS")
                        select m;

      Assert.That(filterQuery.Count(), Is.EqualTo(numFilterRows),
        "Number of nodes to filter = 2");

      localData.FilterMapNodes();

      Assert.That(localData.MapNodes.Rows.Count, Is.EqualTo(numMasterNodes - numFilterRows),
        "Number of nodes left = 3");
      Assert.That(filterQuery.Count(), Is.EqualTo(0),
        "Number of nodes to filter = 0");
    }

    [Test]
    public void TestInitSynthFlag()
    {
      SynthDataProcessor localData = new SynthDataProcessor(masterDataMock, hansenMock);

      foreach (SynthProcessingDataSet.All_Hansen_SegsRow r in localData.HansenSegments.Rows)
      {
        r.Synth_Flag = "X";
      }

      var q = from s in localData.HansenSegments
              where s.Synth_Flag != SynthDataProcessor.SynthFlagDefault
              select s;

      Assert.That(q.Count(), Is.EqualTo(localData.HansenSegments.Count));

      localData.InitSynthFlag();

      Assert.That(q.Count(), Is.EqualTo(0));

    }

    #region SynthFlaggingTests

    [Test]
    public void TestMatchBothNodes()
    {
      // Set up precondition
      SynthDataProcessor localData = new SynthDataProcessor(masterDataMock, hansenMock);

      localData.AppendMasterNodesToMapNodes();

      // Actual test
      var q1 = from s in localData.HansenSegments
               where s.Synth_Flag == SynthDataProcessor.SynthFlagBothNodesMatch
               select s;

      Assert.That(q1.Count(), Is.EqualTo(0));
      localData.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchBothNodesOfSameSegment);
      Assert.That(q1.Count(), Is.GreaterThan(0));

      List<int> compkeyList = new List<int>() { 100000 };
      var q2 = from s in localData.HansenSegments
               where compkeyList.Contains(s.COMPKEY) && s.Synth_Flag.Equals(
                SynthDataProcessor.SynthFlagBothNodesMatch)
               select s;
      Assert.That(compkeyList.Count, Is.EqualTo(q2.Count()));
    }

    [Test]
    public void TestMatchNodesAcrossTwoSegments()
    {
      // Set up precondition
      SynthDataProcessor localData = new SynthDataProcessor(masterDataMock, hansenMock);

      localData.AppendMasterNodesToMapNodes();

      // Actual test
      var q1 = from s in localData.HansenSegments
              where s.Synth_Flag == SynthDataProcessor.SynthFlagNodesMatchAcrossTwoSegments
              select s;

      Assert.That(q1.Count(), Is.EqualTo(0));
      localData.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchBothNodesOfSameSegment);
      localData.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchNodesAcrossTwoSegments);
      Assert.That(q1.Count(), Is.GreaterThan(0));

      List<int> compkeyList = new List<int>() { 173067 };
      var q2 = from s in localData.HansenSegments
               where compkeyList.Contains(s.COMPKEY) && s.Synth_Flag.Equals(
                 SynthDataProcessor.SynthFlagNodesMatchAcrossTwoSegments)
               select s;
      Assert.That(compkeyList.Count, Is.EqualTo(q2.Count()));
    }

    [Test]
    public void TestMatchNodesOnly()
    {
      // Set up precondition
      SynthDataProcessor localData = new SynthDataProcessor(masterDataMock, hansenMock);

      localData.AppendMasterNodesToMapNodes();

      // Actual test
      var q = from s in localData.HansenSegments
              where s.Synth_Flag == SynthDataProcessor.SynthFlagNodesMatchDifferentSegments
              select s;

      Assert.That(q.Count(), Is.EqualTo(0));
      localData.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchBothNodesOfSameSegment);
      localData.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchNodesAcrossTwoSegments);
      localData.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchNodesOfDifferentSegments);
      Assert.That(q.Count(), Is.GreaterThan(0));

      List<int> compkeyList = new List<int>() { 100196, 100198, 100342, 440596, 
        100001, 100004 };
      var q2 = from s in localData.HansenSegments
               where compkeyList.Contains(s.COMPKEY) && s.Synth_Flag.Equals(
                 SynthDataProcessor.SynthFlagNodesMatchDifferentSegments)
               select s;
      Assert.That(compkeyList.Count, Is.EqualTo(q2.Count()));
    }

    [Test]
    public void TestUpdateUpstreamSynthNodes()
    {
      // Set up precondition
      SynthDataProcessor localData = new SynthDataProcessor(masterDataMock, hansenMock);

      localData.AppendMasterNodesToMapNodes();

      localData.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchBothNodesOfSameSegment);
      localData.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchNodesAcrossTwoSegments);
      localData.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchNodesOfDifferentSegments);
      localData.SynthNodes = localData.AssembleSynthNodes();

      // Actual test
      localData.UpdateSynthNodes(localData.SynthNodes, 
        SynthDataProcessor.SynthNodeUpdateSelector.Upstream);

      List<string> nodeList = new List<string>() { "XYZ123" };
      var q1 = from n in localData.SynthNodes
               where n.Source.Equals("initial_from") && nodeList.Contains(n.wcms_id)
               select n;

      Assert.That(q1.Count(), Is.EqualTo(nodeList.Count));
    }

    [Test]
    public void TestUpdateDownstreamSynthNodes()
    {
      // Set up precondition
      SynthDataProcessor localData = new SynthDataProcessor(masterDataMock, hansenMock);

      localData.AppendMasterNodesToMapNodes();

      localData.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchBothNodesOfSameSegment);
      localData.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchNodesAcrossTwoSegments);
      localData.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchNodesOfDifferentSegments);
      localData.SynthNodes = localData.AssembleSynthNodes();

      // Actual test
      localData.UpdateSynthNodes(localData.SynthNodes, 
        SynthDataProcessor.SynthNodeUpdateSelector.Downstream);

      List<string> nodeList = new List<string>() { "ABH425" };
      var q1 = from n in localData.SynthNodes
               where n.Source.Equals("initial_to") && nodeList.Contains(n.wcms_id)
               select n;

      Assert.That(q1.Count(), Is.EqualTo(nodeList.Count));
    }

    [Test]
    public void TestUpdateSynthNodes()
    {
      // Set up precondition
      SynthDataProcessor localData = new SynthDataProcessor(masterDataMock, hansenMock);

      localData.AppendMasterNodesToMapNodes();

      localData.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchBothNodesOfSameSegment);
      localData.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchNodesAcrossTwoSegments);
      localData.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchNodesOfDifferentSegments);
      localData.SynthNodes = localData.AssembleSynthNodes();

      // Actual test
      localData.UpdateSynthNodes(localData.SynthNodes, SynthDataProcessor.SynthNodeUpdateSelector.All);

      List<string> usNodeList = new List<string>() { "XYZ123" };
      var q1 = from n in localData.SynthNodes
               where n.Source.Equals("initial_from") && usNodeList.Contains(n.wcms_id)
               select n;
      Assert.That(q1.Count(), Is.EqualTo(usNodeList.Count));

      List<string> dsNodeList = new List<string>() { "ABH425" };
      var q2 = from n in localData.SynthNodes
               where n.Source.Equals("initial_to") && dsNodeList.Contains(n.wcms_id)
               select n;

      Assert.That(q2.Count(), Is.EqualTo(dsNodeList.Count));
    }

    [Test]
    public void TestUpdateNodesWithAddresses()
    {
      // Set up precondition
      SynthDataProcessor localData = new SynthDataProcessor(masterDataMock, hansenMock);

      localData.AppendMasterNodesToMapNodes();

      localData.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchBothNodesOfSameSegment);
      localData.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchNodesAcrossTwoSegments);
      localData.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchNodesOfDifferentSegments);
      localData.SynthNodes = localData.AssembleSynthNodes();
      localData.UpdateSynthNodes(localData.SynthNodes, SynthDataProcessor.SynthNodeUpdateSelector.All);

      // Actual test
      localData.UpdateSynthNodesWithAddresses(localData.SynthNodes);

      List<string> addressNodeList = new List<string>() { "XYZ126" };
      var q1 = from n in localData.SynthNodes
               where n.Source.Equals("address") && addressNodeList.Contains(n.wcms_id)
               select n;

      Assert.That(q1.Count(), Is.EqualTo(addressNodeList.Count));
    }

    [Test]
    public void TestUpdateSegmentsWithSynthNodes()
    {
      // Set up precondition
      SynthDataProcessor localData = new SynthDataProcessor(masterDataMock, hansenMock);

      localData.AppendMasterNodesToMapNodes();

      localData.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchBothNodesOfSameSegment);
      localData.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchNodesAcrossTwoSegments);
      localData.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchNodesOfDifferentSegments);
      localData.SynthNodes = localData.AssembleSynthNodes();
      localData.UpdateSynthNodes(localData.SynthNodes, SynthDataProcessor.SynthNodeUpdateSelector.All);
      localData.UpdateSynthNodesWithAddresses(localData.SynthNodes);

      // Actual test
      List<int> usTestSegmentList = new List<int>() { 900000, 900001, 900003 };
      var q1 = from s in localData.HansenSegments
               where usTestSegmentList.Contains(s.COMPKEY) &&
                 ((s["From_X"] != DBNull.Value) && (s.From_X > 0)) &&
                 ((s["From_Y"] != DBNull.Value) && (s.From_Y > 0)) &&
                 s.Source.Equals("update_from")
               select s;
      List<int> dsTestSegmentList = new List<int>() { 131746, 900002 };
      var q2 = from s in localData.HansenSegments
               where dsTestSegmentList.Contains(s.COMPKEY) &&
                 ((s["To_X"] != DBNull.Value) && (s.To_X > 0)) &&
                 ((s["To_Y"] != DBNull.Value) && (s.To_Y > 0)) &&
                 s.Source.Equals("update_to")
               select s;
      Assert.That(q1.Count(), Is.EqualTo(0));
      Assert.That(q2.Count(), Is.EqualTo(0));
      localData.UpdateSegmentsWithSynthNodes(localData.SynthNodes);
      Assert.That(q1.Count(), Is.EqualTo(usTestSegmentList.Count));
      Assert.That(q2.Count(), Is.EqualTo(dsTestSegmentList.Count));
    }

    [Test]
    public void TestFlagSegmentsWithUpdatedCoords()
    {
      // Set up precondition
      SynthDataProcessor localData = new SynthDataProcessor(masterDataMock, hansenMock);

      localData.AppendMasterNodesToMapNodes();

      localData.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchBothNodesOfSameSegment);
      localData.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchNodesAcrossTwoSegments);
      localData.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchNodesOfDifferentSegments);
      localData.SynthNodes = localData.AssembleSynthNodes();
      localData.UpdateSynthNodes(localData.SynthNodes, SynthDataProcessor.SynthNodeUpdateSelector.All);
      localData.UpdateSynthNodesWithAddresses(localData.SynthNodes);
      localData.UpdateSegmentsWithSynthNodes(localData.SynthNodes);

      // Actual test
      List<int> segmentList = new List<int>() { 900000, 131746, 900002 };
      var q1 = from s in localData.HansenSegments
               where s.Synth_Flag.Equals(
                 SynthDataProcessor.SynthFlagSegmentsWithToFromCoords) &&
                 segmentList.Contains(s.COMPKEY)
               select s;
      Assert.That(q1.Count(), Is.EqualTo(0));
      localData.FlagSegmentsWithUpdatedCoords();
      Assert.That(q1.Count(), Is.EqualTo(segmentList.Count));
    }

    [Test]
    public void TestFlagRemainingSegmentsWithValidNodes()
    {
      // Set up precondition
      SynthDataProcessor localData = new SynthDataProcessor(masterDataMock, hansenMock);

      localData.AppendMasterNodesToMapNodes();

      localData.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchBothNodesOfSameSegment);
      localData.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchNodesAcrossTwoSegments);
      localData.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchNodesOfDifferentSegments);
      localData.SynthNodes = localData.AssembleSynthNodes();
      localData.UpdateSynthNodes(localData.SynthNodes, SynthDataProcessor.SynthNodeUpdateSelector.All);
      localData.UpdateSynthNodesWithAddresses(localData.SynthNodes);
      localData.UpdateSegmentsWithSynthNodes(localData.SynthNodes);
      localData.FlagSegmentsWithUpdatedCoords();

      // Actual test
      List<int> segmentList = new List<int>() { 900001, 900003, 900005 };
      var q1 = from s in localData.HansenSegments
               where s.Synth_Flag.Equals(
                SynthDataProcessor.SynthFlagSegmentsWithValidLicensePlates) &&
                segmentList.Contains(s.COMPKEY)
               select s;
      Assert.That(q1.Count(), Is.EqualTo(0));
      localData.FlagRemainingSegmentsWithValidNodes();
      Assert.That(q1.Count(), Is.EqualTo(segmentList.Count));
    }

    [Test]
    public void TestRemainingUnsynthedNodes()
    {
      // Set up precondition
      SynthDataProcessor localData = new SynthDataProcessor(masterDataMock, hansenMock);

      localData.AppendMasterNodesToMapNodes();

      localData.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchBothNodesOfSameSegment);
      localData.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchNodesAcrossTwoSegments);
      localData.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchNodesOfDifferentSegments);
      localData.SynthNodes = localData.AssembleSynthNodes();
      localData.UpdateSynthNodes(localData.SynthNodes, SynthDataProcessor.SynthNodeUpdateSelector.All);
      localData.UpdateSynthNodesWithAddresses(localData.SynthNodes);
      localData.UpdateSegmentsWithSynthNodes(localData.SynthNodes);
      localData.FlagSegmentsWithUpdatedCoords();
      localData.FlagRemainingSegmentsWithValidNodes();

      List<int> segmentList = new List<int>() { 900004 };
      var q1 = from s in localData.HansenSegments
               where s.Synth_Flag.Equals(SynthDataProcessor.SynthFlagDefault) &&
                 segmentList.Contains(s.COMPKEY)
               select s;
      Assert.That(q1.Count(), Is.EqualTo(segmentList.Count));
    }

    [Test]
    public void TestFirstLadder()
    {
      // Set up precondition
      SynthDataProcessor localData = new SynthDataProcessor(masterDataMock, hansenMock);

      localData.AppendMasterNodesToMapNodes();

      localData.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchBothNodesOfSameSegment);
      localData.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchNodesAcrossTwoSegments);
      localData.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchNodesOfDifferentSegments);
      localData.SynthNodes = localData.AssembleSynthNodes();
      localData.UpdateSynthNodes(localData.SynthNodes, SynthDataProcessor.SynthNodeUpdateSelector.All);

      // Actual test
      List<string> nodeList1 = new List<string>() { "XYZ127" };
      var q1 = from s in localData.HansenSegments
               join m in localData.SynthNodes on s.UNITID equals m.wcms_id
               join n in localData.SynthNodes on s.UNITID2 equals n.wcms_id
               where (m.IsX_CoordNull() || m.X_Coord == 0) &&
                 (m.IsY_CoordNull() || m.Y_Coord == 0) &&
                 (!n.IsX_CoordNull() && n.X_Coord != 0) &&
                 (!n.IsY_CoordNull() && n.Y_Coord != 0) &&
                 nodeList1.Contains(m.wcms_id)
               select m.wcms_id;
      List<string> nodeList2 = new List<string>() { "XYZ124" };
      var q2 = from s in localData.HansenSegments
               join m in localData.SynthNodes on s.UNITID equals m.wcms_id
               join n in localData.SynthNodes on s.UNITID2 equals n.wcms_id
               where (!m.IsX_CoordNull() && m.X_Coord != 0) &&
                 (!m.IsY_CoordNull() && m.Y_Coord != 0) &&
                 (n.IsX_CoordNull() || n.X_Coord == 0) &&
                 (n.IsY_CoordNull() || n.Y_Coord == 0) &&
                 nodeList2.Contains(n.wcms_id)
               select n.wcms_id;
      Assert.That(q1.Count(), Is.EqualTo(nodeList1.Count));
      Assert.That(q1.Count(), Is.EqualTo(nodeList2.Count));
      localData.UpdateSynthNodesLadder(localData.SynthNodes);
      Assert.That(q1.Count(), Is.EqualTo(0));
      Assert.That(q2.Count(), Is.EqualTo(0));
    }

    [Test]
    public void TestAllLadder()
    {
      // Set up precondition
      SynthDataProcessor localData = new SynthDataProcessor(masterDataMock, hansenMock);

      localData.AppendMasterNodesToMapNodes();

      localData.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchBothNodesOfSameSegment);
      localData.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchNodesAcrossTwoSegments);
      localData.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchNodesOfDifferentSegments);
      localData.SynthNodes = localData.AssembleSynthNodes();
      localData.UpdateSynthNodes(localData.SynthNodes, SynthDataProcessor.SynthNodeUpdateSelector.All);
      localData.UpdateSynthNodesLadder(localData.SynthNodes);
      localData.UpdateSynthNodesWithAddresses(localData.SynthNodes);
      localData.UpdateSynthNodesLadder(localData.SynthNodes);
      localData.UpdateSegmentsWithSynthNodes(localData.SynthNodes);
      localData.FlagSegmentsWithUpdatedCoords();
      localData.FlagRemainingSegmentsWithValidNodes();

      List<int> segmentList = new List<int>() { 900004 };
      var q1 = from s in localData.HansenSegments
               where s.Synth_Flag.Equals(SynthDataProcessor.SynthFlagDefault) &&
                 segmentList.Contains(s.COMPKEY)
               select s;
      Assert.That(q1.Count(), Is.EqualTo(segmentList.Count));
    }
    #endregion

  }
}
