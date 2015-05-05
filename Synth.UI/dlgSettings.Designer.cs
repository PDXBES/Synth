namespace Synth.UI
{
  partial class dlgSettings
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      Infragistics.Win.Layout.GridBagConstraint gridBagConstraint1 = new Infragistics.Win.Layout.GridBagConstraint();
      Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
      Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
      Infragistics.Win.ValueListItem valueListItem3 = new Infragistics.Win.ValueListItem();
      Infragistics.Win.ValueListItem valueListItem4 = new Infragistics.Win.ValueListItem();
      Infragistics.Win.ValueListItem valueListItem5 = new Infragistics.Win.ValueListItem();
      Infragistics.Win.ValueListItem valueListItem6 = new Infragistics.Win.ValueListItem();
      Infragistics.Win.Layout.GridBagConstraint gridBagConstraint2 = new Infragistics.Win.Layout.GridBagConstraint();
      Infragistics.Win.Layout.GridBagConstraint gridBagConstraint3 = new Infragistics.Win.Layout.GridBagConstraint();
      Infragistics.Win.Layout.GridBagConstraint gridBagConstraint4 = new Infragistics.Win.Layout.GridBagConstraint();
      Infragistics.Win.Layout.GridBagConstraint gridBagConstraint7 = new Infragistics.Win.Layout.GridBagConstraint();
      Infragistics.Win.Layout.GridBagConstraint gridBagConstraint5 = new Infragistics.Win.Layout.GridBagConstraint();
      Infragistics.Win.Layout.GridBagConstraint gridBagConstraint6 = new Infragistics.Win.Layout.GridBagConstraint();
      Infragistics.Win.Layout.GridBagConstraint gridBagConstraint8 = new Infragistics.Win.Layout.GridBagConstraint();
      Infragistics.Win.Layout.GridBagConstraint gridBagConstraint9 = new Infragistics.Win.Layout.GridBagConstraint();
      Infragistics.Win.Layout.GridBagConstraint gridBagConstraint10 = new Infragistics.Win.Layout.GridBagConstraint();
      Infragistics.Win.Layout.GridBagConstraint gridBagConstraint11 = new Infragistics.Win.Layout.GridBagConstraint();
      Infragistics.Win.Layout.GridBagConstraint gridBagConstraint12 = new Infragistics.Win.Layout.GridBagConstraint();
      Infragistics.Win.Layout.GridBagConstraint gridBagConstraint13 = new Infragistics.Win.Layout.GridBagConstraint();
      this.ultraGridBagLayoutPanel1 = new Infragistics.Win.Misc.UltraGridBagLayoutPanel();
      this.cmbLogLevel = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
      this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
      this.txtLogFileName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
      this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
      this.ultraGridBagLayoutPanel2 = new Infragistics.Win.Misc.UltraGridBagLayoutPanel();
      this.btnCancel = new Infragistics.Win.Misc.UltraButton();
      this.btnOK = new Infragistics.Win.Misc.UltraButton();
      this.txtSanitaryPipesProcName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
      this.txtStormPipesProcName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
      this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
      this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
      this.txtStormDitchesProcName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
      this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
      ((System.ComponentModel.ISupportInitialize)(this.ultraGridBagLayoutPanel1)).BeginInit();
      this.ultraGridBagLayoutPanel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.cmbLogLevel)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.txtLogFileName)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultraGridBagLayoutPanel2)).BeginInit();
      this.ultraGridBagLayoutPanel2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.txtSanitaryPipesProcName)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.txtStormPipesProcName)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.txtStormDitchesProcName)).BeginInit();
      this.SuspendLayout();
      // 
      // ultraGridBagLayoutPanel1
      // 
      this.ultraGridBagLayoutPanel1.Controls.Add(this.cmbLogLevel);
      this.ultraGridBagLayoutPanel1.Controls.Add(this.ultraLabel5);
      this.ultraGridBagLayoutPanel1.Controls.Add(this.txtLogFileName);
      this.ultraGridBagLayoutPanel1.Controls.Add(this.ultraLabel4);
      this.ultraGridBagLayoutPanel1.Controls.Add(this.ultraGridBagLayoutPanel2);
      this.ultraGridBagLayoutPanel1.Controls.Add(this.txtSanitaryPipesProcName);
      this.ultraGridBagLayoutPanel1.Controls.Add(this.txtStormPipesProcName);
      this.ultraGridBagLayoutPanel1.Controls.Add(this.ultraLabel3);
      this.ultraGridBagLayoutPanel1.Controls.Add(this.ultraLabel2);
      this.ultraGridBagLayoutPanel1.Controls.Add(this.txtStormDitchesProcName);
      this.ultraGridBagLayoutPanel1.Controls.Add(this.ultraLabel1);
      this.ultraGridBagLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultraGridBagLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.ultraGridBagLayoutPanel1.Name = "ultraGridBagLayoutPanel1";
      this.ultraGridBagLayoutPanel1.Padding = new System.Windows.Forms.Padding(8);
      this.ultraGridBagLayoutPanel1.Size = new System.Drawing.Size(464, 282);
      this.ultraGridBagLayoutPanel1.TabIndex = 0;
      // 
      // cmbLogLevel
      // 
      this.cmbLogLevel.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Synth.UI.Properties.Settings.Default, "LogSeverity", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.cmbLogLevel.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
      gridBagConstraint1.Fill = Infragistics.Win.Layout.FillType.Both;
      gridBagConstraint1.Insets.Bottom = 4;
      gridBagConstraint1.Insets.Right = 4;
      gridBagConstraint1.OriginX = 1;
      gridBagConstraint1.OriginY = 4;
      this.ultraGridBagLayoutPanel1.SetGridBagConstraint(this.cmbLogLevel, gridBagConstraint1);
      valueListItem1.DataValue = "Trace";
      valueListItem1.DisplayText = "Trace";
      valueListItem2.DataValue = "Debug";
      valueListItem2.DisplayText = "Debug";
      valueListItem3.DataValue = "Info";
      valueListItem3.DisplayText = "Info";
      valueListItem4.DataValue = "Warn";
      valueListItem4.DisplayText = "Warn";
      valueListItem5.DataValue = "Error";
      valueListItem5.DisplayText = "Error";
      valueListItem6.DataValue = "Fatal";
      valueListItem6.DisplayText = "Fatal";
      this.cmbLogLevel.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem1,
            valueListItem2,
            valueListItem3,
            valueListItem4,
            valueListItem5,
            valueListItem6});
      this.cmbLogLevel.Location = new System.Drawing.Point(162, 128);
      this.cmbLogLevel.Name = "cmbLogLevel";
      this.ultraGridBagLayoutPanel1.SetPreferredSize(this.cmbLogLevel, new System.Drawing.Size(144, 25));
      this.cmbLogLevel.Size = new System.Drawing.Size(290, 25);
      this.cmbLogLevel.TabIndex = 10;
      this.cmbLogLevel.Text = global::Synth.UI.Properties.Settings.Default.LogSeverity;
      // 
      // ultraLabel5
      // 
      gridBagConstraint2.Fill = Infragistics.Win.Layout.FillType.Both;
      gridBagConstraint2.OriginX = 0;
      gridBagConstraint2.OriginY = 4;
      this.ultraGridBagLayoutPanel1.SetGridBagConstraint(this.ultraLabel5, gridBagConstraint2);
      this.ultraLabel5.Location = new System.Drawing.Point(8, 128);
      this.ultraLabel5.Name = "ultraLabel5";
      this.ultraGridBagLayoutPanel1.SetPreferredSize(this.ultraLabel5, new System.Drawing.Size(150, 26));
      this.ultraLabel5.Size = new System.Drawing.Size(154, 29);
      this.ultraLabel5.TabIndex = 9;
      this.ultraLabel5.Text = "Logging severity level";
      // 
      // txtLogFileName
      // 
      this.txtLogFileName.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Synth.UI.Properties.Settings.Default, "LogFile", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      gridBagConstraint3.Fill = Infragistics.Win.Layout.FillType.Both;
      gridBagConstraint3.Insets.Bottom = 4;
      gridBagConstraint3.Insets.Right = 4;
      gridBagConstraint3.OriginX = 1;
      gridBagConstraint3.OriginY = 3;
      this.ultraGridBagLayoutPanel1.SetGridBagConstraint(this.txtLogFileName, gridBagConstraint3);
      this.txtLogFileName.Location = new System.Drawing.Point(162, 98);
      this.txtLogFileName.Name = "txtLogFileName";
      this.ultraGridBagLayoutPanel1.SetPreferredSize(this.txtLogFileName, new System.Drawing.Size(173, 24));
      this.txtLogFileName.Size = new System.Drawing.Size(290, 25);
      this.txtLogFileName.TabIndex = 8;
      this.txtLogFileName.Text = global::Synth.UI.Properties.Settings.Default.LogFile;
      // 
      // ultraLabel4
      // 
      gridBagConstraint4.Fill = Infragistics.Win.Layout.FillType.Both;
      gridBagConstraint4.Insets.Bottom = 4;
      gridBagConstraint4.Insets.Right = 4;
      gridBagConstraint4.OriginX = 0;
      gridBagConstraint4.OriginY = 3;
      this.ultraGridBagLayoutPanel1.SetGridBagConstraint(this.ultraLabel4, gridBagConstraint4);
      this.ultraLabel4.Location = new System.Drawing.Point(8, 98);
      this.ultraLabel4.Name = "ultraLabel4";
      this.ultraGridBagLayoutPanel1.SetPreferredSize(this.ultraLabel4, new System.Drawing.Size(150, 26));
      this.ultraLabel4.Size = new System.Drawing.Size(150, 26);
      this.ultraLabel4.TabIndex = 7;
      this.ultraLabel4.Text = "Log file";
      // 
      // ultraGridBagLayoutPanel2
      // 
      this.ultraGridBagLayoutPanel2.Controls.Add(this.btnCancel);
      this.ultraGridBagLayoutPanel2.Controls.Add(this.btnOK);
      gridBagConstraint7.Fill = Infragistics.Win.Layout.FillType.Both;
      gridBagConstraint7.OriginX = 1;
      gridBagConstraint7.OriginY = 5;
      gridBagConstraint7.WeightY = 1F;
      this.ultraGridBagLayoutPanel1.SetGridBagConstraint(this.ultraGridBagLayoutPanel2, gridBagConstraint7);
      this.ultraGridBagLayoutPanel2.Location = new System.Drawing.Point(162, 157);
      this.ultraGridBagLayoutPanel2.Name = "ultraGridBagLayoutPanel2";
      this.ultraGridBagLayoutPanel1.SetPreferredSize(this.ultraGridBagLayoutPanel2, new System.Drawing.Size(173, 115));
      this.ultraGridBagLayoutPanel2.Size = new System.Drawing.Size(294, 117);
      this.ultraGridBagLayoutPanel2.TabIndex = 6;
      // 
      // btnCancel
      // 
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      gridBagConstraint5.Anchor = Infragistics.Win.Layout.AnchorType.BottomRight;
      gridBagConstraint5.OriginX = 1;
      gridBagConstraint5.OriginY = 0;
      gridBagConstraint5.WeightY = 1F;
      this.ultraGridBagLayoutPanel2.SetGridBagConstraint(this.btnCancel, gridBagConstraint5);
      this.btnCancel.Location = new System.Drawing.Point(207, 91);
      this.btnCancel.Name = "btnCancel";
      this.ultraGridBagLayoutPanel2.SetPreferredSize(this.btnCancel, new System.Drawing.Size(87, 26));
      this.btnCancel.Size = new System.Drawing.Size(87, 26);
      this.btnCancel.TabIndex = 1;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // btnOK
      // 
      this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
      gridBagConstraint6.Anchor = Infragistics.Win.Layout.AnchorType.BottomRight;
      gridBagConstraint6.Insets.Right = 8;
      gridBagConstraint6.OriginX = 0;
      gridBagConstraint6.OriginY = 0;
      gridBagConstraint6.WeightX = 1F;
      this.ultraGridBagLayoutPanel2.SetGridBagConstraint(this.btnOK, gridBagConstraint6);
      this.btnOK.Location = new System.Drawing.Point(112, 91);
      this.btnOK.Name = "btnOK";
      this.ultraGridBagLayoutPanel2.SetPreferredSize(this.btnOK, new System.Drawing.Size(87, 26));
      this.btnOK.Size = new System.Drawing.Size(87, 26);
      this.btnOK.TabIndex = 0;
      this.btnOK.Text = "OK";
      this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
      // 
      // txtSanitaryPipesProcName
      // 
      this.txtSanitaryPipesProcName.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Synth.UI.Properties.Settings.Default, "GetSanitaryPipesUSPName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      gridBagConstraint8.Fill = Infragistics.Win.Layout.FillType.Both;
      gridBagConstraint8.Insets.Bottom = 4;
      gridBagConstraint8.Insets.Right = 4;
      gridBagConstraint8.OriginX = 1;
      gridBagConstraint8.OriginY = 2;
      this.ultraGridBagLayoutPanel1.SetGridBagConstraint(this.txtSanitaryPipesProcName, gridBagConstraint8);
      this.txtSanitaryPipesProcName.Location = new System.Drawing.Point(162, 68);
      this.txtSanitaryPipesProcName.Name = "txtSanitaryPipesProcName";
      this.ultraGridBagLayoutPanel1.SetPreferredSize(this.txtSanitaryPipesProcName, new System.Drawing.Size(173, 24));
      this.txtSanitaryPipesProcName.Size = new System.Drawing.Size(290, 25);
      this.txtSanitaryPipesProcName.TabIndex = 5;
      this.txtSanitaryPipesProcName.Text = global::Synth.UI.Properties.Settings.Default.GetSanitaryPipesUSPName;
      // 
      // txtStormPipesProcName
      // 
      this.txtStormPipesProcName.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Synth.UI.Properties.Settings.Default, "GetStormPipesUSPName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      gridBagConstraint9.Fill = Infragistics.Win.Layout.FillType.Both;
      gridBagConstraint9.Insets.Bottom = 4;
      gridBagConstraint9.Insets.Right = 4;
      gridBagConstraint9.OriginX = 1;
      gridBagConstraint9.OriginY = 1;
      this.ultraGridBagLayoutPanel1.SetGridBagConstraint(this.txtStormPipesProcName, gridBagConstraint9);
      this.txtStormPipesProcName.Location = new System.Drawing.Point(162, 38);
      this.txtStormPipesProcName.Name = "txtStormPipesProcName";
      this.ultraGridBagLayoutPanel1.SetPreferredSize(this.txtStormPipesProcName, new System.Drawing.Size(173, 24));
      this.txtStormPipesProcName.Size = new System.Drawing.Size(290, 25);
      this.txtStormPipesProcName.TabIndex = 4;
      this.txtStormPipesProcName.Text = global::Synth.UI.Properties.Settings.Default.GetStormPipesUSPName;
      // 
      // ultraLabel3
      // 
      gridBagConstraint10.Fill = Infragistics.Win.Layout.FillType.Both;
      gridBagConstraint10.Insets.Bottom = 4;
      gridBagConstraint10.Insets.Right = 4;
      gridBagConstraint10.OriginX = 0;
      gridBagConstraint10.OriginY = 2;
      this.ultraGridBagLayoutPanel1.SetGridBagConstraint(this.ultraLabel3, gridBagConstraint10);
      this.ultraLabel3.Location = new System.Drawing.Point(8, 68);
      this.ultraLabel3.Name = "ultraLabel3";
      this.ultraGridBagLayoutPanel1.SetPreferredSize(this.ultraLabel3, new System.Drawing.Size(150, 26));
      this.ultraLabel3.Size = new System.Drawing.Size(150, 26);
      this.ultraLabel3.TabIndex = 3;
      this.ultraLabel3.Text = "Sanitary pipes proc name";
      // 
      // ultraLabel2
      // 
      gridBagConstraint11.Fill = Infragistics.Win.Layout.FillType.Both;
      gridBagConstraint11.Insets.Bottom = 4;
      gridBagConstraint11.Insets.Right = 4;
      gridBagConstraint11.OriginX = 0;
      gridBagConstraint11.OriginY = 1;
      this.ultraGridBagLayoutPanel1.SetGridBagConstraint(this.ultraLabel2, gridBagConstraint11);
      this.ultraLabel2.Location = new System.Drawing.Point(8, 38);
      this.ultraLabel2.Name = "ultraLabel2";
      this.ultraGridBagLayoutPanel1.SetPreferredSize(this.ultraLabel2, new System.Drawing.Size(150, 26));
      this.ultraLabel2.Size = new System.Drawing.Size(150, 26);
      this.ultraLabel2.TabIndex = 2;
      this.ultraLabel2.Text = "Storm pipes proc name";
      // 
      // txtStormDitchesProcName
      // 
      this.txtStormDitchesProcName.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Synth.UI.Properties.Settings.Default, "GetStormDitchesUSPName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      gridBagConstraint12.Fill = Infragistics.Win.Layout.FillType.Both;
      gridBagConstraint12.Insets.Bottom = 4;
      gridBagConstraint12.Insets.Right = 4;
      gridBagConstraint12.OriginX = 1;
      gridBagConstraint12.OriginY = 0;
      gridBagConstraint12.WeightX = 1F;
      this.ultraGridBagLayoutPanel1.SetGridBagConstraint(this.txtStormDitchesProcName, gridBagConstraint12);
      this.txtStormDitchesProcName.Location = new System.Drawing.Point(162, 8);
      this.txtStormDitchesProcName.Name = "txtStormDitchesProcName";
      this.ultraGridBagLayoutPanel1.SetPreferredSize(this.txtStormDitchesProcName, new System.Drawing.Size(173, 24));
      this.txtStormDitchesProcName.Size = new System.Drawing.Size(290, 25);
      this.txtStormDitchesProcName.TabIndex = 1;
      this.txtStormDitchesProcName.Text = global::Synth.UI.Properties.Settings.Default.GetStormDitchesUSPName;
      // 
      // ultraLabel1
      // 
      gridBagConstraint13.Fill = Infragistics.Win.Layout.FillType.Both;
      gridBagConstraint13.Insets.Bottom = 4;
      gridBagConstraint13.Insets.Right = 4;
      gridBagConstraint13.OriginX = 0;
      gridBagConstraint13.OriginY = 0;
      this.ultraGridBagLayoutPanel1.SetGridBagConstraint(this.ultraLabel1, gridBagConstraint13);
      this.ultraLabel1.Location = new System.Drawing.Point(8, 8);
      this.ultraLabel1.Name = "ultraLabel1";
      this.ultraGridBagLayoutPanel1.SetPreferredSize(this.ultraLabel1, new System.Drawing.Size(150, 26));
      this.ultraLabel1.Size = new System.Drawing.Size(150, 26);
      this.ultraLabel1.TabIndex = 0;
      this.ultraLabel1.Text = "Storm ditches proc name";
      // 
      // dlgSettings
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(464, 282);
      this.Controls.Add(this.ultraGridBagLayoutPanel1);
      this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Name = "dlgSettings";
      this.Text = "dlgSettings";
      this.Load += new System.EventHandler(this.dlgSettings_Load);
      ((System.ComponentModel.ISupportInitialize)(this.ultraGridBagLayoutPanel1)).EndInit();
      this.ultraGridBagLayoutPanel1.ResumeLayout(false);
      this.ultraGridBagLayoutPanel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.cmbLogLevel)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.txtLogFileName)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultraGridBagLayoutPanel2)).EndInit();
      this.ultraGridBagLayoutPanel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.txtSanitaryPipesProcName)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.txtStormPipesProcName)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.txtStormDitchesProcName)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private Infragistics.Win.Misc.UltraGridBagLayoutPanel ultraGridBagLayoutPanel1;
    private Infragistics.Win.Misc.UltraGridBagLayoutPanel ultraGridBagLayoutPanel2;
    private Infragistics.Win.Misc.UltraButton btnCancel;
    private Infragistics.Win.Misc.UltraButton btnOK;
    private Infragistics.Win.UltraWinEditors.UltraTextEditor txtSanitaryPipesProcName;
    private Infragistics.Win.UltraWinEditors.UltraTextEditor txtStormPipesProcName;
    private Infragistics.Win.Misc.UltraLabel ultraLabel3;
    private Infragistics.Win.Misc.UltraLabel ultraLabel2;
    private Infragistics.Win.UltraWinEditors.UltraTextEditor txtStormDitchesProcName;
    private Infragistics.Win.Misc.UltraLabel ultraLabel1;
    private Infragistics.Win.UltraWinEditors.UltraTextEditor txtLogFileName;
    private Infragistics.Win.Misc.UltraLabel ultraLabel4;
    private Infragistics.Win.UltraWinEditors.UltraComboEditor cmbLogLevel;
    private Infragistics.Win.Misc.UltraLabel ultraLabel5;
  }
}