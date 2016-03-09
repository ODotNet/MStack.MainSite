using NHibernate;
using NHibernate.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MStack.Core.Repositories
{
    public class SessionManager
    {
        public static void Rollback()
        {
            ISession session = CurrentSessionContext.Unbind(NHSessionFactory.SessionFactory);
            if (session == null)
                return;
            using (session)
            {
                using (ITransaction trans = session.Transaction)
                {
                    try
                    {
                        if (trans.IsActive && !trans.WasCommitted)
                            trans.Rollback();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        session.Close();
                    }
                }
            }
        }

        internal static ISession OpenSession()
        {
            ISession session = NHSessionFactory.SessionFactory.OpenSession();
            session.FlushMode = FlushMode.Commit;
            session.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
            CurrentSessionContext.Bind(session);
            return session;
        }

        public static void Commit()
        {
            ISession session = CurrentSessionContext.Unbind(NHSessionFactory.SessionFactory);
            if (session == null)
                return;
            using (session)
            {
                using (ITransaction trans = session.Transaction)
                {
                    try
                    {
                        if (trans.IsActive && !trans.WasCommitted)
                            trans.Commit();
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                        throw;
                    }
                    finally
                    {
                        session.Close();
                    }
                }
            }
        }
    }
}
