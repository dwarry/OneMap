using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

using ReactiveUI;

namespace OneMap.Controls
{
    public abstract class TreeItem : ReactiveObject
    {

        bool _isExpanded;


        public bool IsExpanded
        {
            get => _isExpanded;
            set { this.RaiseAndSetIfChanged(ref _isExpanded, value); }
        }


        bool _isSelected;


        public bool IsSelected
        {
            get => _isSelected;
            set { this.RaiseAndSetIfChanged(ref _isSelected, value); }
        }


        protected TreeItem(int index, IEnumerable<TreeItem> children = null)
        {
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));

            Children = new ReactiveList<TreeItem>();

            Index = index;

            ViewModel = this;

            this.WhenAnyValue(x => x.Index).Select(x => x > 0);

            if (children == null) return;

            using (Children.SuppressChangeNotifications())
            {
                foreach (var child in children)
                {
                    AddChild(child);
                }
            }

            this.WhenAnyValue<TreeItem, TreeItem, int>(x => x.Parent, x => x.Index)
                .Select(args => args.Item1 != null && args.Item2 > 0)
                .ToProperty(this, x=>x.CanMoveUp, out _canMoveUp);

            this.WhenAnyValue<TreeItem, TreeItem, int>(x => x.Parent, x => x.Index)
                .Select(args => args.Item1 != null && args.Item2 < args.Item1.Children.Count - 1)
                .ToProperty(this, x => x.CanMoveDown, out _canMoveDown);

            Observable.Return(false).ToProperty(this, x => x.CanPromote, out _canPromote);

            Observable.Return(false).ToProperty(this, x=> x.CanDemote, out _canDemote);
        }

        private string _title;

        public string Title
        {
            get => _title;
            set { this.RaiseAndSetIfChanged(ref _title, value); }
        }


        private int _index;

        public int Index
        {
            get => _index;
            set { this.RaiseAndSetIfChanged(ref _index, value); }
        }


        private TreeItem _parent;


        public TreeItem Parent
        {
            get => _parent;
            set => this.RaiseAndSetIfChanged(ref _parent, value);
        }


        public ReactiveList<TreeItem> Children { get; }

        public object ViewModel { get; }

        public void AddChild(TreeItem child)
        {
            child.Parent = this;
            Children.Add(child);
        }

        public void ExpandPath()
        {
            IsExpanded = true;
            _parent?.ExpandPath();
        }

        public void CollapsePath()
        {
            IsExpanded = false;
            _parent?.CollapsePath();
        }


        private ObservableAsPropertyHelper<bool> _canMoveUp;

        public bool CanMoveUp => _canMoveUp.Value;

        public virtual void MoveUp() { }

        private ObservableAsPropertyHelper<bool> _canMoveDown;

        public bool CanMoveDown => _canMoveDown.Value;

        public virtual void MoveDown()
        {
        }


        private ObservableAsPropertyHelper<bool> _canPromote;

        public bool CanPromote => _canPromote.Value;

        public virtual void Promote()
        {
        }

        private ObservableAsPropertyHelper<bool> _canDemote;

        public bool CanDemote => _canDemote.Value;

        public virtual void Demote()
        {
        }


        protected static Color DeriveForegroundColour(Color c)
        {
            const double threshold = 0.35;

            // from https://stackoverflow.com/questions/3116260/given-a-background-color-how-to-get-a-foreground-color-that-makes-it-readable-o
            var r = Math.Pow(c.R / 255.0, 2.2);
            var g = Math.Pow(c.G / 255.0, 2.2);
            var b = Math.Pow(c.B / 255.0, 2.2);

            var brightness = (0.2126 * r) + (0.7151 * g) + (0.0721 * b);

            return brightness > threshold ? Colors.Black : Colors.WhiteSmoke;
        }

    }


    public class RootTreeItem : TreeItem
    {
        public RootTreeItem(IEnumerable<TreeItem> children = null): base(0, children)
        {
            Title = "OneNote";
        }
    }
}
