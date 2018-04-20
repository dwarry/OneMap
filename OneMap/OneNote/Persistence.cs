using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

using Microsoft.Office.Interop.OneNote;

namespace OneMap.OneNote
{
    public interface IPersistence
    {
        /// <summary>
        /// Get the currently available notebooks
        /// </summary>
        /// <returns></returns>
        Notebooks LoadNotebooks();

        Page GetPage(string pageId);


        string GetCurrentPageId();


        void UpdatePage(Page page);

        void GotoPageOrItem(string pageId, string headingId = null);


        string CreateSectionGroup(Notebook parent, string title);

        string CreateSectionGroup(SectionGroup parent, string title);

        string CreateSection(Notebook parent, string title);

        string CreateSection(SectionGroup parent, string title);

        string CreatePage(Section section, string title);
    }

    public class OneNotePersistence : IPersistence
    {
        private readonly Application _app;

        public OneNotePersistence(Application app = null)
        {
            _app = app ?? new Application();
        }

        private T Read<T>(string xml)
        {
            using (var reader = XmlReader.Create(new StringReader(xml)))
            {
                var serializer = new XmlSerializer(typeof(T));

                return (T) serializer.Deserialize(reader);
            }
        }

        private string Serialize<T>(T hierarchyItem)
        {
            using (var sw = new StringWriter())
            using (var writer = new XmlTextWriter(sw))
            {
                var serializer = new XmlSerializer(typeof(T));

                serializer.Serialize(writer, hierarchyItem);

                return sw.ToString();
            }
        }

        public Notebooks LoadNotebooks()
        {
            _app.GetHierarchy("", HierarchyScope.hsPages, out var xml, XMLSchema.xs2010);

            return Read<Notebooks>(xml);
        }

        public Page GetPage(string pageId)
        {
            _app.GetPageContent(pageId, out var xml, xsSchema: XMLSchema.xs2010);

            return Read<Page>(xml);
        }

        public string GetCurrentPageId()
        {
            foreach (Window w in _app.Windows)
            {
                if (!w.SideNote && w.CurrentPageId != null)
                {
                    return w.CurrentPageId;
                }
            }

            return _app.Windows.CurrentWindow.CurrentPageId;
        }

        public void UpdatePage(Page page)
        {
        }

        public void GotoPageOrItem(string pageId, string headingId = null)
        {
            Window window = null;
            foreach (Window w in _app.Windows)
            {
                if (!w.SideNote)
                {
                    window = w;

                    break;
                }
            }



            if (window != null)
            {
                window.NavigateTo(pageId, headingId);
                SetForegroundWindow((IntPtr)window.WindowHandle);
            }
            else
            {
                _app.NavigateTo(pageId, headingId, true);
            }
        }

        private string Create<TParent, TItem>(TParent parent, string title,
            Action<TItem, string> setTitle,
            Func<TItem, string> getTitle,
            Func<TItem, string> getItemId, 
            Func<TParent, string> getParentId,
            Func<TParent, TItem[]> getCollection,
            Action<TParent, TItem[]> setCollection) where TItem : IOneNoteObject, new()
        {
            var newParent = CreateNewItem(parent, title,
                setTitle, getTitle, getItemId, getParentId, getCollection, setCollection);

            var newXml = Serialize(newParent);

            _app.UpdateHierarchy(newXml, XMLSchema.xs2010);

            TParent ReadParent(string id)
            {
                _app.GetHierarchy(id, HierarchyScope.hsChildren, out var xml, XMLSchema.xs2010);

                return Read<TParent>(xml);
            }

            // Re-read the hierarchy to get the id
            newParent = ReadParent(getParentId(parent));

            var newItem = getCollection(newParent).First(x => getTitle(x) == title);

            return getItemId(newItem);
        }

        public TParent CreateNewItem<TParent, TItem>(TParent parent, string title,
            Action<TItem, string> setTitle,
            Func<TItem, string> getTitle,
            Func<TItem, string> getItemId,
            Func<TParent, string> getParentId,
            Func<TParent, TItem[]> getCollection,
            Action<TParent, TItem[]> setCollection) where TItem : IOneNoteObject, new()
        {
            string xml;

            TParent ReadParent(string id)
            {
                _app.GetHierarchy(id, HierarchyScope.hsChildren, out xml, XMLSchema.xs2010);

                return Read<TParent>(xml);
            }

            string path = (parent as IHasPath)?.path ?? "";


            var parentId = getParentId(parent);

            var newItem = new TItem();

            switch (newItem)
            {
                case SectionGroup sg:
                    sg.ID = null;
//                    sg.isUnread = true;
//                    sg.isUnreadSpecified = true;
                    //sg.lastModifiedTime = DateTime.Now;
                    //sg.path = Path.Combine(path, title);


                    break;
                default:

                    break;
            }



            setTitle(newItem, title);

            var newParent = ReadParent(parentId);

            var newCollection = Add(getCollection(newParent), newItem);

            setCollection(newParent, newCollection);

            return newParent;
        }



        public string CreateSectionGroup(Notebook parent, string title)
        {
            return Create(parent, 
                title, 
                (item, t) => item.name = t, 
                item => item.name,
                x => x.ID,
                x => x.ID, 
                x => x.SectionGroup,
                (p, newCollection) => p.SectionGroup = newCollection);
        }

        public string CreateSectionGroup(SectionGroup parent, string title)
        {
            return Create(parent,
                title,
                (item, t) => item.name = t,
                item => item.name,
                x => x.ID,
                x => x.ID,
                x => x.SectionGroup1,
                (p, newCollection) => p.SectionGroup1 = newCollection);

        }

        public string CreateSection(Notebook parent, string title)
        {
            return Create(parent,
                title,
                (item, t) => item.name = t,
                item => item.name,
                x => x.ID,
                x => x.ID,
                x => x.Section,
                (p, newCollection) => p.Section = newCollection);
        }

        public string CreateSection(SectionGroup parent, string title)
        {
            return Create(parent,
                title,
                (item, t) => item.name = t,
                item => item.name,
                x => x.ID,
                x => x.ID,
                x => x.Section,
                (p, newCollection) => p.Section = newCollection);

        }


        public string CreatePage(Section parent, string title)
        {
            return Create(parent,
                title,
                (item, t) => item.name = t,
                item => item.name,
                x => x.ID,
                x => x.ID,
                x => x.Page,
                (p, newCollection) => p.Page = newCollection);
        }


        private T[] Add<T>(T[] array, T item)
        {
            if (array == null)
            {
                return new T[1]{item};
            }

            Array.Resize(ref array, array.Length + 1);

            array[array.Length - 1] = item;

            return array;
        }

        private T[] Insert<T>(T[] array, T item, int index)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));

            if(index < 0 || index > array.Length) throw new ArgumentOutOfRangeException(nameof(index), "must be in the range 0..(array.length)");

            Array.Resize(ref array, array.Length + 1);

            for (int i = array.Length - 1; i > index; i--)
            {
                array[i] = array[i - 1];
            }

            array[index] = item;

            return array;
        }


        
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
    }

    
}
