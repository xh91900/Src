using System;
using System.Collections.Generic;
using System.Threading;

namespace SuperProducer.Core.Utility
{
    public class QueueManager
    {
        private readonly Queue<Item> WaitItems;

        public QueueManager()
        {
            this.WaitItems = new Queue<Item>();
        }

        public bool AddItem(Item item)
        {
            try
            {
                lock (this)
                {
                    this.WaitItems.Enqueue(item);
                }
                return true;
            }
            catch { }
            return false;
        }

        public Item GetItem()
        {
            try
            {
                lock (this)
                {
                    if (this.HasItem)
                    {
                        return this.WaitItems.Dequeue();
                    }
                }
            }
            catch { }
            return null;
        }

        public int GetItemCount()
        {
            try
            {
                lock (this)
                {
                    return this.WaitItems.Count;
                }
            }
            catch { }
            return 0;
        }

        public bool HasItem
        {
            get
            {
                return this.GetItemCount() > 0;
            }
        }

        public class Item
        {
            public object data { get; set; }

            public Action<object> fn { get; set; }
        }
    }

    internal class QueueExecutor
    {
        private QueueManager _QueueManager;

        protected QueueExecutor(QueueManager manager)
        {
            if (manager == null)
                throw new ArgumentNullException("manager");
            this._QueueManager = manager;
        }

        protected void Execute()
        {
            while (true)
            {
                if (this._QueueManager.HasItem)
                {
                    try
                    {
                        QueueManager.Item tmpItem = this._QueueManager.GetItem();
                        if (tmpItem != null)
                        {
                            tmpItem.fn(tmpItem.data);
                        }
                    }
                    catch { }
                }
                Thread.Sleep(500);
            }
        }

        public static bool Start(QueueManager manager)
        {
            try
            {
                bool tmpComplete;
                TaskHelper.Start<int>(out tmpComplete, 0, () =>
                {
                    new QueueExecutor(manager).Execute();
                    return 1;
                });
                return true;
            }
            catch { }
            return false;
        }
    }

    public class QueueHelper
    {
        /// <summary>
        /// 简单队列
        /// </summary>
        internal static QueueManager InternalQueue { get; set; }

        static QueueHelper()
        {
            InternalQueue = new QueueManager();
        }

        public static void Start()
        {
            QueueExecutor.Start(InternalQueue);
        }

        public static bool AddTask(QueueManager.Item item)
        {
            if (InternalQueue.GetItemCount() + 1 > InternalConstant.DefaultQueueItemMaxCount)
            {
                throw new Exception(string.Format("队列长度已到达最大限制({0})", InternalConstant.DefaultQueueItemMaxCount));
            }
            return InternalQueue.AddItem(item);
        }
    }
}
