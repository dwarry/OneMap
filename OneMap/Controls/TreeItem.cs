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

        protected void SetupDefaultMoveUpAndDownGuards()
        {

        }


        protected static IObservable<bool> AlwaysFalse = Observable.Repeat(false);

//        protected static readonly ReactiveCommand DoNothing = 
//                ReactiveCommand.Create(() => { }, AlwaysFalse);

        protected IObservable<bool> _canMoveUp = AlwaysFalse;

        public ReactiveCommand MoveUp { get; protected set; }

        protected IObservable<bool> _canMoveDown = AlwaysFalse;
        public ReactiveCommand MoveDown { get; protected set; }

        protected IObservable<bool> _canPromote = AlwaysFalse;

        public ReactiveCommand Promote { get; protected set; }

        protected IObservable<bool> _canDemote = AlwaysFalse;

        public ReactiveCommand Demote { get; protected set; }

    }
}
