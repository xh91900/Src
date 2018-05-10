using SuperProducer.Core.Utility;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuperProducer.Framework.Model
{
    public class ModelBase
    {
        public ModelBase()
        {
            this.CreateTime = DateTime.Now;
            this.LastModifyTime = this.CreateTime;
        }

        /// <summary>
        /// 自增ID[主键]
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// 平台类型
        /// </summary>
        public byte PlatformType { get; set; }

        /// <summary>
        /// 软删除(0-未删除,1-已删除)
        /// </summary>
        public bool IsDel { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? LastModifyTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        ///// <summary>
        ///// 版本号
        ///// </summary>
        //[Timestamp]
        //public byte[] VersionCode { get; set; }



        /// <summary>
        /// 以sourceObject对象初始化当前对象
        /// </summary>
        public virtual void InitializeField(object sourceObject)
        {
            if (sourceObject != null)
            {
                ObjectHelper.DeepCopy(sourceObject, this);
            }
        }

        /// <summary>
        /// 以当前对象初始化targetObject对象
        /// </summary>
        public virtual void DeInitializeField(object targetObject)
        {
            if (targetObject != null)
            {
                ObjectHelper.DeepCopy(this, targetObject);
            }
        }
    }
}
