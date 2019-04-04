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
    }

    public class StoreSqls
    {
        public const string SELECT_ORDER_GOODS_LIST_BY_STORE_ID = ""
                + "SELECT T.GOODS_ID,T.GOODS_NAME,T.GOODS_IMG,SUM(T.NUM) AS NUMS,MAX(ASN_TIME) AS ASN_TIME "
                + "FROM T_BUSS_ORDER_GOODS T,T_BUSS_ORDER A,T_BASE_STORE B "
                + "WHERE A.ORDER_CODE = T.ORDER_CODE "
                + "AND B.STORE_CODE = A.STORE_CODE " 
                + "AND T.STATE = {1} "
                + "AND B.STORE_ID = {0} "
                + "GROUP BY T.GOODS_ID,T.GOODS_NAME,T.GOODS_IMG ";

        public const string SELECT_STORE_ACCOUNT_BY_STORE_ID = ""
                + "SELECT * FROM T_BUSS_STORE_ACCOUNT "
                + "WHERE STORE_ID = {0} "
                + "ORDER BY SORT DESC";
    }
}
