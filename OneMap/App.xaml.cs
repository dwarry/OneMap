using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using OneMap.Controls;
using OneMap.OneNote;

using ReactiveUI;

using Splat;

namespace OneMap
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            InitializeLocator();
        }

        private void InitializeLocator()
        {

            Locator.CurrentMutable.Register(() => new NotebookView(), typeof(IViewFor<NotebookTreeItem>));
            Locator.CurrentMutable.Register(() => new SectionGroupView(), typeof(IViewFor<SectionGroupTreeItem>));
            Locator.CurrentMutable.Register(() => new SectionView(), typeof(IViewFor<SectionTreeItem>));
            Locator.CurrentMutable.Register(() => new PageView(), typeof(IViewFor<PageTreeItem>));

            Locator.CurrentMutable.Register(() => new HierarchyMindMap(), typeof(IViewFor<OneNoteHierarchyMindMapViewModel>));
            Locator.CurrentMutable.Register(() => new OneNoteHierarchyMindMapViewModel(), typeof(OneNoteHierarchyMindMapViewModel));
            Locator.CurrentMutable.Register(() => new HierarchyMindMap(), typeof(IViewFor<OneNoteHierarchyMindMapViewModel>));

            Locator.CurrentMutable.Register(() => new OneNotePersistence(), typeof(IPersistence));
        }
    }
}
