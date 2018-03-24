using System;

using OneMap.OneNote;

namespace OneMap.Controls
{
    public class SectionGroupTreeItem : TreeItem{
        private readonly SectionGroup _sectionGroup;

        public SectionGroupTreeItem(SectionGroup sectionGroup)
        {
            _sectionGroup = sectionGroup ?? throw new ArgumentNullException(nameof(sectionGroup));

            Title = sectionGroup.name;

        }

    }
}