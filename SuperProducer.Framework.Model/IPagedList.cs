using System;

namespace SuperProducer.Framework.Model
{
    public interface IPagedList
    {
        int CurrentPageIndex { get; set; }

        int PageSize { get; set; }

        int TotalItemCount { get; set; }
    }
}
