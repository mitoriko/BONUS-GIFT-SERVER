using ACBC.Buss;
using Com.ACBC.Framework.Database;
using System;
using System.Collections;
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
            builder.AppendFormat(MemberSqls.SELECT_STORE_BY_MEMBER_ID, memberId);
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
                        storeAddr = dr["STORE_ADDR"].ToString(),
                        storeCardImg = dr["STORE_CARD_IMG"].ToString(),
                        storeId = dr["STORE_ID"].ToString(),
                        storeName = dr["STORE_NAME"].ToString(),
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

        public bool SetDefaultMemberStore(string memberId, string storeId)
        {
            ArrayList list = new ArrayList();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(MemberSqls.UPDATE_DEFAULT_MEMBER_STORE_1, memberId);
            string sql = builder.ToString();
            list.Add(sql);
            builder.Clear();
            builder.AppendFormat(MemberSqls.UPDATE_DEFAULT_MEMBER_STORE_2, memberId, storeId);
            sql = builder.ToString();
            list.Add(sql);
            return DatabaseOperationWeb.ExecuteDML(list);
        }

        public RemoteStoreMember GetRemoteStoreMember(string storeId, string phone)
        {
            //MemberSqls.SELECT_REMOTE_STORE_MEMBER
            return null;
        }

        public List<RemotePointCommit> GetRemotePointCommitList(string memberId)
        {
            //MemberSqls.SELECT_REMOTE_POINT_COMMIT
            return null;
        }

        public bool BindBindMemberStore(string memberId, RemoteStoreMember remoteStoreMember, bool setDefault)
        {
            //MemberSqls.INSERT_MEMBER_STORE
            return false;
        }

        public bool HandleCommitPoint(List<RemotePointCommit> list, RemoteStoreMember remoteStoreMember)
        {
            //MemberSqls.UPDATE_REMOTE_STORE_MEMBER_POINT_ADD
            //MemberSqls.UPDATE_REMOTE_POINT_COMMIT_STATE
            return false;
        }

        public bool AddCommitPoint()
        {
            //MemberSqls.INSERT_REMOTE_POINT_COMMIT
            //MemberSqls.UPDATE_REMOTE_STORE_MEMBER_POINT_MINUS
            return false;
        }

        private class MemberSqls
        {
            public const string SELECT_ORDER_LIST_BY_MEMBER_ID = ""
                + "SELECT * "
                + "FROM T_BUSS_ORDER "
                + "WHERE MEMBER_ID = {0} ";
            public const string SELECT_MEMBER_BY_MEMBER_ID = ""
                + "SELECT * "
                + "FROM T_BASE_MEMBER "
                + "WHERE MEMBER_ID = {0}";
            public const string SELECT_STORE_BY_MEMBER_ID = ""
                + "SELECT * "
                + "FROM T_BASE_STORE A,T_BUSS_MEMBER_STORE B "
                + "WHERE A.STORE_ID = B.STORE_ID " 
                + "AND B.MEMBER_ID = {0}";
            public const string UPDATE_DEFAULT_MEMBER_STORE_1 = ""
                + "UPDATE T_BUSS_MEMBER_STORE "
                + "SET IS_DEFAULT = 0 "
                + "WHERE MEMBER_ID = {0}";
            public const string UPDATE_DEFAULT_MEMBER_STORE_2 = ""
                + "UPDATE T_BUSS_MEMBER_STORE "
                + "SET IS_DEFAULT = 1 "
                + "WHERE MEMBER_ID = {0} "
                + "AND STORE_ID = {1}";
            public const string SELECT_REMOTE_STORE_MEMBER = ""
                + "";
            public const string INSERT_MEMBER_STORE = ""
                + "";
            public const string SELECT_REMOTE_POINT_COMMIT = ""
                + "";
            public const string UPDATE_REMOTE_STORE_MEMBER_POINT_ADD = ""
                + "";
            public const string UPDATE_REMOTE_POINT_COMMIT_STATE = ""
                + "";
            public const string INSERT_REMOTE_POINT_COMMIT = ""
                + "";
            public const string UPDATE_REMOTE_STORE_MEMBER_POINT_MINUS = ""
                + "";
        }
    }
}
