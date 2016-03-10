using NHibernate.Connection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace MStack.MainSite.WebFramework.Data
{
    public class DbConnectionProvider : DriverConnectionProvider
    {
        protected override string ConnectionString
        {
            get
            {
                return DbConfig.ConnectionString;
            }
        }
    }

    public class DbConfig
    {
        private static string _connectionString;
        private static readonly object Locker = new object();
        public static string ConnectionString
        {
            get
            {
                if (_connectionString == null)
                {

                    lock (Locker)
                    {
                        if (_connectionString == null)
                        {
                            if (ConfigurationManager.ConnectionStrings["MStackDb"] == null)
                                throw new Exception("没有找到名称为MStackDb的ConnectionString");
                            return _connectionString = ConfigurationManager.ConnectionStrings["MStackDb"].ConnectionString;
                        }
                    }
                }
                return _connectionString;
            }
        }
    }
}