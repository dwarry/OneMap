using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;

using OneMap.OneNote;

using ReactiveUI;

using Splat;

namespace OneMap.Controls
{
    public class OneNoteHierarchyMindMapViewModel : MindMapViewModel
    {
        public OneNoteHierarchyMindMapViewModel(IPersistence persistence = null) : base(persistence)
        {
            
            Title = "Hierarchy";

            IsTabClosable = false;
        }

        protected override TreeItem PrepareTreeItems()
        {
            var items = _persistence.LoadNotebooks();

            var notebooks = items.Notebook.Select(x => new NotebookTreeItem(x));

            return new OneNoteTreeItem(notebooks);
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

        public override void ViewPage()
        {

        }
    }
}