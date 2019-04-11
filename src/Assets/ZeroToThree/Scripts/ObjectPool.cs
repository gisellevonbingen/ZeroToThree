using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ZeroToThree.Scripts
{
    public class ObjectPool<T> where T : PoolingObject
    {
        private readonly T Prefab;
        private readonly List<T> Pool;
        private readonly Queue<T> Ready;
        private readonly List<T> Obtains;

        public event EventHandler<PoolGrowEventArgs<T>> Growed;

        public ObjectPool(T prefab)
        {
            this.Prefab = prefab;
            this.Pool = new List<T>();
            this.Ready = new Queue<T>();
            this.Obtains = new List<T>();
        }

        public List<T> GetPool()
        {
            return new List<T>(this.Pool);
        }

        public List<T> GetReady()
        {
            return new List<T>(this.Ready);
        }

        public List<T> GetObtains()
        {
            return new List<T>(this.Obtains);
        }

        public void FreeAll()
        {
            this.Free(new List<T>(this.Obtains));
        }

        public void Free(IEnumerable<T> objs)
        {
            foreach (var obj in objs)
            {
                this.Free(obj);
            }

        }

        public void Free(T obj)
        {
            if (obj == null)
            {
                return;
            }

            var pool = this.Pool;

            if (pool.Contains(obj) == false)
            {
                throw new ArgumentException("Not pooled object");
            }

            var ready = this.Ready;
            ready.Enqueue(obj);

            this.OnFree(obj);
        }

        public T Obtain()
        {
            var ready = this.Ready;
            var readyCount = ready.Count;

            if (readyCount < 1)
            {
                this.Grow(1);
            }

            var obj = ready.Dequeue();
            this.OnObtain(obj);
            return obj;
        }

        public void Obtain(T[] array)
        {
            var count = array.Length;
            var ready = this.Ready;
            var readyCount = ready.Count;

            if (readyCount < count)
            {
                this.Grow(count - readyCount);
            }

            for (int i = 0; i < count; i++)
            {
                var obj = ready.Dequeue();
                this.OnObtain(obj);
                array[i] = obj;
            }

        }

        public T[] Obtain(int count)
        {
            var array = new T[count];
            this.Obtain(array);

            return array;
        }

        protected virtual void OnObtain(T obj)
        {
            this.Obtains.Add(obj);
            obj.OnObtain();
        }

        protected virtual void OnFree(T obj)
        {
            this.Obtains.Remove(obj);
            obj.OnFree();
        }

        public void Grow(int amount)
        {
            var pool = this.Pool;
            var ready = this.Ready;
            var prefab = this.Prefab;

            for (int i = 0; i < amount; i++)
            {
                int index = pool.Count;
                var obj = UnityEngine.Object.Instantiate(prefab);
                pool.Add(obj);
                ready.Enqueue(obj);

                this.OnGrowed(new PoolGrowEventArgs<T>(obj, index));
            }

        }

        protected virtual void OnGrowed(PoolGrowEventArgs<T> e)
        {
            this.Growed?.Invoke(this, e);
        }

    }

}
