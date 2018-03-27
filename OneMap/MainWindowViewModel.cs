using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OneMap.Controls;

using ReactiveUI;

namespace OneMap
{
    public class MainWindowViewModel: ReactiveObject
    {
        public MainWindowViewModel()
        {
            var hierarchyVm = new OneNoteHierarchyMindMapViewModel();

            Tabs.Add(hierarchyVm);

            SelectedTab = hierarchyVm;


        }

        public ReactiveList<MindMapViewModel> Tabs { get; } = new ReactiveList<MindMapViewModel>();

        private MindMapViewModel _selectedTab;

        public MindMapViewModel SelectedTab
        {
            get { return _selectedTab; }
            set { this.RaiseAndSetIfChanged(ref _selectedTab, value); }
        }


        public ReactiveCommand MoveUp { get; }

        public ReactiveCommand MoveDown { get; }

        public ReactiveCommand Promote { get; }

        public ReactiveCommand Demote { get; }
    }
}
