using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using DynamicData.Binding;
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


        protected TreeItem(string id, IEnumerable<TreeItem> children = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));


            ViewModel = this;

            if (children != null)
            {
                using (Children.SuspendNotifications())
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


        public int Index
        {
            get => Parent != null ? Parent.Children.IndexOf(this) : -1;
        }


        private TreeItem _parent;


        public TreeItem Parent
        {
            get => _parent;
            set => this.RaiseAndSetIfChanged(ref _parent, value);
        }


        public ObservableCollectionExtended<TreeItem> Children { get; } = new ObservableCollectionExtended<TreeItem>();

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




        public virtual bool CanMoveUp => Index > 0;

        public virtual void MoveUp()
        {
            int index = this.Index;

            this.Parent.Children.RemoveAt(index);
            this.Parent.Children.Insert(index - 1, this);
        }

        public virtual bool CanMoveDown => Parent != null && Index < Parent.Children.Count - 1;

        public virtual void MoveDown()
        {
            var index = this.Index;
            this.Parent.Children.RemoveAt(index);
            this.Parent.Children.Insert(index + 1, this);
        }


        public virtual bool CanPromote => false;

        public virtual void Promote()
        {
            var (newParent, newIndex) = FindNewPromotionParent();

            if (newParent == null) { return; }

            var index = this.Index;

            Parent.Children.RemoveAt(index);

            newParent.Children.Insert(newIndex, this);

            Parent = newParent;
        }

        /// <summary>
        /// By default, a promoted item will become the next sibling of its parent. 
        /// </summary>
        /// <returns></returns>
        protected virtual (TreeItem newParent, int index) FindNewPromotionParent()
        {
            return (Parent.Parent, Parent.Index + 1);

        }

        public virtual bool CanDemote => false;

        public virtual void Demote()
        {
            var newParent = FindNewDemotionParent();

            if (newParent == null)
            {
                return;
            }

            Parent.Children.RemoveAt(Index);

            newParent.AddChild(this);
        }

        public virtual bool CanCreateChild => false;

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


        public virtual bool CanDelete => Parent != null;

        public virtual void Delete()
        {
            this.Parent.Children.RemoveAt(this.Index);

            this.Parent = null;
        }


        public virtual bool CanViewPage => false;


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
