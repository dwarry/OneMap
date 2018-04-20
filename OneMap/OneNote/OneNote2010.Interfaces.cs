using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneMap.OneNote
{

    public interface IHasName
    {
        string name { get; set; }

    }


    public interface IHasPath
    {
        string path { get; set; }
    }

    public interface IOneNoteObject { }

    partial class Notebook: IOneNoteObject, IHasPath { }

    partial class SectionGroup : IHasName, IHasPath, IOneNoteObject { }


    partial class Section : IHasName, IOneNoteObject { }


    partial class Page: IHasName, IOneNoteObject { }
}
