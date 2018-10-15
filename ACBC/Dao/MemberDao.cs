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
    public class MemberDao
    {
        public List<MemberStore> GetMemberStoreListByMemberId(string memberId)
        {
            List<MemberStore> list = new List<MemberStore>();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(MemberSqls.SELECT_MEMBER_STORE_BY_MEMBER_ID, memberId);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    MemberStore memberStore = new MemberStore
                    {
                        bindDate = dr["BIND_DATE"].ToString(),
                        cardCode = dr["CARD_CODE"].ToString(),
                        memberId = dr["MEMBER_ID"].ToString(),
                        isDefault = dr["IS_DEFAULT"].ToString() == "1",
                        memberStoreId = dr["MEMBER_STORE_ID"].ToString(),
                        regPhone = dr["REG_PHONE"].ToString(),
                    };
                    list.Add(memberStore);
                }
            }
            return list;
        }

        public MemberInfo GetMemberInfo(string memberId)
        {
            MemberInfo memberInfo = new MemberInfo();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(MemberSqls.SELECT_ORDER_LIST_BY_MEMBER_ID, memberId);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null)
            {
                memberInfo.unPay = dt.Select("STATE = 0", "ORDER_ID, ORDER_TIME DESC").Length;
                memberInfo.pay = dt.Select("STATE = 1", "ORDER_ID, ORDER_TIME DESC").Length;
                memberInfo.inStore = dt.Select("STATE = 2", "ORDER_ID, ORDER_TIME DESC").Length;
                memberInfo.done = dt.Select("STATE = 3", "ORDER_ID, ORDER_TIME DESC").Length;
            }

            builder.Clear();
            builder.AppendFormat(MemberSqls.SELECT_MEMBER_BY_MEMBER_ID, memberId);
            sql = builder.ToString();
            dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count == 1)
            {
                memberInfo.heart = Convert.ToInt32(dt.Rows[0]["HEART"]);
            }

            return memberInfo;
        }


        private class MemberSqls
        {
            public const string SELECT_MEMBER_STORE_BY_MEMBER_ID = ""
                + "SELECT * "
                + "FROM T_BUSS_MEMBER_STORE T "
                + "WHERE T.MEMBER_ID = {0} "
                + "AND IS_DEFAULT = 1";
            public const string SELECT_ORDER_LIST_BY_MEMBER_ID = ""
                + "SELECT * "
                + "FROM T_BUSS_ORDER "
                + "WHERE MEMBER_ID = {0} ";
            public const string SELECT_MEMBER_BY_MEMBER_ID = ""
                + "SELECT * "
                + "FROM T_BASE_MEMBER "
                + "WHERE MEMBER_ID = {0}";
        }
    }
}
