using System;
using System.Diagnostics;

using OneMap.OneNote;

using ReactiveUI;

namespace OneMap.Controls
{
    [DebuggerDisplay("PageTreeItem({Title})")]
    public class PageTreeItem : TreeItem
    {
        private const int MaxPageDepth = 3;

        private readonly Page _page;

        public PageTreeItem(Page page, int index): base(index, null)
        {
            _page = page ?? throw new ArgumentNullException(nameof(page));

            Title = page.name;

            PageDepth = page.isSubPageSpecified && page.isSubPage
                ? int.Parse(page.pageLevel)
                : 1;
        }

        private int _pageDepth;

        public int PageDepth
        {
            get => _pageDepth;
            set => this.RaiseAndSetIfChanged(ref _pageDepth, value);
        }
    }
}