using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text.RegularExpressions;
using Synth.Logging;

namespace Synth.DB
{
  public class SynthDataProcessor : ISynthData
  {
    public const string SynthFlagDefault = "0";
    public const string SynthFlagBothNodesMatch = "1";
    public const string SynthFlagNodesMatchAcrossTwoSegments = "3";
    public const string SynthFlagNodesMatchDifferentSegments = "7";
    public const string SynthFlagSegmentsWithToFromCoords = "4";
    public const string SynthFlagSegmentsWithValidLicensePlates = "9";

    private readonly IMasterDataDBAccessor masterDataAccessor;
    private readonly IHansenDBAccessor hansenAccessor;

    public SynthProcessingDataSet.All_Hansen_SegsDataTable HansenSegments { get; set; }
    public SynthProcessingDataSet.synth_nodesDataTable SynthNodes { get; set; }
    public SynthProcessingDataSet.tMapNodesDataTable MapNodes { get; private set; }
    public SynthProcessingDataSet.mst_links_acDataTable MasterLinks { get; private set; }
    public SynthProcessingDataSet.mst_nodes_acDataTable MasterNodes { get; private set; }
    public SynthProcessingDataSet.ADDRESS_GEOCODEDDataTable Addresses { get; private set; }

    Logger logger;
    /// <summary>
    /// Create a SynthDataProcessor used to analyze differences in Hansen data
    /// from ASM Master data
    /// </summary>
    /// <param name="masterDB">ASM Master data to compare against</param>
    /// <param name="hansenDB">Bureau Hansen data source for comparing</param>
    public SynthDataProcessor(IMasterDataDBAccessor masterDB, IHansenDBAccessor hansenDB)
    {
      masterDataAccessor = masterDB;
      hansenAccessor = hansenDB;

      HansenSegments = hansenAccessor.HansenSegments;
      MapNodes = new SynthProcessingDataSet.tMapNodesDataTable();
      SynthNodes = new SynthProcessingDataSet.synth_nodesDataTable();
      MasterLinks = masterDataAccessor.MasterLinks;
      Addresses = hansenAccessor.GetAddresses();
      MasterNodes = masterDataAccessor.MasterNodes;

      logger = new Logger(this.GetType().Name);
    }

    /// <summary>
    /// Clears the Synth Nodes from the processor
    /// </summary>
    public void ClearSynthNodes()
    {
      logger.Debug("Enter ClearSynthNodes");
      SynthNodes.Rows.Clear();
      logger.Debug("Exit ClearSynthNodes");
    }

    /// <summary>
    /// Clears the Mapped Master Nodes from the processor
    /// </summary>
    public void ClearMapNodes()
    {
      logger.Debug("Enter ClearMapNodes");
      MapNodes.Rows.Clear();
      logger.Debug("Exit ClearMapNodes");
    }

    /// <summary>
    /// Appends Master Nodes to the Mapped Master Nodes in the processor
    /// </summary>
    public void AppendMasterNodesToMapNodes()
    {
      logger.Debug("Enter AppendMasterNodesToMapNodes");
      logger.Debug("  {0} MasterNodes", MasterNodes.Count);
      foreach (var r in MasterNodes)
      {
        if (!r.IsNodeNull())
        {
          MapNodes.Rows.Add(r.Node, r.XCoord, r.YCoord);
        }
      }
      logger.Debug("Exit AppendMasterNodesToMapNodes");
    }

    /// <summary>
    /// Filters out abandoned and Clean Water Services pipes from the Mapped
    /// Master Nodes
    /// </summary>
    public void FilterMapNodes()
    {
      logger.Debug("Enter FilterMapNodes");

      var nodesToDelete =
        (from r in MapNodes
         where r.Node.StartsWith("Aban") || r.Node.StartsWith("CWS")
         select r).ToArray();
      foreach (SynthProcessingDataSet.tMapNodesRow r in nodesToDelete)
      {
        MapNodes.Rows.Remove(r);
      }
      logger.Debug("Exit FilterMapNodes");
    }

    /// <summary>
    /// Initializes the synth flags for all Hansen Segments
    /// </summary>
    public void InitSynthFlag()
    {
      logger.Debug("Enter InitSynthFlag");
      if (HansenSegments.Count > 0)
      {
        foreach (SynthProcessingDataSet.All_Hansen_SegsRow r in HansenSegments.Rows)
        {
          r.Synth_Flag = SynthFlagDefault;
        }
      }
      logger.Debug("Exit InitSynthFlag");
    }

    private EnumerableRowCollection<SynthProcessingDataSet.mst_links_acRow>
      ExistingMasterLinks()
    {
      var existingTimeFrames = new string[] { "EX", "AE", "CE" };
      var filteredMasterLinksToExisting =
        from s in MasterLinks
        where existingTimeFrames.Contains(s.TimeFrame)
        select s;
      return filteredMasterLinksToExisting;
    }

    /// <summary>
    /// Type of matching to perform
    /// </summary>
    public enum SynthMatchEnum 
      { MatchBothNodesOfSameSegment, 
        MatchNodesAcrossTwoSegments,
        MatchNodesOfDifferentSegments};

    /// <summary>
    /// Flag Hansen Segments according to how they match with Master Links
    /// </summary>
    /// <param name="matchType">Type of matching</param>
    public void FlagNodesMatch(SynthMatchEnum matchType)
    {
      logger.Debug("Enter FlagNodesMatch {0}", Enum.GetName(typeof(SynthMatchEnum), matchType));
      switch (matchType)
      {
        case SynthMatchEnum.MatchBothNodesOfSameSegment:
          FlagBothNodesMatch();
          break;
        case SynthMatchEnum.MatchNodesAcrossTwoSegments:
          FlagNodesMatchAcrossTwoSegments();
          break;
        case SynthMatchEnum.MatchNodesOfDifferentSegments:
          FlagNodesMatchDifferentSegments();
          break;
      }
      logger.Debug("Exit FlagNodesMatch");
    }

    internal SynthProcessingDataSet.NodeMatchCoordsDataTable
      GetMatchingSegmentsWithNodes(SynthDataProcessor.SynthMatchEnum matchType)
    {
       // Filter the available master links to existing timeframes
      var filteredMasterLinksToExisting = ExistingMasterLinks();

      // Set up collection of master links
      var qMapLinks = from f in filteredMasterLinksToExisting
                      select new
                      {
                        MLinkID = f.MLinkID,
                        USNode = f.USNode,
                        Compkey = f.CompKey,
                        DSNode = f.DSNode,
                        XD_UP = f.FromX,
                        XD_DN = f.ToX,
                        ID_GLBL = string.Empty,
                        XD_ID = string.Empty
                      };

      // Set up collection of map nodes
      var qMapNodes = from n in MapNodes
                      select n;

      SynthProcessingDataSet.NodeMatchCoordsDataTable matchTable =
        new SynthProcessingDataSet.NodeMatchCoordsDataTable();
      switch (matchType)
      {
        case SynthMatchEnum.MatchBothNodesOfSameSegment:
          var qMatchBothNodes =
            from s in HansenSegments
            join l in qMapLinks on new { USNode = s.UNITID, DSNode = s.UNITID2 }
              equals new { USNode = l.USNode, DSNode = l.DSNode }
            join nup in qMapNodes on l.USNode equals nup.Node
            join ndn in qMapNodes on l.DSNode equals ndn.Node
            select new
            {
              s.COMPKEY,
              s.Synth_Flag,
              From_X = nup.XCoord,
              From_Y = nup.YCoord,
              To_X = ndn.XCoord,
              To_Y = ndn.YCoord
            };
          foreach (var q in qMatchBothNodes)
          {
            matchTable.Rows.Add(q.COMPKEY, q.Synth_Flag, 
              q.From_X, q.From_Y, q.To_X, q.To_Y);
          }
          break;
        case SynthMatchEnum.MatchNodesAcrossTwoSegments:
          var qMatchNodesAcrossTwoSegs =
            (
              from s in HansenSegments
              join l in qMapLinks on s.UNITID equals l.USNode
              join m in qMapLinks on s.UNITID2 equals m.DSNode
              join nup in qMapNodes on l.USNode equals nup.Node
              join ndn in qMapNodes on m.DSNode equals ndn.Node
              where (s.Synth_Flag == SynthFlagDefault) && (l.DSNode == m.USNode)
              select new
              {
                s.COMPKEY,
                s.Synth_Flag,
                From_X = nup.XCoord,
                From_Y = nup.YCoord,
                To_X = ndn.XCoord,
                To_Y = ndn.YCoord
              }
            ).Distinct();
          foreach (var q in qMatchNodesAcrossTwoSegs)
          {
            matchTable.Rows.Add(q.COMPKEY, q.Synth_Flag, 
              q.From_X, q.From_Y, q.To_X, q.To_Y);
          }
          break;
      }
      return matchTable;
   }

    internal void FlagBothNodesMatch()
    {
      var qMatch = GetMatchingSegmentsWithNodes(
        SynthMatchEnum.MatchBothNodesOfSameSegment);

      // Get the list of compkeys where both nodes match
      var segmentsAffected = (from s in qMatch
                              select s.Compkey).ToList();
      logger.Debug("  FlagBothNodesMatch segmentsAffected = {0}", segmentsAffected.Count);

      // Get the rows matching the list of compkeys where both nodes match to update
      var updateSegments = from s in HansenSegments
                           where segmentsAffected.Contains(s.COMPKEY)
                           select s;
      var updateSegmentsList = updateSegments.ToList();

      var qMatchLookup = qMatch.ToLookup(a => a.Compkey);
      
      // Update the segments
      int counter = 0;
      foreach (var s in updateSegmentsList)
      {
        var q = qMatchLookup[s.COMPKEY].First();
        SynthProcessingDataSet.NodeMatchCoordsRow updateData = q;

        if (updateData != null)
        {
          s.Synth_Flag = SynthFlagBothNodesMatch;
          s.From_X = updateData.From_X;
          s.From_Y = updateData.From_Y;
          s.To_X = updateData.To_X;
          s.To_Y = updateData.To_Y;
        }
        counter++;
      }
    }

    internal void FlagNodesMatchAcrossTwoSegments()
    {
      var qMatch = GetMatchingSegmentsWithNodes(
        SynthMatchEnum.MatchNodesAcrossTwoSegments);

      // Get the list of compkeys where both nodes match
      var segmentsAffected = (from s in qMatch
                              select s.Compkey).ToList();

      logger.Debug("  FlagNodesMatchAcrossTwoSegments segmentsAffected = {0}", segmentsAffected.Count);

      // Get the rows matching the list of compkeys where both nodes match to
      // update
      var updateSegments = from s in HansenSegments
                           where segmentsAffected.Contains(s.COMPKEY)
                           select s;
      var updateSegmentsList = updateSegments.ToList();

      var qMatchLookup = qMatch.ToLookup(a => a.Compkey);

      // Update the segments
      foreach (var s in updateSegments)
      {
        var q = qMatchLookup[s.COMPKEY].First();
        SynthProcessingDataSet.NodeMatchCoordsRow updateData = q;

        if (updateData != null)
        {
          s.Synth_Flag = SynthFlagNodesMatchAcrossTwoSegments;
          s.From_X = updateData.From_X;
          s.From_Y = updateData.From_Y;
          s.To_X = updateData.To_X;
          s.To_Y = updateData.To_Y;
        }
      }
    }

    internal void FlagNodesMatchDifferentSegments()
    {
      // Set up collection of map nodes
      var qMapNodes = from n in MapNodes
                      select n;

      // Verify master links that match with one downstream node
      var qMatchDSNode =
        (
          from s in HansenSegments
          join ndn in qMapNodes on s.UNITID2 equals ndn.Node
          where (
            !s.IsUNITID2Null() && s.IsTo_XNull() && s.IsTo_YNull() &&
            s.Synth_Flag == SynthFlagDefault)
          select new
          {
            s.COMPKEY,
            To_X = ndn.XCoord,
            To_Y = ndn.YCoord
          }
        ).Distinct();

      // Get the list of compkeys where downstream node matches
      var DSNodeSegmentsAffected = (from s in qMatchDSNode
                              select s.COMPKEY).ToList();

      logger.Debug("  FlagNodesMatchDifferentSegments DSNodeSegmentsAffected = {0}", 
        DSNodeSegmentsAffected.Count);

      // Get the rows matching the list of compkeys where downstream
      // node matches to update
      var updateDSNodeSegments = from s in HansenSegments
                           where DSNodeSegmentsAffected.Contains(s.COMPKEY)
                           select s;

      var qMatchDSNodeLookup = qMatchDSNode.ToLookup(a => a.COMPKEY);

      // Update the segments
      foreach (var s in updateDSNodeSegments)
      {
        var updateData = qMatchDSNodeLookup[s.COMPKEY].First();

        if (updateData != null)
        {
          s.To_X = updateData.To_X;
          s.To_Y = updateData.To_Y;
        }
      }

      // Verify master links that match with one upstream node
      var qMatchUSNode =
        (
          from s in HansenSegments
          join nup in qMapNodes on s.UNITID equals nup.Node
          where (
            !s.IsUNITIDNull() && s.IsFrom_XNull() && s.IsFrom_YNull() &&
            s.Synth_Flag == SynthFlagDefault)
          select new
          {
            s.COMPKEY,
            From_X = nup.XCoord,
            From_Y = nup.YCoord
          }
        ).Distinct();

      // Get the list of compkeys where upstream node matches
      var USNodeSegmentsAffected = (from s in qMatchUSNode
                                    select s.COMPKEY).ToList();

      logger.Debug("  FlagNodesMatchDifferentSegments USNodeSegmentsAffected = {0}",
        USNodeSegmentsAffected.Count);

      // Get the rows matching the list of compkeys where upstream
      // node matches to update
      var updateUSNodeSegments = from s in HansenSegments
                                 where USNodeSegmentsAffected.Contains(s.COMPKEY)
                                 select s;

      var qMatchUSNodeLookup = qMatchUSNode.ToLookup(a => a.COMPKEY);

      // Update the segments
      foreach (var s in updateUSNodeSegments)
      {
        var updateData = qMatchUSNodeLookup.Where(a => a.Key.Equals(s.COMPKEY)).First().First();
        if (updateData != null)
        {
          s.From_X = updateData.From_X;
          s.From_Y = updateData.From_Y;
        }
      }

      // Update all segments with both pairs of coordinates and synthflag 0
      // to synthflag 7
      var segsWithCoords = from s in HansenSegments
                           where s.Synth_Flag == SynthFlagDefault && 
                           (!s.IsFrom_XNull() && !s.IsTo_XNull()) &&
                           (s.From_X > 0 && s.To_X > 0)
                           select s.COMPKEY;
      var segsWithCoordsList = segsWithCoords.ToList();

      var updateSegsWithCoords = from s in HansenSegments
                                 where segsWithCoordsList.Contains(s.COMPKEY)
                                 select s;
      var updateSegsWithCoordsList = updateSegsWithCoords.ToList();

      logger.Debug("  FlagNodesMatchDifferentSegments updateSegsWithCoordsList = {0}",
        updateSegsWithCoordsList.Count);

      foreach (var u in updateSegsWithCoordsList)
      {
        u.Synth_Flag = SynthFlagNodesMatchDifferentSegments;
      }
    }

    internal static bool IsValidLicensePlate(string nodeName)
    {
      Match test = Regex.Match(nodeName, @"[A-Za-z]{3}[\d]{3}");
      return test.Success;
    }

    internal SynthProcessingDataSet.synth_nodesDataTable GetUpstreamSynthNodes()
    {
      var usSynthNodes = (from s in HansenSegments
                          where IsValidLicensePlate(s.UNITID) &&
                           s.IsFrom_XNull() && s.IsFrom_YNull()
                          group s by new { s.UNITID, s.mhfrom_type } into g
                          let selected =
                           (from groupedItem in g
                            orderby groupedItem.mhfrom_type
                            select groupedItem).FirstOrDefault()
                          select new
                          {
                            wcms_id = selected.UNITID,
                            selected.mhfrom_type,
                            Expr1 = "AddStep1"
                          }).Distinct();
      SynthProcessingDataSet.synth_nodesDataTable usNodes =
        new SynthProcessingDataSet.synth_nodesDataTable();
      foreach (var s in usSynthNodes)
      {
        SynthProcessingDataSet.synth_nodesRow newRow = usNodes.Newsynth_nodesRow();
        newRow.wcms_id = s.wcms_id;
        newRow.MHType = s.mhfrom_type;
        newRow.Source = s.Expr1;
        usNodes.Rows.Add(newRow);
      }

      return usNodes;
    }

    internal SynthProcessingDataSet.synth_nodesDataTable GetDownstreamSynthNodes()
    {
      var dsSynthNodes = (from s in HansenSegments
                          where IsValidLicensePlate(s.UNITID2) &&
                           s.IsTo_XNull() && s.IsTo_YNull()
                          group s by new { s.UNITID2, s.mhto_type } into g
                          let selected =
                           (from groupedItem in g
                            orderby groupedItem.mhto_type
                            select groupedItem).FirstOrDefault()
                          select new
                          {
                            wcms_id = selected.UNITID2,
                            selected.mhto_type,
                            Expr1 = "AddStep2"
                          }).Distinct();
      SynthProcessingDataSet.synth_nodesDataTable dsNodes =
        new SynthProcessingDataSet.synth_nodesDataTable();
      foreach (var s in dsSynthNodes)
      {
        SynthProcessingDataSet.synth_nodesRow newRow = dsNodes.Newsynth_nodesRow();
        newRow.wcms_id = s.wcms_id;
        newRow.MHType = s.mhto_type;
        newRow.Source = s.Expr1;
        dsNodes.Rows.Add(newRow);
      }

      return dsNodes;
    }

    /// <summary>
    /// Creates the Synth Nodes
    /// </summary>
    /// <returns>Table of Synth Nodes</returns>
    public SynthProcessingDataSet.synth_nodesDataTable AssembleSynthNodes()
    {
      logger.Debug("Enter AssembleSynthNodes");

      SynthProcessingDataSet.synth_nodesDataTable upstreamSynthNodes =
        GetUpstreamSynthNodes();
      SynthProcessingDataSet.synth_nodesDataTable synthNodes =
        new SynthProcessingDataSet.synth_nodesDataTable();
      var availableSynthNodes = from n in synthNodes
                                select n.wcms_id;


      foreach (var n in upstreamSynthNodes)
      {
        if (!availableSynthNodes.Contains(n.wcms_id))
          synthNodes.Rows.Add(n.ItemArray);
      }
      SynthProcessingDataSet.synth_nodesDataTable downstreamSynthNodes =
        GetDownstreamSynthNodes();
      foreach (var n in downstreamSynthNodes)
      {
        if (!availableSynthNodes.Contains(n.wcms_id))
          synthNodes.Rows.Add(n.ItemArray);
      }

      logger.Debug("  availableSynthNodes: {0}", availableSynthNodes.Count());
      logger.Debug("Exit AssembleSynthNodes");
      return synthNodes;
    }

    public enum SynthNodeUpdateSelector { Upstream, Downstream, All };

    internal void UpdateSynthNodesData(
      SynthProcessingDataSet.synth_nodesDataTable synthNodes,
      SynthProcessingDataSet.UpdateNodeDataTable updateNodes)
    {
      var nodesToUpdate = from u in updateNodes
                          select u.updateNode;

      logger.Debug("  nodesToUpdate: {0}", nodesToUpdate.Count());
      
      var updateSet = from n in synthNodes
                      where nodesToUpdate.Contains(n.wcms_id)
                      select n;
      foreach (var u in updateSet)
      {
        var coords = (from c in updateNodes
                      where c.updateNode.Equals(u.wcms_id)
                      select c).First();
        u.X_Coord = coords.xCoord;
        u.Y_Coord = coords.yCoord;
        u.Source = coords.source;
      }
    }

    internal void SelectUpstreamSynthNodesForUpdate(
      SynthProcessingDataSet.synth_nodesDataTable synthNodes,
      SynthProcessingDataSet.UpdateNodeDataTable updateNodes)
    {
      var updateUSCoords = from n in synthNodes
                           join h in HansenSegments on n.wcms_id equals h.UNITID
                           where ((n.IsX_CoordNull() || n.X_Coord == 0) &&
                             (n.IsY_CoordNull() || n.Y_Coord == 0)) &&
                             ((!h.IsTo_XNull() && h.To_X > 0) &&
                             (!h.IsTo_YNull() && h.To_Y > 0))
                           select new
                           {
                             updateNode = n.wcms_id,
                             xCoord = h.To_X + h.PIPELEN * Math.Sqrt(2) + 20,
                             yCoord = h.To_Y + h.PIPELEN * Math.Sqrt(2) + 20,
                             source = "initial_from"
                           };

      logger.Debug(" upstream synth nodes to update: {0}", updateUSCoords.Count());

      foreach (var c in updateUSCoords)
      {
        updateNodes.Rows.Add(c.updateNode, c.xCoord, c.yCoord, c.source);
      }
    }

    internal void SelectDownstreamSynthNodesForUpdate(
      SynthProcessingDataSet.synth_nodesDataTable synthNodes,
      SynthProcessingDataSet.UpdateNodeDataTable updateNodes)
    {
      var updateDSCoords = from n in synthNodes
                           join h in HansenSegments on n.wcms_id equals h.UNITID2
                           where ((n.IsX_CoordNull() || n.X_Coord == 0) &&
                             (n.IsY_CoordNull() || n.Y_Coord == 0)) &&
                             ((!h.IsFrom_XNull() && h.From_X > 0) &&
                             (!h.IsFrom_YNull() && h.From_Y > 0))
                           select new
                           {
                             updateNode = n.wcms_id,
                             xCoord = h.From_X + h.PIPELEN * Math.Sqrt(2) + 20,
                             yCoord = h.From_Y + h.PIPELEN * Math.Sqrt(2) + 20,
                             source = "initial_to"
                           };

      logger.Debug(" downstream  synth nodes to update: {0}", updateDSCoords.Count());

      foreach (var c in updateDSCoords)
      {
        updateNodes.Rows.Add(c.updateNode, c.xCoord, c.yCoord, c.source);
      }
    }

    /// <summary>
    /// Updates Synth Nodes with coordinates based on connected segment lengths
    /// </summary>
    /// <param name="synthNodes">The Synth Nodes table</param>
    /// <param name="whichNode">Chooses the upstream or downstream node to update</param>
    public void UpdateSynthNodes(
      SynthProcessingDataSet.synth_nodesDataTable synthNodes,
      SynthNodeUpdateSelector whichNode)
    {
      logger.Debug("Enter UpdateSynthNodes");

      SynthProcessingDataSet.UpdateNodeDataTable updateNodes =
        new SynthProcessingDataSet.UpdateNodeDataTable();
      switch (whichNode)
      {
        case SynthNodeUpdateSelector.Upstream:
          SelectUpstreamSynthNodesForUpdate(synthNodes, updateNodes);
          break;
        case SynthNodeUpdateSelector.Downstream:
          SelectDownstreamSynthNodesForUpdate(synthNodes, updateNodes);
          break;
        case SynthNodeUpdateSelector.All:
          SelectUpstreamSynthNodesForUpdate(synthNodes, updateNodes);
          SelectDownstreamSynthNodesForUpdate(synthNodes, updateNodes);
          break;
      }

      UpdateSynthNodesData(synthNodes, updateNodes);
      logger.Debug("Exit UpdateSynthNodes");
    }

     /// <summary>
     /// Updates Synth Nodes with address coordinates
     /// </summary>
     /// <param name="synthNodes">The Synth Nodes table</param>
    public void UpdateSynthNodesWithAddresses(
      SynthProcessingDataSet.synth_nodesDataTable synthNodes)
    {
      logger.Debug("Enter UpdateSynthNodesWithAddresses");

      var nodesWithAddressMatches = from s in HansenSegments
              join a in Addresses on s.Addrkey equals a.ADDRKEY
              join n in synthNodes on s.UNITID equals n.wcms_id
              where n.IsX_CoordNull() || n.X_Coord == 0
              select new { n.wcms_id, a.X, a.Y };
      var synthNodesDictionary = synthNodes.ToDictionary(a => a.wcms_id);

      logger.Debug("  address matches: {0}", nodesWithAddressMatches.Count());

      foreach (var n in nodesWithAddressMatches)
      {
        var nodeToUpdate = synthNodesDictionary[n.wcms_id];
        nodeToUpdate.X_Coord = n.X;
        nodeToUpdate.Y_Coord = n.Y;
        nodeToUpdate.Source = "address";
      }

      logger.Debug("Exit UpdateSynthNodesWithAddresses");
    }

    internal enum SynthNodeUpdateDirection { Upstream, Downstream }

    internal void UpdateNodesWithSynthNodes(
      SynthProcessingDataSet.synth_nodesDataTable synthNodes,
      SynthNodeUpdateDirection direction)
    {
      // Select out the coordinates from SynthNodes to use for update
      SynthProcessingDataSet.UpdateSegmentCoordsDataTable coords =
        new SynthProcessingDataSet.UpdateSegmentCoordsDataTable();
      switch (direction)
      {
        case SynthNodeUpdateDirection.Upstream:
          var USSegmentsToUpdate = from s in HansenSegments
                                 join n in synthNodes on s.UNITID equals n.wcms_id
                                 where ((s.IsFrom_XNull()) || (s.From_X == 0)) &&
                                   ((s.IsFrom_YNull()) || (s.From_Y == 0)) &&
                                   ((!n.IsX_CoordNull()) && (n.X_Coord != 0)) &&
                                   ((!n.IsY_CoordNull()) && (n.Y_Coord != 0))
                                 select new { s.COMPKEY, n.X_Coord, n.Y_Coord };
          foreach (var s in USSegmentsToUpdate)
          {
            coords.Rows.Add(s.COMPKEY, s.X_Coord, s.Y_Coord);
          }
          break;
        case SynthNodeUpdateDirection.Downstream:
          var DSSegmentsToUpdate = from s in HansenSegments
                                 join n in synthNodes on s.UNITID2 equals n.wcms_id
                                 where ((s.IsTo_XNull()) || (s.To_X == 0)) &&
                                   ((s.IsTo_YNull()) || (s.To_Y == 0)) &&
                                   ((!n.IsX_CoordNull()) && (n.X_Coord != 0)) &&
                                   ((!n.IsY_CoordNull()) && (n.Y_Coord != 0))
                                 select new { s.COMPKEY, n.X_Coord, n.Y_Coord };
          foreach (var s in DSSegmentsToUpdate)
          {
            coords.Rows.Add(s.COMPKEY, s.X_Coord, s.Y_Coord);
          }
          break;
      }
      var uniqueCoords = (from c in coords
                          select c).Distinct().ToLookup(a => a.Compkey);
      var synthNodesList = synthNodes.ToDictionary(a => a.wcms_id);

      var hansenSegmentsDictionary = HansenSegments.ToDictionary(a => a.COMPKEY);

      // Update the segments with the coordinates
      var updateSet = (from s in coords
                       select s.Compkey).Distinct().ToList();

      logger.Debug("  segments to update: {0} {1}", updateSet.Count, 
        Enum.GetName(typeof(SynthNodeUpdateDirection), direction));

      foreach (var c in updateSet)
      {
        var segmentToUpdate = hansenSegmentsDictionary[c];
        var updateData = uniqueCoords[c].First();
        switch (direction)
        {
          case SynthNodeUpdateDirection.Upstream:
            synthNodesList[segmentToUpdate.UNITID].Source = "update_from";
            segmentToUpdate.From_X = updateData.xCoord;
            segmentToUpdate.From_Y = updateData.yCoord;
            break;
          case SynthNodeUpdateDirection.Downstream:
            synthNodesList[segmentToUpdate.UNITID2].Source = "update_to";
            segmentToUpdate.To_X = updateData.xCoord;
            segmentToUpdate.To_Y = updateData.yCoord;
            break;
        }
      }
    }

    internal void UpdateUpstreamNodesWithSynthNodes(
      SynthProcessingDataSet.synth_nodesDataTable synthNodes)
    {
      UpdateNodesWithSynthNodes(synthNodes, SynthNodeUpdateDirection.Upstream);
    }

    internal void UpdateDownstreamNodesWithSynthNodes(
      SynthProcessingDataSet.synth_nodesDataTable synthNodes)
    {
      UpdateNodesWithSynthNodes(synthNodes, SynthNodeUpdateDirection.Downstream);
    }

    /// <summary>
    /// Updates Hansen Segment coordinates with Synth Node coordinates
    /// </summary>
    /// <param name="synthNodes">The Synth Nodes table</param>
    public void UpdateSegmentsWithSynthNodes(
      SynthProcessingDataSet.synth_nodesDataTable synthNodes)
    {
      logger.Debug("Enter UpdateSegmentsWithSynthNodes");

      UpdateUpstreamNodesWithSynthNodes(synthNodes);
      UpdateDownstreamNodesWithSynthNodes(synthNodes);

      logger.Debug("Enter UpdateSegmentsWithSynthNodes");
    }

    /// <summary>
    /// Set the synth flag for segments whose coordinates were updated with Synth Nodes
    /// </summary>
    public void FlagSegmentsWithUpdatedCoords()
    {
      logger.Debug("Enter FlagSegmentsWithUpdatedCoords");

      var segsWithUpdatedCoords = from s in HansenSegments
                                  where s.Synth_Flag.Equals("0") &&
                                    ((!s.IsFrom_XNull()) && (s.From_X != 0)) &&
                                    ((!s.IsTo_XNull()) && (s.To_X != 0))
                                  select s;
      var segsWithUpdatedCoordsList = segsWithUpdatedCoords.ToList();

      logger.Debug("  segswithUpdateCoords: {0}", segsWithUpdatedCoordsList.Count);
      foreach (var s in segsWithUpdatedCoordsList)
      {
        s.Synth_Flag = SynthFlagSegmentsWithToFromCoords;
      }

      logger.Debug("Exit FlagSegmentsWithUpdatedCoords");
    }

    /// <summary>
    /// Set the synth flag for segments that do not have coordinates but have
    /// valid license plate node IDs
    /// </summary>
    public void FlagRemainingSegmentsWithValidNodes()
    {
      logger.Debug("Enter FlagRemainingSegmentsWithValidNodes");

      var segsLeftWithValidLicensePlates = from s in HansenSegments
                                           where s.Synth_Flag.Equals("0") &&
                                             IsValidLicensePlate(s.UNITID) &&
                                             IsValidLicensePlate(s.UNITID2)
                                           select s;
      var segsLeftWithValidLicensePlatesList = segsLeftWithValidLicensePlates.ToList();
      foreach (var s in segsLeftWithValidLicensePlatesList)
      {
        s.Synth_Flag = SynthFlagSegmentsWithValidLicensePlates;
      }

      logger.Debug("Exit FlagRemainingSegmentsWithValidNodes");
    }

    internal enum LadderDirection { Upstream, Downstream };

    internal void UpdateSynthNodesLadderData(
      Dictionary<int, SynthProcessingDataSet.All_Hansen_SegsRow> hansenSegmentsDictionary,
      Dictionary<string, SynthProcessingDataSet.synth_nodesRow> synthNodesDictionary,
      List<int> compkeyList,
      LadderDirection direction)
    {
      foreach (int c in compkeyList)
      {
        var USNodeID = hansenSegmentsDictionary[c].UNITID;
        var DSNodeID = hansenSegmentsDictionary[c].UNITID2;
        var nodeToLadder = synthNodesDictionary[direction == LadderDirection.Upstream ? DSNodeID : USNodeID];
        var nodeToUpdate = synthNodesDictionary[direction == LadderDirection.Upstream ? USNodeID : DSNodeID];
        var rung = hansenSegmentsDictionary[c];
        nodeToUpdate.X_Coord = nodeToLadder.X_Coord + rung.PIPELEN * Math.Sqrt(2) + 20;
        nodeToUpdate.Y_Coord = nodeToLadder.Y_Coord + rung.PIPELEN * Math.Sqrt(2) + 20;
      }
    }

    /// <summary>
    /// Update Synth Node coordinates if connected by a segment to a node that
    /// already has coordinates
    /// </summary>
    /// <param name="synthNodes">The Synth Nodes table</param>
    public void UpdateSynthNodesLadder(
      SynthProcessingDataSet.synth_nodesDataTable synthNodes)
    {
      logger.Debug("Enter UpdateSynthNodesLadder");

      Dictionary<int, SynthProcessingDataSet.All_Hansen_SegsRow> hansenSegmentsDictionary = 
        HansenSegments.ToDictionary(a => a.COMPKEY);
      Dictionary<string, SynthProcessingDataSet.synth_nodesRow> synthNodesDictionary =
        synthNodes.ToDictionary(a => a.wcms_id);

      var zeroCoordSynthNodes = from n in synthNodes
                                 where (n.IsX_CoordNull() || n.X_Coord == 0) &&
                                   (n.IsY_CoordNull() || n.Y_Coord == 0)
                                 select n;
      int oldN = zeroCoordSynthNodes.Count();
      int cycle = 1;
      bool done = false;
      while (!done)
      {
        logger.Debug("  cycle: {0}", cycle);

        var segsToLadderUp = from s in HansenSegments
                             join m in SynthNodes on s.UNITID equals m.wcms_id
                             join n in SynthNodes on s.UNITID2 equals n.wcms_id
                             where (m.IsX_CoordNull() || m.X_Coord == 0) &&
                               (m.IsY_CoordNull() || m.Y_Coord == 0) &&
                               (!n.IsX_CoordNull() && n.X_Coord != 0) &&
                               (!n.IsY_CoordNull() && n.Y_Coord != 0)
                             select s;
        var upCompkeyRungs = (from s in segsToLadderUp
                              select s.COMPKEY).ToList();
        logger.Debug("    ladder up: {0}", upCompkeyRungs.Count);

        UpdateSynthNodesLadderData(hansenSegmentsDictionary, synthNodesDictionary, upCompkeyRungs, 
          LadderDirection.Upstream);

        var segsToLadderDn = from s in HansenSegments
                             join m in SynthNodes on s.UNITID equals m.wcms_id
                             join n in SynthNodes on s.UNITID2 equals n.wcms_id
                             where (!m.IsX_CoordNull() && m.X_Coord != 0) &&
                               (!m.IsY_CoordNull() && m.Y_Coord != 0) &&
                               (n.IsX_CoordNull() || n.X_Coord == 0) &&
                               (n.IsY_CoordNull() || n.Y_Coord == 0)
                             select s;
        var dnCompkeyRungs = (from s in segsToLadderDn
                              select s.COMPKEY).ToList();
        logger.Debug("    ladder down: {0}", dnCompkeyRungs.Count);

        UpdateSynthNodesLadderData(hansenSegmentsDictionary, synthNodesDictionary, dnCompkeyRungs, 
          LadderDirection.Downstream);

        int newN = zeroCoordSynthNodes.Count();

        if (newN == oldN || newN == 0)
          done = true;
        else
          oldN = newN;

        cycle++;
      }

      logger.Debug("Exit UpdateSynthNodesLadder");
    }
  }
}
