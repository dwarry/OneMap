using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OneMap.Controls;

using Xunit;
using FluentAssertions;

namespace OneMap.Tests
{
    public class TreeItemManipulation
    {
        [Fact]
        public void MovingAnItemUpPreservesIndexes()
        {
            var child0 = new TestTreeItem();
            var child1 = new TestTreeItem();
            var child2 = new TestTreeItem();
            var child3 = new TestTreeItem();
            var child4 = new TestTreeItem();

            var parent = new TestTreeItem(child0, child1, child2, child3, child4);

            
            child2.MoveUp();

            for(int i = 0; i < parent.Children.Count; ++i)
            {
                parent.Children[i].Index.Should().Be(i);
            }

            parent.Children[0].Should().Be(child0);
            parent.Children[1].Should().Be(child2);
            parent.Children[2].Should().Be(child1);
            parent.Children[3].Should().Be(child3);
            parent.Children[4].Should().Be(child4);
        }

        [Fact]
        public void MovingAnItemDownPreservesIndexes()
        {
            var child0 = new TestTreeItem();
            var child1 = new TestTreeItem();
            var child2 = new TestTreeItem();
            var child3 = new TestTreeItem();
            var child4 = new TestTreeItem();

            var parent = new TestTreeItem(child0, child1, child2, child3, child4);


            child2.MoveDown();

            for (int i = 0; i < parent.Children.Count; ++i)
            {
                parent.Children[i].Index.Should().Be(i);
            }

            parent.Children[0].Should().Be(child0);
            parent.Children[1].Should().Be(child1);
            parent.Children[2].Should().Be(child3);
            parent.Children[3].Should().Be(child2);
            parent.Children[4].Should().Be(child4);
        }

        [Fact]
        public void DeletingAnItemResetsIndexes()
        {
            var child0 = new TestTreeItem();
            var child1 = new TestTreeItem();
            var child2 = new TestTreeItem();
            var child3 = new TestTreeItem();
            var child4 = new TestTreeItem();

            var parent = new TestTreeItem(child0, child1, child2, child3, child4);


            child2.Delete();

            for (int i = 0; i < parent.Children.Count; ++i)
            {
                parent.Children[i].Index.Should().Be(i);
            }

            parent.Children.Should().HaveCount(4);

            parent.Children[0].Should().Be(child0);
            parent.Children[1].Should().Be(child1);
            parent.Children[2].Should().Be(child3);
            parent.Children[3].Should().Be(child4);
        }

        [Fact]
        public void FirstItemCannotMoveUpButSubsequentOnesCan()
        {
            var child0 = new TestTreeItem();
            var child1 = new TestTreeItem();
            var child2 = new TestTreeItem();
            var child3 = new TestTreeItem();
            var child4 = new TestTreeItem();

            var parent = new TestTreeItem(child0, child1, child2, child3, child4);

            child0.CanMoveUp.Should().BeFalse();
            child1.CanMoveUp.Should().BeTrue();
            child2.CanMoveUp.Should().BeTrue();
            child3.CanMoveUp.Should().BeTrue();
            child4.CanMoveUp.Should().BeTrue();
        }

        [Fact]
        public void LastItemCannotMoveDownButSubsequentOnesCan()
        {
            var child0 = new TestTreeItem();
            var child1 = new TestTreeItem();
            var child2 = new TestTreeItem();
            var child3 = new TestTreeItem();
            var child4 = new TestTreeItem();

            var parent = new TestTreeItem(child0, child1, child2, child3, child4);

            child0.CanMoveDown.Should().BeTrue();
            child1.CanMoveDown.Should().BeTrue();
            child2.CanMoveDown.Should().BeTrue();
            child3.CanMoveDown.Should().BeTrue();
            child4.CanMoveDown.Should().BeFalse();
        }

        [Fact]
        public void ItemsWithParentCanBeDeleted()
        {
            var child0 = new TestTreeItem();
            var child1 = new TestTreeItem();
            var child2 = new TestTreeItem();
            var child3 = new TestTreeItem();
            var child4 = new TestTreeItem();

            var parent = new TestTreeItem(child0, child1, child2, child3, child4);

            parent.CanDelete.Should().BeFalse();
            child0.CanDelete.Should().BeTrue();
            child1.CanDelete.Should().BeTrue();
            child2.CanDelete.Should().BeTrue();
            child3.CanDelete.Should().BeTrue();
            child4.CanDelete.Should().BeTrue();
        }

        [Fact]
        public void MovingItemsResetsGuardProperties()
        {
            var child0 = new TestTreeItem();
            var child1 = new TestTreeItem();
            var child2 = new TestTreeItem();
            var child3 = new TestTreeItem();
            var child4 = new TestTreeItem();

            var parent = new TestTreeItem(child0, child1, child2, child3, child4);

            child0.CanMoveUp.Should().BeFalse();
            child1.CanMoveUp.Should().BeTrue();

            child1.MoveUp();

            child0.CanMoveUp.Should().BeTrue();
            child1.CanMoveUp.Should().BeFalse();


            child3.CanMoveDown.Should().BeTrue();
            child4.CanMoveDown.Should().BeFalse();

            child3.MoveDown();

            child3.CanMoveDown.Should().BeFalse();
            child4.CanMoveDown.Should().BeTrue();
        }


        [DebuggerDisplay("TestTreeItem({Index})")]
        public class TestTreeItem : TreeItem
        {
            public TestTreeItem(params TestTreeItem[] children) : base(Guid.NewGuid().ToString(), children)
            {

            }
        }
    }
}
