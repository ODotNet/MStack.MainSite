using NHibernate;
using NHibernate.Cfg;
using System.Data;
using System.Web;

namespace MStack.Core.Repositories
{
    public static class NHSessionFactory
    {
        public static ISessionFactory SessionFactory { get; private set; }

        //public static ISession Session
        //{
        //    get
        //    {
        //        try
        //        {
        //            if (!HttpContext.Current.Items.Contains("NHibernate.Context.WebSessionContext.SessionFactoryMapKey"))
        //            {
        //                SessionManager.OpenSession();
        //            }
        //            return SessionFactory.GetCurrentSession();
        //        }
        //        catch (HibernateException)
        //        {
        //            return SessionManager.OpenSession();
        //        }
        //    }
        //}

        public static ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }

        public static IStatelessSession StatelessSession
        {
            get { return SessionFactory.OpenStatelessSession(); }
        }

        public static IDbConnection DbConnection
        {
            get { return StatelessSession.Connection; }
        }

        static NHSessionFactory()
        {
            Init();
        }

        private static void Init()
        {
            Configuration nhConfig = new Configuration().Configure();
            SessionFactory = nhConfig.BuildSessionFactory();
        }
    }
}