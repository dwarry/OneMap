using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

using Microsoft.Office.Interop.OneNote;

using OneMap.OneNote;

using Xunit;

namespace OneMap.Tests
{
    public class HierarchyManipulationTests : IClassFixture<OneNoteIntegrationTestFixture>
    {
        private OneNoteIntegrationTestFixture _fixture;

        public HierarchyManipulationTests(OneNoteIntegrationTestFixture fixture)
        {
            _fixture = fixture;
        }

        public const string TestNotebookName = "test";
        const string TestSectionGroup1 = "Test SG1";
        const string TestSectionGroup2 = "Test SG2";

        private Notebook GetTestNotebook(OneNotePersistence persistence)
        {
            var notebooks = persistence.LoadNotebooks();

            return notebooks.Notebook.First(x => x.name == TestNotebookName);
        }


        [Fact]
        public void CanCreateSectionGroups()
        {
            var target = new OneNotePersistence();

            var nb = GetTestNotebook(target);

            var newId = target.CreateSectionGroup(nb, TestSectionGroup1);
          
            newId.Should().NotBeNullOrEmpty("A new SectionGroup should have been created");
           
            nb = GetTestNotebook(target);
            
            var sg = nb.SectionGroup.First(x => x.name == TestSectionGroup1);

            sg.Should().NotBeNull("The new SectionGroup should have been persisted");


            newId = target.CreateSectionGroup(sg, TestSectionGroup2);

            newId.Should().NotBeNullOrEmpty("A new sub-SectionGroup should have been created");
        }

        [Fact]
        public void CanCreateSectionInNotebook()
        {
            var target = new OneNotePersistence();

            var nb = GetTestNotebook(target);

            var newId = target.CreateSection(nb, "Test Section");

            newId.Should().NotBeNullOrWhiteSpace("New Section should have been created");

            newId = target.CreateSectionBefore(nb, "Test Section Before", newId);

            newId.Should().NotBeNullOrWhiteSpace("New Section should have been created");

            newId = target.CreateSectionAfter(nb, "Test Section After", newId);

            newId.Should().NotBeNullOrWhiteSpace("New Section should have been created");

            nb = GetTestNotebook(target);

            nb.Section.Select(x => x.name).Should()
                .ContainInOrder("Test Section Before", "Test Section After", "Test Section");
        }

        [Fact]
        public void CanCreatePages()
        {
            var target = new OneNotePersistence();

            var nb = GetTestNotebook(target);

            var sectionId = target.CreateSection(nb, "Pages");

            nb = GetTestNotebook(target);

            var section = nb.Section.First(x => x.name == "Pages");

            var p1Id = target.CreatePage(section, "P1");

            p1Id.Should().NotBeNullOrWhiteSpace("P1 should have been created");

            var p2Id = target.CreatePageBefore(section, "P2", p1Id);

            p2Id.Should().NotBeNullOrWhiteSpace("P2 should have been created");

            var p3Id = target.CreatePageAfter(section, "P3", p2Id);

            p3Id.Should().NotBeNullOrWhiteSpace("P3 should have been created");

            var p1_2Id = target.CreateChildPage(section, "P1_1", p1Id);

            p1_2Id.Should().NotBeNullOrWhiteSpace("P1_2 should have been created");

            var p1_3Id = target.CreateChildPage(section, "P1_2", p1_2Id);

            p1_3Id.Should().NotBeNullOrWhiteSpace("P1_3 should have been created");

            var p4Id = target.CreatePageAfter(section, "P4", p1Id);

            p4Id.Should().NotBeNullOrWhiteSpace("P4 should have been created");

            var nb2 = GetTestNotebook(target);

            nb2.Section.First(x => x.ID == section.ID).Page.Select(x => x.name)
                .Should().ContainInOrder("P2", "P3", "P1", "P1_1", "P1_2", "P4");
        }


        private T Read<T>(string xml)
        {
            using (var reader = XmlReader.Create(new StringReader(xml)))
            {
                var serializer = new XmlSerializer(typeof(T));

                return (T)serializer.Deserialize(reader);
            }
        }


    }


    internal static class XmlValidationExtensions
    {
        public static XmlValidationAssertions Should(this IOneNoteObject subject)
        {
            return new XmlValidationAssertions(subject);
        }
    }


    internal class XmlValidationAssertions : ReferenceTypeAssertions<IOneNoteObject, XmlValidationAssertions>
    {
        private static readonly Lazy<XmlSchemaSet> _schemaSet = new Lazy<XmlSchemaSet>(() =>
            {
                var nameTable = new NameTable();

                var result = new XmlSchemaSet(nameTable);

                using (var reader = new StringReader(Schemata.OneNote2010Schema))
                using (var xmlReader = new XmlTextReader(reader))
                {

                    result.Add(Schemata.OneNote2010Namespace, xmlReader);
                }

                return result;
            });


        public XmlValidationAssertions(IOneNoteObject instance)
        {
            Subject = instance;

        }

        protected override string Identifier => Subject.GetType().Name;

        public AndConstraint<XmlValidationAssertions> PassSchemaValidation(string because, params object[] becauseArgs)
        {
            string xml = null; 

            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject != null)
                .FailWith("Subject cannot be null")
                .Then
                .Given(SubjectValidationErrors)
                .ForCondition(x => x.Count == 0)
                .FailWith("XML Validation errors: {0}", errors => String.Join(Environment.NewLine, errors));

            return new AndConstraint<XmlValidationAssertions>(this);
        }

        private List<string> SubjectValidationErrors()
        {
            string xml;

            using (var sw = new StringWriter())
            using (var writer = new XmlTextWriter(sw))
            {
                var serializer = new XmlSerializer(Subject.GetType());

                serializer.Serialize(writer, Subject);

                xml = sw.ToString();
            }

            XDocument doc = XDocument.Parse(xml);

            var result = new List<string>();

            doc.Validate(_schemaSet.Value, (o, e) => { result.Add(e.Message);});

            return result;
        }

    }
}


