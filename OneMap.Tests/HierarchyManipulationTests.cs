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

using OneMap.OneNote;

using Xunit;

namespace OneMap.Tests
{
    public class HierarchyManipulationTests
    {
        private const string TestNotebookName = "test";

        private Notebook GetTestNotebook(OneNotePersistence persistence)
        {
            var notebooks = persistence.LoadNotebooks();

            return notebooks.Notebook.First(x => x.name == TestNotebookName);
        }

        [Fact]
        public void CanGenerateValidXmlForNewSectionGroup()
        {
            var target = new OneNotePersistence();

            var nb = GetTestNotebook(target);
    
            var update = 
            target.CreateNewItem(nb, "Test SG1", 
                (item, t) => item.name = t, 
                item => item.name,
                x => x.ID,
                x => x.ID, 
                x => x.SectionGroup,
                (p, newCollection) => p.SectionGroup = newCollection);

            update.Should().PassSchemaValidation("the serialization should have worked");
        }

        [Fact]
        public void CanCreateNewSectionGroup()
        {
            var target = new OneNotePersistence();

            var nb = GetTestNotebook(target);

            var update = target.CreateSectionGroup(nb, "Test SG1");

            update.Should().NotBeNullOrEmpty("A new SectionGroup should have been created");

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


