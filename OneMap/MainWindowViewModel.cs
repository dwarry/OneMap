﻿using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using OneMap.Controls;

using ReactiveUI;

using Splat;

namespace OneMap
{
    public class MainWindowViewModel: ReactiveObject, IEnableLogger  
    {
        public MainWindowViewModel()
        {
            this.Log().Debug("Creating VM");
            
            var hierarchyVm = new OneNoteHierarchyMindMapViewModel();

            Tabs.Add(hierarchyVm);

            var falseWhenNothingSelected =
                this.WhenAnyValue(x => x.SelectedTab).Where(x => x == null).Select(x => false);

            var canMoveUp = this.WhenAnyValue(x => x.SelectedTab.CanMoveUp).Merge(falseWhenNothingSelected).Log(this, "MoveUp", b => b.ToString());

            MoveUp = ReactiveCommand.Create(() =>
                {
                    this.Log().Debug("MoveUp");
                    SelectedTab.MoveUp();
                }
                , canMoveUp);

            var canMoveDown = this.WhenAnyValue(x => x.SelectedTab.CanMoveDown).Merge(falseWhenNothingSelected);

            MoveDown = ReactiveCommand.Create(() => SelectedTab.MoveDown(), canMoveDown);

            var canDemote = this.WhenAnyValue(x => x.SelectedTab.CanDemote).Merge(falseWhenNothingSelected);

            Demote = ReactiveCommand.Create(() => SelectedTab.Demote(), canDemote);

            var canPromote = this.WhenAnyValue(x => x.SelectedTab.CanPromote).Merge(falseWhenNothingSelected);

            Promote = ReactiveCommand.Create(() => SelectedTab.Promote(), canPromote);

            var canViewPage = this.WhenAnyValue(x => x.SelectedTab.CanViewPage).Merge(falseWhenNothingSelected);

            ViewPage = ReactiveCommand.Create(OpenPageMindMap, canViewPage);
        }

        private void OpenPageMindMap()
        {
            var pti = (PageTreeItem) SelectedTab.SelectedItem.ViewModel;

            var pageId = pti.PageId;

            var pageMap = new PageContentMindMapViewModel(pageId, pti.Title);

            Tabs.Add(pageMap);

            SelectedTab = pageMap;

        }

        public void SelectFirstTab()
        {
            SelectedTab = Tabs[0];
        }

        public ReactiveList<MindMapViewModel> Tabs { get; } = new ReactiveList<MindMapViewModel>();

        private MindMapViewModel _selectedTab;

        public MindMapViewModel SelectedTab
        {
            get => _selectedTab;
            set {
                this.Log().Debug("SelectedTab changed to: " + (value?.Title ?? "null" ));
                this.RaiseAndSetIfChanged(ref _selectedTab, value);
            }
        }

        
        public ReactiveCommand MoveUp { get; }

        public ReactiveCommand MoveDown { get; }

        public ReactiveCommand Promote { get; }

        public ReactiveCommand Demote { get; }

        public ReactiveCommand ViewPage { get; }
    }
}
