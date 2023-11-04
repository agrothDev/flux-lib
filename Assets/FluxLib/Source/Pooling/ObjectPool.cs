using System;
using System.Collections.Generic;

namespace FluxLib.Source.Pooling
{
    public class ObjectPool<T> where T : IPoolable, new()
    {
        /// <summary>
        /// The amount of initialized objects in the pool.
        /// </summary>
        public int ObjectCount { get; private set; }

        /// <summary>
        /// The maximum amount of objects that the pool can hold.
        /// </summary>
        public int MaxSize { get; private set; }

        private const int DefaultMaxSize = 100;

        private readonly Stack<T> _pool = new();

        private readonly ObjectInitializationMode _objectInitializationMode;
        private readonly bool _allowResize;

        public ObjectPool() : this(ObjectInitializationMode.Eager, true, DefaultMaxSize)
        {
        }

        public ObjectPool(ObjectInitializationMode objectInitializationMode, bool allowResize, int maxSize)
        {
            _objectInitializationMode = objectInitializationMode;
            _allowResize = allowResize;
            MaxSize = maxSize;

            if (_objectInitializationMode == ObjectInitializationMode.Eager)
            {
                InitializePoolObjects(maxSize);
            }
        }

        public T Get()
        {
            if (_pool.Count > 0)
            {
                return RetrieveFromPool();
            }

            switch (_objectInitializationMode)
            {
                case ObjectInitializationMode.Eager:
                    if (_allowResize)
                    {
                        ResizeAndInitialize();
                        return RetrieveFromPool();
                    }

                    break;
                case ObjectInitializationMode.Lazy:
                    if (ObjectCount < MaxSize)
                    {
                        CreateLazy();
                        return RetrieveFromPool();
                    }

                    if (_allowResize)
                    {
                        ResizePool();
                        CreateLazy();
                        return RetrieveFromPool();
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            throw new PoolOverflowException();
        }

        public void Release(T pooledObject)
        {
            if (_pool.Count == MaxSize)
            {
                throw new PoolOverflowException();
            }

            pooledObject.OnRelease();
            _pool.Push(pooledObject);
        }

        private T RetrieveFromPool()
        {
            var pooledObject = _pool.Pop();
            pooledObject.OnGet();
            return pooledObject;
        }

        private void CreateLazy()
        {
            var pooledObject = new T();
            ObjectCount++;
            _pool.Push(pooledObject);
        }

        private void ResizeAndInitialize()
        {
            ResizePool();
            InitializePoolObjects(MaxSize - ObjectCount);
        }

        private void InitializePoolObjects(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var pooledObject = new T();
                _pool.Push(pooledObject);
            }

            ObjectCount += count;
        }

        private void ResizePool()
        {
            int newMaxSize = MaxSize * 2;
            MaxSize = newMaxSize;
        }
    }
}