﻿using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

#pragma warning disable IDE1006

namespace LiveSplit.FFXSplits
{
    public class TitleComponent : IComponent
    {
        public SplitsSettings Settings { get; set; }
        public float VerticalHeight { get; set; }
        public GraphicsCache Cache { get; set; }
        protected int FrameCount { get; set; }
        protected Image OldImage { get; set; }
        protected int FinishedRunsCount { get; set; }

        public float MinimumWidth => GameNameLabel.X + AttemptCountLabel.ActualWidth + 5;

        public float HorizontalWidth
            => Math.Max(GameNameLabel.ActualWidth, CategoryNameLabel.ActualWidth + (Settings.ShowCount ? AttemptCountLabel.ActualWidth : 0))
                + GameNameLabel.X + 5;

        public IDictionary<string, Action> ContextMenuControls => null;

        public float PaddingTop => 0f;
        public float PaddingLeft => 7f;
        public float PaddingBottom => 0f;
        public float PaddingRight => 7f;

        protected SimpleLabel GameNameLabel = new SimpleLabel();
        protected SimpleLabel CategoryNameLabel = new SimpleLabel();
        protected SimpleLabel AttemptCountLabel = new SimpleLabel();

        protected Font TitleFont { get; set; }

        public float MinimumHeight { get; set; }

        public TitleComponent(SplitsSettings settings)
        {
            VerticalHeight = 10;
            Settings = settings;
            Cache = new GraphicsCache();
            GameNameLabel = new SimpleLabel();
            CategoryNameLabel = new SimpleLabel();
            AttemptCountLabel = new SimpleLabel();
        }

        private void DrawGeneral(Graphics g, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            DrawBackground(g, width, height);

            TitleFont = state.LayoutSettings.TextFont;

            MinimumHeight = g.MeasureString("A", TitleFont).Height * 1.7f;
            VerticalHeight = g.MeasureString("A", TitleFont).Height * 1.7f;
            var showGameIcon = state.Run.GameIcon != null && Settings.DisplayGameIcon;
            if (showGameIcon)
            {
                DrawGameIcon(g, state, height);
            }

            DrawAttemptCount(g, state, width, height);
            CalculatePadding(height, mode, showGameIcon, out float startPadding, out float titleEndPadding, out float categoryEndPadding);

            DrawGameName(g, state, width, height, showGameIcon, startPadding, titleEndPadding);
            DrawCategoryName(g, state, width, height, showGameIcon, startPadding, categoryEndPadding);
        }

        private void DrawBackground(Graphics g, float width, float height)
        {
            g.DrawImage(Properties.Resources.row_single_title, 0, 0, width, height);
        }

        private void CalculatePadding(float height, LayoutMode mode, bool showGameIcon, out float startPadding, out float titleEndPadding, out float categoryEndPadding)
        {
            startPadding = 5;
            titleEndPadding = 5;
            categoryEndPadding = 5;

            if (showGameIcon)
            {
                startPadding += height + 3;
            }
            if (mode == LayoutMode.Vertical && Settings.ShowCount)
            {
                if (string.IsNullOrEmpty(CategoryNameLabel.Text))
                {
                    titleEndPadding += AttemptCountLabel.ActualWidth;
                }
                else
                {
                    categoryEndPadding += AttemptCountLabel.ActualWidth;
                }
            }
        }

        private void DrawCategoryName(Graphics g, LiveSplitState state, float width, float height, bool showGameIcon, float startPadding, float categoryEndPadding)
        {
            if (Settings.CenterTitle || !showGameIcon)
            {
                CategoryNameLabel.CalculateAlternateText(g, width - startPadding - categoryEndPadding);
                float stringWidth = CategoryNameLabel.ActualWidth;
                PositionAndWidth positionAndWidth = calculateCenteredPositionAndWidth(width, stringWidth, startPadding, categoryEndPadding);
                CategoryNameLabel.X = positionAndWidth.position;
                CategoryNameLabel.Width = positionAndWidth.width;
            }
            else
            {
                CategoryNameLabel.X = startPadding;
                CategoryNameLabel.Width = width - startPadding - categoryEndPadding;
            }
            CategoryNameLabel.Y = 0;
            CategoryNameLabel.HorizontalAlignment = StringAlignment.Near;
            CategoryNameLabel.VerticalAlignment = string.IsNullOrEmpty(GameNameLabel.Text) ? StringAlignment.Center : StringAlignment.Far;
            CategoryNameLabel.Font = TitleFont;
            CategoryNameLabel.Brush = new SolidBrush(state.LayoutSettings.TextColor);
            CategoryNameLabel.HasShadow = state.LayoutSettings.DropShadows;
            CategoryNameLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
            CategoryNameLabel.OutlineColor = state.LayoutSettings.TextOutlineColor;
            CategoryNameLabel.Height = height;
            CategoryNameLabel.Draw(g);
        }

        private void DrawAttemptCount(Graphics g, LiveSplitState state, float width, float height)
        {
            if (Settings.ShowCount)
            {
                AttemptCountLabel.HorizontalAlignment = StringAlignment.Far;
                AttemptCountLabel.VerticalAlignment = StringAlignment.Far;
                AttemptCountLabel.X = 0;
                AttemptCountLabel.Y = height - 40;
                AttemptCountLabel.Width = width - 5;
                AttemptCountLabel.Height = 40;
                AttemptCountLabel.Font = TitleFont;
                AttemptCountLabel.Brush = new SolidBrush(state.LayoutSettings.TextColor);
                AttemptCountLabel.HasShadow = state.LayoutSettings.DropShadows;
                AttemptCountLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
                AttemptCountLabel.OutlineColor = state.LayoutSettings.TextOutlineColor;
                AttemptCountLabel.Draw(g);
            }
        }

        private void DrawGameName(Graphics g, LiveSplitState state, float width, float height, bool showGameIcon, float startPadding, float titleEndPadding)
        {
            if (Settings.CenterTitle)
            {
                GameNameLabel.CalculateAlternateText(g, width - startPadding - titleEndPadding);
                float stringWidth = GameNameLabel.ActualWidth;
                PositionAndWidth positionAndWidth = calculateCenteredPositionAndWidth(width, stringWidth, startPadding, titleEndPadding);
                GameNameLabel.X = positionAndWidth.position;
                GameNameLabel.Width = positionAndWidth.width;
            }
            else
            {
                GameNameLabel.X = startPadding;
                GameNameLabel.Width = width - startPadding - titleEndPadding;
            }

            GameNameLabel.HorizontalAlignment = StringAlignment.Near;
            GameNameLabel.VerticalAlignment = string.IsNullOrEmpty(CategoryNameLabel.Text) ? StringAlignment.Center : StringAlignment.Near;
            GameNameLabel.Y = 0;
            GameNameLabel.Height = height;
            GameNameLabel.Font = TitleFont;
            GameNameLabel.Brush = new SolidBrush(state.LayoutSettings.TextColor);
            GameNameLabel.HasShadow = state.LayoutSettings.DropShadows;
            GameNameLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
            GameNameLabel.OutlineColor = state.LayoutSettings.TextOutlineColor;
            GameNameLabel.Draw(g);
        }

        private void DrawGameIcon(Graphics g, LiveSplitState state, float height)
        {
            var icon = state.Run.GameIcon;

            if (OldImage != icon)
            {
                ImageAnimator.Animate(icon, (s, o) => { });
                OldImage = icon;
            }

            var aspectRatio = (float)icon.Width / icon.Height;
            var drawWidth = height - 4;
            var drawHeight = height - 4;
            if (icon.Width > icon.Height)
            {
                var ratio = icon.Height / (float)icon.Width;
                drawHeight *= ratio;
            }
            else
            {
                var ratio = icon.Width / (float)icon.Height;
                drawWidth *= ratio;
            }

            ImageAnimator.UpdateFrames(icon);

            g.DrawImage(
                icon,
                7 + ((height - 4 - drawWidth) / 2),
                2 + ((height - 4 - drawHeight) / 2),
                drawWidth,
                drawHeight);
        }

        /*
         * Returns coordinate and width of the string element so that the text is centered in the total width
         * while not overlapping into the start or ending padding.
         */
        private PositionAndWidth calculateCenteredPositionAndWidth(float totalWidth, float stringWidth, float startPadding, float endPadding)
        {
            float position, width;
            if (startPadding + stringWidth + endPadding > totalWidth)
            {
                // We cant fit no matter what we do, so start the string after the start padding
                position = startPadding;
            }
            else
            {
                // Try to center, but push the string left or right if it overlaps the padding
                position = (totalWidth - stringWidth) / 2;
                position = Math.Max(position, startPadding);
                if (position + stringWidth > totalWidth - endPadding)
                {
                    position = totalWidth - endPadding - stringWidth;
                }
            }
            width = totalWidth - endPadding - position;
            return new PositionAndWidth(position, width);
        }

        private class PositionAndWidth
        {
            public float position { get; set; }
            public float width { get; set; }
            public PositionAndWidth(float position, float width)
            {
                this.position = position;
                this.width = width;
            }
        }

        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
        {
            DrawGeneral(g, state, HorizontalWidth, height, LayoutMode.Horizontal);
        }

        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
        {
            DrawGeneral(g, state, width, VerticalHeight, LayoutMode.Vertical);
        }

        public string ComponentName => "Title";

        public Control GetSettingsControl(LayoutMode mode)
        {
            throw new NotSupportedException();
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            throw new NotSupportedException();
        }

        public void SetSettings(XmlNode settings)
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

        private IEnumerable<string> getCategoryNameAbbreviations(string categoryName)
        {
            var indexStart = categoryName.IndexOf('(');
            var indexEnd = categoryName.IndexOf(')', indexStart + 1);
            var afterParentheses = "";
            if (indexStart >= 0 && indexEnd >= 0)
            {
                var inside = categoryName.Substring(indexStart + 1, indexEnd - indexStart - 1);
                afterParentheses = categoryName.Substring(indexEnd + 1).Trim();
                categoryName = categoryName.Substring(0, indexStart).Trim();
                var splits = inside.Split(',');

                for (var i = splits.Length - 1; i > 0; --i)
                {
                    yield return $"{ categoryName } ({ string.Join(",", splits.Take(i)) }) { afterParentheses }".Trim();
                }
            }
            yield return $"{ categoryName } { afterParentheses }".Trim();
        }

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            var extendedCategoryName = state.Run.GetExtendedCategoryName(Settings.ShowRegion, Settings.ShowPlatform, Settings.ShowVariables);

            Cache.Restart();
            Cache["GameName"] = state.Run.GameName;
            Cache["CategoryName"] = extendedCategoryName;
            Cache["LayoutMode"] = mode;
            Cache["ShowGameName"] = Settings.ShowGameName;
            Cache["ShowCategoryName"] = Settings.ShowCategoryName;

            if (Cache.HasChanged)
            {
                if (Settings.ShowGameName && Settings.ShowCategoryName)
                {
                    var text = string.Format("{0} - {1}", state.Run.GameName, extendedCategoryName);
                    var gameAbbreviations = state.Run.GameName.GetAbbreviations();
                    var shortestGameName = gameAbbreviations.Last();
                    var categoryAbbreviations = getCategoryNameAbbreviations(extendedCategoryName);
                    var combinedAbbreviations1 = gameAbbreviations.Select(x => string.Format("{0} - {1}", x, extendedCategoryName));
                    var combinedAbbreviations2 = categoryAbbreviations.Select(x => string.Format("{0} - {1}", shortestGameName, x));
                    var abbreviations = combinedAbbreviations1.Concat(combinedAbbreviations2).ToList();
                    GameNameLabel.Text = text;
                    GameNameLabel.AlternateText = mode == LayoutMode.Vertical ? abbreviations : new List<string>();
                    CategoryNameLabel.Text = "";
                }
                else
                {
                    if (Settings.ShowGameName)
                    {
                        GameNameLabel.Text = state.Run.GameName;
                        GameNameLabel.AlternateText = mode == LayoutMode.Vertical ? state.Run.GameName.GetAbbreviations().ToList() : new List<string>();
                    }
                    else
                    {
                        GameNameLabel.Text = "";
                        GameNameLabel.AlternateText = new List<string>();
                    }
                    if (Settings.ShowCategoryName)
                    {
                        CategoryNameLabel.Text = extendedCategoryName;
                        CategoryNameLabel.AlternateText = mode == LayoutMode.Vertical ? getCategoryNameAbbreviations(extendedCategoryName).ToList() : new List<string>();
                    }
                    else
                    {
                        CategoryNameLabel.Text = "";
                        CategoryNameLabel.AlternateText = new List<string>();
                    }
                }
            }

            Cache.Restart();
            Cache["AttemptHistoryCount"] = state.Run.AttemptHistory.Count;
            Cache["Run"] = state.Run;
            if (Cache.HasChanged)
                FinishedRunsCount = state.Run.AttemptHistory.Where(x => x.Time.RealTime != null).Count();

            if (Settings.ShowAttemptCount && Settings.ShowFinishedRunsCount)
                AttemptCountLabel.Text = string.Format("{0}/{1}", FinishedRunsCount, state.Run.AttemptCount);
            else if (Settings.ShowAttemptCount)
                AttemptCountLabel.Text = state.Run.AttemptCount.ToString();
            else if (Settings.ShowFinishedRunsCount)
                AttemptCountLabel.Text = FinishedRunsCount.ToString();

            Cache.Restart();
            Cache["GameIcon"] = state.Run.GameIcon;
            if (Cache.HasChanged)
            {
                if (state.Run.GameIcon == null)
                    FrameCount = 0;
                else
                    FrameCount = state.Run.GameIcon.GetFrameCount(new FrameDimension(state.Run.GameIcon.FrameDimensionsList[0]));
            }
            Cache["GameNameLabel"] = GameNameLabel.Text;
            Cache["CategoryNameLabel"] = CategoryNameLabel.Text;
            Cache["AttemptCountLabel"] = AttemptCountLabel.Text;

            if (invalidator != null && (Cache.HasChanged || FrameCount > 1))
            {
                invalidator.Invalidate(0, 0, width, height);
            }
        }

        public void Dispose()
        {
        }

        public int GetSettingsHashCode() => Settings.GetSettingsHashCode();
    }
}