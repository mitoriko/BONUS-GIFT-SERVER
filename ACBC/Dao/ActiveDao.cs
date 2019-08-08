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
    public class ActiveDao
    {
        public List<Qbuy> GetQbuyList(string memberId)
        {
            List<Qbuy> list = new List<Qbuy>();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(OrderSqls.SELECT_QBUY_LIST_BY_MEMBER_ID, memberId);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string startTime = dr["START_TIME"].ToString();
                    string endTime = dr["END_TIME"].ToString();
                    Qbuy qbuy = new Qbuy
                    {
                        qbuyCode = dr["QBUY_CODE"].ToString(),
                        storeId = dr["STORE_ID"].ToString(),
                        storeName = dr["STORE_NAME"].ToString(),
                        startTime = startTime,
                        endTime = endTime,
                        activeName = dr["REMARK"].ToString(),
                    };
                    list.Add(qbuy);
                }
            }

            return list;
        }

        public List<QBuyGoods> GetQbuyGoodsListByQbuyId(string qbuyCode)
        {
            List<QBuyGoods> list = new List<QBuyGoods>();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(OrderSqls.SELECT_QBUYGOODS_BY_QBUY_CODE, qbuyCode);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    QBuyGoods qBuyGoods = new QBuyGoods
                    {
                        qBuyGoodsId= dr["QBUY_GOODS_ID"].ToString(),
                        qBuyCode = dr["QBUY_CODE"].ToString(),
                        goodsId = dr["GOODS_ID"].ToString(),
                        goodsName = dr["GOODS_NAME"].ToString(),
                        price = dr["Q_PRICE"].ToString(),
                        num = dr["Q_NUM"].ToString(),
                        slt = dr["GOODS_IMG"].ToString(),
                    };
                    list.Add(qBuyGoods);
                }
            }

            return list;
        }

        public QBuyGoods GetQbuyGoodsByQBuyIdAndQBuyGoodsId(string qBuyCode, string qBuyGoodsId)
        {
            QBuyGoods qBuyGoods = new QBuyGoods();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(OrderSqls.SELECT_QBUYGOODS_BY_QBUY_CODE_AND_QBUY_GOODS_ID, qBuyCode, qBuyGoodsId);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count == 1)
            {
                QBuyGoods qbuyGoods = new QBuyGoods
                {
                    qBuyGoodsId = dt.Rows[0]["QBUY_GOODS_ID"].ToString(),
                    qBuyCode = dt.Rows[0]["QBUY_CODE"].ToString(),
                    goodsId = dt.Rows[0]["GOODS_ID"].ToString(),
                    goodsName = dt.Rows[0]["GOODS_NAME"].ToString(),
                    price = dt.Rows[0]["Q_PRICE"].ToString(),
                    num = dt.Rows[0]["Q_NUM"].ToString(),
                    slt = dt.Rows[0]["GOODS_IMG"].ToString(),
                };
            }

            return qBuyGoods;
        }

        public bool updateQBuy(string qBuyCode, string orderCode)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(OrderSqls.UPDATE_QBUYGOODS_STATE, qBuyCode, orderCode);
            string sql = builder.ToString();
            return DatabaseOperationWeb.ExecuteDML(sql);
        }

        private class OrderSqls
        {
            public const string SELECT_QBUY_LIST_BY_MEMBER_ID = ""
                + "SELECT BQ.* ,BA.REMARK ,S.STORE_NAME " +
                "FROM T_BUSS_QBUY BQ ,T_BUSS_ACTIVE BA,T_BASE_STORE S " +
                "WHERE BQ.ACTIVE_ID = BA.ACTIVE_ID " +
                "AND BQ.STORE_ID = S.STORE_ID " +
                "AND BQ.MEMBER_ID = '{0}' ";

            public const string SELECT_QBUYGOODS_BY_QBUY_CODE = ""
                + "SELECT G.GOODS_IMG, G.GOODS_NAME,QG.* " +
                "FROM T_BUSS_GOODS G ,T_BUSS_QBUY_GOODS QG " +
                "WHERE G.GOODS_ID = QG.GOODS_ID " +
                "AND QG.QBUY_CODE = '{0}'";

            public const string SELECT_QBUYGOODS_BY_QBUY_CODE_AND_QBUY_GOODS_ID = ""
                + "SELECT G.GOODS_IMG, G.GOODS_NAME,QG.* "
                + "FROM T_BUSS_GOODS G ,T_BUSS_QBUY_GOODS QG "
                + "WHERE G.GOODS_ID = QG.GOODS_ID "
                + "AND G.STATE = 0 "
                + "AND QG.QBUY_CODE = '{0}' AND QG.QBUY_GOODS_ID = {1}";

            public const string UPDATE_QBUYGOODS_STATE = ""
                + "UPDATE T_BUSS_GOODS SET STATE = 1,ORDER_CODE = '{1}' WHERE QBUY_CODE = '{0}' ";
        }
    }
}
