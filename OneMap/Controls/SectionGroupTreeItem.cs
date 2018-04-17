using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;

using OneMap.OneNote;

namespace OneMap.Controls
{
    [DebuggerDisplay("SectionGroupTreeItem({Title})")]
    public class SectionGroupTreeItem : TreeItem
    {
        private readonly SectionGroup _sectionGroup;

        public SectionGroupTreeItem(SectionGroup sectionGroup): base(sectionGroup.ID, MakeChildren(sectionGroup))
        {
            _sectionGroup = sectionGroup ?? throw new ArgumentNullException(nameof(sectionGroup));

            Title = sectionGroup.name;

            BackgroundColor = Colors.DarkSlateGray;
            ForegroundColor = BackgroundColor.DeriveForegroundColour();

        }

        private static IEnumerable<TreeItem> MakeChildren(SectionGroup group)
        {
            foreach (var sg in group.SectionGroup1 ?? Enumerable.Empty<SectionGroup>())
            {
                yield return new SectionGroupTreeItem(sg);
            }

            foreach (var s in group.Section ?? Enumerable.Empty<Section>())
            {
                yield return new SectionTreeItem(s);
            }
        }

    }
}