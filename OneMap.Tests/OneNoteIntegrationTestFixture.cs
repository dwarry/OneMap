using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

using Microsoft.Office.Interop.OneNote;

using OneMap.OneNote;

namespace OneMap.Tests
{
    public class OneNoteIntegrationTestFixture: IDisposable
    {
        public OneNoteIntegrationTestFixture()
        {
            App = new Application();

            CleanUpTestNotebook();
        }

        private void CleanUpTestNotebook()
        {
            var testNotebook = GetTestNotebook();

            if (testNotebook != null)
            {
                foreach (var sg in testNotebook.SectionGroup ?? Enumerable.Empty<SectionGroup>())
                {
                    App.DeleteHierarchy(sg.ID, deletePermanently: true);
                }

                foreach (var s in testNotebook.Section ?? Enumerable.Empty<Section>())
                {
                    App.DeleteHierarchy(s.ID, deletePermanently: true);
                }
            }
            else
            {
                throw new InvalidOperationException($"Please create a notebook call '{HierarchyManipulationTests.TestNotebookName}'.");
            }
        }

        public T Read<T>(string xml)
        {
            using (var reader = XmlReader.Create(new StringReader(xml)))
            {
                var serializer = new XmlSerializer(typeof(T));

                return (T)serializer.Deserialize(reader);
            }
        }

        public Notebook GetTestNotebook()
        {
            App.GetHierarchy("", HierarchyScope.hsSections, out var xml, XMLSchema.xs2010);

            var notebooks = Read<Notebooks>(xml);

            var testNotebook =
                notebooks.Notebook.FirstOrDefault(x => x.name == HierarchyManipulationTests.TestNotebookName);

            return testNotebook;
        }

        public void Dispose()
        {
            App = null;
        }

        public Application App { get; private set; }
    }
}
