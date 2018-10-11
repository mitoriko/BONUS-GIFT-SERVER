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
    public class OrderDao
    {
        public OrderList GetOrderList(string openId)
        {
            OrderList orderList = new OrderList();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(OrderSqls.SELECT_ORDER_GOODS_LIST_BY_OPEN_ID, openId);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null)
            {
                DataRow[] unPayListRows = dt.Select("STATE = 0", "ORDER_ID, ORDER_TIME DESC");
                DataRow[] payListRows = dt.Select("STATE = 1", "ORDER_ID, ORDER_TIME DESC");
                DataRow[] inShopListRows = dt.Select("STATE = 2", "ORDER_ID, ORDER_TIME DESC");
                DataRow[] doneListRows = dt.Select("STATE = 3", "ORDER_ID, ORDER_TIME DESC");

                string unPayListOrderId = "";
                string payListOrderId = "";
                string inShopListOrderId = "";
                string doneListOrderId = "";

                Order unPayOrder = new Order();
                Order payOrder = new Order();
                Order inShopOrder = new Order();
                Order doneOrder = new Order();

                foreach (DataRow dr in unPayListRows)
                {
                    string orderId = dr["ORDER_ID"].ToString();
                    if(orderId != unPayListOrderId && unPayListOrderId != "")
                    {
                        orderList.unPayList.Add(unPayOrder);
                        unPayOrder = new Order();
                    }
                    unPayListOrderId = orderId;
                    unPayOrder.orderId = orderId;
                    unPayOrder.orderCode = dr["ORDER_CODE"].ToString();
                    unPayOrder.orderTime = dr["ORDER_TIME"].ToString();
                    unPayOrder.state = dr["STATE"].ToString();
                    unPayOrder.total = dr["TOTAL"].ToString();
                    unPayOrder.list.Add(new OrderGoods
                    {
                        goodsId = dr["GOODS_ID"].ToString(),
                        goodsImg = dr["GOODS_IMG"].ToString(),
                        goodsName = dr["GOODS_NAME"].ToString(),
                        num = dr["NUM"].ToString(),
                        price = dr["PRICE"].ToString(),
                    });
                }
                orderList.unPayList.Add(unPayOrder);

                foreach (DataRow dr in payListRows)
                {
                    string orderId = dr["ORDER_ID"].ToString();
                    if (orderId != payListOrderId && payListOrderId != "")
                    {
                        orderList.payList.Add(payOrder);
                        payOrder = new Order();
                    }
                    payListOrderId = orderId;
                    payOrder.orderId = orderId;
                    payOrder.orderCode = dr["ORDER_CODE"].ToString();
                    payOrder.orderTime = dr["ORDER_TIME"].ToString();
                    payOrder.state = dr["STATE"].ToString();
                    payOrder.total = dr["TOTAL"].ToString();
                    payOrder.list.Add(new OrderGoods
                    {
                        goodsId = dr["GOODS_ID"].ToString(),
                        goodsImg = dr["GOODS_IMG"].ToString(),
                        goodsName = dr["GOODS_NAME"].ToString(),
                        num = dr["NUM"].ToString(),
                        price = dr["PRICE"].ToString(),
                    });
                }
                orderList.payList.Add(payOrder);

                foreach (DataRow dr in inShopListRows)
                {
                    string orderId = dr["ORDER_ID"].ToString();
                    if (orderId != inShopListOrderId && inShopListOrderId != "")
                    {
                        orderList.inShopList.Add(inShopOrder);
                        inShopOrder = new Order();
                    }
                    inShopListOrderId = orderId;
                    inShopOrder.orderId = orderId;
                    inShopOrder.orderCode = dr["ORDER_CODE"].ToString();
                    inShopOrder.orderTime = dr["ORDER_TIME"].ToString();
                    inShopOrder.state = dr["STATE"].ToString();
                    inShopOrder.total = dr["TOTAL"].ToString();
                    inShopOrder.list.Add(new OrderGoods
                    {
                        goodsId = dr["GOODS_ID"].ToString(),
                        goodsImg = dr["GOODS_IMG"].ToString(),
                        goodsName = dr["GOODS_NAME"].ToString(),
                        num = dr["NUM"].ToString(),
                        price = dr["PRICE"].ToString(),
                    });
                }
                orderList.inShopList.Add(inShopOrder);

                foreach (DataRow dr in doneListRows)
                {
                    string orderId = dr["ORDER_ID"].ToString();
                    if (orderId != doneListOrderId && doneListOrderId != "")
                    {
                        orderList.doneList.Add(doneOrder);
                        doneOrder = new Order();
                    }
                    doneListOrderId = orderId;
                    doneOrder.orderId = orderId;
                    doneOrder.orderCode = dr["ORDER_CODE"].ToString();
                    doneOrder.orderTime = dr["ORDER_TIME"].ToString();
                    doneOrder.state = dr["STATE"].ToString();
                    doneOrder.total = dr["TOTAL"].ToString();
                    doneOrder.list.Add(new OrderGoods
                    {
                        goodsId = dr["GOODS_ID"].ToString(),
                        goodsImg = dr["GOODS_IMG"].ToString(),
                        goodsName = dr["GOODS_NAME"].ToString(),
                        num = dr["NUM"].ToString(),
                        price = dr["PRICE"].ToString(),
                    });
                }
                orderList.doneList.Add(doneOrder);
            }

            return orderList;
        }

        public List<CartGoods> GetCartList(string memberId)
        {
            List<CartGoods> list = new List<CartGoods>();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(OrderSqls.SELECT_CART_LIST_BY_MEMBER_ID, memberId);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach(DataRow dr in dt.Rows)
                {
                    CartGoods cartGoods = new CartGoods
                    {
                        cartChecked = dr["CART_CHECKED"].ToString() == "0" ? false : true,
                        goodsId = dr["GOODS_ID"].ToString(),
                        goodsImg = dr["GOODS_IMG"].ToString(),
                        goodsName = dr["GOODS_NAME"].ToString(),
                        goodsNum = Convert.ToInt32(dr["GOODS_NUM"]),
                        goodsPrice = Convert.ToInt32(dr["GOODS_PRICE"]),
                    };
                    list.Add(cartGoods);
                }
            }

            return list;
        }

        private class OrderSqls
        {
            public const string SELECT_ORDER_GOODS_LIST_BY_OPEN_ID = ""
                + "SELECT * "
                + "FROM T_BUSS_ORDER_GOODS A, T_BUSS_ORDER B "
                + "WHERE A.ORDER_ID = B.ORDER_ID "
                + "AND B.OPENID = '{0}' ";
            public const string SELECT_CART_LIST_BY_MEMBER_ID = ""
                + "SELECT * "
                + "FROM T_BUSS_CART A,T_BUSS_GOODS B "
                + "WHERE A.GOODS_ID = B.GOODS_ID "
                + "AND MEMBER_ID = {0} "
                + "AND B.IF_USE = 1 "
                + "ORDER BY CART_TIME DESC";
        }
    }
}
