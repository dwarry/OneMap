using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using OneMap.Controls;
using OneMap.OneNote;

using ReactiveUI;

using Splat;

namespace OneMap
{
    public class MainWindowViewModel: ReactiveObject, IEnableLogger  
    {
        private readonly IPersistence _persistence;

        public MainWindowViewModel(IPersistence persistence = null)
        {
            _persistence = persistence ?? Locator.Current.GetService<IPersistence>();

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
            string pageId;
            switch (SelectedTab.SelectedItem)
            {
                case PageTreeItem pti:
                    pageId = pti.PageId;

                    var pageMap = new PageContentMindMapViewModel(pageId, pti.Title);

                    Tabs.Add(pageMap);

                    SelectedTab = pageMap;

                    break;
                case HeadingTreeItem hti:
                    pageId = (SelectedTab as PageContentMindMapViewModel).PageId;

                    var headingId = hti.Id;

                    _persistence.GotoPageOrItem(pageId, headingId);

                    break;

                case null when SelectedTab is PageContentMindMapViewModel pvm:
                    pageId = pvm.PageId;

                    _persistence.GotoPageOrItem(pageId);

                    break;

                default:
                    this.Log().Warn("Unexpected SelectedItem: " + (SelectedTab.SelectedItem == null ? "null" : SelectedTab.SelectedItem.ToString()));

                    break;
            }




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
