using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

using OneMap.OneNote;

using ReactiveUI;

namespace OneMap.Controls
{
    [DebuggerDisplay("NotebookTreeItem({Title})")]
    public class NotebookTreeItem : TreeItem
    {
        private readonly Notebook _notebook;

        public NotebookTreeItem(Notebook notebook, int index) : base(notebook.ID, index, MakeChildren(notebook))
        {
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));

            _notebook = notebook ?? throw new ArgumentNullException(nameof(notebook));

            Title = notebook.name;

            BackgroundColor = (Color) ColorConverter.ConvertFromString(notebook.color ?? "#dddddd");
            ForegroundColor = BackgroundColor.DeriveForegroundColour();
        }

        private static IEnumerable<TreeItem> MakeChildren(Notebook notebook)
        {
            int index = 0;

            foreach (var sg in notebook.SectionGroup ?? Enumerable.Empty<SectionGroup>())
            {
                yield return new SectionGroupTreeItem(sg, index++);
            }

            foreach (var s in notebook.Section ?? Enumerable.Empty<Section>())
            {
                yield return new SectionTreeItem(s, index++);
            }
        }


    }
}
