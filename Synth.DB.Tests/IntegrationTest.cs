using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Synth.DB
{
  [TestFixture]
  [Category("Integration")]
  public class IntegrationTest
  {
    private string ditchFileName;
    string masterLinksConnectionString;
    string masterNodesConnectionString;
    private string sanitaryFileName;
    private string stormFileName;
    private string mifMidBaseFileName;
    private string addressFileName;
    private string debugFileName;
    
    [SetUp]
    public void Setup()
    {
      masterLinksConnectionString =
        @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\SAMaster_22\Links\mst_links_ac.mdb";
      masterNodesConnectionString =
        @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\SAMaster_22\Nodes\mst_nodes_ac.mdb";
      ditchFileName = @"C:\SAMaster_22\Hansen\USP_SYNTH_GET_STORM_DITCHES.csv";
      sanitaryFileName = @"C:\SAMaster_22\Hansen\USP_SYNTH_GET_SANITARY_PIPES.csv";
      stormFileName = @"C:\SAMaster_22\Hansen\USP_SYNTH_GET_STORM_PIPES.csv";
      addressFileName = @"C:\SAMaster_22\Hansen\ADDRESS_GEOCODED.csv";
      mifMidBaseFileName = @"C:\SAMaster_22\Hansen\TestMifMid";
      debugFileName = @"C:\SAMaster_22\Hansen\debug.txt";
    }

    [Test]
    public void TestAccessMasterLinks()
    {
      MasterDataAccessor accessor = new MasterDataAccessor(masterLinksConnectionString, 
        masterNodesConnectionString);

      Assert.That(accessor.MasterLinks.Count, Is.GreaterThan(0));
      Console.WriteLine(string.Format("# MasterLinks = {0}", accessor.MasterLinks.Count));
      Assert.That(accessor.MasterNodes.Count, Is.GreaterThan(0));
      Console.WriteLine(string.Format("# MasterNodes = {0}", accessor.MasterNodes.Count));
    }

    [Test]
    public void TestAccessTestHansen()
    {
      TestCSVHansenAccessor accessor = new TestCSVHansenAccessor(
        ditchFileName, sanitaryFileName, stormFileName, addressFileName);

      Assert.That(accessor.HansenSegments.Count, Is.GreaterThan(0));
      Console.WriteLine(string.Format("# Hansen Segments = {0}", accessor.HansenSegments.Count));
    }

    [Test]
    public void TestSynth()
    {
      System.Diagnostics.Stopwatch overallTimer = new System.Diagnostics.Stopwatch();

      overallTimer.Start();

      TestCSVHansenAccessor hansenAccessor = new TestCSVHansenAccessor(ditchFileName, sanitaryFileName,
        stormFileName, addressFileName);
      MasterDataAccessor masterDataAccessor = new MasterDataAccessor(masterLinksConnectionString,
        masterNodesConnectionString);

      SynthDataProcessor processor = new SynthDataProcessor(masterDataAccessor, hansenAccessor);

      var q1 = processor.HansenSegments.Count;
      Console.WriteLine(string.Format("# Hansen segments = {0}, # Master Links = {1}",
        processor.HansenSegments.Count, processor.MasterLinks.Count));

      processor.AppendMasterNodesToMapNodes();

      //------------------------------------------------------------------------------------------
      System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
      timer.Start();
      processor.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchBothNodesOfSameSegment);
      timer.Stop();

      var q2 = (from s in processor.HansenSegments
                where s.Synth_Flag == SynthDataProcessor.SynthFlagBothNodesMatch
                select s).Count();
      Console.WriteLine(string.Format("# Synth=1: {0}; Time: {1} s", q2, timer.Elapsed.TotalSeconds));
      Assert.That(q2, Is.GreaterThan(0));

      //------------------------------------------------------------------------------------------
      timer.Restart();
      processor.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchNodesAcrossTwoSegments);
      timer.Stop();

      var q3 = (from s in processor.HansenSegments
                where s.Synth_Flag == SynthDataProcessor.SynthFlagNodesMatchAcrossTwoSegments
                select s).Count();
      Console.WriteLine(string.Format("# Synth=3: {0}; Time: {1} s", q3, timer.Elapsed.TotalSeconds));
      Assert.That(q3, Is.GreaterThan(0));

      //------------------------------------------------------------------------------------------
      timer.Restart();
      processor.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchNodesOfDifferentSegments);
      timer.Stop();

      var q4 = (from s in processor.HansenSegments
                where s.Synth_Flag == SynthDataProcessor.SynthFlagNodesMatchDifferentSegments
                select s).Count();
      Console.WriteLine(string.Format("# Synth=7: {0}; Time: {1} s", q4, timer.Elapsed.TotalSeconds));
      Assert.That(q4, Is.GreaterThan(0));

      //------------------------------------------------------------------------------------------
      timer.Restart();
      processor.SynthNodes = processor.AssembleSynthNodes();
      timer.Stop();

      var q5 = (from n in processor.SynthNodes
                select n).Count();
      var q5a = (from n in processor.SynthNodes
                 where n.Source == "AddStep1"
                 select n).Count();
      var q5b = (from n in processor.SynthNodes
                 where n.Source == "AddStep2"
                 select n).Count();
      Console.WriteLine(string.Format("# Synth Nodes = {0}; Time: {1} s", q5, timer.Elapsed.TotalSeconds));
      Console.WriteLine(string.Format("  # AddStep1 = {0}", q5a));
      Console.WriteLine(string.Format("  # AddStep2 = {0}", q5b));
      Assert.That(q5, Is.GreaterThan(0));

      //------------------------------------------------------------------------------------------
      timer.Restart();
      processor.UpdateSynthNodes(processor.SynthNodes, SynthDataProcessor.SynthNodeUpdateSelector.All);
      timer.Stop();

      var q6a = (from n in processor.SynthNodes
                where n.Source == "initial_from"
                select n).Count();
      var q6b = (from n in processor.SynthNodes
                 where n.Source == "initial_to"
                 select n).Count();
      Console.WriteLine(string.Format("Update Synth Nodes time: {0} s", timer.Elapsed.TotalSeconds));
      Console.WriteLine(string.Format("  # initial_from = {0}", q6a));
      Console.WriteLine(string.Format("  # initial_to = {0}", q6b));

      //------------------------------------------------------------------------------------------
      timer.Restart();
      processor.UpdateSynthNodesLadder(processor.SynthNodes);
      timer.Stop();

      Console.WriteLine(string.Format("First ladder time: {0} s", timer.Elapsed.TotalSeconds));

      //------------------------------------------------------------------------------------------
      timer.Restart();
      processor.UpdateSynthNodesWithAddresses(processor.SynthNodes);
      timer.Stop();

      Console.WriteLine(string.Format("Addresses time: {0} s", timer.Elapsed.TotalSeconds));

      //------------------------------------------------------------------------------------------
      timer.Restart();
      processor.UpdateSynthNodesLadder(processor.SynthNodes);
      timer.Stop();

      Console.WriteLine(string.Format("Second ladder time: {0} s", timer.Elapsed.TotalSeconds));

      //------------------------------------------------------------------------------------------
      timer.Restart();
      processor.UpdateSegmentsWithSynthNodes(processor.SynthNodes);
      timer.Stop();

      Console.WriteLine(string.Format("Segment update with synths time: {0} s", timer.Elapsed.TotalSeconds));

      //------------------------------------------------------------------------------------------
      timer.Restart();
      processor.FlagSegmentsWithUpdatedCoords();
      timer.Stop();

      Console.WriteLine(string.Format("Segments with coords flagged: {0} s", timer.Elapsed.TotalSeconds));

      //------------------------------------------------------------------------------------------
      timer.Restart();
      processor.FlagRemainingSegmentsWithValidNodes();
      timer.Stop();

      Console.WriteLine(string.Format("Remaining segments flagged: {0} s", timer.Elapsed.TotalSeconds));

      //------------------------------------------------------------------------------------------
      timer.Restart();
      SynthExporter exporter = new SynthExporter(processor);
      MifMidWriter mifWriter = new MifMidWriter(mifMidBaseFileName + ".MIF");
      MifMidWriter midWriter = new MifMidWriter(mifMidBaseFileName + ".MID");
      exporter.WriteSynthSegmentMifMidHeader(mifWriter);
      exporter.WriteSynthSegmentMifMidMifData(mifWriter);
      exporter.WriteSynthSegmentMifMidMidData(midWriter);
      timer.Stop();

      Console.WriteLine(string.Format("MifMid writing: {0} s", timer.Elapsed.TotalSeconds));

      overallTimer.Stop();
      Console.WriteLine(string.Format("Overall time: {0} m", overallTimer.Elapsed.TotalMinutes));

      // dump HansenSegs out
      using (System.IO.StreamWriter debugStream = new System.IO.StreamWriter(debugFileName))
      {
        string[] flagsToExport = new string[] { "3", "4", "7" };
        var hansenFlaggedDump = from s in processor.HansenSegments
                                where flagsToExport.Contains(s.Synth_Flag)
                                orderby s.COMPKEY ascending
                                select s;
        foreach (var s in hansenFlaggedDump)
        {
          StringBuilder sb = new StringBuilder();
          foreach (var o in s.ItemArray)
          {
            sb.Append(o);
            sb.Append(",");
          }
          sb.Remove(sb.Length - 1, 1);
          debugStream.WriteLine(sb.ToString());
        }

        debugStream.WriteLine("----------");

        var synthNodeDump = from n in processor.SynthNodes
                            orderby n.wcms_id ascending
                            select n;
        foreach (var n in synthNodeDump)
        {
          StringBuilder sb = new StringBuilder();
          foreach (var o in n.ItemArray)
          {
            sb.Append(o);
            sb.Append(",");
          }
          sb.Remove(sb.Length - 1, 1);
          debugStream.WriteLine(sb.ToString());
        }
      }

    }
  }
}
