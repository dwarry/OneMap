using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneMap.Controls
{
    public class OneNoteTreeItem : TreeItem
    {
        public OneNoteTreeItem(IEnumerable<NotebookTreeItem> notebooks) : base(0, notebooks)
        {
            
        }
    }
}
