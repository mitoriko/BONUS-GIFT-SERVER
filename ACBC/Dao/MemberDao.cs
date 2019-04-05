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

        public List<RemoteStoreMember> GetRemoteStoreMemberList(string memberId)
        {
            List<RemoteStoreMember> list = new List<RemoteStoreMember>();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(MemberSqls.SELECT_REMOTE_STORE_MEMBER_LIST, memberId);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null)
            {
                foreach(DataRow dr in dt.Rows)
                {
                    RemoteStoreMember remoteStoreMember = new RemoteStoreMember
                    {
                        cardCode = dr["CARD_CODE"].ToString(),
                        phone = dr["PHONE"].ToString(),
                        point = Convert.ToInt32(dr["POINT"]),
                        regTime = dr["REG_TIME"].ToString(),
                        storeId = dr["STORE_ID"].ToString(),
                        storeMemberId = dr["STORE_MEMBER_ID"].ToString(),
                        storeRate = Convert.ToInt32(dr["STORE_RATE"]),
                        storeImg = dr["STORE_IMG"].ToString(),
                        storeName = dr["STORE_NAME"].ToString(),
                    };
                    list.Add(remoteStoreMember);
                }
                
            }

            return list;
        }

        public RemoteStoreMember GetRemoteStoreMember(string storeId, string phone)
        {
            RemoteStoreMember remoteStoreMember = null;
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(MemberSqls.SELECT_REMOTE_STORE_MEMBER, storeId, phone);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count == 0)
            {
                builder.Clear();
                builder.AppendFormat(MemberSqls.INSERT_REMOTE_STORE_MEMBER, storeId, phone);
                sql = builder.ToString();
                DatabaseOperationWeb.ExecuteDML(sql);
            }
            builder.Clear();
            builder.AppendFormat(MemberSqls.SELECT_REMOTE_STORE_MEMBER, storeId, phone);
            sql = builder.ToString();
            dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count == 1)
            {
                remoteStoreMember = new RemoteStoreMember
                {
                    cardCode = dt.Rows[0]["CARD_CODE"].ToString(),
                    phone = dt.Rows[0]["PHONE"].ToString(),
                    point = Convert.ToInt32(dt.Rows[0]["POINT"]),
                    regTime = dt.Rows[0]["REG_TIME"].ToString(),
                    storeId = dt.Rows[0]["STORE_ID"].ToString(),
                    storeMemberId = dt.Rows[0]["STORE_MEMBER_ID"].ToString(),
                };
            }

            return remoteStoreMember;
        }

        public RemoteStoreMember GetRemoteStoreMember(string storeMemberId)
        {
            RemoteStoreMember remoteStoreMember = null;
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(MemberSqls.SELECT_REMOTE_STORE_MEMBER_BY_ID, storeMemberId);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count == 1)
            {
                remoteStoreMember = new RemoteStoreMember
                {
                    cardCode = dt.Rows[0]["CARD_CODE"].ToString(),
                    phone = dt.Rows[0]["PHONE"].ToString(),
                    point = Convert.ToInt32(dt.Rows[0]["POINT"]),
                    regTime = dt.Rows[0]["REG_TIME"].ToString(),
                    storeId = dt.Rows[0]["STORE_ID"].ToString(),
                    storeMemberId = dt.Rows[0]["STORE_MEMBER_ID"].ToString(),
                    storeRate = Convert.ToInt32(dt.Rows[0]["STORE_RATE"]),
                    exchangeLimit = Convert.ToInt32(dt.Rows[0]["EXCHANGE_LIMIT"]),
                };


            }

            return remoteStoreMember;
        }

        public List<RemotePointCommit> GetRemotePointCommitList(RemoteStoreMember remoteStoreMember)
        {
            List<RemotePointCommit> list = new List<RemotePointCommit>();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(MemberSqls.SELECT_REMOTE_POINT_COMMIT, remoteStoreMember.storeId, remoteStoreMember.phone);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    RemotePointCommit remotePointCommit = new RemotePointCommit
                    {
                        phone = dr["PHONE"].ToString(),
                        point = Convert.ToInt32(dr["POINT"]),
                        memberId = dr["MEMBER_ID"].ToString(),
                        storeId = dr["STORE_ID"].ToString(),
                        pointCommitId = dr["POINT_COMMIT_ID"].ToString(),
                        state = dr["STATE"].ToString(),
                        type = dr["TYPE"].ToString(),
                    };
                    list.Add(remotePointCommit);
                }
            }
            return list;
        }

        public bool CheckPhone(string phone, string storeId)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(MemberSqls.SELECT_STORE_MEMBER_BY_PHONE_AND_STORE, phone, storeId);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count == 0)
            {
                return true;

            }
            return false;
        }

        public bool BindMemberStore(string memberId, RemoteStoreMember remoteStoreMember, bool setDefault)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(
                MemberSqls.INSERT_MEMBER_STORE,
                remoteStoreMember.storeId,
                memberId,
                remoteStoreMember.phone,
                remoteStoreMember.cardCode,
                setDefault
                );
            string sql = builder.ToString();
            return DatabaseOperationWeb.ExecuteDML(sql);
        }

        public bool HandleCommitPoint(RemoteStoreMember remoteStoreMember, List<RemotePointCommit> remotePointCommitlist)
        {
            int totalCommit = 0;
            string remotePointCommitIds = "";
            foreach (RemotePointCommit remotePointCommit in remotePointCommitlist)
            {
                totalCommit += remotePointCommit.point;
                remotePointCommitIds += remotePointCommit.pointCommitId + ",";
            }
            if (remotePointCommitIds.Length > 0)
            {
                remotePointCommitIds = remotePointCommitIds.Substring(0, remotePointCommitIds.Length - 1);
            }
            ArrayList list = new ArrayList();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(
                MemberSqls.UPDATE_REMOTE_POINT_COMMIT_STATE,
                remotePointCommitIds
                );
            string sql = builder.ToString();
            list.Add(sql);
            builder.Clear();
            builder.AppendFormat(
                MemberSqls.UPDATE_REMOTE_STORE_MEMBER_POINT_ADD,
                remoteStoreMember.storeMemberId,
                totalCommit
                );
            sql = builder.ToString();
            list.Add(sql);
            return DatabaseOperationWeb.ExecuteDML(list);
        }

        public bool AddCommitPoint(string memberId, RemoteStoreMember remoteStoreMember, int point, int heart, int oriHeart)
        {
            ArrayList list = new ArrayList();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(
                MemberSqls.INSERT_REMOTE_POINT_COMMIT,
                remoteStoreMember.storeId,
                remoteStoreMember.phone,
                memberId,
                0,
                1,
                point
                );
            string sql = builder.ToString();
            list.Add(sql);
            builder.Clear();
            builder.AppendFormat(
                MemberSqls.UPDATE_REMOTE_STORE_MEMBER_POINT_MINUS,
                remoteStoreMember.storeMemberId,
                point
                );
            sql = builder.ToString();
            list.Add(sql);
            builder.Clear();
            builder.AppendFormat(
                MemberSqls.UPDATE_MEMBER_HEART_BY_MEMBER_ID,
                memberId,
                heart
                );
            sql = builder.ToString();
            list.Add(sql);
            builder.Clear();
            builder.AppendFormat(
                MemberSqls.INSERT_HEART_CHANGE,
                heart,
                point,
                memberId,
                oriHeart,
                remoteStoreMember.storeId
                );
            sql = builder.ToString();
            list.Add(sql);
            return DatabaseOperationWeb.ExecuteDML(list);
        }

        public int GetTodayChangeHeart(string memberId, string storeId)
        {
            int todayChangeHeart = 9999;
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(MemberSqls.SELECT_MEMBER_CHANGE_BY_STORE, memberId, storeId);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count == 1)
            {
                todayChangeHeart = Convert.ToInt32(dt.Rows[0][0]);
            }

            return todayChangeHeart;
        }

        public bool UpdateScanCode(string memberId, string scanCode)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(
                MemberSqls.UPDATE_SCAN_CODE,
                memberId,
                scanCode
                );
            string sql = builder.ToString();
            return DatabaseOperationWeb.ExecuteDML(sql);
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
            public const string SELECT_REMOTE_STORE_MEMBER_LIST = ""
                + "SELECT * "
                + "FROM T_REMOTE_STORE_MEMBER A,T_BUSS_MEMBER_STORE B,T_BASE_STORE C "
                + "WHERE A.STORE_ID = B.STORE_ID "
                + "AND C.STORE_ID = B.STORE_ID "
                + "AND B.REG_PHONE = A.PHONE "
                + "AND B.MEMBER_ID = {0}";
            public const string SELECT_REMOTE_STORE_MEMBER = ""
                + "SELECT * "
                + "FROM T_REMOTE_STORE_MEMBER "
                + "WHERE STORE_ID = {0} "
                + "AND PHONE = '{1}'";
            public const string INSERT_REMOTE_STORE_MEMBER = ""
                + "INSERT INTO T_REMOTE_STORE_MEMBER "
                + "(STORE_ID,PHONE,REG_TIME,CARD_CODE,POINT) "
                + "VALUES({0},'{1}',NOW(),'{1}',0) ";
            public const string SELECT_REMOTE_STORE_MEMBER_BY_ID = ""
                + "SELECT * "
                + "FROM T_REMOTE_STORE_MEMBER A, T_BASE_STORE B "
                + "WHERE A.STORE_ID = B.STORE_ID "
                + "AND STORE_MEMBER_ID = {0} ";
            public const string SELECT_STORE_MEMBER_BY_PHONE_AND_STORE = ""
                + "SELECT * "
                + "FROM T_BUSS_MEMBER_STORE "
                + "WHERE REG_PHONE = '{0}' "
                + "AND STORE_ID = {1}";
            public const string INSERT_MEMBER_STORE = ""
                + "INSERT INTO T_BUSS_MEMBER_STORE "
                + "(STORE_ID,MEMBER_ID,REG_PHONE,CARD_CODE,IS_DEFAULT) "
                + "VALUES({0},{1},'{2}','{3}',{4})";
            public const string SELECT_REMOTE_POINT_COMMIT = ""
                + "SELECT * "
                + "FROM T_REMOTE_POINT_COMMIT "
                + "WHERE STORE_ID = {0} "
                + "AND PHONE = {1} "
                + "AND STATE = 0 "
                + "AND TYPE = 0";
            public const string UPDATE_REMOTE_STORE_MEMBER_POINT_ADD = ""
                + "UPDATE T_REMOTE_STORE_MEMBER "
                + "SET POINT = POINT + {1} "
                + "WHERE STORE_MEMBER_ID = {0}";
            public const string UPDATE_REMOTE_POINT_COMMIT_STATE = ""
                + "UPDATE T_REMOTE_POINT_COMMIT "
                + "SET STATE = 1 "
                + "WHERE POINT_COMMIT_ID IN({0}) ";
            public const string INSERT_REMOTE_POINT_COMMIT = ""
                + "INSERT INTO T_REMOTE_POINT_COMMIT "
                + "(STORE_ID,PHONE,MEMBER_ID,STATE,TYPE,POINT) "
                + "VALUES({0},'{1}',{2},{3},{4},{5})";
            public const string UPDATE_REMOTE_STORE_MEMBER_POINT_MINUS = ""
                + "UPDATE T_REMOTE_STORE_MEMBER "
                + "SET POINT = POINT - {1} "
                 + "WHERE STORE_MEMBER_ID = {0}";
            public const string INSERT_HEART_CHANGE = ""
                + "INSERT INTO T_BUSS_HEART_CHANGE "
                + "(CHANGE_TYPE,NUM,POINTS,MEMBER_ID,BEFORE_MOD,STORE_ID) "
                + "VALUES(0,{0},{1},{2},{3},{4}) ";
            public const string UPDATE_MEMBER_HEART_BY_MEMBER_ID = ""
                + "UPDATE T_BASE_MEMBER "
                + "SET HEART = HEART + {1} "
                + "WHERE MEMBER_ID = {0} ";
            public const string SELECT_MEMBER_CHANGE_BY_STORE = ""
                + "SELECT IFNULL(SUM(NUM),0) "
                + "FROM T_BUSS_HEART_CHANGE "
                + "WHERE MEMBER_ID = {0} "
                + "AND STORE_ID = {1} "
                + "AND CURDATE() = DATE(CHANGE_TIME)";
            public const string UPDATE_SCAN_CODE = ""
                + "UPDATE T_BASE_MEMBER SET SCAN_CODE = '{1}' "
                + "WHERE MEMBER_ID = {0}";
            public const string SELECT_STORE_LIMIT_ADD = ""
                + "SELECT * "
                + "FROM T_BUSS_STORE_LIMIT_ADD "
                + "WHERE MEMBER_ID = {0} "
                + "AND STORE_ID = {1} ";
        }
    }
}
