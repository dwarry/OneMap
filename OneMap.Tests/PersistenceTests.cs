using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentAssertions;

using OneMap.Controls;
using OneMap.OneNote;

using Xunit;

namespace OneMap.Tests
{
    public class PersistenceTests
    {
        [Fact]
        public void CanRetrieveNotebooks()
        {
            var target = new OneNotePersistence();

            var notebooks = target.LoadNotebooks();

            notebooks.Should().NotBeNull("Notebooks should have been loaded");

            notebooks.Notebook.Should().NotBeEmpty("it should have found at least one Notebook");
        }

        [Fact]
        public void CanParseHierarchyStructure()
        {

            var target = new OneNotePersistence();

            var notebooks = target.LoadNotebooks();

            var notebook = notebooks.Notebook[0];

            var sectionGroupAndSectionCount = notebook.SectionGroup.Length + notebook.Section.Length;

            var notebookTreeItem = new NotebookTreeItem(notebooks.Notebook[0]);

            notebookTreeItem.Should().NotBeNull();

            notebookTreeItem.Children.Should().HaveCount(sectionGroupAndSectionCount);
        }

        private const string TestPageId = "{21BAD9B9-BE9E-0A21-2CDE-DE5A5E68D484}{1}{E187919655180231383241923265548572506132521}";


        [Fact]
        public void CanRetrievePage()
        {
            var target = new OneNotePersistence();

            // substitute the value here with a page id of a page that exists
            var page = target.GetPage(TestPageId);

            page.Should().NotBeNull("Page should exist");

        }

        [Fact]
        public void CanParsePageStructure()
        {
            var target = new OneNotePersistence();

            // substitute the value here with a page id of a page that exists

            var vm = new PageContentMindMapViewModel(TestPageId, "Test Page", target);

            vm.Refresh();

            vm.AllTreeItems.Should().NotBeEmpty();

        }
    }
}
