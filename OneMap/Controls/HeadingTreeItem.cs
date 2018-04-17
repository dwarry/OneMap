using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media;

using OneMap.OneNote;

using ReactiveUI;

namespace OneMap.Controls
{
    public class HeadingTreeItem : TreeItem
    {
        private readonly IDictionary<string, QuickStyleDef> _styleDefs;
        private readonly OE _element;


        public HeadingTreeItem(IDictionary<string, QuickStyleDef> styleDefs, OE element): base(element.objectID)
        {
            _styleDefs = styleDefs;
            _element = element;

            Title = GetTextContents(element);

            var qsd = styleDefs[element.quickStyleIndex];

            if (qsd.name.StartsWith("h"))
            {
                HeadingLevel = int.Parse(qsd.name.Substring(1));
            }

            BackgroundColor = Colors.Cyan;
        }

        public override bool CanPromote => HeadingLevel > 1;

        public override bool CanDemote => HeadingLevel < 6;

        public override bool CanViewPage => true;

        public override bool CanCreateChild => HeadingLevel < 6;

        private int _headingLevel;


        public int HeadingLevel
        {
            get { return _headingLevel; }
            set { this.RaiseAndSetIfChanged(ref _headingLevel, value); }
        }

        private static readonly Regex _simpleTagStripper = new Regex(@"\>(.+)\</", RegexOptions.Compiled);

        private static string GetTextContents(OE oe)
        {
            var t = oe.Items.OfType<TextRange>().FirstOrDefault()?.Value ?? "[No text]";

            if (t.StartsWith("<"))
            {
                var m = _simpleTagStripper.Match(t);

                if (m.Success)
                {
                    t = m.Groups[1].Value;
                }
            }

            return t;
        }

    }
}