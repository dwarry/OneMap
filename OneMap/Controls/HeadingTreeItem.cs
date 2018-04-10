using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;

using OneMap.OneNote;

using ReactiveUI;

namespace OneMap.Controls
{
    public class HeadingTreeItem : TreeItem
    {
        private readonly IDictionary<string, QuickStyleDef> _styleDefs;
        private readonly OE _element;


        public HeadingTreeItem(int index, IDictionary<string, QuickStyleDef> styleDefs, OE element): base(element.objectID, index)
        {
            _styleDefs = styleDefs;
            _element = element;

            Title = GetTextContents(element);

            var qsd = styleDefs[element.quickStyleIndex];

            if (qsd.name.StartsWith("h"))
            {
                HeadingLevel = int.Parse(qsd.name.Substring(1));
            }


            this.WhenAnyValue(x => x.HeadingLevel)
                .Select(x => x > 1)
                .ToProperty(this, x => x.CanPromote, out _canPromote);

            this.WhenAnyValue(x => x.HeadingLevel).Select(x => x < 6)
                .ToProperty(this, x => x.CanDemote, out _canDemote);

            _canCreateChild = _canDemote;
        }


        private int _headingLevel;


        public int HeadingLevel
        {
            get { return _headingLevel; }
            set { this.RaiseAndSetIfChanged(ref _headingLevel, value); }
        }

        private static readonly Regex _simpleTagStripper = new Regex(@"\>(.+)\</", RegexOptions.Compiled | RegexOptions.Multiline);

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