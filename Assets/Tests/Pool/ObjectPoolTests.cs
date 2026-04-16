using MFramework.Pool;

using NUnit.Framework;

namespace MFramework.Tests.Pool
{
    public class ObjectPoolTests
    {
        [Test]
        public void GetItem_WhenPoolEmpty_CreatesContainerAndMarksUsed()
        {
            int createCount = 0;
            var pool = new ObjectPool<int>(() => ++createCount);

            ObjectPoolContainer<int> container = pool.GetItem();

            Assert.That(container, Is.Not.Null);
            Assert.That(container.Item, Is.EqualTo(1));
            Assert.That(container.Used, Is.True);
            Assert.That(pool.TotalCount, Is.EqualTo(1));
            Assert.That(pool.UsedCount, Is.EqualTo(1));
            Assert.That(pool.UnusedCount, Is.EqualTo(0));
        }

        [Test]
        public void ReleaseItem_AfterBorrow_ReusesSameContainer()
        {
            var pool = new ObjectPool<string>(() => "item");

            ObjectPoolContainer<string> first = pool.GetItem();
            bool released = pool.ReleaseItem(first);
            ObjectPoolContainer<string> second = pool.GetItem();

            Assert.That(released, Is.True);
            Assert.That(object.ReferenceEquals(first, second), Is.True);
            Assert.That(second.Used, Is.True);
            Assert.That(pool.TotalCount, Is.EqualTo(1));
        }

        [Test]
        public void ReleaseItem_Twice_DoesNotDuplicateUnusedContainer()
        {
            var pool = new ObjectPool<int>(() => 1);

            ObjectPoolContainer<int> container = pool.GetItem();
            bool firstRelease = pool.ReleaseItem(container);
            bool secondRelease = pool.ReleaseItem(container);

            Assert.That(firstRelease, Is.True);
            Assert.That(secondRelease, Is.False);
            Assert.That(pool.TotalCount, Is.EqualTo(1));
            Assert.That(pool.UnusedCount, Is.EqualTo(1));
            Assert.That(pool.UsedCount, Is.EqualTo(0));
        }
    }
}
