using ACBC.Buss;
using Com.ACBC.Framework.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACBC.Dao
{
    public class MallDao
    {
        public Home GetHome()
        {
            Home home = null;
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(MallSqls.SELECT_HOME_BY_USE_THEME);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count == 1)
            {
                home = new Home
                {
                    homeId = dt.Rows[0]["HOME_ID"].ToString(),
                    backgroundImg = dt.Rows[0]["BACKGROUND_IMG"].ToString(),
                    titleMaster = dt.Rows[0]["TITLE_MASTER"].ToString(),
                    titleSlaver = dt.Rows[0]["TITLE_SLAVER"].ToString(),
                    titleTagImg = dt.Rows[0]["TITLE_TAG_IMG"].ToString(),
                };
            }

            return home;
        }

        public List<HomeList> GetHomeList(string homeId)
        {
            List<HomeList> list = new List<HomeList>();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(MallSqls.SELECT_HOME_LIST_BY_HOME_ID, homeId);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null)
            {
                foreach(DataRow dr in dt.Rows)
                {
                    HomeList homeList = new HomeList
                    {
                        img = dr["IMG"].ToString(),
                        title = dr["TITLE"].ToString(),
                        price = dr["PRICE"].ToString(),
                        praise = dr["PRAISE"].ToString(),
                        urlType = dr["URL_TYPE"].ToString(),
                        urlValue = dr["URL_VALUE"].ToString(),
                    };
                    list.Add(homeList);
                }
            }

            return list;
        }
    }

    public class MallSqls
    {
        public const string SELECT_HOME_BY_USE_THEME = ""
            + "SELECT * "
            + "FROM T_BASE_HOME A, T_BASE_THEME B "
            + "WHERE A.THEME_ID = B.THEME_ID "
            + "AND USE_DATE = (SELECT MAX(USE_DATE) FROM T_BASE_THEME) ";
        public const string SELECT_HOME_LIST_BY_HOME_ID = ""
            + "SELECT * "
            + "FROM T_BASE_HOME_LIST "
            + "WHERE HOME_ID = {0} "
            + "ORDER BY SORT DESC";
    }
}
