using ACBC.Buss;
using Com.ACBC.Framework.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ACBC.Dao
{
    public class MallDao
    {
        public HomeInfo GetHome()
        {
            HomeInfo homeInfo = null;
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(MallSqls.SELECT_HOME_BY_USE_THEME);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count == 1)
            {
                homeInfo = new HomeInfo
                {
                    homeId = dt.Rows[0]["HOME_ID"].ToString(),
                    backgroundImg = dt.Rows[0]["BACKGROUND_IMG"].ToString(),
                    titleMaster = dt.Rows[0]["TITLE_MASTER"].ToString(),
                    titleSlaver = dt.Rows[0]["TITLE_SLAVER"].ToString(),
                    titleTagImg = dt.Rows[0]["TITLE_TAG_IMG"].ToString(),
                };
            }

            return homeInfo;
        }

        public List<HomeList> GetHomeList(string homeId)
        {
            List<HomeList> list = new List<HomeList>();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(MallSqls.SELECT_HOME_LIST_BY_HOME_ID, homeId);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null)
            {
                foreach(DataRow dr in dt.Rows)
                {
                    HomeList homeList = new HomeList
                    {
                        img = dr["IMG"].ToString(),
                        title = dr["TITLE"].ToString(),
                        price = dr["PRICE"].ToString(),
                        praise = dr["PRAISE"].ToString(),
                        urlType = dr["URL_TYPE"].ToString(),
                        urlValue = dr["URL_VALUE"].ToString(),
                    };
                    list.Add(homeList);
                }
            }

            return list;
        }

        public ShowDayList GetShowDay()
        {
            List<int> monthList = new List<int>();
            Dictionary<int, List<ShowDay>> dayList = new Dictionary<int, List<ShowDay>>();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(MallSqls.SELECT_SHOW_DAY);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if(dt != null && dt.Rows.Count > 0)
            {
                int lastMonth = Convert.ToInt32(dt.Rows[0]["SHOW_MONTH"]);
                monthList.Add(lastMonth);
                List<ShowDay> curDayList = new List<ShowDay>();
                foreach (DataRow dr in dt.Rows)
                {
                    int curMonth = Convert.ToInt32(dr["SHOW_MONTH"]);
                    if (curMonth != lastMonth)
                    {
                        dayList.Add(lastMonth, curDayList);
                        curDayList = new List<ShowDay>();
                        lastMonth = curMonth;
                        monthList.Add(lastMonth);
                    }
                    DateTime dtm = new DateTime(
                        Convert.ToInt32(dr["SHOW_YEAR"]),
                        Convert.ToInt32(dr["SHOW_MONTH"]),
                        Convert.ToInt32(dr["SHOW_DAY"])
                        );
                    ShowDay showDay = new ShowDay
                    {
                        showId = dr["SHOW_ID"].ToString(),
                        showImg = dr["SHOW_IMG"].ToString(),
                        showTitle = dr["SHOW_TITLE"].ToString(),
                        showDay = dr["SHOW_DAY"].ToString(),
                        showMonth = dtm.ToString("MMMM", new System.Globalization.CultureInfo("en-us")).Substring(0, 3) + "." + dtm.Year,
                    };
                    curDayList.Add(showDay);
                }
                dayList.Add(lastMonth, curDayList);
            }

            return new ShowDayList { monthList = monthList, dayList = dayList };
        }

        public ShowDayGoodsList GetShowDayGoodsList(string showId)
        {
            ShowDayGoodsList showDayGoodsList = new ShowDayGoodsList();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(MallSqls.SELECT_SHOW_DAY_GOODS_BY_SHOW_ID, showId);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null)
            {
                showDayGoodsList.list = new List<ShowDayGoods>();
                foreach (DataRow dr in dt.Rows)
                {
                    ShowDayGoods showDayGoods = new ShowDayGoods
                    {
                        goodsId = dr["GOODS_ID"].ToString(),
                        goodsImg = dr["GOODS_IMG"].ToString(),
                        goodsTitle = dr["GOODS_TITLE"].ToString(),
                    };
                    showDayGoodsList.list.Add(showDayGoods);
                }
            }

            return showDayGoodsList;
        }

        public Goods GetGoodsByGoodsId(string goodsId)
        {
            Goods goods = null;
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(MallSqls.SELECT_GOODS_BY_GOODS_ID, goodsId);
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

                builder.Clear();
                builder.AppendFormat(MallSqls.SELECT_GOODS_SELL_SUM_BY_GOODS_ID, goodsId);
                sql = builder.ToString();
                DataTable dtSum = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
                if (dtSum != null && dtSum.Rows.Count == 1)
                {
                    goods.sales = Convert.ToInt32(dtSum.Rows[0][0]);
                }

                DataRow[] drMain = dt.Select("IMG_TYPE = 0", "IMG_SORT");
                DataRow[] drInfo = dt.Select("IMG_TYPE = 1", "IMG_SORT");

                foreach(DataRow dr in drMain)
                {
                    goods.mainImgs.Add(new GoodsImg
                    {
                        img = dr["IMG"].ToString(),
                        imgSort = Convert.ToInt32(dr["IMG_SORT"]),
                    });
                }
                foreach (DataRow dr in drInfo)
                {
                    goods.infoImgs.Add(new GoodsImg
                    {
                        img = dr["IMG"].ToString(),
                        imgSort = Convert.ToInt32(dr["IMG_SORT"]),
                    });
                }
            }
            return goods;
        }

        public List<Store> GetStoreList()
        {
            List<Store> list = new List<Store>();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(MallSqls.SELECT_STORE);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Store store = new Store
                    {
                        storeAddr = dr["STORE_ADDR"].ToString(),
                        storeId = dr["STORE_ID"].ToString(),
                        storeCardImg = dr["STORE_CARD_IMG"].ToString(),
                        storeCode = dr["STORE_CODE"].ToString(),
                        storeDesc = dr["STORE_DESC"].ToString(),
                        storeImg = dr["STORE_IMG"].ToString(),
                        storeName = dr["STORE_NAME"].ToString(),
                        storeTel = dr["STORE_TEL"].ToString(),
                        storeRate = Convert.ToInt32(dr["STORE_RATE"]),
                    };
                    list.Add(store);
                }
            }
            return list;
        }

        public StoreInfo GetStoreInfo(string storeId)
        {
            StoreInfo storeInfo = null;
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(MallSqls.SELECT_STORE_BY_STORE_ID, storeId);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count == 1)
            {
                storeInfo = new StoreInfo
                {
                    storeAddr = dt.Rows[0]["STORE_ADDR"].ToString(),
                    storeId = dt.Rows[0]["STORE_ID"].ToString(),
                    storeCardImg = dt.Rows[0]["STORE_CARD_IMG"].ToString(),
                    storeCode = dt.Rows[0]["STORE_CODE"].ToString(),
                    storeDesc = dt.Rows[0]["STORE_DESC"].ToString(),
                    storeImg = dt.Rows[0]["STORE_IMG"].ToString(),
                    storeName = dt.Rows[0]["STORE_NAME"].ToString(),
                    storeTel = dt.Rows[0]["STORE_TEL"].ToString(),
                    storeRate = Convert.ToInt32(dt.Rows[0]["STORE_RATE"]),
                };
                builder.Clear();
                builder.AppendFormat(MallSqls.SELECT_STORE_IMG_BY_STORE_ID, storeId);
                sql = builder.ToString();
                dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    StoreImg storeImg = new StoreImg
                    {
                        storeId = dr["STORE_ID"].ToString(),
                        storeImg = dr["STORE_IMG"].ToString(),
                        storeImgId = dr["STORE_IMG_ID"].ToString(),
                    };

                    storeInfo.storeImgs.Add(storeImg);
                }
                builder.Clear();
                builder.AppendFormat(MallSqls.SELECT_STORE_GOODS_BY_STORE_ID, storeId);
                sql = builder.ToString();
                dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    StoreGoods storeGoods = new StoreGoods
                    {
                        storeId = dr["STORE_ID"].ToString(),
                        storeGoodsDesc = dr["STORE_GOODS_DESC"].ToString(),
                        storeGoodsId = dr["STORE_GOODS_ID"].ToString(),
                        storeGoodsImg = dr["STORE_GOODS_IMG"].ToString(),
                        storeGoodsName = dr["STORE_GOODS_NAME"].ToString(),
                    };

                    storeInfo.storeGoods.Add(storeGoods);
                }
            }
            return storeInfo;
        }

        private class MallSqls
        {
            public const string SELECT_HOME_BY_USE_THEME = ""
                + "SELECT * "
                + "FROM T_BASE_HOME A, T_BASE_THEME B "
                + "WHERE A.THEME_ID = B.THEME_ID "
                + "AND USE_DATE = (SELECT MAX(USE_DATE) FROM T_BASE_THEME) ";
            public const string SELECT_HOME_LIST_BY_HOME_ID = ""
                + "SELECT * "
                + "FROM T_BASE_HOME_LIST "
                + "WHERE HOME_ID = {0} "
                + "ORDER BY SORT DESC";
            public const string SELECT_SHOW_DAY = ""
                + "SELECT * "
                + "FROM T_BUSS_SHOW_DAY T "
                + "WHERE DATE_ADD(NOW(), INTERVAL -12 MONTH) < T.SHOW_DATE "
                + "AND NOW() > T.SHOW_DATE "
                + "AND T.IF_USE = 1 "
                + "ORDER BY T.SHOW_YEAR DESC, T.SHOW_MONTH DESC, T.SHOW_DAY DESC";
            public const string SELECT_SHOW_DAY_GOODS_BY_SHOW_ID = ""
                + "SELECT * "
                + "FROM T_BUSS_SHOW_DAY_GOODS "
                + "WHERE SHOW_ID = {0}";
            public const string SELECT_GOODS_BY_GOODS_ID = ""
                + "SELECT * "
                + "FROM T_BUSS_GOODS A,T_BUSS_GOODS_IMGS B "
                + "WHERE A.GOODS_ID = B.GOODS_ID "
                + "AND A.GOODS_ID = {0}";
            public const string SELECT_GOODS_SELL_SUM_BY_GOODS_ID = ""
                + "SELECT SUM(NUM) "
                + "FROM T_BUSS_ORDER_GOODS A "
                + "WHERE A.GOODS_ID = {0}";
            public const string SELECT_STORE = ""
                + "SELECT * "
                + "FROM T_BASE_STORE A ";
            public const string SELECT_STORE_BY_STORE_ID = ""
                + "SELECT * "
                + "FROM T_BASE_STORE A "
                + "WHERE STORE_ID = {0} ";
            public const string SELECT_STORE_IMG_BY_STORE_ID = ""
                + "SELECT * "
                + "FROM T_BASE_STORE_IMG A "
                + "WHERE STORE_ID = {0}";
            public const string SELECT_STORE_GOODS_BY_STORE_ID = ""
                + "SELECT * "
                + "FROM T_BUSS_STORE_GOODS A "
                + "WHERE STORE_ID = {0}";
        }
    }
}
