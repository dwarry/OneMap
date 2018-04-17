using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Media;

using OneMap.OneNote;

using ReactiveUI;

namespace OneMap.Controls
{
    
    [DebuggerDisplay("SectionTreeItem({Title})")]
    public class SectionTreeItem : TreeItem
    {
        private readonly Section _section;

        public SectionTreeItem(Section section): base(section.ID, MakeChildren(section))
        {
            _section = section ?? throw new ArgumentNullException(nameof(section));

            Title = section.name;

     
            BackgroundColor = (section.color ?? "").StartsWith("#")
                ? (Color) ColorConverter.ConvertFromString(section.color ?? "#aaaaaa")
                : Color.FromRgb(34,34,34);

            ForegroundColor = BackgroundColor.DeriveForegroundColour();

        }

        private static IEnumerable<TreeItem> MakeChildren(Section section)
        {
            var nestedPages = new Stack<PageTreeItem>();

            foreach (var p in section.Page ?? Enumerable.Empty<Page>())
            {
                var pti = new PageTreeItem(p);

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

    }
}