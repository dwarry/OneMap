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



        public PageContentMindMapViewModel(string pageId, string title,  IPersistence persistence = null) : base(persistence)
        {
            PageId = pageId ?? _persistence.GetCurrentPageId();

            Title = title;

            IsTabClosable = true;

            var falseWhenNothingSelected =
                this.WhenAnyValue(x => x.SelectedItem).Where(x => x == null).Select(x => false);

        }

        protected override TreeItem PrepareTreeItems()
        {
            var p = _persistence.GetPage(PageId);
            
            Title = p.Title.OE.Items.OfType<TextRange>().FirstOrDefault()?.Value ?? "[Untitled Page]";;

            var titleId = p.Title.OE.objectID;

            var styles = ExtractStyles(p);

            var headings =  GetHeadings(p, styles);

            return new PageContentTreeItem(PageId, titleId, Title, headings);
        }


        private IDictionary<string, QuickStyleDef> ExtractStyles(Page p)
        {
            return p.QuickStyleDef.ToDictionary(x => x.index);
        }

        private IEnumerable<HeadingTreeItem> GetHeadings(Page page, IDictionary<string, QuickStyleDef> styles)
        {
            Stack<HeadingTreeItem> headings = new Stack<HeadingTreeItem>();

            int topLevelIndex = 0;

            foreach (var outline in page.Items.OfType<Outline>())
            {
                foreach (var oe in outline.OEChildren.SelectMany(x => x.Items.OfType<OE>()))
                {
                    var level = GetLevel(oe.quickStyleIndex, styles);

                    if (level == 0) continue;

                    while (headings.Count > 0 && headings.Peek().HeadingLevel >= level)
                    {
                        headings.Pop();
                    }

                    var index = headings.Count == 0 
                        ? topLevelIndex++ 
                        : headings.Peek().Children.Count;

                    var newHeading = new HeadingTreeItem(styles, oe);

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


        private int GetLevel(string quickStyleIndex, IDictionary<string, QuickStyleDef> styles)
        {
            if (quickStyleIndex == null || !styles.ContainsKey(quickStyleIndex))
            {
                return 0;
            }

            var style = styles[quickStyleIndex];

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
            else
            {
                headingId = RootTreeItem.Id;
            }

            _persistence.GotoPageOrItem(PageId, headingId);
        }
    }


    public class PageContentTreeItem : TreeItem
    {
        public string TitleId { get; }

        public PageContentTreeItem(string id, string titleId, string title, IEnumerable<TreeItem> children = null) : base(id, children)
        {
            Title = title ?? "[Untitled page]";
            TitleId = titleId;
        }

        public override bool CanMoveUp => false;

        public override bool CanMoveDown => false;

        public override bool CanPromote => false;

        public override bool CanDemote => false;

        public override bool CanDelete => false;

        public override bool CanViewPage => true;
    }
}