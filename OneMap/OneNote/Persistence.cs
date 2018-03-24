using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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


        Page GetCurrentPage();


        void UpdatePage(Page page);
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

        public Notebooks LoadNotebooks()
        {
            _app.GetHierarchy("", HierarchyScope.hsPages, out var xml, XMLSchema.xs2010);

            return Read<Notebooks>(xml);
        }

        public Page GetPage(string pageId)
        {
            return null;
        }

        public Page GetCurrentPage()
        {
            return null;
        }

        public void UpdatePage(Page page)
        {
        }
    }
}
