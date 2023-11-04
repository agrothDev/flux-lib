using FluxLib.Source.Pooling;
using NUnit.Framework;

namespace FluxLib.Extras.FluxLib.Tests.Pooling
{
    public class ObjectPoolTests
    {
        [Test]
        public void ObjectPool_WithEagerInitialization_FillsPoolToMaxSize()
        {
            var pool = new ObjectPool<ExamplePoolItem>(ObjectInitializationMode.Eager, false, 10);

            int objectCount = pool.ObjectCount;

            Assert.AreEqual(objectCount, 10);
        }

        [Test]
        public void ObjectPool_WithLazyInitialization_StartsOutEmpty()
        {
            var pool = new ObjectPool<ExamplePoolItem>(ObjectInitializationMode.Lazy, false, 10);

            int objectCount = pool.ObjectCount;

            Assert.AreEqual(objectCount, 0);
        }

        [TestCase(ObjectInitializationMode.Lazy)]
        [TestCase(ObjectInitializationMode.Eager)]
        public void Get_WhenGettingObject_CallsOnGet(ObjectInitializationMode objectInitializationMode)
        {
            var pool = new ObjectPool<ExamplePoolItem>(objectInitializationMode, false, 10);

            var retrievedObject = pool.Get();

            Assert.AreEqual(retrievedObject.Value, 1);
        }

        [TestCase(ObjectInitializationMode.Lazy)]
        [TestCase(ObjectInitializationMode.Eager)]
        public void GetOnEmptyPool_WithResizingDisabled_ThrowsException(
            ObjectInitializationMode objectInitializationMode)
        {
            var pool = new ObjectPool<ExamplePoolItem>(objectInitializationMode, false, 10);

            for (int i = 0; i < 10; i++)
            {
                pool.Get();
            }

            Assert.Throws<PoolOverflowException>(() => pool.Get());
        }

        [TestCase(ObjectInitializationMode.Lazy)]
        [TestCase(ObjectInitializationMode.Eager)]
        public void GetOnEmptyPool_WithResizingEnabled_ResizesCorrectly(
            ObjectInitializationMode objectInitializationMode)
        {
            var pool = new ObjectPool<ExamplePoolItem>(objectInitializationMode, true, 10);

            for (int i = 0; i < 10 + 1; i++)
            {
                pool.Get();
            }

            Assert.AreEqual(pool.MaxSize, 20);
        }

        [TestCase(ObjectInitializationMode.Lazy)]
        [TestCase(ObjectInitializationMode.Eager)]
        public void Release_WhenReleasingObject_CallsOnRelease(ObjectInitializationMode objectInitializationMode)
        {
            var pool = new ObjectPool<ExamplePoolItem>(objectInitializationMode, false, 10);
            var retrievedObject = pool.Get();

            pool.Release(retrievedObject);

            Assert.AreEqual(retrievedObject.Value, 0);
        }
    }
}