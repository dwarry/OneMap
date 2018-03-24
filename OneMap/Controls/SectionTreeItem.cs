using System;
using System.Windows.Media;

using OneMap.OneNote;

using ReactiveUI;

namespace OneMap.Controls
{
    public class SectionTreeItem : TreeItem
    {
        private readonly Section _section;

        public SectionTreeItem(Section section)
        {
            _section = section ?? throw new ArgumentNullException(nameof(section));

            Title = section.name;

            Color = (Color) ColorConverter.ConvertFromString(section.color ?? "#aaaaaa");
        }

        private Color _color;


        public Color Color
        {
            get { return _color; }
            set { this.RaiseAndSetIfChanged(ref _color, value); }
        }
    }
}