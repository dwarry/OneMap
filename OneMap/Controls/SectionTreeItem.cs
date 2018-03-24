using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

using OneMap.OneNote;

using ReactiveUI;

namespace OneMap.Controls
{
    public class SectionTreeItem : TreeItem
    {
        private readonly Section _section;

        public SectionTreeItem(Section section, int index): base(index, MakeChildren(section))
        {
            _section = section ?? throw new ArgumentNullException(nameof(section));

            Title = section.name;

     
            Color = (section.color ?? "").StartsWith("#")
                ? (Color) ColorConverter.ConvertFromString(section.color ?? "#aaaaaa")
                : Color.FromRgb(34,34,34);
        }

        private static IEnumerable<TreeItem> MakeChildren(Section section)
        {
            int index = 0;

            foreach (var p in section.Page ?? Enumerable.Empty<Page>())
            {
                yield return new PageTreeItem(p, index++);
            }
        }

        private Color _color;


        public Color Color
        {
            get { return _color; }
            set { this.RaiseAndSetIfChanged(ref _color, value); }
        }
    }
}