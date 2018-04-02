using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using OneMap.OneNote;

using ReactiveUI;

using Splat;

namespace OneMap.Controls
{



    public abstract class MindMapViewModel : ReactiveObject, ISupportsActivation, IEnableLogger
    {
        protected readonly IPersistence _persistence;


        private TreeItem _leftSelection;


        private TreeItem _rightSelection;


        private TreeItem _selectedItem;

        protected MindMapViewModel(IPersistence persistence = null)
        {
            this.Log().Debug("Creating {0}", this.GetType().Name);

            ViewModel = this;

            _persistence = persistence ?? Locator.Current.GetService<IPersistence>();

            LeftTreeItems = AllTreeItems.CreateDerivedCollection(x => x, x => x.Index > (AllTreeItems.Count / 2) - 1);

            RightTreeItems = AllTreeItems.CreateDerivedCollection(x => x, x => x.Index <= (AllTreeItems.Count / 2) - 1);

            var settingSelectedItem = false;

            this.WhenActivated(d =>
            {
                this.WhenAnyValue(x => x.LeftSelection).Subscribe(x =>
                {
                    if (settingSelectedItem) return;

                    settingSelectedItem = true;

                    RightSelection = null;

                    SelectedItem = x;

                    settingSelectedItem = false;
                }).DisposeWith(d);

                this.WhenAnyValue(x => x.RightSelection).Subscribe(x =>
                {
                    if (settingSelectedItem) return;

                    settingSelectedItem = true;

                    LeftSelection = null;

                    SelectedItem = x;

                    settingSelectedItem = false;
                }).DisposeWith(d);

            });

           
            var falseWhenNothingSelected =
                this.WhenAnyValue(x => x.SelectedItem).Where(x => x == null).Select(x => false);

            this.WhenAnyValue(x => x.SelectedItem.CanMoveUp).Merge(falseWhenNothingSelected)
                .Log(this, "canMoveUp ")
                .ToProperty(this, x => x.CanMoveUp, out _canMoveUp);

            this.WhenAnyValue(x => x.SelectedItem.CanMoveDown).Merge(falseWhenNothingSelected)
                .Log(this, "canMoveDown ")
                .ToProperty(this, x => x.CanMoveDown, out _canMoveDown);

            this.WhenAnyValue(x => x.SelectedItem.CanPromote).Merge(falseWhenNothingSelected)
                .Log(this, "canPromote ")
                .ToProperty(this, x => x.CanPromote, out _canPromote);

            this.WhenAnyValue(x => x.SelectedItem.CanDemote).Merge(falseWhenNothingSelected)
                .Log(this, "canDemote ")
                .ToProperty(this, x => x.CanDemote, out _canDemote);



        }

        public MindMapViewModel ViewModel { get; }

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

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }


    public class OneNoteHierarchyMindMapViewModel : MindMapViewModel
    {
        public OneNoteHierarchyMindMapViewModel(IPersistence persistence = null) : base(persistence)
        {
            Title = "Hierarchy";

            var items = _persistence.LoadNotebooks();

            var treeItems = items.Notebook.Select((x, i) => new NotebookTreeItem(x, i));

            AllTreeItems.AddRange(treeItems);

        }


        public override void MoveUp()
        {
            this.Log().Debug("MoveUp {0}({1})", SelectedItem.GetType().Name, SelectedItem.Title);
        }

        public override void MoveDown()
        {
            this.Log().Debug("MoveDown {0}({1})", SelectedItem.GetType().Name, SelectedItem.Title);
        }

        public override void Demote()
        {
            this.Log().Debug("Demote {0}({1})", SelectedItem.GetType().Name, SelectedItem.Title);
        }

        public override void Promote()
        {
            this.Log().Debug("Promote {0}({1})", SelectedItem.GetType().Name, SelectedItem.Title);
        }
    }


    public class PageContentMindMapViewModel : MindMapViewModel
    {
        public PageContentMindMapViewModel(string pageId, IPersistence persistence = null) : base(persistence)
        {
            // _persistence.LoadPageContents(pageId)
        }

        public override void MoveUp()
        {
        }

        public override void MoveDown()
        {
        }

        public override void Demote()
        {
        }

        public override void Promote()
        {
        }
    }
}