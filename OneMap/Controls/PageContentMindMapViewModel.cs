using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text.RegularExpressions;

using OneMap.OneNote;

using ReactiveUI;

namespace OneMap.Controls
{
    public class PageContentMindMapViewModel : MindMapViewModel
    {
        private static readonly Regex _simpleTagStripper = new Regex(@"^\<.+\>(.+)\</");
        private readonly string _pageId;
        private IDictionary<string, QuickStyleDef> _styles;

        public PageContentMindMapViewModel(string pageId, IPersistence persistence = null) : base(persistence)
        {
            _pageId = pageId ?? persistence.GetCurrentPageId();

            IsTabClosable = true;
        }

        public override void Refresh()
        {
            var p = _persistence.GetPage(_pageId);

            Title = GetTextContents(p.Title.OE);

            _styles = ExtractStyles(p);

            using (AllTreeItems.SuppressChangeNotifications())
            {
                AllTreeItems.Clear();

                AllTreeItems.AddRange(GetHeadings(p));
            }

        }

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

        private IDictionary<string, QuickStyleDef> ExtractStyles(Page p)
        {
            return p.QuickStyleDef.ToDictionary(x => x.index);
        }

        private IEnumerable<HeadingTreeItem> GetHeadings(Page page)
        {
            Stack<HeadingTreeItem> headings = new Stack<HeadingTreeItem>();
            
            foreach (var outline in page.Items.OfType<Outline>())
            {
                foreach (var oe in outline.OEChildren.SelectMany(x => x.Items.OfType<OE>()))
                {
                    var level = GetLevel(oe.quickStyleIndex);

                    if (level == 0) continue;

                    while (headings.Count > 0 && headings.Peek().HeadingLevel >= level)
                    {
                        headings.Pop();
                    }

                    var index = headings.Count == 0 ? AllTreeItems.Count : headings.Peek().Children.Count;

                    var newHeading = new HeadingTreeItem(index, _styles, oe);

                    if (headings.Count != 0)
                    {
                        headings.Peek().AddChild(newHeading);
                        headings.Push(newHeading);
                    }
                    else
                    {
                        yield return newHeading;
                    }

                }
            }
        }


        private int GetLevel(string quickStyleIndex)
        {
            if (quickStyleIndex == null || !_styles.ContainsKey(quickStyleIndex))
            {
                return 0;
            }

            var style = _styles[quickStyleIndex];

            return style.name.StartsWith("h") ? int.Parse(style.name.Substring(1)) : 0;
        }

        public override void MoveUp()
        {
        }

        public override void MoveDown()
        {
        }

        public override void Demote()
        {
        }

        public override void Promote()
        {
        }

            
    }

    public class HeadingTreeItem : TreeItem
    {
        private readonly IDictionary<string, QuickStyleDef> _styleDefs;
        private readonly OE _element;


        public HeadingTreeItem(int index, IDictionary<string, QuickStyleDef> styleDefs, OE element): base(index)
        {
            _styleDefs = styleDefs;
            _element = element;

            Title = element.OEChildren.OfType<TextRange>().FirstOrDefault()?.Value ?? "[Unknown title]";

            this.WhenAnyValue(x => x.HeadingLevel)
                .Select(x => x > 1)
                .ToProperty(this, x => x.CanMoveUp, out _canMoveUp);

            this.WhenAnyValue(x => x.HeadingLevel).Select(x => x < 6)
                .ToProperty(this, x => x.CanMoveDown, out _canMoveDown);
        }


        private int _headingLevel;


        public int HeadingLevel
        {
            get { return _headingLevel; }
            set { this.RaiseAndSetIfChanged(ref _headingLevel, value); }
        }
    }
}