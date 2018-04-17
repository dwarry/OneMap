using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using OneMap.OneNote;

using ReactiveUI;

using Splat;

namespace OneMap.Controls
{

    public abstract class MindMapViewModel : ReactiveObject, IEnableLogger
    {
        protected readonly IPersistence _persistence;


        private TreeItem _leftSelection;


        private TreeItem _rightSelection;


        private TreeItem _selectedItem;


        protected IDictionary<string, TreeItem> _allItemsById = new Dictionary<string, TreeItem>();

        private IList<string> _previouslyExpandedItems = new List<string>();

        protected MindMapViewModel(IPersistence persistence = null)
        {
            this.Log().Debug("Creating {0}", this.GetType().Name);

            _persistence = persistence ?? Locator.Current.GetService<IPersistence>();

            LeftTreeItems = AllTreeItems.CreateDerivedCollection(x => x, x => x.Index > (AllTreeItems.Count / 2) - 1);

            RightTreeItems = AllTreeItems.CreateDerivedCollection(x => x, x => x.Index <= (AllTreeItems.Count / 2) - 1);

            var settingSelectedItem = false;

            this.WhenAnyValue(x => x.LeftSelection).Subscribe(x =>
            {
                if (settingSelectedItem) return;

                settingSelectedItem = true;

                if (RightSelection != null)
                {
                    RightSelection.IsSelected = false;
                }

                SelectedItem = x;

                settingSelectedItem = false;
            });

            this.WhenAnyValue(x => x.RightSelection).Subscribe(x =>
            {
                if (settingSelectedItem) return;

                settingSelectedItem = true;

                if (LeftSelection != null)
                {
                    LeftSelection.IsSelected = false;
                }
    
                SelectedItem = x;

                settingSelectedItem = false;
            });

            this.WhenAnyValue(x => x.RootTreeItem).Subscribe(x => RefreshCore() );

            var falseWhenNothingSelected =
                this.WhenAnyValue(x => x.SelectedItem).Where(x => x == null).Select(x => false);

            var whenSelectedItemChanged = this.WhenAnyValue(x => x.SelectedItem).Where(x => x != null);


            whenSelectedItemChanged.Select(x => x.CanMoveUp).Merge(falseWhenNothingSelected)
                .Log(this, "canMoveUp ")
                .ToProperty(this, x => x.CanMoveUp, out _canMoveUp);

            whenSelectedItemChanged.Select(x => x.CanMoveDown).Merge(falseWhenNothingSelected)
                .Log(this, "canMoveDown ")
                .ToProperty(this, x => x.CanMoveDown, out _canMoveDown);

            whenSelectedItemChanged.Select(x => x.CanPromote).Merge(falseWhenNothingSelected)
                .Log(this, "canPromote ")
                .ToProperty(this, x => x.CanPromote, out _canPromote);

            whenSelectedItemChanged.Select(x => x.CanDemote).Merge(falseWhenNothingSelected)
                .Log(this, "canDemote ")
                .ToProperty(this, x => x.CanDemote, out _canDemote);

            whenSelectedItemChanged.Select(x => x.CanViewPage).Merge(falseWhenNothingSelected)
                .Log(this, "canViewPage")
                .ToProperty(this, x => x.CanViewPage, out _canViewPage);


        }

        public MindMapViewModel ViewModel => this;

        private TreeItem _rootTreeItem;


        public TreeItem RootTreeItem
        {
            get { return _rootTreeItem; }
            set { this.RaiseAndSetIfChanged(ref _rootTreeItem, value); }
        }


        private string _title;


        public string Title
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }


        public TreeItem SelectedItem
        {
            get => _selectedItem;
            set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
        }


        public TreeItem LeftSelection
        {
            get => _leftSelection;
            set => this.RaiseAndSetIfChanged(ref _leftSelection, value);
        }


        public TreeItem RightSelection
        {
            get => _rightSelection;
            set => this.RaiseAndSetIfChanged(ref _rightSelection, value);
        }

        
        public ReactiveList<TreeItem> AllTreeItems { get; } = new ReactiveList<TreeItem>();


        public IReactiveDerivedList<TreeItem> LeftTreeItems { get; }


        public IReactiveDerivedList<TreeItem> RightTreeItems { get; }

        public void Refresh()
        {
            var expandedItems = _allItemsById.Values.Where(x => x.IsExpanded).Select(x => x.Id).ToList();

            RootTreeItem = PrepareTreeItems();

            RefreshCore();

            Title = RootTreeItem.Title;

            foreach (var expandedItemId in expandedItems)
            {
                if (_allItemsById.TryGetValue(expandedItemId, out var item))
                {
                    item.IsExpanded = true;
                }
            }
        }



        protected virtual void RefreshCore()
        {
            void ProcessTreeItem(TreeItem item)
            {
                this.Log().Debug("Processing {0}({1}):{2}", item.GetType().Name, item.Title, item.Id);
                _allItemsById.Add(item.Id, item);

                foreach (var child in item.Children)
                {
                    ProcessTreeItem(child);
                }
            }

            using (AllTreeItems.SuppressChangeNotifications())
            {
                AllTreeItems.Clear();

                _allItemsById.Clear();

                if (RootTreeItem == null)
                {
                    return;

                }

                ProcessTreeItem(RootTreeItem);

                AllTreeItems.AddRange(RootTreeItem.Children);
            }
        }

        protected abstract TreeItem PrepareTreeItems();

        protected ObservableAsPropertyHelper<bool> _canMoveUp;

        public bool CanMoveUp => _canMoveUp.Value;

        public abstract void MoveUp();

        protected ObservableAsPropertyHelper<bool> _canMoveDown;

        public bool CanMoveDown => _canMoveDown.Value;

        public abstract void MoveDown();

        protected ObservableAsPropertyHelper<bool> _canDemote;

        public bool CanDemote => _canDemote.Value;

        public abstract void Demote();

        protected ObservableAsPropertyHelper<bool> _canPromote;

        public bool CanPromote => _canPromote.Value;

        public abstract void Promote();


        protected ObservableAsPropertyHelper<bool> _canViewPage;

        public bool CanViewPage => _canViewPage.Value;

        public abstract void ViewPage();


        private bool _isTabClosable;


        public bool IsTabClosable
        {
            get { return _isTabClosable; }
            set { this.RaiseAndSetIfChanged(ref _isTabClosable, value); }
        }
    }
}