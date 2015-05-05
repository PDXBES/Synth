using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Synth.DB;
using System.IO;
using Synth.Logging;

namespace Synth.UI
{
  public partial class frmMain : Form
  {
    Logger logger;

    public frmMain()
    {
      InitializeComponent();

      LoggingConfigurator.Setup(Properties.Settings.Default.LogFile, 
        Properties.Settings.Default.LogSeverity);

      logger = new Logger(this.GetType().Name);
      logger.Debug("--------------------------------------------------------");
      logger.Debug("Begin application");
    }

    private bool CheckEnabledUI()
    {
      return
        (txtServer.Text.Length) > 0 &&
        (txtStoredProcsDB.Text.Length > 0) &&
        (txtAddressesFile.Text.Length > 0) &&
        (txtMasterLinks.Text.Length > 0) &&
        (txtMasterNodes.Text.Length > 0) &&
        (txtSynthExportFile.Text.Length > 0);
    }

    private void txtAddressesFile_EditorButtonClick(object sender, 
      Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
    {
      dlgOpen.Title = "Open address file";
      dlgOpen.DefaultExt = "csv";
      if (dlgOpen.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      {
        txtAddressesFile.Text = dlgOpen.FileName;
        Properties.Settings.Default.Save();
      }
    }

    private void txtMasterNodes_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
    {
      dlgOpen.Title = "Open master nodes";
      dlgOpen.DefaultExt = "tab";
      dlgOpen.FileName = Properties.Settings.Default.MasterNodesFile;
      if (dlgOpen.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      {
        txtMasterNodes.Text = dlgOpen.FileName;
        Properties.Settings.Default.LastDirectory = Path.GetDirectoryName(dlgOpen.FileName);
        Properties.Settings.Default.Save();
      }
    }

    private void txtMasterLinks_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
    {
      dlgOpen.Title = "Open master links";
      dlgOpen.DefaultExt = "tab";
      dlgOpen.FileName = Properties.Settings.Default.MasterLinksFile;
      if (dlgOpen.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      {
        txtMasterLinks.Text = dlgOpen.FileName;
        Properties.Settings.Default.LastDirectory = Path.GetDirectoryName(dlgOpen.FileName);
        Properties.Settings.Default.Save();
      }
    }

    private void txtSynthExportFile_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
    {
      dlgSave.Title = "Save synth file";
      dlgSave.DefaultExt = "mif";
      if (dlgSave.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      {
        txtSynthExportFile.Text = dlgSave.FileName;
        Properties.Settings.Default.LastDirectory = Path.GetDirectoryName(dlgSave.FileName);
        Properties.Settings.Default.Save();
      }
    }

    private void txtDifferencesFile_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
    {
      dlgSave.Title = "Save differences file";
      dlgSave.DefaultExt = "mif";
      if (dlgSave.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      {
        txtDifferencesFile.Text = dlgSave.FileName;
        Properties.Settings.Default.LastDirectory = Path.GetDirectoryName(dlgSave.FileName);
        Properties.Settings.Default.Save();
      }
    }

    private void frmMain_Load(object sender, EventArgs e)
    {
      btnRun.Enabled = CheckEnabledUI();
      txtServer_ValueChanged(this, e);
    }

    private void btnExit_Click(object sender, EventArgs e)
    {
      logger.Debug("Exit application");
      Close();
    }

    private static int synthProgressPercent(int value)
    {
      return Convert.ToInt32(((double)value / 18 * 100));
    }

    private void bkgdSynthProcessor_DoWork(object sender, DoWorkEventArgs e)
    {
      System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
      timer.Start();

      logger.Debug("Entering background process");
      SynthProcessorArgs args = (SynthProcessorArgs)e.Argument;

      bkgdSynthProcessor.ReportProgress(synthProgressPercent(1), 
        "Setting up Hansen connection");
      HansenAccessor hansenAccessor = new HansenAccessor(args.HansenConnectionString,
        args.GetSanitaryPipesUSP, args.GetStormDitchesUSP, args.GetStormPipesUSP, args.AddressFile);

      bkgdSynthProcessor.ReportProgress(synthProgressPercent(2), 
        "Setting up master links/nodes connection");
      string masterLinksConnectionString =
        string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}",
          args.MasterLinksFile);
      string masterNodesConnectionString =
        string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}",
          args.MasterNodesFile);
      MasterDataAccessor masterDataAccessor = new MasterDataAccessor(masterLinksConnectionString,
        masterNodesConnectionString);

      bkgdSynthProcessor.ReportProgress(synthProgressPercent(3),
        "Setting up synth processor");
      SynthDataProcessor processor = new SynthDataProcessor(masterDataAccessor, hansenAccessor);
      bkgdSynthProcessor.ReportProgress(synthProgressPercent(4), 
        "Appending master nodes to map nodes");
      processor.AppendMasterNodesToMapNodes();
      bkgdSynthProcessor.ReportProgress(synthProgressPercent(5),
        "Finding exact matches (node-to-node)");
      processor.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchBothNodesOfSameSegment);
      bkgdSynthProcessor.ReportProgress(synthProgressPercent(6),
        "Finding near matches (US node of one segment and DS node of next segment)");
      processor.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchNodesAcrossTwoSegments);
      bkgdSynthProcessor.ReportProgress(synthProgressPercent(7),
        "Finding disparate matches (nodes of different segments)");
      processor.FlagNodesMatch(SynthDataProcessor.SynthMatchEnum.MatchNodesOfDifferentSegments);
      bkgdSynthProcessor.ReportProgress(synthProgressPercent(8),
        "Assembling synth nodes");
      processor.SynthNodes = processor.AssembleSynthNodes();
      bkgdSynthProcessor.ReportProgress(synthProgressPercent(9),
        "Updating synth nodes");
      processor.UpdateSynthNodes(processor.SynthNodes, SynthDataProcessor.SynthNodeUpdateSelector.All);
      bkgdSynthProcessor.ReportProgress(synthProgressPercent(10),
        "Performing ladder operation");
      processor.UpdateSynthNodesLadder(processor.SynthNodes);
      bkgdSynthProcessor.ReportProgress(synthProgressPercent(11),
        "Updating synth nodes with segment address matches");
      processor.UpdateSynthNodesWithAddresses(processor.SynthNodes);
      bkgdSynthProcessor.ReportProgress(synthProgressPercent(12),
        "Performing ladder operation");
      processor.UpdateSynthNodesLadder(processor.SynthNodes);
      bkgdSynthProcessor.ReportProgress(synthProgressPercent(13),
        "Updating segments with synth nodes");
      processor.UpdateSegmentsWithSynthNodes(processor.SynthNodes);
      bkgdSynthProcessor.ReportProgress(synthProgressPercent(14),
        "Flagging segments with updated nodes");
      processor.FlagSegmentsWithUpdatedCoords();
      bkgdSynthProcessor.ReportProgress(synthProgressPercent(15),
        "Flagging remaining segments");
      processor.FlagRemainingSegmentsWithValidNodes();

      SynthExporter exporter = new SynthExporter(processor);
      string mifFileName = Path.GetDirectoryName(args.SynthFile) + Path.DirectorySeparatorChar +
        Path.GetFileNameWithoutExtension(args.SynthFile) + ".MIF";
      string midFileName = Path.GetDirectoryName(args.SynthFile) + Path.DirectorySeparatorChar +
        Path.GetFileNameWithoutExtension(args.SynthFile) + ".MID";
      bkgdSynthProcessor.ReportProgress(synthProgressPercent(16),
        "Setting up MIF file");
      MifMidWriter mifWriter = new MifMidWriter(mifFileName);
      MifMidWriter midWriter = new MifMidWriter(midFileName);
      exporter.WriteSynthSegmentMifMidHeader(mifWriter);
      bkgdSynthProcessor.ReportProgress(synthProgressPercent(17),
        "Writing MIF data");
      exporter.WriteSynthSegmentMifMidMifData(mifWriter);
      bkgdSynthProcessor.ReportProgress(synthProgressPercent(18),
        "Writing MID data");
      exporter.WriteSynthSegmentMifMidMidData(midWriter);

      mifWriter.Close();
      midWriter.Close();

      timer.Stop();
      logger.Info("Elapsed processing time: {0} sec", timer.Elapsed.TotalSeconds);

      logger.Debug("Exiting background process");
    }

    private void bkgdSynthProcessor_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      prgSynthProcessorWork.Value = e.ProgressPercentage;
      lblSynthProcessorWork.Text = (string)e.UserState;
    }

    private void bkgdSynthProcessor_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      prgSynthProcessorWork.Value = 0;
      pnlSynthProcessorWork.Hide();
      lblEndStatus.Text = "Synth complete.";
      lblEndStatus.Show();
    }

    private void btnRun_Click(object sender, EventArgs e)
    {
      logger.Debug("Enter btnOK_Click");
      lblEndStatus.Hide();

      Properties.Settings.Default.Save();

      string hansenServer = Properties.Settings.Default.HansenReportingServer;
      string storedProcsDB = Properties.Settings.Default.StoredProcsDB;
      string hansenConnectionString = string.Format(
        "Data Source={0};Initial Catalog={1};Integrated Security=True",
        Properties.Settings.Default.HansenReportingServer, 
        Properties.Settings.Default.StoredProcsDB);
      string getSanitaryPipesUSP = Properties.Settings.Default.GetSanitaryPipesUSPName;
      string getStormDitchesUSP = Properties.Settings.Default.GetStormDitchesUSPName;
      string getStormPipesUSP = Properties.Settings.Default.GetStormPipesUSPName;
      string addressFile = Properties.Settings.Default.AddressName;
      string masterLinksFileName = Properties.Settings.Default.MasterLinksFile;
      string masterNodesFileName = Properties.Settings.Default.MasterNodesFile;
      string synthFileName = Properties.Settings.Default.SynthFile;

      logger.Info("Hansen server: {0}", hansenServer);
      logger.Info("Stored procedures db: {0}", storedProcsDB);
      logger.Info("Hansen connection string: {0}", hansenConnectionString);
      logger.Info("Sanitary pipes USP: {0}", getSanitaryPipesUSP);
      logger.Info("Storm ditches USP: {0}", getStormDitchesUSP);
      logger.Info("Storm pipes USP: {0}", getStormPipesUSP);
      logger.Info("Address file: {0}", addressFile);
      logger.Info("Master links filename: {0}", masterLinksFileName);
      logger.Info("Master nodes filename: {0}", masterNodesFileName);
      logger.Info("Synth file: {0}", synthFileName);

      SynthProcessorArgs args = new SynthProcessorArgs(
        HansenServer: hansenServer,
        StoredProcsDB: storedProcsDB,
        HansenConnectionString: hansenConnectionString,
        GetSanitaryPipesUSP: getSanitaryPipesUSP,
        GetStormDitchesUSP: getStormDitchesUSP,
        GetStormPipesUSP: getStormPipesUSP,
        AddressFile: addressFile,
        MasterLinksFile: masterLinksFileName,
        MasterNodesFile: masterNodesFileName,
        SynthFile: synthFileName);

      prgSynthProcessorWork.Value = 0;
      lblSynthProcessorWork.Text = string.Empty;
      pnlSynthProcessorWork.Show();

      bkgdSynthProcessor.RunWorkerAsync(args);
    }

    private void btnSettings_Click(object sender, EventArgs e)
    {
      dlgSettings settings = new dlgSettings();
      settings.ShowDialog();
    }

    private void txtServer_ValueChanged(object sender, EventArgs e)
    {
      btnRun.Enabled = CheckEnabledUI();
      lblEndStatus.Text = "Complete required fields and click run to create synth files.";
      errorProvider.Clear();
    }

    private void txtServer_Validating(object sender, CancelEventArgs e)
    {
      if (txtServer.Text.Length == 0)
      {
        errorProvider.SetError(txtServer, "Provide a server name for Hansen data");
      }
    }

    private void txtStoredProcsDB_Validating(object sender, CancelEventArgs e)
    {
      if (txtStoredProcsDB.Text.Length == 0)
      {
        errorProvider.SetError(txtStoredProcsDB, 
          "Provide a name for the reporting database containing the ASM Hansen retrieval procedures");
      }
    }

    private void txtAddressesFile_Validating(object sender, CancelEventArgs e)
    {
      if (txtStoredProcsDB.Text.Length == 0)
      {
        errorProvider.SetError(txtAddressesFile, "Provide a geocoded address file");
      }
      else if (!File.Exists(txtAddressesFile.Text))
      {
        errorProvider.SetError(txtAddressesFile, "This file does not exist");
      }
    }

    private void txtMasterLinks_Validating(object sender, CancelEventArgs e)
    {
      if (txtMasterLinks.Text.Length == 0)
      {
        errorProvider.SetError(txtMasterLinks, "Provide a master links database file");
      }
      else if (!File.Exists(txtMasterLinks.Text))
      {
        errorProvider.SetError(txtMasterLinks, "This file does not exist");
      }
    }

    private void txtMasterNodes_Validating(object sender, CancelEventArgs e)
    {
      if (txtMasterNodes.Text.Length == 0)
      {
        errorProvider.SetError(txtMasterNodes, "Provide a master nodes database file");
      }
      else if (!File.Exists(txtMasterNodes.Text))
      {
        errorProvider.SetError(txtMasterNodes, "This file does not exist");
      }
    }

    private void txtSynthExportFile_Validating(object sender, CancelEventArgs e)
    {
      if (txtSynthExportFile.Text.Length == 0)
      {
        errorProvider.SetError(txtSynthExportFile, "Provide a synth export file");
      }
    }
  }
}
