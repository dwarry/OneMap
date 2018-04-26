using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneMap.OneNote
{
    public interface IOneNoteObject
    {
        string ID { get; set; }

        string name { get; set; }
    }



    public interface IHasPath: IOneNoteObject
    {
        string path { get; set; }
    }


    public interface IHasSectionGroups: IOneNoteObject
    {
        SectionGroup[] SectionGroup { get; set; }
    }


    public interface IHasSections: IOneNoteObject
    {
        Section[] Section { get; set; }
    }


    [DebuggerDisplay("Notebook({name})")]
    partial class Notebook: IHasPath, IHasSectionGroups, IHasSections { }


    [DebuggerDisplay("SectionGroup({name})")]
    partial class SectionGroup : IHasPath, IHasSectionGroups, IHasSections
    {
        SectionGroup[] IHasSectionGroups.SectionGroup
        {
            get => this.SectionGroup1;
            set => this.SectionGroup1 = value;
        }
    }


    [DebuggerDisplay("Section({name})")]
    partial class Section : IOneNoteObject
    {
    }


    [DebuggerDisplay("Page({name})")]
    partial class Page: IOneNoteObject { }
}
