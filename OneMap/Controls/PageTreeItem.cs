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

        public PageTreeItem(Page page, int index): base(page.ID, index, null)
        {
            _page = page ?? throw new ArgumentNullException(nameof(page));

            Title = page.name;

            PageDepth = page.isSubPageSpecified && page.isSubPage
                ? int.Parse(page.pageLevel)
                : 1;

            Observable.Return(true).ToProperty(this, x => x.CanViewPage, out _canViewPage);

        
        }

        private int _pageDepth;
        
        public int PageDepth
        {
            get => _pageDepth;
            set => this.RaiseAndSetIfChanged(ref _pageDepth, value);
        }


        public string PageId => _page.ID;

        public override void ViewPage()
        {
        }
    }
}