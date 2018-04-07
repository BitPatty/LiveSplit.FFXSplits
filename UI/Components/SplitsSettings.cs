using LiveSplit.Model;
using LiveSplit.TimeFormatters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

#pragma warning disable RCS1029
#pragma warning disable IDE1006

namespace LiveSplit.UI.Components
{
    public partial class SplitsSettings : UserControl
    {
        private int _VisualSplitCount { get; set; }
        public int VisualSplitCount
        {
            get { return _VisualSplitCount; }
            set
            {
                _VisualSplitCount = value;
                var max = Math.Max(0, _VisualSplitCount - (AlwaysShowLastSplit ? 2 : 1));
                if (dmnUpcomingSegments.Value > max)
                    dmnUpcomingSegments.Value = max;
                dmnUpcomingSegments.Maximum = max;
            }
        }

        public int SplitPreviewCount { get; set; }
        public float SplitWidth { get; set; }
        public float SplitHeight { get; set; }
        public float ScaledSplitHeight { get { return SplitHeight * 10f; } set { SplitHeight = value / 10f; } }

        public bool Display2Rows { get; set; }

        public LiveSplitState CurrentState { get; set; }

        public bool ShowTitle { get; set; }
        public bool ShowGameName { get; set; }
        public bool ShowCategoryName { get; set; }
        public bool ShowFinishedRunsCount { get; set; }
        public bool ShowAttemptCount { get; set; }
        public bool DisplayGameIcon { get; set; }
        public bool CenterTitle { get; set; }
        public bool ShowRegion { get; set; }
        public bool ShowPlatform { get; set; }
        public bool ShowVariables { get; set; }
        public bool ShowCount => ShowAttemptCount || ShowFinishedRunsCount;

        public bool ShowThinSeparators { get; set; }
        public bool AlwaysShowLastSplit { get; set; }
        public bool ShowBlankSplits { get; set; }
        public bool LockLastSplit { get; set; }
        public bool SeparatorLastSplit { get; set; }

        public bool DropDecimals { get; set; }
        public TimeAccuracy DeltasAccuracy { get; set; }

        public bool OverrideDeltasColor { get; set; }
        public Color DeltasColor { get; set; }

        public string SplitsLabel { get; set; }
        public bool ShowColumnLabels { get; set; }
        public Color LabelsColor { get; set; }

        public bool AutomaticAbbreviations { get; set; }
        public Color BeforeNamesColor { get; set; }
        public Color CurrentNamesColor { get; set; }
        public Color AfterNamesColor { get; set; }
        public bool OverrideTextColor { get; set; }
        public Color BeforeTimesColor { get; set; }
        public Color CurrentTimesColor { get; set; }
        public Color AfterTimesColor { get; set; }
        public bool OverrideTimesColor { get; set; }

        public TimeAccuracy SplitTimesAccuracy { get; set; }

        public event EventHandler SplitLayoutChanged;

        public LayoutMode Mode { get; set; }

        public IList<ColumnSettings> ColumnsList { get; set; }
        public Size StartingSize { get; set; }
        public Size StartingTableLayoutSize { get; set; }

        public SplitsSettings(LiveSplitState state)
        {
            InitializeComponent();

            CurrentState = state;

            StartingSize = Size;
            StartingTableLayoutSize = tableColumns.Size;

            VisualSplitCount = 8;
            SplitPreviewCount = 1;
            ShowThinSeparators = true;
            AlwaysShowLastSplit = true;
            ShowBlankSplits = true;
            LockLastSplit = true;
            SeparatorLastSplit = true;
            SplitTimesAccuracy = TimeAccuracy.Seconds;
            SplitWidth = 20;
            SplitHeight = 3.6f;
            AutomaticAbbreviations = false;
            BeforeNamesColor = Color.FromArgb(255, 255, 255);
            CurrentNamesColor = Color.FromArgb(255, 255, 255);
            AfterNamesColor = Color.FromArgb(255, 255, 255);
            OverrideTextColor = false;
            BeforeTimesColor = Color.FromArgb(255, 255, 255);
            CurrentTimesColor = Color.FromArgb(255, 255, 255);
            AfterTimesColor = Color.FromArgb(255, 255, 255);
            OverrideTimesColor = false;
            DropDecimals = true;
            DeltasAccuracy = TimeAccuracy.Tenths;
            OverrideDeltasColor = false;
            DeltasColor = Color.FromArgb(255, 255, 255);
            Display2Rows = false;
            ShowColumnLabels = false;
            LabelsColor = Color.FromArgb(255, 255, 255);
            SplitsLabel = "Splits";

            dmnTotalSegments.DataBindings.Add("Value", this, "VisualSplitCount", false, DataSourceUpdateMode.OnPropertyChanged);
            dmnUpcomingSegments.DataBindings.Add("Value", this, "SplitPreviewCount", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAutomaticAbbreviations.DataBindings.Add("Checked", this, "AutomaticAbbreviations", false, DataSourceUpdateMode.OnPropertyChanged);
            btnBeforeNamesColor.DataBindings.Add("BackColor", this, "BeforeNamesColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnCurrentNamesColor.DataBindings.Add("BackColor", this, "CurrentNamesColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnAfterNamesColor.DataBindings.Add("BackColor", this, "AfterNamesColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnBeforeTimesColor.DataBindings.Add("BackColor", this, "BeforeTimesColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnCurrentTimesColor.DataBindings.Add("BackColor", this, "CurrentTimesColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnAfterTimesColor.DataBindings.Add("BackColor", this, "AfterTimesColor", false, DataSourceUpdateMode.OnPropertyChanged);
            chkThinSeparators.DataBindings.Add("Checked", this, "ShowThinSeparators", false, DataSourceUpdateMode.OnPropertyChanged);
            chkLastSplit.DataBindings.Add("Checked", this, "AlwaysShowLastSplit", false, DataSourceUpdateMode.OnPropertyChanged);
            chkOverrideTextColor.DataBindings.Add("Checked", this, "OverrideTextColor", false, DataSourceUpdateMode.OnPropertyChanged);
            chkOverrideTimesColor.DataBindings.Add("Checked", this, "OverrideTimesColor", false, DataSourceUpdateMode.OnPropertyChanged);
            chkShowBlankSplits.DataBindings.Add("Checked", this, "ShowBlankSplits", false, DataSourceUpdateMode.OnPropertyChanged);
            chkLockLastSplit.DataBindings.Add("Checked", this, "LockLastSplit", false, DataSourceUpdateMode.OnPropertyChanged);
            chkSeparatorLastSplit.DataBindings.Add("Checked", this, "SeparatorLastSplit", false, DataSourceUpdateMode.OnPropertyChanged);
            chkDropDecimals.DataBindings.Add("Checked", this, "DropDecimals", false, DataSourceUpdateMode.OnPropertyChanged);
            chkOverrideDeltaColor.DataBindings.Add("Checked", this, "OverrideDeltasColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnDeltaColor.DataBindings.Add("BackColor", this, "DeltasColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnLabelColor.DataBindings.Add("BackColor", this, "LabelsColor", false, DataSourceUpdateMode.OnPropertyChanged);
            txtSplitsLabel.DataBindings.Add("Text", this, "SplitsLabel", false, DataSourceUpdateMode.OnPropertyChanged);

            //Title
            chkShowTitle.DataBindings.Add("Checked", this, "ShowTitle", false, DataSourceUpdateMode.OnPropertyChanged);
            chkShowGameName.DataBindings.Add("Checked", this, "ShowGameName", false, DataSourceUpdateMode.OnPropertyChanged);
            chkShowCategoryName.DataBindings.Add("Checked", this, "ShowCategoryName", false, DataSourceUpdateMode.OnPropertyChanged);
            chkShowFinishedRunsCount.DataBindings.Add("Checked", this, "ShowFinishedRunsCount", false, DataSourceUpdateMode.OnPropertyChanged);
            chkShowAttemptCount.DataBindings.Add("Checked", this, "ShowAttemptCount", false, DataSourceUpdateMode.OnPropertyChanged);
            chkDisplayGameIcon.DataBindings.Add("Checked", this, "DisplayGameIcon", false, DataSourceUpdateMode.OnPropertyChanged);
            chkCenterText.DataBindings.Add("Checked", this, "CenterTitle", false, DataSourceUpdateMode.OnPropertyChanged);
            chkShowRegion.DataBindings.Add("Checked", this, "ShowRegion", false, DataSourceUpdateMode.OnPropertyChanged);
            chkShowPlatform.DataBindings.Add("Checked", this, "ShowPlatform", false, DataSourceUpdateMode.OnPropertyChanged);
            chkShowVariables.DataBindings.Add("Checked", this, "ShowVariables", false, DataSourceUpdateMode.OnPropertyChanged);

            ColumnsList = new List<ColumnSettings>();
            ColumnsList.Add(new ColumnSettings(CurrentState, "+/-", ColumnsList) { Data = new ColumnData("+/-", ColumnType.Delta, "Current Comparison", "Current Timing Method") });
            ColumnsList.Add(new ColumnSettings(CurrentState, "Time", ColumnsList) { Data = new ColumnData("Time", ColumnType.SplitTime, "Current Comparison", "Current Timing Method") });
        }

        private void chkShowTitle_CheckedChanged(object sender, EventArgs e)
        {
            chkShowGameName.Enabled =
            chkShowCategoryName.Enabled =
            chkShowFinishedRunsCount.Enabled =
            chkShowAttemptCount.Enabled =
            chkDisplayGameIcon.Enabled =
            chkCenterText.Enabled =
            chkShowRegion.Enabled =
            chkShowPlatform.Enabled =
            chkShowVariables.Enabled = chkShowTitle.Checked;
        }

        private void chkColumnLabels_CheckedChanged(object sender, EventArgs e)
        {
            btnLabelColor.Enabled = lblLabelsColor.Enabled = chkColumnLabels.Checked;
        }

        private void chkOverrideTimesColor_CheckedChanged(object sender, EventArgs e)
        {
            label6.Enabled = label9.Enabled = label7.Enabled = btnBeforeTimesColor.Enabled
                = btnCurrentTimesColor.Enabled = btnAfterTimesColor.Enabled = chkOverrideTimesColor.Checked;
        }

        private void chkOverrideDeltaColor_CheckedChanged(object sender, EventArgs e)
        {
            label8.Enabled = btnDeltaColor.Enabled = chkOverrideDeltaColor.Checked;
        }

        private void chkOverrideTextColor_CheckedChanged(object sender, EventArgs e)
        {
            label3.Enabled = label10.Enabled = label13.Enabled = btnBeforeNamesColor.Enabled
            = btnCurrentNamesColor.Enabled = btnAfterNamesColor.Enabled = chkOverrideTextColor.Checked;
        }

        private void rdoDeltaTenths_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDeltaAccuracy();
        }

        private void rdoDeltaSeconds_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDeltaAccuracy();
        }

        private void chkSeparatorLastSplit_CheckedChanged(object sender, EventArgs e)
        {
            SeparatorLastSplit = chkSeparatorLastSplit.Checked;
            SplitLayoutChanged(this, null);
        }

        private void chkLockLastSplit_CheckedChanged(object sender, EventArgs e)
        {
            LockLastSplit = chkLockLastSplit.Checked;
            SplitLayoutChanged(this, null);
        }

        private void chkShowBlankSplits_CheckedChanged(object sender, EventArgs e)
        {
            ShowBlankSplits = chkLockLastSplit.Enabled = chkShowBlankSplits.Checked;
            SplitLayoutChanged(this, null);
        }

        private void rdoTenths_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAccuracy();
        }

        private void rdoSeconds_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAccuracy();
        }

        private void UpdateAccuracy()
        {
            if (rdoSeconds.Checked)
                SplitTimesAccuracy = TimeAccuracy.Seconds;
            else if (rdoTenths.Checked)
                SplitTimesAccuracy = TimeAccuracy.Tenths;
            else
                SplitTimesAccuracy = TimeAccuracy.Hundredths;
        }

        private void UpdateDeltaAccuracy()
        {
            if (rdoDeltaSeconds.Checked)
                DeltasAccuracy = TimeAccuracy.Seconds;
            else if (rdoDeltaTenths.Checked)
                DeltasAccuracy = TimeAccuracy.Tenths;
            else
                DeltasAccuracy = TimeAccuracy.Hundredths;
        }

        private void chkLastSplit_CheckedChanged(object sender, EventArgs e)
        {
            AlwaysShowLastSplit = chkLastSplit.Checked;
            VisualSplitCount = VisualSplitCount;
            SplitLayoutChanged(this, null);
        }

        private void chkThinSeparators_CheckedChanged(object sender, EventArgs e)
        {
            ShowThinSeparators = chkThinSeparators.Checked;
            SplitLayoutChanged(this, null);
        }

        private void SplitsSettings_Load(object sender, EventArgs e)
        {
            chkShowTitle_CheckedChanged(null, null);

            ResetColumns();

            chkOverrideDeltaColor_CheckedChanged(null, null);
            chkOverrideTextColor_CheckedChanged(null, null);
            chkOverrideTimesColor_CheckedChanged(null, null);
            chkColumnLabels_CheckedChanged(null, null);
            chkLockLastSplit.Enabled = chkShowBlankSplits.Checked;

            rdoSeconds.Checked = SplitTimesAccuracy == TimeAccuracy.Seconds;
            rdoTenths.Checked = SplitTimesAccuracy == TimeAccuracy.Tenths;
            rdoHundredths.Checked = SplitTimesAccuracy == TimeAccuracy.Hundredths;

            rdoDeltaSeconds.Checked = DeltasAccuracy == TimeAccuracy.Seconds;
            rdoDeltaTenths.Checked = DeltasAccuracy == TimeAccuracy.Tenths;
            rdoDeltaHundredths.Checked = DeltasAccuracy == TimeAccuracy.Hundredths;

            if (Mode == LayoutMode.Horizontal)
            {
                trkSize.DataBindings.Clear();
                trkSize.Minimum = 5;
                trkSize.Maximum = 120;
                SplitWidth = Math.Min(Math.Max(trkSize.Minimum, SplitWidth), trkSize.Maximum);
                trkSize.DataBindings.Add("Value", this, "SplitWidth", false, DataSourceUpdateMode.OnPropertyChanged);
                lblSplitSize.Text = "Split Width:";
                chkDisplayRows.Enabled = false;
                chkDisplayRows.DataBindings.Clear();
                chkDisplayRows.Checked = true;
                chkColumnLabels.DataBindings.Clear();
                chkColumnLabels.Enabled = chkColumnLabels.Checked = false;
            }
            else
            {
                trkSize.DataBindings.Clear();
                trkSize.Minimum = 0;
                trkSize.Maximum = 250;
                ScaledSplitHeight = Math.Min(Math.Max(trkSize.Minimum, ScaledSplitHeight), trkSize.Maximum);
                trkSize.DataBindings.Add("Value", this, "ScaledSplitHeight", false, DataSourceUpdateMode.OnPropertyChanged);
                lblSplitSize.Text = "Split Height:";
                chkDisplayRows.Enabled = true;
                chkDisplayRows.DataBindings.Clear();
                chkDisplayRows.DataBindings.Add("Checked", this, "Display2Rows", false, DataSourceUpdateMode.OnPropertyChanged);
                chkColumnLabels.DataBindings.Clear();
                chkColumnLabels.Enabled = true;
                chkColumnLabels.DataBindings.Add("Checked", this, "ShowColumnLabels", false, DataSourceUpdateMode.OnPropertyChanged);
            }
        }

        public void SetSettings(XmlNode node)
        {
            var element = (XmlElement)node;
            Version version = SettingsHelper.ParseVersion(element["Version"]);

            VisualSplitCount = SettingsHelper.ParseInt(element["VisualSplitCount"]);
            SplitPreviewCount = SettingsHelper.ParseInt(element["SplitPreviewCount"]);
            ShowThinSeparators = SettingsHelper.ParseBool(element["ShowThinSeparators"]);
            AlwaysShowLastSplit = SettingsHelper.ParseBool(element["AlwaysShowLastSplit"]);
            SplitWidth = SettingsHelper.ParseFloat(element["SplitWidth"]);
            AutomaticAbbreviations = SettingsHelper.ParseBool(element["AutomaticAbbreviations"], false);
            ShowColumnLabels = SettingsHelper.ParseBool(element["ShowColumnLabels"], false);
            LabelsColor = SettingsHelper.ParseColor(element["LabelsColor"], Color.FromArgb(255, 255, 255));
            OverrideTimesColor = SettingsHelper.ParseBool(element["OverrideTimesColor"], false);
            BeforeTimesColor = SettingsHelper.ParseColor(element["BeforeTimesColor"], Color.FromArgb(255, 255, 255));
            CurrentTimesColor = SettingsHelper.ParseColor(element["CurrentTimesColor"], Color.FromArgb(255, 255, 255));
            AfterTimesColor = SettingsHelper.ParseColor(element["AfterTimesColor"], Color.FromArgb(255, 255, 255));
            SplitHeight = SettingsHelper.ParseFloat(element["SplitHeight"], 6);
            SeparatorLastSplit = SettingsHelper.ParseBool(element["SeparatorLastSplit"], true);
            DropDecimals = SettingsHelper.ParseBool(element["DropDecimals"], true);
            DeltasAccuracy = SettingsHelper.ParseEnum(element["DeltasAccuracy"], TimeAccuracy.Tenths);
            OverrideDeltasColor = SettingsHelper.ParseBool(element["OverrideDeltasColor"], false);
            DeltasColor = SettingsHelper.ParseColor(element["DeltasColor"], Color.FromArgb(255, 255, 255));
            Display2Rows = SettingsHelper.ParseBool(element["Display2Rows"], false);
            SplitTimesAccuracy = SettingsHelper.ParseEnum(element["SplitTimesAccuracy"], TimeAccuracy.Seconds);
            ShowBlankSplits = SettingsHelper.ParseBool(element["ShowBlankSplits"], true);
            LockLastSplit = SettingsHelper.ParseBool(element["LockLastSplit"], false);
            SplitsLabel = SettingsHelper.ParseString(element["SplitsLabel"], "Splits");

            //Title
            ShowTitle = SettingsHelper.ParseBool(element["ShowTitle"], false);
            ShowGameName = SettingsHelper.ParseBool(element["ShowGameName"], false);
            ShowCategoryName = SettingsHelper.ParseBool(element["ShowCategoryName"], false);
            ShowFinishedRunsCount = SettingsHelper.ParseBool(element["ShowFinishedRunsCount"], false);
            ShowAttemptCount = SettingsHelper.ParseBool(element["ShowAttemptCount"], false);
            DisplayGameIcon = SettingsHelper.ParseBool(element["DisplayGameIcon"], false);
            CenterTitle = SettingsHelper.ParseBool(element["CenterTitle"], false);
            ShowRegion = SettingsHelper.ParseBool(element["ShowRegion"], false);
            ShowPlatform = SettingsHelper.ParseBool(element["ShowPlatform"], false);
            ShowVariables = SettingsHelper.ParseBool(element["ShowVariables"], false);

            if (version >= new Version(1, 5))
            {
                var columnsElement = element["Columns"];
                ColumnsList.Clear();
                foreach (var child in columnsElement.ChildNodes)
                {
                    var columnData = ColumnData.FromXml((XmlNode)child);
                    ColumnsList.Add(new ColumnSettings(CurrentState, columnData.Name, ColumnsList) { Data = columnData });
                }
            }
            else
            {
                ColumnsList.Clear();
                var comparison = SettingsHelper.ParseString(element["Comparison"]);
                if (SettingsHelper.ParseBool(element["ShowSplitTimes"]))
                {
                    ColumnsList.Add(new ColumnSettings(CurrentState, "+/-", ColumnsList) { Data = new ColumnData("+/-", ColumnType.Delta, comparison, "Current Timing Method") });
                    ColumnsList.Add(new ColumnSettings(CurrentState, "Time", ColumnsList) { Data = new ColumnData("Time", ColumnType.SplitTime, comparison, "Current Timing Method") });
                }
                else
                {
                    ColumnsList.Add(new ColumnSettings(CurrentState, "+/-", ColumnsList) { Data = new ColumnData("+/-", ColumnType.DeltaorSplitTime, comparison, "Current Timing Method") });
                }
            }
            if (version >= new Version(1, 3))
            {
                BeforeNamesColor = SettingsHelper.ParseColor(element["BeforeNamesColor"]);
                CurrentNamesColor = SettingsHelper.ParseColor(element["CurrentNamesColor"]);
                AfterNamesColor = SettingsHelper.ParseColor(element["AfterNamesColor"]);
                OverrideTextColor = SettingsHelper.ParseBool(element["OverrideTextColor"]);
            }
            else
            {
                if (version >= new Version(1, 2))
                    BeforeNamesColor = CurrentNamesColor = AfterNamesColor = SettingsHelper.ParseColor(element["SplitNamesColor"]);
                else
                {
                    BeforeNamesColor = Color.FromArgb(255, 255, 255);
                    CurrentNamesColor = Color.FromArgb(255, 255, 255);
                    AfterNamesColor = Color.FromArgb(255, 255, 255);
                }
                OverrideTextColor = !SettingsHelper.ParseBool(element["UseTextColor"], true);
            }
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            var parent = document.CreateElement("Settings");
            CreateSettingsNode(document, parent);
            return parent;
        }

        public int GetSettingsHashCode()
        {
            return CreateSettingsNode(null, null);
        }

        private int CreateSettingsNode(XmlDocument document, XmlElement parent)
        {
            var hashCode = SettingsHelper.CreateSetting(document, parent, "Version", "1.6") ^
            SettingsHelper.CreateSetting(document, parent, "VisualSplitCount", VisualSplitCount) ^
            SettingsHelper.CreateSetting(document, parent, "SplitPreviewCount", SplitPreviewCount) ^
            SettingsHelper.CreateSetting(document, parent, "ShowThinSeparators", ShowThinSeparators) ^
            SettingsHelper.CreateSetting(document, parent, "AlwaysShowLastSplit", AlwaysShowLastSplit) ^
            SettingsHelper.CreateSetting(document, parent, "SplitWidth", SplitWidth) ^
            SettingsHelper.CreateSetting(document, parent, "SplitTimesAccuracy", SplitTimesAccuracy) ^
            SettingsHelper.CreateSetting(document, parent, "AutomaticAbbreviations", AutomaticAbbreviations) ^
            SettingsHelper.CreateSetting(document, parent, "BeforeNamesColor", BeforeNamesColor) ^
            SettingsHelper.CreateSetting(document, parent, "CurrentNamesColor", CurrentNamesColor) ^
            SettingsHelper.CreateSetting(document, parent, "AfterNamesColor", AfterNamesColor) ^
            SettingsHelper.CreateSetting(document, parent, "OverrideTextColor", OverrideTextColor) ^
            SettingsHelper.CreateSetting(document, parent, "BeforeTimesColor", BeforeTimesColor) ^
            SettingsHelper.CreateSetting(document, parent, "CurrentTimesColor", CurrentTimesColor) ^
            SettingsHelper.CreateSetting(document, parent, "AfterTimesColor", AfterTimesColor) ^
            SettingsHelper.CreateSetting(document, parent, "OverrideTimesColor", OverrideTimesColor) ^
            SettingsHelper.CreateSetting(document, parent, "ShowBlankSplits", ShowBlankSplits) ^
            SettingsHelper.CreateSetting(document, parent, "LockLastSplit", LockLastSplit) ^
            SettingsHelper.CreateSetting(document, parent, "SplitHeight", SplitHeight) ^
            SettingsHelper.CreateSetting(document, parent, "SeparatorLastSplit", SeparatorLastSplit) ^
            SettingsHelper.CreateSetting(document, parent, "DeltasAccuracy", DeltasAccuracy) ^
            SettingsHelper.CreateSetting(document, parent, "DropDecimals", DropDecimals) ^
            SettingsHelper.CreateSetting(document, parent, "OverrideDeltasColor", OverrideDeltasColor) ^
            SettingsHelper.CreateSetting(document, parent, "DeltasColor", DeltasColor) ^
            SettingsHelper.CreateSetting(document, parent, "Display2Rows", Display2Rows) ^
            SettingsHelper.CreateSetting(document, parent, "ShowColumnLabels", ShowColumnLabels) ^
            SettingsHelper.CreateSetting(document, parent, "LabelsColor", LabelsColor) ^
            SettingsHelper.CreateSetting(document, parent, "SplitsLabel", SplitsLabel) ^
            SettingsHelper.CreateSetting(document, parent, "ShowTitle", ShowTitle) ^
            SettingsHelper.CreateSetting(document, parent, "ShowGameName", ShowGameName) ^
            SettingsHelper.CreateSetting(document, parent, "ShowCategoryName", ShowCategoryName) ^
            SettingsHelper.CreateSetting(document, parent, "ShowFinishedRunsCount", ShowFinishedRunsCount) ^
            SettingsHelper.CreateSetting(document, parent, "ShowAttemptCount", ShowAttemptCount) ^
            SettingsHelper.CreateSetting(document, parent, "DisplayGameIcon", DisplayGameIcon) ^
            SettingsHelper.CreateSetting(document, parent, "CenterTitle", CenterTitle) ^
            SettingsHelper.CreateSetting(document, parent, "ShowRegion", ShowRegion) ^
            SettingsHelper.CreateSetting(document, parent, "ShowPlatform", ShowPlatform) ^
            SettingsHelper.CreateSetting(document, parent, "ShowVariables", ShowVariables);

            XmlElement columnsElement = null;
            if (document != null)
            {
                columnsElement = document.CreateElement("Columns");
                parent.AppendChild(columnsElement);
            }

            var count = 1;
            foreach (var columnData in ColumnsList.Select(x => x.Data))
            {
                XmlElement settings = null;
                if (document != null)
                {
                    settings = document.CreateElement("Settings");
                    columnsElement.AppendChild(settings);
                }
                hashCode ^= columnData.CreateElement(document, settings) * count;
                count++;
            }

            return hashCode;
        }

        private void ColorButtonClick(object sender, EventArgs e)
        {
            SettingsHelper.ColorButtonClick((Button)sender, this);
        }

        private void ResetColumns()
        {
            ClearLayout();
            var index = 2;
            foreach (var column in ColumnsList)
            {
                UpdateLayoutForColumn();
                AddColumnToLayout(column, index);
                column.UpdateEnabledButtons();
                index++;
            }
        }

        private void AddColumnToLayout(ColumnSettings column, int index)
        {
            tableColumns.Controls.Add(column, 0, index);
            tableColumns.SetColumnSpan(column, 4);
            column.ColumnRemoved -= column_ColumnRemoved;
            column.MovedUp -= column_MovedUp;
            column.MovedDown -= column_MovedDown;
            column.ColumnRemoved += column_ColumnRemoved;
            column.MovedUp += column_MovedUp;
            column.MovedDown += column_MovedDown;
        }

        private void column_MovedDown(object sender, EventArgs e)
        {
            var column = (ColumnSettings)sender;
            var index = ColumnsList.IndexOf(column);
            ColumnsList.Remove(column);
            ColumnsList.Insert(index + 1, column);
            ResetColumns();
            column.SelectControl();
        }

        private void column_MovedUp(object sender, EventArgs e)
        {
            var column = (ColumnSettings)sender;
            var index = ColumnsList.IndexOf(column);
            ColumnsList.Remove(column);
            ColumnsList.Insert(index - 1, column);
            ResetColumns();
            column.SelectControl();
        }

        private void column_ColumnRemoved(object sender, EventArgs e)
        {
            var column = (ColumnSettings)sender;
            var index = ColumnsList.IndexOf(column);
            ColumnsList.Remove(column);
            ResetColumns();
            if (ColumnsList.Count > 0)
                ColumnsList.Last().SelectControl();
            else
                chkColumnLabels.Select();
        }

        private void ClearLayout()
        {
            tableColumns.RowCount = 2;
            tableColumns.RowStyles.Clear();
            tableColumns.RowStyles.Add(new RowStyle(SizeType.Absolute, 29f));
            tableColumns.RowStyles.Add(new RowStyle(SizeType.Absolute, 29f));
            tableColumns.Size = StartingTableLayoutSize;
            foreach (var control in tableColumns.Controls.OfType<ColumnSettings>().ToList())
            {
                tableColumns.Controls.Remove(control);
            }
            Size = StartingSize;
        }

        private void UpdateLayoutForColumn()
        {
            tableColumns.RowCount++;
            tableColumns.RowStyles.Add(new RowStyle(SizeType.Absolute, 179f));
            tableColumns.Size = new Size(tableColumns.Size.Width, tableColumns.Size.Height + 179);
            Size = new Size(Size.Width, Size.Height + 179);
            groupColumns.Size = new Size(groupColumns.Size.Width, groupColumns.Size.Height + 179);
        }

        private void btnAddColumn_Click(object sender, EventArgs e)
        {
            UpdateLayoutForColumn();

            var columnControl = new ColumnSettings(CurrentState, "#" + (ColumnsList.Count + 1), ColumnsList);
            ColumnsList.Add(columnControl);
            AddColumnToLayout(columnControl, ColumnsList.Count + 1);

            foreach (var column in ColumnsList)
                column.UpdateEnabledButtons();
        }
    }
}
