using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentAssertions;

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
    }
}
