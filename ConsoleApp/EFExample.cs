using SuperProducer.Framework.DAL;
using System;
using System.Linq;

namespace ConsoleApp
{
    /// <summary>
    /// 
    /// </summary>
    public class EFExample
    {
        public static void QueryAndPaged()
        {
            var orderID = 3L;
            var orderNo = "BC20170301919B0000003";

            orderID = 0;
            orderNo = string.Empty;

            if (orderID > 0) { }

            #region "方式一"

            //using (var mainContext = new MainDbContext())
            //{
            //    var query = mainContext.OrderHandleLog.AsQueryable();

            //    if (orderID > 0)
            //        query = query.Where(item => item.OrderID == orderID || item.OrderID == 10);

            //    if (!string.IsNullOrEmpty(orderNo))
            //        query = query.Where(item => item.OrderNo == orderNo);

            //    var oks1 = query.OrderBy(item => item.OrderID).ThenByDescending(item => item.ID).ToPagedList(1, 3);

            //    Console.WriteLine("【EF-查询并分页1】");

            //    foreach (var item in oks1)
            //    {
            //        Console.WriteLine(string.Format("ID:{0},OrderID:{1},OrderNo:{2},CreateTime:{3}", item.ID, item.OrderID, item.OrderNo, item.CreateTime));
            //    }
            //}

            #endregion

            #region "方式二"

            //using (var mainContext = new MainDbContext())
            //{
            //    var where = PredicateExtensionses.True<OrderHandleLog>();

            //    if (orderID > 0)
            //        where = where.And(item => item.OrderID == orderID || item.OrderID == 10);

            //    if (!string.IsNullOrEmpty(orderNo))
            //        where = where.And(item => item.OrderNo == orderNo);

            //    var order = new Action<IOrderable<OrderHandleLog>>((item) =>
            //    {
            //        item.Asc(value => value.OrderID).ThenDesc(value => value.ID);
            //    });

            //    var oks1 = mainContext.FindAll(where, order, 3);

            //    Console.WriteLine("【EF-查询并分页2】");

            //    foreach (var item in oks1)
            //    {
            //        Console.WriteLine(string.Format("ID:{0},OrderID:{1},OrderNo:{2},CreateTime:{3}", item.ID, item.OrderID, item.OrderNo, item.CreateTime));
            //    }
            //}

            #endregion
        }
    }
}
