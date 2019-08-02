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

                    //if (startTime.Length > 10)
                    //{
                    //    startTime = startTime.Substring(0, 10);
                    //}
                    //if (endTime.Length > 10)
                    //{
                    //    endTime = endTime.Substring(0, 10);
                    //}
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

        public List<QbuyGoods> GetQbuyGoodsListByQbuyId(string qbuyCode)
        {
            List<QbuyGoods> list = new List<QbuyGoods>();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(OrderSqls.SELECT_QBUYGOODS_BY_QBUY_CODE, qbuyCode);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    QbuyGoods qbuyGoods = new QbuyGoods
                    {
                        qbuyGoodsId= dr["QBUY_GOODS_ID"].ToString(),
                        qbuyCode = dr["QBUY_CODE"].ToString(),
                        goodsId = dr["GOODS_ID"].ToString(),
                        goodsName = dr["GOODS_NAME"].ToString(),
                        price = dr["Q_PRICE"].ToString(),
                        num = dr["Q_NUM"].ToString(),
                        slt = dr["GOODS_IMG"].ToString(),
                    };
                    list.Add(qbuyGoods);
                }
            }

            return list;
        }

        private class OrderSqls
        {
            public const string SELECT_QBUY_LIST_BY_MEMBER_ID = ""
                + "SELECT BQ.* ,BA.REMARK ,S.STORE_NAME " +
                "FROM T_BUSS_QBUY BQ ,T_BUSS_ACTIVE BA,T_BASE_STORE S " +
                "WHERE BQ.ACTIVE_ID = BA.ACTIVE_ID " +
                "AND BQ.STORE_ID = S.STORE_ID " +
                "AND BQ.MEMBER_ID = '{0}' " +
                "AND STATE = '0'";

            public const string SELECT_QBUYGOODS_BY_QBUY_CODE = ""
                + "SELECT G.GOODS_IMG, G.GOODS_NAME,QG.* " +
                "FROM T_BUSS_GOODS G ,T_BUSS_QBUY_GOODS QG " +
                "WHERE G.GOODS_ID = QG.GOODS_ID " +
                "AND QG.QBUY_CODE = '{0}'";
        }
    }
}
