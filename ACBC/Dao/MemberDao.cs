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


        private class MemberSqls
        {
            public const string SELECT_MEMBER_STORE_BY_MEMBER_ID = ""
                + "SELECT * "
                + "FROM T_BUSS_MEMBER_STORE T "
                + "WHERE T.MEMBER_ID = {0} "
                + "AND IS_DEFAULT = 1";
        }
    }
}
