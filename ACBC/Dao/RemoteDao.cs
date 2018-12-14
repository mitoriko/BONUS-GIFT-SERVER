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
    public class RemoteDao
    {
        public bool AddRemoteStoreMember(string code, string phone, string cardCode, string point)
        {
            ArrayList list = new ArrayList();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(RemoteSqls.INSERT_STORE_MEMBER, code, phone, cardCode);
            string sql = builder.ToString();
            list.Add(sql);
            builder.Clear();
            builder.AppendFormat(RemoteSqls.INSERT_POINT_COMMIT, code, phone, point);
            sql = builder.ToString();
            list.Add(sql);
            return DatabaseOperationWeb.ExecuteDML(list);
        }

        public bool AddPointCommit(string code, string phone, string point)
        {
            ArrayList list = new ArrayList();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(RemoteSqls.INSERT_POINT_COMMIT, code, phone, point);
            string sql = builder.ToString();
            list.Add(sql);
            return DatabaseOperationWeb.ExecuteDML(list);
        }

        public bool GetStoreMemberByCode(string code, string phone)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(RemoteSqls.SELECT_STORE_MEMBER_BY_CODE_AND_PHONE, code, phone);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt == null)
                return false;
            if(dt.Rows.Count == 1)
                return false;
            return true;
        }

        public List<RemotePointCommit> GetPointCommitList(string code)
        {
            List<RemotePointCommit> list = new List<RemotePointCommit>();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(RemoteSqls.SELECT_POINT_COMMIT, code);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if(dt != null)
            {
                foreach(DataRow dr in dt.Rows)
                {
                    RemotePointCommit remotePointCommit = new RemotePointCommit
                    {
                        pointCommitId = dr["POINT_COMMIT_ID"].ToString(),
                        storeId = dr["STORE_ID"].ToString(),
                        phone = dr["PHONE"].ToString(),
                        state = dr["STATE"].ToString(),
                        type = dr["TYPE"].ToString(),
                        point = Convert.ToInt32(dr["POINT"]),
                    };
                    list.Add(remotePointCommit);
                }

            }

            return list;
        }

        public bool UpdatePointCommit(string pointCommitId)
        {
            ArrayList list = new ArrayList();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(RemoteSqls.UPDATE_POINT_COMMIT, pointCommitId);
            string sql = builder.ToString();
            list.Add(sql);
            return DatabaseOperationWeb.ExecuteDML(list);
        }

        public class RemoteSqls
        {
            public const string SELECT_STORE_MEMBER_BY_CODE_AND_PHONE = ""
                + "SELECT * "
                + "FROM T_REMOTE_STORE_MEMBER "
                + "WHERE STORE_ID = {0} "
                + "AND PHONE = '{1}' ";
            public const string INSERT_STORE_MEMBER = ""
                + "INSERT INTO T_REMOTE_STORE_MEMBER(STORE_ID,PHONE,REG_TIME,CARD_CODE,POINT) "
                + "VALUES({0},'{1}',NOW(),'{2}', 0) ";
            public const string INSERT_POINT_COMMIT = ""
                + "INSERT INTO T_REMOTE_POINT_COMMIT(STORE_ID,PHONE,STATE,TYPE,POINT) "
                + "VALUES({0},'{1}', 0, 0, {2}) ";
            public const string SELECT_POINT_COMMIT = ""
                + "SELECT * "
                + "FROM T_REMOTE_POINT_COMMIT "
                + "WHERE STORE_ID = {0} "
                + "AND STATE = 0 "
                + "AND TYPE = 1";
            public const string UPDATE_POINT_COMMIT = ""
                + "UPDATE T_REMOTE_POINT_COMMIT "
                + "SET STATE = 1 "
                + "WHERE POINT_COMMIT_ID = {0}";
        }
    }
}
