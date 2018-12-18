using Com.ACBC.Framework.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACBC.Dao
{
    public class AuthDao
    {
        /// <summary>
        /// 验证当前用户是否可调用api路径
        /// </summary>
        /// <param name="route">api路径</param>
        /// <param name="code">用户id</param>
        /// <returns></returns>
        public bool CheckAuth(string route, string code)
        {
            return true;
        }

        /// <summary>
        /// 获取用户的AppSecret
        /// </summary>
        /// <param name="code">用户id</param>
        /// <param name="appId">用户appId</param>
        /// <returns></returns>
        public string GetAccess(string code, string appId)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(AuthSqls.SELECT_APP_SECRET_BY_APP_ID, appId, code);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count == 1)
            {
                return dt.Rows[0]["APP_SECRET"].ToString();
            }
            else
            {
                return "";
            }
        }

        public class AuthSqls
        {
            public const string SELECT_APP_SECRET_BY_APP_ID = ""
                + "SELECT * "
                + "FROM T_BASE_STORE "
                + "WHERE APP_ID = '{0}' "
                + "AND STORE_ID = '{1}' ";
        }
    }

}
