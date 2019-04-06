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
    public class StoreDao
    {
        public List<AsnGoods> GetAsnGoodsList(string storeId)
        {
            List<AsnGoods> list = new List<AsnGoods>();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(StoreSqls.SELECT_ORDER_GOODS_LIST_BY_STORE_ID, storeId, "1");
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if(dt != null)
            {
                foreach(DataRow dr in dt.Rows)
                {
                    AsnGoods asnGoods = new AsnGoods
                    {
                        goodsId = dr["GOODS_ID"].ToString(),
                        goodsImg = dr["GOODS_IMG"].ToString(),
                        goodsName = dr["GOODS_NAME"].ToString(),
                        num = dr["NUMS"].ToString(),
                    };
                    list.Add(asnGoods);
                }
            }

            return list;
        }

        public List<StockGoods> GetStockGoodsList(string storeId)
        {
            List<StockGoods> list = new List<StockGoods>();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(StoreSqls.SELECT_ORDER_GOODS_LIST_BY_STORE_ID, storeId, "2");
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    StockGoods stockGoods = new StockGoods
                    {
                        goodsId = dr["GOODS_ID"].ToString(),
                        goodsImg = dr["GOODS_IMG"].ToString(),
                        goodsName = dr["GOODS_NAME"].ToString(),
                        num = dr["NUMS"].ToString(),
                        lastUpdateTime = dr["ASN_TIME"].ToString(),
                    };
                    list.Add(stockGoods);
                }
            }

            return list;
        }

        public List<StoreAccount> GetStoreAccount(string storeId)
        {
            List<StoreAccount> list = new List<StoreAccount>();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(StoreSqls.SELECT_STORE_ACCOUNT_BY_STORE_ID, storeId);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    StoreAccount storeAccount = new StoreAccount
                    {
                        heartNum = dr["HEARTS"].ToString(),
                        paymentDays = dr["ACCOUNT_FROM"].ToString() + " - " + dr["ACCOUNT_TO"].ToString(),
                        state = dr["STATE"].ToString() == "0" ? "待付款" : "已付款",
                    };
                    list.Add(storeAccount);
                }
            }

            return list;
        }

        public bool UpdateOrderState(string orderId, string storeUserId)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(StoreSqls.UPDATE_ORDER_STATE, orderId, storeUserId);
            string sql = builder.ToString();
            return DatabaseOperationWeb.ExecuteDML(sql);
        }

        public string GetStoreId(string storeCode)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(StoreSqls.SELECT_STORE_ID_BY_STORE_CODE, storeCode);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count == 1)
            {
                return dt.Rows[0]["STORE_ID"].ToString();
            }

            return "";
        }

        public string CheckStoreMember(string storeId, string memberId)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(StoreSqls.SELECT_REMOTE_STORE_AND_STORE_MEMBER, storeId, memberId);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count == 1)
            {
                return dt.Rows[0]["PHONE"].ToString();
            }

            return "";
        }

        public bool InserRemoteCommit(string storeId, string phone, int score)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(StoreSqls.INSERT_POINT_COMMIT, storeId, phone, score);
            string sql = builder.ToString();
            return DatabaseOperationWeb.ExecuteDML(sql);
        }
    }

    public class StoreSqls
    {
        public const string SELECT_ORDER_GOODS_LIST_BY_STORE_ID = ""
                + "SELECT T.GOODS_ID,T.GOODS_NAME,T.GOODS_IMG,SUM(T.NUM) AS NUMS,MAX(ASN_TIME) AS ASN_TIME "
                + "FROM T_BUSS_ORDER_GOODS T,T_BUSS_ORDER A,T_BASE_STORE B "
                + "WHERE A.ORDER_CODE = T.ORDER_CODE "
                + "AND B.STORE_CODE = A.STORE_CODE " 
                + "AND T.GOODS_STATE = {1} "
                + "AND B.STORE_ID = {0} "
                + "GROUP BY T.GOODS_ID,T.GOODS_NAME,T.GOODS_IMG ";
        public const string SELECT_STORE_ACCOUNT_BY_STORE_ID = ""
                + "SELECT * FROM T_BUSS_STORE_ACCOUNT "
                + "WHERE STORE_ID = {0} "
                + "ORDER BY SORT DESC";
        public const string UPDATE_ORDER_STATE = ""
                + "UPDATE T_BUSS_ORDER "
                + "SET STATE = 3, "
                + "PICKUP_TIME = NOW(), "
                + "PICKUP_STORE_USER = {1} "
                + "WHERE ORDER_ID = {0} ";
        public const string SELECT_STORE_ID_BY_STORE_CODE = ""
                + "SELECT * FROM T_BASE_STORE "
                + "WHERE STORE_CODE = '{0}' ";
        public const string SELECT_REMOTE_STORE_AND_STORE_MEMBER = ""
                + "SELECT * "
                + "FROM T_REMOTE_STORE_MEMBER A, "
                + "T_BUSS_MEMBER_STORE B "
                + "WHERE A.PHONE = B.REG_PHONE "
                + "AND A.STORE_ID = B.STORE_ID "
                + "AND A.STORE_ID = {0} "
                + "AND B.MEMBER_ID = {1}";
        public const string INSERT_POINT_COMMIT = ""
                + "INSERT INTO T_REMOTE_POINT_COMMIT(STORE_ID,PHONE,STATE,TYPE,POINT) "
                + "VALUES({0},'{1}', 0, 0, {2}) ";
    }
}
