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
            builder.AppendFormat(RemoteSqls.INSERT_STORE_MEMBER, code, phone, cardCode, point);
            string sql = builder.ToString();
            list.Add(sql);
            builder.Clear();
            builder.AppendFormat(RemoteSqls.INSERT_POINT_COMMIT, code, phone, point);
            sql = builder.ToString();
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

        public class RemoteSqls
        {
            public const string SELECT_STORE_MEMBER_BY_CODE_AND_PHONE = ""
                + "SELECT * "
                + "FROM T_REMOTE_STORE_MEMBER A, T_BASE_STORE B "
                + "WHERE A.STORE_ID = B.STORE_ID "
                + "AND B.STORE_CODE = '{0}' "
                + "AND A.PHONE = '{1}' ";
            public const string INSERT_STORE_MEMBER = ""
                + "INSERT INTO T_REMOTE_STORE_MEMBER(STORE_ID,PHONE,REG_TIME,CARD_CODE,POINT) "
                + "VALUES((SELECT STORE_ID FROM T_BASE_STORE WHERE STORE_CODE = '{0}'),'{1}',NOW(),'{2}',{3}) ";
            public const string INSERT_POINT_COMMIT = ""
                + "INSERT INTO T_REMOTE_POINT_COMMIT(STORE_ID,PHONE,STATE,TYPE,POINT) "
                + "VALUES((SELECT STORE_ID FROM T_BASE_STORE WHERE STORE_CODE = '{0}'),'{1}', 1, 0, {2}) ";
        }
    }
}
