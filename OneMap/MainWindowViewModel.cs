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
        }

        public ReactiveList<MindMapViewModel> Tabs { get; } = new ReactiveList<MindMapViewModel>();
    }
}
