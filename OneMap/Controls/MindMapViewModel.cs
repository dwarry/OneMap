﻿using System;
using System.Linq;
using System.Reactive.Linq;

using OneMap.OneNote;

using ReactiveUI;

using Splat;

namespace OneMap.Controls
{



    public class MindMapViewModel : ReactiveObject
    {
        protected readonly IPersistence _persistence;


        private TreeItem _leftSelection;


        private TreeItem _rightSelection;


        private TreeItem _selectedItem;

        protected MindMapViewModel(IPersistence persistence = null)
        {
            _persistence = persistence ?? Locator.Current.GetService<IPersistence>();

            LeftTreeItems = AllTreeItems.CreateDerivedCollection(x => x, x => x.Index > (AllTreeItems.Count / 2) - 1);

            RightTreeItems = AllTreeItems.CreateDerivedCollection(x => x, x => x.Index <= (AllTreeItems.Count / 2) - 1);

            var settingSelectedItem = false;

            this.WhenAnyValue(x => x.LeftSelection).Subscribe(x =>
            {
                if (settingSelectedItem) return;

                settingSelectedItem = true;

                RightSelection = null;

                SelectedItem = x;

                settingSelectedItem = false;
            });

            this.WhenAnyValue(x => x.RightSelection).Subscribe(x =>
            {
                if (settingSelectedItem) return;

                settingSelectedItem = true;

                LeftSelection = null;

                SelectedItem = x;

                settingSelectedItem = false;
            });

            ViewModel = this;
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

        public ReactiveCommand MoveUp { get; protected set; }

        public ReactiveCommand MoveDown { get; protected set; }

        public ReactiveCommand Demote { get; protected set; }

        public ReactiveCommand Promote { get; protected set; }
    }


    public class OneNoteHierarchyMindMapViewModel : MindMapViewModel, ISupportsActivation
    {
        public OneNoteHierarchyMindMapViewModel(IPersistence persistence = null) : base(persistence)
        {
            Title = "Hierarchy";

            var items = _persistence.LoadNotebooks();

            var treeItems = items.Notebook.Select((x, i) => new NotebookTreeItem(x, i));

            AllTreeItems.AddRange(treeItems);

            var canMoveUp = this.WhenAnyValue(x => x.SelectedItem)
                .Select(x => x != null && x.CanMoveUp);

            MoveUp = ReactiveCommand.Create(() => SelectedItem.MoveUp(), canMoveUp);

            var canMoveDown = this.WhenAnyValue(x => x.SelectedItem)
                .Select(x => x != null && x.CanMoveDown);

            MoveDown = ReactiveCommand.Create(() => SelectedItem.MoveDown(), canMoveDown);

            var canPromote = this.WhenAnyValue(x => x.SelectedItem)
                .Select(x => x != null && x.CanPromote);

            Promote = ReactiveCommand.Create(() => SelectedItem.Promote(), canPromote);

            var canDemote = this.WhenAnyValue(x => x.SelectedItem)
                .Select(x => x != null && x.CanDemote);

            Demote = ReactiveCommand.Create(() => SelectedItem.Demote(), canDemote);
        }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }


    public class PageContentMindMapViewModel : MindMapViewModel
    {
        public PageContentMindMapViewModel(string pageId, IPersistence persistence = null) : base(persistence)
        {
            // _persistence.LoadPageContents(pageId)
        }
    }
}