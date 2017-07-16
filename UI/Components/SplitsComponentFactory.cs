using LiveSplit.Model;
using System;

namespace LiveSplit.UI.Components
{
    public class SplitsComponentFactory : IComponentFactory
    {
        public string ComponentName => "Splits (FFX Theme)";
        public string Description => "Displays a Final Fantasy X themed list of split times and deltas in relation to a comparison.";
        public ComponentCategory Category => ComponentCategory.List;
        public IComponent Create(LiveSplitState state) => new SplitsComponent(state);

        public string UpdateName => ComponentName;
        public string XMLURL => "";
        public string UpdateURL => "";
        public Version Version => Version.Parse("1.7.3");
    }
}
