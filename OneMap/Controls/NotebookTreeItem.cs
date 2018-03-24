﻿using System;
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

        public NotebookTreeItem(Notebook notebook, int index) : base(index, MakeChildren(notebook))
        {
            if (index <= 0) throw new ArgumentOutOfRangeException(nameof(index));

            _notebook = notebook ?? throw new ArgumentNullException(nameof(notebook));

            Title = notebook.name;

            Color = (Color) ColorConverter.ConvertFromString(notebook.color ?? "#dddddd");

            MoveUp = MoveDown = Promote = Demote = DoNothing;
        }

        private static IEnumerable<TreeItem> MakeChildren(Notebook notebook)
        {
            int index = 0;
            foreach (var sg in notebook.SectionGroup)
            {
                yield return new SectionGroupTreeItem(sg, index++);
            }

            foreach (var s in notebook.Section)
            {
                yield return new SectionTreeItem(s, index++);
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
