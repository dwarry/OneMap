using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;

using OneMap.OneNote;

using ReactiveUI;

namespace OneMap.Controls
{
    
    [DebuggerDisplay("SectionTreeItem({Title})")]
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
            var nestedPages = new Stack<PageTreeItem>();

            foreach (var p in section.Page ?? Enumerable.Empty<Page>())
            {
                var pti = new PageTreeItem(p, index++);

                if (nestedPages.Count == 0 || pti.PageDepth == 1)
                {
                    yield return pti;
                    nestedPages.Clear();
                    nestedPages.Push(pti);
                }
                else if( pti.PageDepth > nestedPages.Peek().PageDepth)
                {
                    nestedPages.Peek().AddChild(pti);
                    nestedPages.Push(pti);
                }
                else
                {
                    PageTreeItem previousPti = nestedPages.Pop();

                    while (pti.PageDepth >= previousPti.PageDepth && nestedPages.Count > 0)
                    {
                        previousPti = nestedPages.Pop();
                    }

                    previousPti.AddChild(pti);
                    nestedPages.Push(previousPti);
                }
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