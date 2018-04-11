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
        public string Id { get; }

        bool _isExpanded;


        public bool IsExpanded
        {
            get => _isExpanded;
            set => this.RaiseAndSetIfChanged(ref _isExpanded, value);
        }


        bool _isSelected;


        public bool IsSelected
        {
            get => _isSelected;
            set => this.RaiseAndSetIfChanged(ref _isSelected, value);
        }


        protected TreeItem(string id, int index, IEnumerable<TreeItem> children = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));

            Children = new ReactiveList<TreeItem>();

            Index = index;

            ViewModel = this;

            this.WhenAnyValue(x => x.Index).Select(x => x > 0);

            if (children != null)
            {
                using (Children.SuppressChangeNotifications())
                {
                    foreach (var child in children)
                    {
                        AddChild(child);
                    }
                }
            }

            this.ForegroundColor = Colors.Black;
            this.BackgroundColor = Colors.White;

            this.WhenAnyValue(x => x.BackgroundColor, x => x.IsSelected)
                .Select(args => GetBorderColor(args.Item1, args.Item2))
                .ToProperty(this, x => x.BorderColor, out _borderColor);

            this.WhenAnyValue<TreeItem, TreeItem, int>(x => x.Parent, x => x.Index)
                .Select(args => args.Item1 != null && args.Item2 > 0)
                .ToProperty(this, x=>x.CanMoveUp, out _canMoveUp);

            this.WhenAnyValue<TreeItem, TreeItem, int, int>(x => x.Parent, x => x.Parent.Children.Count, x => x.Index)
                .Select(args => args.Item1 != null && args.Item3 < args.Item2 - 1)
                .ToProperty(this, x => x.CanMoveDown, out _canMoveDown);

            this.WhenAnyValue(x => x.Parent).Select(x => x != null).ToProperty(this, x => x.CanDelete, out _canDelete);

            Observable.Return(false).ToProperty(this, x => x.CanPromote, out _canPromote);

            Observable.Return(false).ToProperty(this, x=> x.CanDemote, out _canDemote);

            Observable.Return(false).ToProperty(this, x => x.CanViewPage, out _canViewPage);
        }

        protected virtual Color GetBorderColor(Color background, bool isSelected)
        {
            if (!isSelected)
            {
                return BackgroundColor;
            }

            var average = (background.R + background.G + background.B) / 3;
            var factor = average > 128 ? -0.5f : 0.5f;
            return background.AdjustBrightness(factor);

        }

        private string _title;

        public string Title
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }


        private int _index;

        public int Index
        {
            get => _index;
            protected set => this.RaiseAndSetIfChanged(ref _index, value);
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
            child.Index = Children.Count - 1;
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


        protected ObservableAsPropertyHelper<bool> _canMoveUp;

        public bool CanMoveUp => _canMoveUp.Value;

        public virtual void MoveUp()
        {
            this.Parent.Children[this.Index - 1].Index = this.Index;
            this.Parent.Children.RemoveAt(this.Index);
            this.Parent.Children.Insert(this.Index - 1, this);
            this.Index -= 1;
        }

        protected ObservableAsPropertyHelper<bool> _canMoveDown;

        public bool CanMoveDown => _canMoveDown.Value;

        public virtual void MoveDown()
        {
            this.Parent.Children[this.Index + 1].Index = this.Index;
            this.Parent.Children.RemoveAt(this.Index);
            this.Parent.Children.Insert(this.Index + 1, this);
            this.Index += 1;
        }


        protected ObservableAsPropertyHelper<bool> _canPromote;

        public bool CanPromote => _canPromote.Value;

        public virtual void Promote()
        {
            var (newParent, newIndex) = FindNewPromotionParent();

            if (newParent == null) return;

            Parent.Children.RemoveAt(Index);

            newParent.Children.Insert(newIndex, this);

            Parent = newParent;

            Index = newIndex;

            for(int i = newIndex; i < Parent.Children.Count; ++i)
            {
                newParent.Children[i].Index = i;
            }
        }

        /// <summary>
        /// By default, a promoted item will become the next sibling of its parent. 
        /// </summary>
        /// <returns></returns>
        protected virtual (TreeItem newParent, int index) FindNewPromotionParent()
        {
            return (Parent.Parent, Parent.Index + 1);

        }

        protected ObservableAsPropertyHelper<bool> _canDemote;

        public bool CanDemote => _canDemote.Value;
        
        public virtual void Demote()
        {
            var newParent = FindNewDemotionParent();

            if (newParent == null)
            {
                return;
            }

            Parent.Children.RemoveAt(Index);

            newParent.AddChild(this);

            Index = newParent.Children.Count - 1;
        }

        protected ObservableAsPropertyHelper<bool> _canCreateChild;

        public bool CanCreateChild => _canCreateChild.Value;

        public void CreateChild(string title, ChildOption option)
        {
            var newChild = option.Factory(title, Children.Count - 1);
            AddChild(newChild);
        }

        public virtual IReadOnlyCollection<ChildOption> ChildOptions => new ChildOption[0];

        protected virtual TreeItem FindNewDemotionParent()
        {
            // by default this will become a child of its preceding sibling
            return Parent.Children[Index - 1];
        }

        protected ObservableAsPropertyHelper<bool> _canDelete;

        public bool CanDelete => _canDelete.Value;

        public virtual void Delete()
        {
            this.Parent.Children.RemoveAt(this.Index);

            for (int i = this.Index; i < Parent.Children.Count; ++i)
            {
                this.Parent.Children[i].Index = i;
            }

            this.Parent = null;
        }


        protected ObservableAsPropertyHelper<bool> _canViewPage;

        public bool CanViewPage => _canViewPage.Value;

        public virtual void ViewPage()
        {
            throw new NotImplementedException();
        }


        private Color _foregroundColor;


        public Color ForegroundColor
        {
            get { return _foregroundColor; }
            set { this.RaiseAndSetIfChanged(ref _foregroundColor, value); }
        }


        private Color _backgroundColor;


        public Color BackgroundColor
        {
            get { return _backgroundColor; }
            set { this.RaiseAndSetIfChanged(ref _backgroundColor, value); }
        }


        private ObservableAsPropertyHelper<Color> _borderColor;

        public Color BorderColor => _borderColor.Value;
    }


    public class ChildOption
    {
        public string IconResourceName { get; }

        public Func<string, int, TreeItem> Factory { get; }

        public ChildOption(string title, string iconResourceName, Func<string, int, TreeItem> factory)
        {
            Title = title;
            IconResourceName = iconResourceName;
            Factory = factory;
        }

        public string Title { get; }
    }
}
