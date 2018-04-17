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

        public NotebookTreeItem(Notebook notebook) : base(notebook.ID, MakeChildren(notebook))
        {
            _notebook = notebook ?? throw new ArgumentNullException(nameof(notebook));

            Title = notebook.name;

            BackgroundColor = (Color) ColorConverter.ConvertFromString(notebook.color ?? "#dddddd");
            ForegroundColor = BackgroundColor.DeriveForegroundColour();
        }

        private static IEnumerable<TreeItem> MakeChildren(Notebook notebook)
        {

            foreach (var sg in notebook.SectionGroup ?? Enumerable.Empty<SectionGroup>())
            {
                yield return new SectionGroupTreeItem(sg);
            }

            foreach (var s in notebook.Section ?? Enumerable.Empty<Section>())
            {
                yield return new SectionTreeItem(s);
            }
        }
    }
}
