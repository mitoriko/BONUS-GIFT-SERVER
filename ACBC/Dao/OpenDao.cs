using ACBC.Buss;
using Com.ACBC.Framework.Database;
using System;
using System.Collections;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace ACBC.Dao
{
    public class OpenDao
    {
        public bool UpdateMemberOpenID(string tempOpenID, string openID)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(OpenSqls.UPDATE_MEMBER_OPENID, tempOpenID, openID);
            string sqlInsert = builder.ToString();

            return DatabaseOperationWeb.ExecuteDML(sqlInsert);
        }

        public Member GetMember(string openID)
        {
            Member member = null;

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(OpenSqls.SELECT_MEMBER_BY_OPENID, openID);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count == 1)
            {
                member = new Member
                {
                    memberId = dt.Rows[0]["MEMBER_ID"].ToString(),
                    memberImg = dt.Rows[0]["MEMBER_IMG"].ToString(),
                    memberName = dt.Rows[0]["MEMBER_NAME"].ToString(),
                    memberPhone = dt.Rows[0]["MEMBER_PHONE"].ToString(),
                    memberSex = dt.Rows[0]["MEMBER_SEX"].ToString(),
                    openid = dt.Rows[0]["OPENID"].ToString(),
                    scanCode = "CHECK_" + dt.Rows[0]["SCAN_CODE"].ToString(),
                    heart = Convert.ToInt32(dt.Rows[0]["OPENID"])
                };
            }

            return member;
        }

        public bool MemberReg(MemberRegParam memberRegParam, string openID)
        {
            string scanCode = "";
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(openID));
                var strResult = BitConverter.ToString(result);
                scanCode = strResult.Replace("-", "");
            }

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(OpenSqls.INSERT_MEMBER,
                memberRegParam.nickName,
                memberRegParam.avatarUrl,
                memberRegParam.gender,
                openID,
                scanCode);
            string sqlInsert = builder.ToString();
            
            return DatabaseOperationWeb.ExecuteDML(sqlInsert);
        }

        public StoreUser GetStoreUser(string openID)
        {
            StoreUser storeUser = null;

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(OpenSqls.SELECT_STORE_USER_BY_OPENID, openID);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count == 1)
            {
                storeUser = new StoreUser
                {
                    storeUserId = dt.Rows[0]["STORE_USER_ID"].ToString(),
                    storeUserImg = dt.Rows[0]["STORE_USER_IMG"].ToString(),
                    storeUserName = dt.Rows[0]["STORE_USER_NAME"].ToString(),
                    storeUserPhone = dt.Rows[0]["STORE_USER_PHONE"].ToString(),
                    storeUserSex = dt.Rows[0]["STORE_USER_SEX"].ToString(),
                    openid = dt.Rows[0]["OPENID"].ToString(),
                    storeId = dt.Rows[0]["STORE_ID"].ToString(),
                };
            }

            return storeUser;
        }

        public string GetStoreId(string storeCode)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(OpenSqls.SELECT_STORE_CODE, storeCode);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count == 1)
            {
                return dt.Rows[0]["STORE_ID"].ToString();
            }

            return "";
        }

        public bool StoreUserReg(StoreUserRegParam storeUserRegParam, string openID, string storeId)
        {
            ArrayList list = new ArrayList();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(OpenSqls.INSERT_STORE_USER,
                storeUserRegParam.nickName,
                storeUserRegParam.avatarUrl,
                storeUserRegParam.gender,
                openID,
                storeId);
            string sqlInsert = builder.ToString();
            list.Add(sqlInsert);
            builder.Clear();
            builder.AppendFormat(OpenSqls.UPDATE_STORE_CODE,
                storeUserRegParam.storeCode);
            sqlInsert = builder.ToString();
            list.Add(sqlInsert);
            return DatabaseOperationWeb.ExecuteDML(list);
        }

        public bool MoveMember(
           string memberId1,
           string memberId2,
           int changeHeart)
        {
            ArrayList list = new ArrayList();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(
                OpenSqls.UPDATE_HEART_COMMIT,
                memberId1,
                memberId2
                );
            string sql = builder.ToString();
            list.Add(sql);
            builder.Clear();
            builder.AppendFormat(
                OpenSqls.UPDATE_ORDER,
                memberId1,
                memberId2
                );
            sql = builder.ToString();
            list.Add(sql);
            builder.Clear();
            builder.AppendFormat(
                OpenSqls.INSERT_HEART_COMMIT,
                2,
                memberId2,
                memberId1,
                changeHeart
                );
            sql = builder.ToString();
            list.Add(sql);
            return DatabaseOperationWeb.ExecuteDML(list);
        }

        private class OpenSqls
        {
            public const string SELECT_MEMBER_BY_OPENID = ""
                + "SELECT * "
                + "FROM T_BASE_MEMBER "
                + "WHERE OPENID = '{0}'";
            public const string INSERT_MEMBER = ""
                + "INSERT INTO T_BASE_MEMBER "
                + "(MEMBER_NAME,MEMBER_IMG,MEMBER_SEX,OPENID,SCAN_CODE)"
                + "VALUES( "
                + "'{0}','{1}','{2}','{3}','{4}')";
            public const string SELECT_STORE_USER_BY_OPENID = ""
                + "SELECT * "
                + "FROM T_BASE_STORE_USER "
                + "WHERE OPENID = '{0}'";
            public const string INSERT_STORE_USER = ""
                + "INSERT INTO T_BASE_STORE_USER "
                + "(STORE_USER_NAME,STORE_USER_IMG,STORE_USER_SEX,OPENID,STORE_ID)"
                + "VALUES( "
                + "'{0}','{1}','{2}','{3}',{4})";
            public const string UPDATE_STORE_CODE = ""
                + "UPDATE T_BUSS_STORE_CODE "
                + "SET STATE = STATE - 1 "
                + "WHERE STORE_CODE = '{0}' ";
            public const string SELECT_STORE_CODE = ""
                + "SELECT * "
                + "FROM T_BUSS_STORE_CODE "
                + "WHERE STORE_CODE = '{0}' "
                + "AND STATE > 0";
            public const string UPDATE_MEMBER_OPENID = ""
                + "UPDATE T_BASE_MEMBER "
                + "SET OPENID = '{1}' "
                + "WHERE OPENID = '{0}' ";
            public const string UPDATE_HEART_COMMIT = ""
                + "UPDATE T_REMOTE_HEART_COMMIT "
                + "SET MEMBER_ID = {0} "
                + "WHERE MEMBER_ID = {1} ";
            public const string INSERT_HEART_COMMIT = ""
                + "INSERT INTO T_REMOTE_HEART_COMMIT "
                + "(HEART_TYPE,HEART_FROM_ID,MEMBER_ID,HEART) "
                + "VALUES({0},'{1}',{2},{3})";
            public const string UPDATE_ORDER = ""
                + "UPDATE T_BUSS_ORDER "
                + "SET MEMBER_ID = {0} "
                + "WHERE MEMBER_ID = {1}";
        }
    }
}
