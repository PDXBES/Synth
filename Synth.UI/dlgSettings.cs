using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Synth.UI
{
  public partial class dlgSettings : Form
  {
    string getStormDitchesProcName;
    string getSanitaryPipesProcName;
    string getStormPipesProcName;
    string logFileName;

    public dlgSettings()
    {
      InitializeComponent();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
      Properties.Settings.Default.GetStormDitchesUSPName = getStormDitchesProcName;
      Properties.Settings.Default.GetSanitaryPipesUSPName = getSanitaryPipesProcName;
      Properties.Settings.Default.GetStormPipesUSPName = getStormPipesProcName;
      Properties.Settings.Default.LogFile = logFileName;
      Properties.Settings.Default.Save();
    }

    private void dlgSettings_Load(object sender, EventArgs e)
    {
      getStormDitchesProcName = Properties.Settings.Default.GetStormDitchesUSPName;
      getSanitaryPipesProcName = Properties.Settings.Default.GetSanitaryPipesUSPName;
      getStormPipesProcName = Properties.Settings.Default.GetStormPipesUSPName;
      logFileName = Properties.Settings.Default.LogFile;
    }

    private void btnOK_Click(object sender, EventArgs e)
    {
      Properties.Settings.Default.Save();
      Logging.LoggingConfigurator.Setup(Properties.Settings.Default.LogFile, 
        Properties.Settings.Default.LogSeverity);
    }
  }
}
