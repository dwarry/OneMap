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
        

        string CreateSectionGroup<TParent>(TParent parent, string title) where TParent: IHasSectionGroups;


        string CreateSection<TParent>(TParent parent, string title) where TParent: IHasSections;

        string CreateSectionAfter<TParent>(TParent parent, string title, string siblingSectionId) where TParent: IHasSections;

        string CreateSectionBefore<TParent>(TParent parent, string title, string siblingSectionId) where TParent: IHasSections;


        string CreatePage(Section section, string title);

        string CreatePageAfter(Section section, string title, string siblingPageId);

        string CreatePageBefore(Section section, string title, string siblingPageId);

        string CreateChildPage(Section section, string title, string parentPageId);
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

        /// <summary>
        /// Creates a specified hierarchy item and add it to the specified child collection of its parent. 
        /// </summary>
        /// <typeparam name="TParent">The type of the parent item. </typeparam>
        /// <typeparam name="TItem">The type of the new item.</typeparam>
        /// <param name="parent">The parent item. </param>
        /// <param name="title">Title of the new item</param>
        /// <param name="getCollection">Accessor for the parent collection</param>
        /// <param name="setCollection">Settor for the parent collection</param>
        /// <param name="initializeItem">Optional function used to set up the new item</param>
        /// <param name="siblingId">Optional id of the sibling used to position the new item. If null, the new item will just be appended to the collection.</param>
        /// <param name="offset">+1 to add the new item after the sibling, -1 to add it before the sibling.</param>
        /// <returns>The Id of the new Item.</returns>
        private string Create<TParent, TItem>(TParent parent, string title,
            Func<TParent, TItem[]> getCollection,
            Action<TParent, TItem[]> setCollection,
            Action<TItem> initializeItem = null,
            string siblingId = null, 
            int? offset = null)
                where TParent: IOneNoteObject
                where TItem : IOneNoteObject, new()
        {
            if (siblingId == null && offset.HasValue)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "can be specified only in conjuction with siblingId");
            }

            if (offset.HasValue && (offset.Value < 0 || offset.Value > 1))
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "must be 0 or 1");
            }

            var newParent = CreateNewItem(parent, title,
                  getCollection, setCollection, initializeItem, siblingId, offset);

            var newXml = Serialize(newParent);

            _app.UpdateHierarchy(newXml, XMLSchema.xs2010);

            TParent ReadParent(string id)
            {
                _app.GetHierarchy(id, HierarchyScope.hsChildren, out var xml, XMLSchema.xs2010);

                return Read<TParent>(xml);
            }

            // Re-read the hierarchy to get the id
            newParent = ReadParent(parent.ID);

            var newItem = getCollection(newParent).First(x => x.name == title);

            return newItem.ID;
        }

        private void Swap<T>(T[] array, int i, int j)
        {
            T temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }

        private TParent CreateNewItem<TParent, TItem>(
            TParent parent, 
            string title,
            Func<TParent, TItem[]> getCollection,
            Action<TParent, TItem[]> setCollection,
            Action<TItem> initializeItem = null,
            string siblingId = null, 
            int? offset = null) 
                where TParent: IOneNoteObject
                where TItem: IOneNoteObject, new()
        {
            string xml;

            TParent ReadParent(string id)
            {
                _app.GetHierarchy(id, HierarchyScope.hsChildren, out xml, XMLSchema.xs2010);

                return Read<TParent>(xml);
            }

            var path = (parent as IHasPath)?.path ?? "";

            var parentId = parent.ID;

            var newItem = new TItem();

            newItem.name = title;

            initializeItem?.Invoke(newItem);

            var newParent = ReadParent(parentId);

            var children = (getCollection(newParent) ?? new TItem[0]).ToList();

            if (siblingId == null)
            {
                children.Add(newItem);
            }
            else
            {
                int siblingIndex = children.FindIndex(x => x.ID == siblingId);

                if (siblingIndex > -1)
                {
                    var newIndex = siblingIndex + offset.Value;

                    children.Insert(newIndex, newItem);
                }
                else
                {
                    children.Add(newItem);
                }
            }


            setCollection(newParent, children.ToArray());

            return newParent;
        }



        public string CreateSectionGroup<TParent>(TParent parent, string title) where TParent: IHasSectionGroups
        {
            return Create(parent, 
                title, 
                x => x.SectionGroup,
                (p, newCollection) => p.SectionGroup = newCollection);
        }


        public string CreateSection<TParent>(TParent parent, string title) where TParent: IHasSections
        {
            return Create(parent,
                title,
                x => x.Section,
                (p, newCollection) => p.Section = newCollection);
        }

        public string CreateSectionAfter<TParent>(TParent parent, string title, string siblingSectionId) where TParent : IHasSections
        {
            return Create(parent,
                title,
                x => x.Section,
                (p, newCollection) => p.Section = newCollection,
                siblingId: siblingSectionId, 
                offset: 1);
        }

        public string CreateSectionBefore<TParent>(TParent parent, string title, string siblingSectionId) where TParent : IHasSections
        {
            return Create(parent,
                title,
                x => x.Section,
                (p, newCollection) => p.Section = newCollection,
                siblingId: siblingSectionId, 
                offset: 0);
        }


        private Page CreatePageAndSetTitle(Section parent, string title)
        {
            _app.CreateNewPage(parent.ID, out var pageId, NewPageStyle.npsBlankPageWithTitle);

            _app.GetPageContent(pageId, out var xml, PageInfo.piBasic, XMLSchema.xs2010);

            var page = Read<Page>(xml);

            page.name = title;

            page.Title.OE.Items.OfType<TextRange>().First().Value = title;

            xml = Serialize(page);

            _app.UpdatePageContent(xml, xsSchema:XMLSchema.xs2010);

            return page;
        }

        /// <summary>
        /// Creates a new page at the end of the parent section. 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public string CreatePage(Section parent, string title)
        {
            var page = CreatePageAndSetTitle(parent, title);

            return page.ID;
        }

        private int FindPageIndexes(Section section, string pageId, string siblingPageId)
        {

            for (int i = 0; i < section.Page.Length; ++i)
            {
                var p = section.Page[i];

                if (p.ID == siblingPageId)
                {
                    return i;
                }
            }

            throw new InvalidOperationException("Sibling page not found");
        }

        public string CreatePageAfter(Section parent, string title, string siblingPageId)
        {
            var page = CreatePageAndSetTitle(parent, title);

            _app.GetHierarchy(parent.ID, HierarchyScope.hsChildren, out var xml, XMLSchema.xs2010);

            var section = Read<Section>(xml);

            var siblingIndex = FindPageIndexes(section, page.ID, siblingPageId);

            var pages = section.Page.ToList();

            var sibling = pages[siblingIndex];

            // the new page is always the last one of the section.
            // Ignoring any race conditions with another user adding a page at that exact moment
            var newPage = pages.Last();

            newPage.pageLevel = sibling.pageLevel;
            newPage.isSubPageSpecified = true;
            newPage.isSubPage = newPage.pageLevel != "1";


            int siblingPageLevel = Int32.Parse(sibling.pageLevel);

            // skip any sub-pages
            for (int i = siblingIndex + 1; i < pages.Count; ++i)
            {
                var p = pages[i];
                var currentPageLevel = Int32.Parse(p.pageLevel);

                if (currentPageLevel <= siblingPageLevel)
                {

                    pages.RemoveAt(pages.Count - 1);

                    pages.Insert(i, newPage);

                    section.Page = pages.ToArray();

                    _app.UpdateHierarchy(Serialize(section), XMLSchema.xs2010);

                    break;
                }
            }

            // if we completed the loop, the new page should be at the end, where it already is
            // so we don't have to do anything

            return page.ID;
        }

        public string CreatePageBefore(Section parent, string title, string siblingPageId)
        {
            var page = CreatePageAndSetTitle(parent, title);

            _app.GetHierarchy(parent.ID, HierarchyScope.hsChildren, out var xml, XMLSchema.xs2010);

            var section = Read<Section>(xml);

            var siblingIndex = FindPageIndexes(section, page.ID, siblingPageId);

            var pages = section.Page.ToList();

            var sibling = pages[siblingIndex];

            // the new page is always the last one of the section.
            // Ignoring any race conditions with another user adding a page at that exact moment
            var newPage = pages.Last();

            newPage.pageLevel = sibling.pageLevel;

            newPage.isSubPageSpecified = true;

            newPage.isSubPage = newPage.pageLevel != "1";

            pages.RemoveAt(pages.Count - 1);

            pages.Insert(siblingIndex, newPage);

            section.Page = pages.ToArray();

            _app.UpdateHierarchy(Serialize(section), XMLSchema.xs2010);

            return newPage.ID;
        }

        public string CreateChildPage(Section parent, string title, string parentPageId)
        {
            var page = CreatePageAndSetTitle(parent, title);

            _app.GetHierarchy(parent.ID, HierarchyScope.hsChildren, out var xml, XMLSchema.xs2010);

            var section = Read<Section>(xml);

            var pages = section.Page.ToList();

            var newPage = pages.Last();

            var parentIndex = pages.FindIndex(x => x.ID == parentPageId);

            var parentPage = pages[parentIndex];

            var newPageLevel = "";

            switch (parentPage.pageLevel)
            {
                case "1":
                    newPage.pageLevel = "2";

                    break;

                case "2":
                    newPage.pageLevel = "3";

                    break;

                default:
                    throw new InvalidOperationException($"Cannot create subpage for parent page with pageLevel = {parentPage.pageLevel}");
            }

            newPage.isSubPageSpecified = true;

            newPage.isSubPage = true;


            // find last child page of the parent and insert after it
            // then update the hierarchy. 

            for (int i = parentIndex + 1; i < pages.Count; ++i)
            {
                var p = pages[i];

                if (p.pageLevel == parentPage.pageLevel)
                {
                    pages.Insert(i, newPage);

                    pages.RemoveAt(pages.Count - 1);

                    break;
                }
            }

            section.Page = pages.ToArray();

            xml = Serialize(section);

            _app.UpdateHierarchy(xml, XMLSchema.xs2010);

            return newPage.ID;
        }


        private int FindPageIndex(List<Page> pages, string pageId)
        {
            for (int i = pages.Count - 1; i >= 0; --i)
            {
                if (pages[i].ID == pageId)
                {
                    return i;
                }
            }

            throw new InvalidOperationException($"Page {pageId} was not found in the section");
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
