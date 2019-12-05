using ACBC.Buss;
using Com.ACBC.Framework.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
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

        public Store GetStoreByStoreId(string storeId)
        {
            Store store = null;
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(RemoteSqls.SELECT_STORE_BY_STORE_ID, storeId);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count == 1)
            {
                store = new Store
                {
                    storeAddr = dt.Rows[0]["STORE_ADDR"].ToString(),
                    storeCode = dt.Rows[0]["STORE_CODE"].ToString(),
                    storeDesc = dt.Rows[0]["STORE_DESC"].ToString(),
                    storeId = dt.Rows[0]["STORE_ID"].ToString(),
                    storeImg = dt.Rows[0]["STORE_IMG"].ToString(),
                    storeName = dt.Rows[0]["STORE_NAME"].ToString(),
                    storeRate = Convert.ToInt32(dt.Rows[0]["STORE_RATE"]),
                    openReg = Convert.ToInt32(dt.Rows[0]["OPEN_REG"]),
                };

            }

            return store;
        }

        public bool GetMemberStoreById(string code, string phone)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(RemoteSqls.SELECT_STORE_MEMBER_BY_ID_AND_PHONE, code, phone);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt == null)
                return false;
            if (dt.Rows.Count == 1)
                return false;
            return true;
        }

        public bool GetOrderByOrderCode(string orderCode)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(RemoteSqls.SELECT_ORDER_BY_ORDER_CODE, orderCode);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt == null)
                return false;
            if (dt.Rows.Count == 1)
                return false;
            return true;
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
            builder.AppendFormat(RemoteSqls.INSERT_MEMBER,
                memberRegParam.nickName,
                memberRegParam.avatarUrl,
                memberRegParam.gender,
                openID,
                scanCode);
            string sqlInsert = builder.ToString();

            return DatabaseOperationWeb.ExecuteDML(sqlInsert);
        }

        public Member GetMember(string openID)
        {
            Member member = null;
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(RemoteSqls.SELECT_MEMBER_BY_OPENID, openID);
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
                };
            }

            return member;
        }

        public Member GetMember(string storeId, string phone)
        {
            Member member = null;
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(RemoteSqls.SELECT_MEMBER_BY_STORE_AND_PHONE, storeId, phone);
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
                };
            }

            return member;
        }

        public bool BindMemberStore(string memberId, RemoteStoreMember remoteStoreMember, bool setDefault)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(
                RemoteSqls.INSERT_MEMBER_STORE,
                remoteStoreMember.storeId,
                memberId,
                remoteStoreMember.phone,
                remoteStoreMember.cardCode,
                setDefault
                );
            string sql = builder.ToString();
            return DatabaseOperationWeb.ExecuteDML(sql);
        }

        public bool CreateOrder(
            string orderCode,
            string state, 
            string memberId, 
            string addr, 
            int num,
            Store store, 
            Goods goods)
        {
            ArrayList list = new ArrayList();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(
                RemoteSqls.INSERT_ORDER,
                orderCode,
                (goods.goodsPrice * num),
                store.storeCode,
                state,
                memberId,
                store.storeName,
                addr
                );
            string sql = builder.ToString();
            list.Add(sql);
            builder.Clear();
            builder.AppendFormat(
                RemoteSqls.INSERT_ORDER_GOODS,
                goods.goodsId,
                goods.goodsImg,
                goods.goodsName,
                goods.goodsPrice,
                num,
                orderCode
                );
            sql = builder.ToString();
            list.Add(sql);
            return DatabaseOperationWeb.ExecuteDML(list);
        }

        public Goods GetGoodsByGoodsId(string goodsId)
        {
            Goods goods = null;
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(RemoteSqls.SELECT_GOODS_BY_GOODS_ID, goodsId);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                goods = new Goods
                {
                    goodsDesc = dt.Rows[0]["GOODS_DESC"].ToString(),
                    goodsId = dt.Rows[0]["GOODS_ID"].ToString(),
                    goodsName = dt.Rows[0]["GOODS_NAME"].ToString(),
                    goodsImg = dt.Rows[0]["GOODS_IMG"].ToString(),
                    goodsPrice = Convert.ToInt32(dt.Rows[0]["GOODS_PRICE"]),
                    goodsStock = Convert.ToInt32(dt.Rows[0]["GOODS_STOCK"]),
                    sales = 0,
                };
            }
            return goods;
        }

        public bool AddHeart(string heartType, string heartFromId, string memberId, int heart)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(
                RemoteSqls.INSERT_HEART_COMMIT,
                heartType,
                heartFromId,
                memberId,
                heart
                );
            string sql = builder.ToString();
            return DatabaseOperationWeb.ExecuteDML(sql);
        }

        public bool GetHeartCommitByFromId(string heartFromId)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(RemoteSqls.SELECT_HEART_COMMIT, heartFromId);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt == null)
                return false;
            if (dt.Rows.Count == 1)
                return false;
            return true;
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
            public const string SELECT_STORE_BY_STORE_ID = ""
                + "SELECT * "
                + "FROM T_BASE_STORE "
                + "WHERE STORE_ID = {0}";
            public const string SELECT_STORE_MEMBER_BY_ID_AND_PHONE = ""
                + "SELECT * "
                + "FROM T_BUSS_MEMBER_STORE "
                + "WHERE STORE_ID = {0} "
                + "AND REG_PHONE = '{1}' ";
            public const string INSERT_MEMBER_STORE = ""
                + "INSERT INTO T_BUSS_MEMBER_STORE "
                + "(STORE_ID,MEMBER_ID,REG_PHONE,CARD_CODE,IS_DEFAULT) "
                + "VALUES({0},{1},'{2}','{3}',{4})";
            public const string INSERT_MEMBER = ""
                + "INSERT INTO T_BASE_MEMBER "
                + "(MEMBER_NAME,MEMBER_IMG,MEMBER_SEX,OPENID,SCAN_CODE)"
                + "VALUES( "
                + "'{0}','{1}','{2}','{3}','{4}')";
            public const string SELECT_MEMBER_BY_OPENID = ""
                + "SELECT * "
                + "FROM T_BASE_MEMBER "
                + "WHERE OPENID = '{0}'";
            public const string SELECT_MEMBER_BY_STORE_AND_PHONE = ""
                + "SELECT * "
                + "FROM T_BASE_MEMBER A, T_BUSS_MEMBER_STORE B "
                + "WHERE A.MEMBER_ID = B.MEMBER_ID "
                + "AND B.STORE_ID = {0} "
                + "AND B.REG_PHONE = '{1}'";
            public const string INSERT_ORDER = ""
                + "INSERT INTO T_BUSS_ORDER "
                + "(ORDER_CODE,TOTAL,STORE_CODE,PAY_TIME,STATE,MEMBER_ID,REMARK,ADDR) "
                + "VALUES('{0}',{1},'{2}',NOW(),{3},{4},'{5}','{6}')";
            public const string INSERT_ORDER_GOODS = ""
                + "INSERT INTO T_BUSS_ORDER_GOODS "
                + "(GOODS_ID,GOODS_IMG,GOODS_NAME,PRICE,NUM,ORDER_CODE) "
                + "VALUES({0},'{1}','{2}',{3},{4},'{5}')";
            public const string SELECT_GOODS_BY_GOODS_ID = ""
                + "SELECT * "
                + "FROM T_BUSS_GOODS  "
                + "WHERE GOODS_ID = {0}";
            public const string INSERT_HEART_COMMIT = ""
                + "INSERT INTO T_REMOTE_HEART_COMMIT "
                + "(HEART_TYPE,HEART_FROM_ID,MEMBER_ID,HEART) "
                + "VALUES({0},'{1}',{2},{3})";
            public const string SELECT_ORDER_BY_ORDER_CODE = ""
                + "SELECT * "
                + "FROM T_BUSS_ORDER  "
                + "WHERE ORDER_CODE = '{0}'";
            public const string SELECT_HEART_COMMIT = ""
                + "SELECT * "
                + "FROM T_REMOTE_HEART_COMMIT "
                + "WHERE HEART_FROM_ID = '{0}' "
                + "AND HEART_TYPE = 1";
        }
    }
}
