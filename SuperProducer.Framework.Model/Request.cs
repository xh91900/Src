using System;
using System.Collections.Generic;

namespace SuperProducer.Framework.Model
{
    /// <summary>
    /// 用作BLL的分页查询基类
    /// </summary>
    public class Request : ModelBase
    {
        public Request()
        {
            PageSize = 5000;
        }

        public int Top
        {
            set
            {
                this.PageSize = value;
                this.PageIndex = 1;
            }
        }

        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }
}
