using SuperProducer.Core.Cache;

namespace SuperProducer.Framework.DAL
{
    public class DbContextFactory
    {
        public static T CreateContext<T>() where T : DbContextBase, new()
        {
            var context = CacheHelper.GetItem<T>(() =>
            {
                return new T();
            });

            if (context.IsDisposable)
            {
                CacheHelper.RemoveItem<T>();

                context = CacheHelper.GetItem<T>(() =>
                {
                    return new T();
                });
            }

            return context;
        }
    }
}
