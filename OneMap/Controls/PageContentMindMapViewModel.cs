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

        public string PageId { get; }

        private IDictionary<string, QuickStyleDef> _styles;

        public PageContentMindMapViewModel(string pageId, string title,  IPersistence persistence = null) : base(persistence)
        {
            PageId = pageId ?? _persistence.GetCurrentPageId();

            Title = title;

            IsTabClosable = true;

            var falseWhenNothingSelected =
                this.WhenAnyValue(x => x.SelectedItem).Where(x => x == null).Select(x => false);

//
//            this.WhenAnyValue(x => x.SelectedItem as HeadingTreeItem)
//                .Select(x => x.HeadingLevel > 1)
//                .Merge(falseWhenNothingSelected)
//                .ToProperty(this, x => x.Can)

            this.WhenAnyValue(x => x.SelectedItem)
                .Select(x => (x as HeadingTreeItem) != null)
                .ToProperty(this, x => x.CanViewPage, out _canViewPage);
        }

        protected override IEnumerable<TreeItem> PrepareTreeItems()
        {
            var p = _persistence.GetPage(PageId);

            Title = p.Title.OE.Items.OfType<TextRange>().FirstOrDefault()?.Value ?? "[No text]";;

            _styles = ExtractStyles(p);

            return GetHeadings(p);
        }


        private IDictionary<string, QuickStyleDef> ExtractStyles(Page p)
        {
            return p.QuickStyleDef.ToDictionary(x => x.index);
        }

        private IEnumerable<HeadingTreeItem> GetHeadings(Page page)
        {
            Stack<HeadingTreeItem> headings = new Stack<HeadingTreeItem>();

            int topLevelIndex = 0;

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

                    var index = headings.Count == 0 
                        ? topLevelIndex++ 
                        : headings.Peek().Children.Count;

                    var newHeading = new HeadingTreeItem(index, _styles, oe);

                    if (headings.Count != 0)
                    {
                        headings.Peek().AddChild(newHeading);
                        headings.Push(newHeading);
                    }
                    else
                    {
                        headings.Push(newHeading);
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

        public override void ViewPage()
        {
            string headingId = null;

            if (SelectedItem is HeadingTreeItem h)
            {
                headingId = h.Id;
            }

            _persistence.GotoPageOrItem(PageId, headingId);
        }
    }
}