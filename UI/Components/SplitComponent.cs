using LiveSplit.Model;
using LiveSplit.TimeFormatters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;

namespace LiveSplit.UI.Components
{
    public class SplitComponent : IComponent
    {
        public ISegment Split { get; set; }

        protected SimpleLabel NameLabel { get; set; }
        protected SimpleLabel MeasureTimeLabel { get; set; }
        protected SimpleLabel MeasureDeltaLabel { get; set; }
        public SplitsSettings Settings { get; set; }

        protected int FrameCount { get; set; }

        public GraphicsCache Cache { get; set; }
        protected bool NeedUpdateAll { get; set; }
        protected bool IsActive { get; set; }

        protected TimeAccuracy CurrentAccuracy { get; set; }
        protected TimeAccuracy CurrentDeltaAccuracy { get; set; }
        protected bool CurrentDropDecimals { get; set; }

        protected ITimeFormatter TimeFormatter { get; set; }
        protected ITimeFormatter DeltaTimeFormatter { get; set; }

        public Image SplitCursor = Properties.Resources.cursor;


        public float PaddingTop => 0f;
        public float PaddingLeft => 0f;
        public float PaddingBottom => 0f;
        public float PaddingRight => 0f;

        public IEnumerable<ColumnData> ColumnsList { get; set; }
        public IList<SimpleLabel> LabelsList { get; set; }

        public float VerticalHeight { get; set; }

        public float MinimumWidth
            => CalculateLabelsWidth() + 10;

        public float HorizontalWidth
            => Settings.SplitWidth + CalculateLabelsWidth();

        public float MinimumHeight { get; set; }

        public Bitmap img { get; set; }

        public IDictionary<string, Action> ContextMenuControls => null;

        public SplitComponent(SplitsSettings settings, IEnumerable<ColumnData> columnsList)
        {
            NameLabel = new SimpleLabel()
            {
                HorizontalAlignment = StringAlignment.Near,
                X = 8,
            };
            MeasureTimeLabel = new SimpleLabel();
            MeasureDeltaLabel = new SimpleLabel();
            Settings = settings;
            ColumnsList = columnsList;
            TimeFormatter = new RegularSplitTimeFormatter(Settings.SplitTimesAccuracy);
            DeltaTimeFormatter = new DeltaSplitTimeFormatter(Settings.DeltasAccuracy, Settings.DropDecimals);
            MinimumHeight = 25;
            VerticalHeight = 31;

            NeedUpdateAll = true;
            IsActive = false;

            Cache = new GraphicsCache();
            LabelsList = new List<SimpleLabel>();
        }

        private void DrawGeneral(Graphics g, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            if (NeedUpdateAll)
                UpdateAll(state);

            int splitIndex = state.Run.IndexOf(Split) % 7;

            if (IsActive)
            {
                switch (splitIndex)
                {
                    case 0:
                        img = Properties.Resources.row_0_s;
                        break;
                    case 1:
                        img = Properties.Resources.row_1_s;
                        break;
                    case 2:
                        img = Properties.Resources.row_2_s;
                        break;
                    case 3:
                        img = Properties.Resources.row_3_s;
                        break;
                    case 4:
                        img = Properties.Resources.row_4_s;
                        break;
                    case 5:
                        img = Properties.Resources.row_5_s;
                        break;
                    case 6:
                        img = Properties.Resources.row_6_s;
                        break;
                    default:
                        img = Properties.Resources.row_0_s;
                        break;
                }
            }
            else
            {
                switch (splitIndex)
                {
                    case 0:
                        img = Properties.Resources.row_0;
                        break;
                    case 1:
                        img = Properties.Resources.row_1;
                        break;
                    case 2:
                        img = Properties.Resources.row_2;
                        break;
                    case 3:
                        img = Properties.Resources.row_3;
                        break;
                    case 4:
                        img = Properties.Resources.row_4;
                        break;
                    case 5:
                        img = Properties.Resources.row_5;
                        break;
                    case 6:
                        img = Properties.Resources.row_6;
                        break;
                    default:
                        img = Properties.Resources.row_0;
                        break;
                }
            }

            g.DrawImage(img, 0, 0, width, height);


            MeasureTimeLabel.Text = TimeFormatter.Format(new TimeSpan(24, 0, 0));
            MeasureDeltaLabel.Text = DeltaTimeFormatter.Format(new TimeSpan(0, 9, 0, 0));

            MeasureTimeLabel.Font = state.LayoutSettings.TimesFont;
            MeasureTimeLabel.IsMonospaced = true;
            MeasureDeltaLabel.Font = state.LayoutSettings.TimesFont;
            MeasureDeltaLabel.IsMonospaced = true;

            MeasureTimeLabel.SetActualWidth(g);
            MeasureDeltaLabel.SetActualWidth(g);

            NameLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
            NameLabel.OutlineColor = state.LayoutSettings.TextOutlineColor;
            foreach (var label in LabelsList)
            {
                label.ShadowColor = state.LayoutSettings.ShadowsColor;
                label.OutlineColor = state.LayoutSettings.TextOutlineColor;
            }

            if (Settings.SplitTimesAccuracy != CurrentAccuracy)
            {
                TimeFormatter = new RegularSplitTimeFormatter(Settings.SplitTimesAccuracy);
                CurrentAccuracy = Settings.SplitTimesAccuracy;
            }
            if (Settings.DeltasAccuracy != CurrentDeltaAccuracy || Settings.DropDecimals != CurrentDropDecimals)
            {
                DeltaTimeFormatter = new DeltaSplitTimeFormatter(Settings.DeltasAccuracy, Settings.DropDecimals);
                CurrentDeltaAccuracy = Settings.DeltasAccuracy;
                CurrentDropDecimals = Settings.DropDecimals;
            }

            if (Split != null)
            {

                if (mode == LayoutMode.Vertical)
                {
                    NameLabel.VerticalAlignment = StringAlignment.Center;
                    NameLabel.Y = 0;
                    NameLabel.Height = height;
                    foreach (var label in LabelsList)
                    {
                        label.VerticalAlignment = StringAlignment.Center;
                        label.Y = 0;
                        label.Height = height;
                    }
                }
                else
                {
                    NameLabel.VerticalAlignment = StringAlignment.Near;
                    NameLabel.Y = 0;
                    NameLabel.Height = 50;
                    foreach (var label in LabelsList)
                    {
                        label.VerticalAlignment = StringAlignment.Far;
                        label.Y = height - 50;
                        label.Height = 50;
                    }
                }

                if (IsActive)
                    g.DrawImage(SplitCursor, 2, ((height - 24f) / 2.0f), 30f, 24f);

                NameLabel.Font = state.LayoutSettings.TextFont;
                NameLabel.X = IsActive ? 35 : 5;
                NameLabel.HasShadow = state.LayoutSettings.DropShadows;

                if (ColumnsList.Count() == LabelsList.Count)
                {
                    var curX = width - 12;
                    var nameX = width - 7;
                    foreach (var label in LabelsList.Reverse())
                    {
                        var column = ColumnsList.ElementAt(LabelsList.IndexOf(label));

                        var labelWidth = 0f;
                        if (column.Type == ColumnType.DeltaorSplitTime || column.Type == ColumnType.SegmentDeltaorSegmentTime)
                            labelWidth = Math.Max(MeasureDeltaLabel.ActualWidth, MeasureTimeLabel.ActualWidth);
                        else if (column.Type == ColumnType.Delta || column.Type == ColumnType.SegmentDelta)
                            labelWidth = MeasureDeltaLabel.ActualWidth;
                        else
                            labelWidth = MeasureTimeLabel.ActualWidth;

                        label.Width = labelWidth + 20;
                        curX -= labelWidth + 5;
                        label.X = curX - 15;

                        label.Font = state.LayoutSettings.TimesFont;
                        label.HasShadow = state.LayoutSettings.DropShadows;
                        label.OutlineColor = state.LayoutSettings.TextOutlineColor;

                        label.IsMonospaced = true;
                        label.Draw(g);

                        if (!string.IsNullOrEmpty(label.Text))
                            nameX = curX + labelWidth + 5 - label.ActualWidth;

                    }

                    NameLabel.Width = (mode == LayoutMode.Horizontal ? width - 10 : width * 0.4f);
                    NameLabel.Draw(g);
                }
            }
        }

        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
        {
            if (Settings.Display2Rows)
            {
                VerticalHeight = Settings.SplitHeight + 0.85f * (g.MeasureString("A", state.LayoutSettings.TimesFont).Height + g.MeasureString("A", state.LayoutSettings.TextFont).Height);
                DrawGeneral(g, state, width, VerticalHeight, LayoutMode.Horizontal);
            }
            else
            {
                VerticalHeight = Settings.SplitHeight + 27;
                DrawGeneral(g, state, width, VerticalHeight, LayoutMode.Vertical);
            }
        }

        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
        {
            MinimumHeight = 0.85f * (g.MeasureString("A", state.LayoutSettings.TimesFont).Height + g.MeasureString("A", state.LayoutSettings.TextFont).Height);
            DrawGeneral(g, state, HorizontalWidth, height, LayoutMode.Horizontal);
        }

        public string ComponentName => "Split";


        public Control GetSettingsControl(LayoutMode mode)
        {
            throw new NotSupportedException();
        }

        public void SetSettings(System.Xml.XmlNode settings)
        {
            throw new NotSupportedException();
        }


        public System.Xml.XmlNode GetSettings(System.Xml.XmlDocument document)
        {
            throw new NotSupportedException();
        }

        public string UpdateName
        {
            get { throw new NotSupportedException(); }
        }

        public string XMLURL
        {
            get { throw new NotSupportedException(); }
        }

        public string UpdateURL
        {
            get { throw new NotSupportedException(); }
        }

        public Version Version
        {
            get { throw new NotSupportedException(); }
        }

        protected void UpdateAll(LiveSplitState state)
        {
            if (Split != null)
            {
                RecreateLabels();

                if (Settings.AutomaticAbbreviations)
                {
                    if (NameLabel.Text != Split.Name || NameLabel.AlternateText == null || !NameLabel.AlternateText.Any())
                        NameLabel.AlternateText = Split.Name.GetAbbreviations().ToList();
                }
                else if (NameLabel.AlternateText != null && NameLabel.AlternateText.Any())
                    NameLabel.AlternateText.Clear();

                NameLabel.Text = Split.Name;

                var splitIndex = state.Run.IndexOf(Split);
                if (splitIndex < state.CurrentSplitIndex)
                {
                    NameLabel.ForeColor = Settings.OverrideTextColor ? Settings.BeforeNamesColor : state.LayoutSettings.TextColor;
                }
                else
                {
                    if (Split == state.CurrentSplit)
                        NameLabel.ForeColor = Settings.OverrideTextColor ? Settings.CurrentNamesColor : state.LayoutSettings.TextColor;
                    else
                        NameLabel.ForeColor = Settings.OverrideTextColor ? Settings.AfterNamesColor : state.LayoutSettings.TextColor;
                }

                foreach (var label in LabelsList)
                {
                    var column = ColumnsList.ElementAt(LabelsList.IndexOf(label));
                    UpdateColumn(state, label, column);
                }
            }
        }

        protected void UpdateColumn(LiveSplitState state, SimpleLabel label, ColumnData data)
        {
            var comparison = data.Comparison == "Current Comparison" ? state.CurrentComparison : data.Comparison;
            if (!state.Run.Comparisons.Contains(comparison))
                comparison = state.CurrentComparison;

            var timingMethod = state.CurrentTimingMethod;
            if (data.TimingMethod == "Real Time")
                timingMethod = TimingMethod.RealTime;
            else if (data.TimingMethod == "Game Time")
                timingMethod = TimingMethod.GameTime;

            var type = data.Type;

            var splitIndex = state.Run.IndexOf(Split);
            if (splitIndex < state.CurrentSplitIndex)
            {
                if (type == ColumnType.SplitTime || type == ColumnType.SegmentTime)
                {
                    label.ForeColor = Settings.OverrideTimesColor ? Settings.BeforeTimesColor : state.LayoutSettings.TextColor;

                    if (type == ColumnType.SplitTime)
                    {
                        label.Text = TimeFormatter.Format(Split.SplitTime[timingMethod]);
                    }
                    else //SegmentTime
                    {
                        var segmentTime = LiveSplitStateHelper.GetPreviousSegmentTime(state, splitIndex, timingMethod);
                        label.Text = TimeFormatter.Format(segmentTime);
                    }
                }

                if (type == ColumnType.DeltaorSplitTime || type == ColumnType.Delta)
                {
                    var deltaTime = Split.SplitTime[timingMethod] - Split.Comparisons[comparison][timingMethod];
                    var color = LiveSplitStateHelper.GetSplitColor(state, deltaTime, splitIndex, true, true, comparison, timingMethod);
                    if (color == null)
                        color = Settings.OverrideTimesColor ? Settings.BeforeTimesColor : state.LayoutSettings.TextColor;
                    label.ForeColor = color.Value;

                    if (type == ColumnType.DeltaorSplitTime)
                    {
                        if (deltaTime != null)
                            label.Text = DeltaTimeFormatter.Format(deltaTime);
                        else
                            label.Text = TimeFormatter.Format(Split.SplitTime[timingMethod]);
                    }

                    else if (type == ColumnType.Delta)
                        label.Text = DeltaTimeFormatter.Format(deltaTime);
                }

                else if (type == ColumnType.SegmentDeltaorSegmentTime || type == ColumnType.SegmentDelta)
                {
                    var segmentDelta = LiveSplitStateHelper.GetPreviousSegmentDelta(state, splitIndex, comparison, timingMethod);
                    var color = LiveSplitStateHelper.GetSplitColor(state, segmentDelta, splitIndex, false, true, comparison, timingMethod);
                    if (color == null)
                        color = Settings.OverrideTimesColor ? Settings.BeforeTimesColor : state.LayoutSettings.TextColor;
                    label.ForeColor = color.Value;

                    if (type == ColumnType.SegmentDeltaorSegmentTime)
                    {
                        if (segmentDelta != null)
                            label.Text = DeltaTimeFormatter.Format(segmentDelta);
                        else
                            label.Text = TimeFormatter.Format(LiveSplitStateHelper.GetPreviousSegmentTime(state, splitIndex, timingMethod));
                    }
                    else if (type == ColumnType.SegmentDelta)
                    {
                        label.Text = DeltaTimeFormatter.Format(segmentDelta);
                    }
                }
            }
            else
            {
                if (type == ColumnType.SplitTime || type == ColumnType.SegmentTime || type == ColumnType.DeltaorSplitTime || type == ColumnType.SegmentDeltaorSegmentTime)
                {
                    if (Split == state.CurrentSplit)
                        label.ForeColor = Settings.OverrideTimesColor ? Settings.CurrentTimesColor : state.LayoutSettings.TextColor;
                    else
                        label.ForeColor = Settings.OverrideTimesColor ? Settings.AfterTimesColor : state.LayoutSettings.TextColor;

                    if (type == ColumnType.SplitTime || type == ColumnType.DeltaorSplitTime)
                    {
                        label.Text = TimeFormatter.Format(Split.Comparisons[comparison][timingMethod]);
                    }
                    else //SegmentTime or SegmentTimeorSegmentDeltaTime
                    {
                        var previousTime = TimeSpan.Zero;
                        for (var index = splitIndex - 1; index >= 0; index--)
                        {
                            var comparisonTime = state.Run[index].Comparisons[comparison][timingMethod];
                            if (comparisonTime != null)
                            {
                                previousTime = comparisonTime.Value;
                                break;
                            }
                        }
                        label.Text = TimeFormatter.Format(Split.Comparisons[comparison][timingMethod] - previousTime);
                    }
                }

                //Live Delta
                var splitDelta = type == ColumnType.DeltaorSplitTime || type == ColumnType.Delta;
                var bestDelta = LiveSplitStateHelper.CheckLiveDelta(state, splitDelta, comparison, timingMethod);
                if (bestDelta != null && Split == state.CurrentSplit &&
                    (type == ColumnType.DeltaorSplitTime || type == ColumnType.Delta || type == ColumnType.SegmentDeltaorSegmentTime || type == ColumnType.SegmentDelta))
                {
                    if (splitDelta) //DeltaorSplitTime or Delta
                    {
                        label.Text = DeltaTimeFormatter.Format(bestDelta);
                    }
                    else //SegmentDeltaorSegmentTime or SegmentDelta
                    {
                        label.Text = DeltaTimeFormatter.Format(LiveSplitStateHelper.GetLiveSegmentDelta(state, splitIndex, comparison, timingMethod));
                    }

                    label.ForeColor = Settings.OverrideDeltasColor ? Settings.DeltasColor : state.LayoutSettings.TextColor;
                }
                else if (type == ColumnType.Delta || type == ColumnType.SegmentDelta)
                {
                    label.Text = "";
                }
            }
        }

        protected float CalculateLabelsWidth()
        {
            if (ColumnsList != null)
            {
                var mixedCount = ColumnsList.Count(x => x.Type == ColumnType.DeltaorSplitTime || x.Type == ColumnType.SegmentDeltaorSegmentTime);
                var deltaCount = ColumnsList.Count(x => x.Type == ColumnType.Delta || x.Type == ColumnType.SegmentDelta);
                var timeCount = ColumnsList.Count(x => x.Type == ColumnType.SplitTime || x.Type == ColumnType.SegmentTime);
                return mixedCount * (Math.Max(MeasureDeltaLabel.ActualWidth, MeasureTimeLabel.ActualWidth) + 5)
                    + deltaCount * (MeasureDeltaLabel.ActualWidth + 5)
                    + timeCount * (MeasureTimeLabel.ActualWidth + 5);
            }
            return 0f;
        }

        protected void RecreateLabels()
        {
            if (ColumnsList != null && LabelsList.Count != ColumnsList.Count())
            {
                LabelsList.Clear();
                foreach (var column in ColumnsList)
                {
                    LabelsList.Add(new SimpleLabel
                    {
                        HorizontalAlignment = StringAlignment.Far
                    });
                }
            }
        }

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            if (Split != null)
            {
                UpdateAll(state);
                NeedUpdateAll = false;

                IsActive = (state.CurrentPhase == TimerPhase.Running
                            || state.CurrentPhase == TimerPhase.Paused) &&
                                                    state.CurrentSplit == Split;

                Cache.Restart();
                Cache["Icon"] = Split.Icon;
                if (Cache.HasChanged)
                {
                    if (Split.Icon == null)
                        FrameCount = 0;
                    else
                        FrameCount = Split.Icon.GetFrameCount(new FrameDimension(Split.Icon.FrameDimensionsList[0]));
                }
                Cache["SplitName"] = NameLabel.Text;
                Cache["IsActive"] = IsActive;
                Cache["NameColor"] = NameLabel.ForeColor.ToArgb();
                Cache["ColumnsCount"] = ColumnsList.Count();
                foreach (var label in LabelsList)
                {
                    Cache["Columns" + LabelsList.IndexOf(label) + "Text"] = label.Text;
                    Cache["Columns" + LabelsList.IndexOf(label) + "Color"] = label.ForeColor.ToArgb();
                }

                if (invalidator != null && (Cache.HasChanged || FrameCount > 1))
                {
                    invalidator.Invalidate(0, 0, width, height);
                }
            }
        }

        public void Dispose()
        {
        }
    }
}
