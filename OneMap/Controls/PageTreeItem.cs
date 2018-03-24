using System;

using OneMap.OneNote;

namespace OneMap.Controls
{
    public class PageTreeItem : TreeItem
    {
        private readonly Page _page;

        public PageTreeItem(Page page, int index): base(index, null)
        {
            _page = page ?? throw new ArgumentNullException(nameof(page));

            Title = page.name;
        }
    }
}