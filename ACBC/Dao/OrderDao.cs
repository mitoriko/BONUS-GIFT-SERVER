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
    public class OrderDao
    {
        public OrderList GetOrderList(string memberId)
        {
            OrderList orderList = new OrderList();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(OrderSqls.SELECT_ORDER_GOODS_LIST_BY_MEMBER_ID, memberId);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null)
            {
                DataRow[] unPayListRows = dt.Select("STATE = 0", "ORDER_ID, ORDER_TIME DESC");
                DataRow[] payListRows = dt.Select("STATE = 1", "ORDER_ID, ORDER_TIME DESC");
                DataRow[] inStoreListRows = dt.Select("STATE = 2", "ORDER_ID, ORDER_TIME DESC");
                DataRow[] doneListRows = dt.Select("STATE = 3", "ORDER_ID, ORDER_TIME DESC");

                string unPayListOrderId = "";
                string payListOrderId = "";
                string inStoreListOrderId = "";
                string doneListOrderId = "";

                Order unPayOrder = new Order();
                Order payOrder = new Order();
                Order inStoreOrder = new Order();
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

                foreach (DataRow dr in inStoreListRows)
                {
                    string orderId = dr["ORDER_ID"].ToString();
                    if (orderId != inStoreListOrderId && inStoreListOrderId != "")
                    {
                        orderList.inStoreList.Add(inStoreOrder);
                        inStoreOrder = new Order();
                    }
                    inStoreListOrderId = orderId;
                    inStoreOrder.orderId = orderId;
                    inStoreOrder.orderCode = dr["ORDER_CODE"].ToString();
                    inStoreOrder.orderTime = dr["ORDER_TIME"].ToString();
                    inStoreOrder.state = dr["STATE"].ToString();
                    inStoreOrder.total = dr["TOTAL"].ToString();
                    inStoreOrder.list.Add(new OrderGoods
                    {
                        goodsId = dr["GOODS_ID"].ToString(),
                        goodsImg = dr["GOODS_IMG"].ToString(),
                        goodsName = dr["GOODS_NAME"].ToString(),
                        num = dr["NUM"].ToString(),
                        price = dr["PRICE"].ToString(),
                    });
                }
                orderList.inStoreList.Add(inStoreOrder);

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
                        cartId = dr["CART_ID"].ToString(),
                        goodsStock = Convert.ToInt32(dr["GOODS_STOCK"]),
                    };
                    list.Add(cartGoods);
                }
            }

            return list;
        }

        public Goods GetGoodsByCartId(string cartId)
        {
            Goods goods = null;
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(OrderSqls.SELECT_GOODS_BY_CART_ID, cartId);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count == 1)
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

        public CartGoods GetCartGoodsByGoodsId(string memberId, string goodsId)
        {
            CartGoods cartGoods = null;
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(OrderSqls.SELECT_CART_BY_MEMBER_ID_AND_GOODS_ID, memberId, goodsId);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if(dt != null && dt.Rows.Count == 1)
            {
                cartGoods = new CartGoods
                {
                    cartChecked = dt.Rows[0]["CART_CHECKED"].ToString() == "0" ? false : true,
                    goodsId = dt.Rows[0]["GOODS_ID"].ToString(),
                    goodsNum = Convert.ToInt32(dt.Rows[0]["GOODS_NUM"]),
                    cartId = dt.Rows[0]["CART_ID"].ToString(),
                };
            }
            return cartGoods;
        }

        public bool InsertCart(string memberId, string goodsId, int goodsNum)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(OrderSqls.INSERT_CART, memberId, goodsId, goodsNum);
            string sql = builder.ToString();
            return DatabaseOperationWeb.ExecuteDML(sql);
        }

        public bool UpdateCart(string cartId, int goodsNum)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(OrderSqls.UPDATE_CART_BY_CART_ID, goodsNum, cartId);
            string sql = builder.ToString();
            return DatabaseOperationWeb.ExecuteDML(sql);
        }

        public bool DeleteCart(string cartId)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(OrderSqls.DELETE_CART_BY_CART_ID, cartId);
            string sql = builder.ToString();
            return DatabaseOperationWeb.ExecuteDML(sql);
        }

        public List<Goods> GetGoodsByGoodsIds(string[] goodsIds)
        {
            List<Goods> list = new List<Goods>();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(OrderSqls.SELECT_GOODS_LIST_BY_GOODS_IDS, String.Join(",", goodsIds));
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null)
            {
                foreach(DataRow dr in dt.Rows)
                {
                    Goods goods = new Goods
                    {
                        goodsDesc = dr["GOODS_DESC"].ToString(),
                        goodsId = dr["GOODS_ID"].ToString(),
                        goodsName = dr["GOODS_NAME"].ToString(),
                        goodsImg = dr["GOODS_IMG"].ToString(),
                        goodsPrice = Convert.ToInt32(dr["GOODS_PRICE"]),
                        goodsStock = Convert.ToInt32(dr["GOODS_STOCK"]),
                    };

                    list.Add(goods);
                }
                
            }
            return list;
        }

        public Store GetStoreByMemberId(string memberId)
        {
            Store store = null;
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(OrderSqls.SELECT_STORE_BY_MEMBER_ID, memberId);
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
                };
            }
            return store;
        }

        public bool InsertOrder(string memberId, string orderCode, PreOrder preOrder)
        {
            ArrayList list = new ArrayList();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(OrderSqls.INSERT_ORDER, memberId, orderCode, preOrder.total, preOrder.storeCode);
            string orderSql = builder.ToString();
            list.Add(orderSql);
            foreach(PreOrderGoods preOrderGoods in preOrder.list)
            {
                builder.Clear();
                builder.AppendFormat(
                    OrderSqls.INSERT_ORDER_GOODS,
                    orderCode,
                    preOrderGoods.goodsId,
                    preOrderGoods.goodsImg,
                    preOrderGoods.goodsName,
                    preOrderGoods.goodsPrice,
                    preOrderGoods.num
                    );
                string orderGoodsSql = builder.ToString();
                list.Add(orderGoodsSql);

                if(preOrderGoods.cartId != null && preOrderGoods.cartId.Length > 0)
                {
                    builder.Clear();
                    builder.AppendFormat(OrderSqls.DELETE_CART_BY_CART_ID, preOrderGoods.cartId);
                    string deleteCartSql = builder.ToString();
                    list.Add(deleteCartSql);
                }
            }
            return DatabaseOperationWeb.ExecuteDML(list);
        }

        public Order GetOrderInfo(string orderId)
        {
            Order order = new Order();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(OrderSqls.SELECT_ORDER_GOODS_LIST_BY_ORDER_ID, orderId);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    order.orderId = orderId;
                    order.orderCode = dr["ORDER_CODE"].ToString();
                    order.orderTime = dr["ORDER_TIME"].ToString();
                    order.state = dr["STATE"].ToString();
                    order.total = dr["TOTAL"].ToString();
                    order.list.Add(new OrderGoods
                    {
                        goodsId = dr["GOODS_ID"].ToString(),
                        goodsImg = dr["GOODS_IMG"].ToString(),
                        goodsName = dr["GOODS_NAME"].ToString(),
                        num = dr["NUM"].ToString(),
                        price = dr["PRICE"].ToString(),
                    });
                }
            }

            return order;
        }

        public bool PayForOrder(string memberId, Order order)
        {
            ArrayList list = new ArrayList();
            StringBuilder builder = new StringBuilder();
            foreach(OrderGoods orderGoods in order.list)
            {
                builder.Clear();
                builder.AppendFormat(OrderSqls.UPDATE_GOODS_STOCK_BY_GOODS_ID, orderGoods.goodsId, orderGoods.num);
                list.Add(builder.ToString());
            }
            builder.Clear();
            builder.AppendFormat(OrderSqls.UPDATE_ORDER_STATE_BY_ORDER_ID, order.orderId);
            list.Add(builder.ToString());
            builder.Clear();
            builder.AppendFormat(OrderSqls.UPDATE_MEMBER_HEART_BY_MEMBER_ID, memberId, order.total);
            list.Add(builder.ToString());

            return DatabaseOperationWeb.ExecuteDML(list);
        }

        private class OrderSqls
        {
            public const string SELECT_ORDER_GOODS_LIST_BY_MEMBER_ID = ""
                + "SELECT * "
                + "FROM T_BUSS_ORDER_GOODS A, T_BUSS_ORDER B "
                + "WHERE A.ORDER_CODE = B.ORDER_CODE "
                + "AND B.MEMBER_ID = '{0}' ";
            public const string SELECT_CART_LIST_BY_MEMBER_ID = ""
                + "SELECT * "
                + "FROM T_BUSS_CART A,T_BUSS_GOODS B "
                + "WHERE A.GOODS_ID = B.GOODS_ID "
                + "AND MEMBER_ID = {0} "
                + "AND B.IF_USE = 1 "
                + "ORDER BY CART_TIME DESC";
            public const string SELECT_CART_BY_MEMBER_ID_AND_GOODS_ID = ""
                + "SELECT * "
                + "FROM T_BUSS_CART T "
                + "WHERE MEMBER_ID = {0} "
                + "AND GOODS_ID = {1}";
            public const string SELECT_GOODS_BY_CART_ID = ""
                + "SELECT * "
                + "FROM T_BUSS_CART T,T_BUSS_GOODS A "
                + "WHERE T.GOODS_ID = A.GOODS_ID "
                + "AND CART_ID = {0} ";
            public const string INSERT_CART = ""
                + "INSERT INTO T_BUSS_CART("
                + "MEMBER_ID,GOODS_ID,GOODS_NUM,CART_TIME) "
                + "VALUES({0},{1},{2},NOW())";
            public const string UPDATE_CART_BY_CART_ID = ""
                + "UPDATE T_BUSS_CART "
                + "SET GOODS_NUM = {0}, "
                + "CART_TIME = NOW() "
                + "WHERE CART_ID = {1}";
            public const string DELETE_CART_BY_CART_ID = ""
                + "DELETE FROM T_BUSS_CART "
                + "WHERE CART_ID = {0}";
            public const string SELECT_GOODS_LIST_BY_GOODS_IDS = ""
                + "SELECT * "
                + "FROM T_BUSS_GOODS T "
                + "WHERE T.GOODS_ID IN({0}) ";
            public const string SELECT_STORE_BY_MEMBER_ID = ""
                + "SELECT * "
                + "FROM T_BASE_STORE T, T_BUSS_MEMBER_STORE A "
                + "WHERE T.STORE_ID = A.STORE_ID " 
                + "AND A.MEMBER_ID = {0} "
                + "AND A.IS_DEFAULT = 1";
            public const string INSERT_ORDER = ""
                + "INSERT INTO T_BUSS_ORDER "
                + "(ORDER_CODE,TOTAL,STORE_CODE,MEMBER_ID) "
                + "VALUES('{1}',{2},'{3}',{0})";
            public const string INSERT_ORDER_GOODS = ""
                + "INSERT INTO T_BUSS_ORDER_GOODS "
                + "(GOODS_ID,GOODS_IMG,GOODS_NAME,PRICE,NUM,ORDER_CODE) "
                + "VALUES({1},'{2}','{3}',{4},{5},'{0}')";
            public const string SELECT_ORDER_GOODS_LIST_BY_ORDER_ID = ""
                + "SELECT * "
                + "FROM T_BUSS_ORDER_GOODS A, T_BUSS_ORDER B "
                + "WHERE A.ORDER_CODE = B.ORDER_CODE "
                + "AND B.ORDER_ID = {0} ";
            public const string UPDATE_GOODS_STOCK_BY_GOODS_ID = ""
                + "UPDATE T_BUSS_GOODS "
                + "SET GOODS_STOCK = GOODS_STOCK - {1} "
                + "WHERE GOODS_ID = {0} ";
            public const string UPDATE_ORDER_STATE_BY_ORDER_ID = ""
                + "UPDATE T_BUSS_ORDER "
                + "SET STATE = 1, PAY_TIME = NOW() "
                + "WHERE ORDER_ID = {0} ";
            public const string UPDATE_MEMBER_HEART_BY_MEMBER_ID = ""
                + "UPDATE T_BASE_MEMBER "
                + "SET HEART = HEART - {1} "
                + "WHERE MEMBER_ID = {0} ";
        }
    }
}
