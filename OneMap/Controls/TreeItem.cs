using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

using ReactiveUI;

namespace OneMap.Controls
{
    public abstract class TreeItem : ReactiveObject
    {

        bool _isExpanded;


        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { this.RaiseAndSetIfChanged(ref _isExpanded, value); }
        }


        bool _isSelected;


        public bool IsSelected
        {
            get { return _isSelected; }
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

            CanMoveUp = this.WhenAnyValue<TreeItem, TreeItem, int>(x => x.Parent, x => x.Index)
                .Select(args => args.Item1 != null && args.Item2 > 0);

            CanMoveDown = this.WhenAnyValue<TreeItem, TreeItem, int>(x => x.Parent, x => x.Index)
                .Select(args => args.Item1 != null && args.Item2 < args.Item1.Children.Count - 1);

            CanPromote = Observable.Defer(() => Observable.Return(false));

            CanDemote = Observable.Defer(() => Observable.Return(false));
        }

        private string _title;


        public string Title
        {
            get { return _title; }
            set { this.RaiseAndSetIfChanged(ref _title, value); }
        }


        private int _index;


        public int Index
        {
            get { return _index; }
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


        public IObservable<bool> CanMoveUp { get; protected set; }

        public virtual void MoveUp() { }

        public IObservable<bool> CanMoveDown { get; protected set; }

        public virtual void MoveDown()
        {
        }

        public IObservable<bool> CanPromote { get; protected set; }

        public virtual void Promote()
        {
        }

        public IObservable<bool> CanDemote { get; protected set; }

        public virtual void Demote()
        {
        }
    }
}
