using System;
using System.Diagnostics;
using System.Reactive.Linq;

using OneMap.OneNote;

using ReactiveUI;

namespace OneMap.Controls
{
    [DebuggerDisplay("PageTreeItem({Title})")]
    public class PageTreeItem : TreeItem
    {
        private const int MaxPageDepth = 3;

        private readonly Page _page;

        public PageTreeItem(Page page): base(page.ID, null)
        {
            _page = page ?? throw new ArgumentNullException(nameof(page));

            Title = page.name;

            PageDepth = page.isSubPageSpecified && page.isSubPage
                ? int.Parse(page.pageLevel)
                : 1;

        }

        public override bool CanPromote => PageDepth > 1;

        public override void Promote()
        {
            base.Promote();
            PageDepth--;
        }

        public override void Demote()
        {
            base.Demote();
            PageDepth++;
        }

        public override bool CanDemote => PageDepth < 3 && Index > 0 && Parent.Children[Index - 1] is PageTreeItem;

        public override bool CanViewPage => true;

        private int _pageDepth;
        
        public int PageDepth
        {
            get => _pageDepth;
            private set => this.RaiseAndSetIfChanged(ref _pageDepth, value);
        }


        public string PageId => _page.ID;
    }
}