using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

using OneMap.OneNote;

using ReactiveUI;

namespace OneMap.Controls
{
    public class NotebookTreeItem : TreeItem
    {
        private readonly Notebook _notebook;

        public NotebookTreeItem(Notebook notebook) 
        {
            _notebook = notebook ?? throw new ArgumentNullException(nameof(notebook));

            Title = notebook.name;

            Color = (Color) ColorConverter.ConvertFromString(notebook.color ?? "#dddddd");

            MoveUp = MoveDown = Promote = Demote = DoNothing;
        }

        private Color _color;


        public Color Color
        {
            get { return _color; }
            set { this.RaiseAndSetIfChanged(ref _color, value); }
        }
    }
}
