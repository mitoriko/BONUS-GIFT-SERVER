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
    public class HomePageDao
    {
        public HomePage getHomePage()
        {
            HomePage homePage = new HomePage();
            List<HomePageData> dataList = GetHomePageTopBanner();
            foreach (HomePageData homePageData in dataList)
            {
                if (homePageData.pageType == "1")
                {
                    HomePageBanner homePageBanner = new HomePageBanner
                    {
                        bannerImg = homePageData.pageImg,
                        bannerUrl = homePageData.pageUrl,
                        urlType = homePageData.urlType
                    };
                    homePage.bannerList.Add(homePageBanner);
                }
                else if (homePageData.pageType == "2")
                {
                    HomePageMenu homePageMenu = new HomePageMenu
                    {
                        menuName = homePageData.pageName,
                        menuImg = homePageData.pageImg,
                        menuUrl = homePageData.pageUrl
                    };
                    homePage.menuList.Add(homePageMenu);
                }
                else if (homePageData.pageType == "3")
                {
                    HomePageBanner homePageBanner = new HomePageBanner
                    {
                        bannerImg = homePageData.pageImg,
                        bannerUrl = homePageData.pageUrl,
                        urlType = homePageData.urlType
                    };
                    homePage.banner = homePageBanner;
                }
                else if (homePageData.pageType == "4")
                {
                    HomePageBanner homePageBanner = new HomePageBanner
                    {
                        bannerImg = homePageData.pageImg,
                        bannerUrl = homePageData.pageUrl,
                        urlType = homePageData.urlType
                    };
                    homePage.advert = homePageBanner;
                }
            }
            homePage.goodsList = GetHomePageGoods();
            return homePage;
        }

        private List<HomePageData> GetHomePageTopBanner()
        {
            List<HomePageData> list = new List<HomePageData>();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(OrderSqls.SELECT_HOMEPAGE);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    HomePageData homePageData = new HomePageData
                    {
                        pageId = dr["page_id"].ToString(),
                        pageType = dr["page_type"].ToString(),
                        pageName = dr["page_name"].ToString(),
                        pageImg = dr["page_img"].ToString(),
                        pageUrl = dr["page_url"].ToString(),
                        urlType = dr["url_type"].ToString(),
                        sort = dr["sort"].ToString(),
                        remark = dr["remark"].ToString(),
                        flag = dr["flag"].ToString(),
                    };
                    list.Add(homePageData);
                }
            }

            return list;
        }
        private List<HomePageGoods> GetHomePageGoods()
        {
            List<HomePageGoods> list = new List<HomePageGoods>();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(OrderSqls.SELECT_HOME_GOODSLIST);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    HomePageGoods homePageGoods = new HomePageGoods
                    {
                        goodsId = dr["url_value"].ToString(),
                        goodsName = dr["title"].ToString(),
                        goodsPrice = dr["price"].ToString(),
                        goodsImg = dr["img"].ToString()
                    };
                    list.Add(homePageGoods);
                }
            }

            return list;
        }
        public List<Explain> getExplainList()
        {
            List<Explain> list = new List<Explain>();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(OrderSqls.SELECT_EXPLAINLIST);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Explain explain = new Explain
                    {
                        explainUrl = dr["page_url"].ToString(),
                        explainImg = dr["page_img"].ToString()
                    };
                    list.Add(explain);
                }
            }

            return list;
        }
        public List<HomePageData> getActiveList()
        {
            List<HomePageData> list = new List<HomePageData>();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(OrderSqls.SELECT_ACTIVELIST);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    HomePageData homePageData = new HomePageData
                    {
                        pageId = dr["page_id"].ToString(),
                        pageType = dr["page_type"].ToString(),
                        pageName = dr["page_name"].ToString(),
                        pageImg = dr["page_img"].ToString(),
                        pageUrl = dr["page_url"].ToString(),
                        urlType = dr["url_type"].ToString(),
                        sort = dr["sort"].ToString(),
                        remark = dr["remark"].ToString(),
                        flag = dr["flag"].ToString(),
                    };
                    list.Add(homePageData);
                }
            }

            return list;
        }
        public Category getCategoryList()
        {
            Category category = new Category();
            category.parentList = new List<string>();
            category.categoryList = new Dictionary<string, List<CategoryData>>();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(OrderSqls.SELECT_CATEGORYLIST);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                string parentName = "";
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["GOODSCOUNT"].ToString()!="0")
                    {
                        if (dr["parentName"].ToString() != parentName)
                        {
                            parentName = dr["parentName"].ToString();
                            category.parentList.Add(parentName);
                            category.categoryList.Add(parentName, new List<CategoryData>());
                        }

                        CategoryData categoryData = new CategoryData
                        {
                            categoryId = dr["id"].ToString(),
                            categoryName = dr["name"].ToString(),
                            categoryImg = dr["thumb"].ToString(),
                        };
                        category.categoryList[parentName].Add(categoryData);
                    }
                    
                }
            }

            return category;
        }
        public ShowDayGoodsList GetShowDayGoodsList(string showId)
        {
            ShowDayGoodsList showDayGoodsList = new ShowDayGoodsList();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(OrderSqls.SELECT_CATEGORY_GOODS_BY_SHOW_ID, showId);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null)
            {
                showDayGoodsList.list = new List<ShowDayGoods>();
                foreach (DataRow dr in dt.Rows)
                {
                    string goodsTitle = dr["GOODS_NAME"].ToString();
                    if (goodsTitle.Length>22)
                    {
                        goodsTitle = dr["GOODS_NAME"].ToString().Substring(0, 20) + "...";
                    }
                    ShowDayGoods showDayGoods = new ShowDayGoods
                    {
                        goodsId = dr["GOODS_ID"].ToString(),
                        goodsImg = dr["GOODS_IMG"].ToString(),
                        goodsTitle = goodsTitle,
                        goodsPrice = dr["GOODS_PRICE"].ToString(),
                    };
                    showDayGoodsList.list.Add(showDayGoods);
                }
            }

            return showDayGoodsList;
        }


        public List<ActiveCheckInfo> getActiveCheckListByActiveType()
        {
            List<ActiveCheckInfo> list = new List<ActiveCheckInfo>();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(OrderSqls.SELECT_ACTIVECHECKLIST_BY_ACTIVETYPE);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string activeState = dr["active_state"].ToString();
                    string activeType = dr["active_type"].ToString();
                    string valueType = dr["value_type"].ToString();
                    if (activeState == "-1")
                    {
                        activeState = "已结束";
                    }
                    else if (activeState == "1")
                    {
                        activeState = "进行中";
                    }
                    if (valueType == "0")
                    {
                        valueType = "赠送礼品";
                    }
                    else if (valueType == "1")
                    {
                        valueType = "赠送心值";
                    }
                    else if (valueType == "2")
                    {
                        valueType = "提高兑换上限";
                    }
                    string activeImg = dr["active_img"].ToString();
                    if (activeImg == "")
                    {
                        activeImg = "http://bonus-gift-server.oss-cn-beijing.aliyuncs.com/imgs/cardImg.jpg";
                    }
                    ActiveCheckInfo activeCheckInfo = new ActiveCheckInfo
                    {
                        activeId = dr["active_id"].ToString(),
                        activeStore = dr["active_store"].ToString(),
                        remark = dr["remark"].ToString(),
                        activeState = activeState,
                        activeType = "到店签到",
                        activeTimeFrom = dr["active_time_from"].ToString().Substring(0, 10),
                        activeTimeTo = dr["active_time_to"].ToString().Substring(0, 10),
                        activeCheckId = dr["active_check_id"].ToString(),
                        itemNums = dr["item_nums"].ToString(),
                        itemValue = dr["item_value"].ToString(),
                        valueType = valueType,
                        storeName = dr["store_name"].ToString(),
                        activeImg = activeImg,
                    };
                    list.Add(activeCheckInfo);
                }
            }

            return list;
        }


        public List<ActiveConsumeInfo> getActiveConsumeListByActiveType()
        {
            List<ActiveConsumeInfo> list = new List<ActiveConsumeInfo>();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(OrderSqls.SELECT_ACTIVECONSUMELIST_BY_ACTIVETYPE);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string activeState = dr["active_state"].ToString();
                    string activeType = dr["active_type"].ToString();
                    string valueType = dr["value_type"].ToString();
                    if (activeState == "-1")
                    {
                        activeState = "已结束";
                    }
                    else if (activeState == "1")
                    {
                        activeState = "进行中";
                    }
                    if (valueType == "0")
                    {
                        valueType = "赠送礼品";
                    }
                    else if (valueType == "1")
                    {
                        valueType = "赠送心值";
                    }
                    else if (valueType == "2")
                    {
                        valueType = "提高兑换上限";
                    }
                    string activeImg = dr["active_img"].ToString();
                    if (activeImg == "")
                    {
                        activeImg = "http://bonus-gift-server.oss-cn-beijing.aliyuncs.com/imgs/cardImg.jpg";
                    }
                    ActiveConsumeInfo activeConsumeInfo = new ActiveConsumeInfo
                    {
                        activeId = dr["active_id"].ToString(),
                        activeStore = dr["active_store"].ToString(),
                        remark = dr["remark"].ToString(),
                        activeState = activeState,
                        activeType = "到店签到",
                        activeTimeFrom = dr["active_time_from"].ToString().Substring(0, 10),
                        activeTimeTo = dr["active_time_to"].ToString().Substring(0, 10),
                        consume = dr["consume"].ToString(),
                        activeConsumeId = dr["active_consume_id"].ToString(),
                        itemNums = dr["item_nums"].ToString(),
                        itemValue = dr["item_value"].ToString(),
                        valueType = valueType,
                        storeName = dr["store_name"].ToString(),
                        activeImg = activeImg,
                    };
                    list.Add(activeConsumeInfo);
                }
            }

            return list;
        }

        public List<QBuyInfo> getQBuyList()
        {
            List<QBuyInfo> list = new List<QBuyInfo>();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(OrderSqls.SELECT_QBUYLIST_BY_STATE);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string activeState = dr["active_state"].ToString();
                    if (activeState == "-1")
                    {
                        activeState = "已结束";
                    }
                    else if (activeState == "1")
                    {
                        activeState = "进行中";
                    }
                    QBuyInfo qBuyInfo = new QBuyInfo
                    {
                        activeId = dr["active_id"].ToString(),
                        activeStore = dr["active_store"].ToString(),
                        storeName = dr["store_name"].ToString(),
                        remark = dr["remark"].ToString(),
                        activeState = activeState,
                        activeType = dr["active_type"].ToString(),
                        activeTimeFrom = dr["active_time_from"].ToString().Substring(0, 10),
                        activeTimeTo = dr["active_time_to"].ToString().Substring(0, 10),
                        activeImg = dr["active_img"].ToString(),
                        activeQBuyId = dr["active_qbuy_id"].ToString(),
                        beforeStart = dr["before_start"].ToString(),
                        lastDays = dr["last_days"].ToString(),
                        checkNum = dr["check_num"].ToString(),
                        consumeNum = dr["consume_num"].ToString(),
                        minConsume = dr["min_consume"].ToString(),
                    };
                    list.Add(qBuyInfo);
                }
            }

            return list;
        }
        public List<QBuyGoods> GetQbuyGoodsList(string activeQBuyId)
        {
            List<QBuyGoods> list = new List<QBuyGoods>();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(OrderSqls.SELECT_QBUYGOODSLIST_BY_ACTIVEQBUYID, activeQBuyId);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    bool ifUse = false;
                    if (dr["IF_USE"].ToString() == "1")
                    {
                        int stock = 0;
                        int.TryParse(dr["GOODS_STOCK"].ToString(), out stock);
                        if (stock > 0)
                        {
                            ifUse = true;
                        }
                    }

                    QBuyGoods qBuyGoods = new QBuyGoods
                    {
                        qBuyGoodsId = dr["ACTIVE_QBUY_GOODS_ID"].ToString(),
                        qBuyCode = dr["ACTIVE_QBUY_ID"].ToString(),
                        goodsId = dr["GOODS_ID"].ToString(),
                        goodsName = dr["GOODS_NAME"].ToString(),
                        price = dr["Q_PRICE"].ToString(),
                        num = dr["Q_NUM"].ToString(),
                        slt = dr["GOODS_IMG"].ToString(),
                        ifUse = ifUse,
                    };
                    list.Add(qBuyGoods);
                }
            }

            return list;
        }
        private class OrderSqls
        {
            public const string SELECT_HOMEPAGE = ""
                + "SELECT * " +
                "FROM T_BASE_HOME_PAGE " +
                "WHERE  flag = '1' " +
                "ORDER BY PAGE_TYPE, SORT ASC";
            public const string SELECT_HOME_GOODSLIST = " " +
                "SELECT * " +
                "FROM T_BASE_HOME_LIST " +
                "WHERE HOME_ID IN (SELECT MAX(HOME_ID) FROM T_BASE_HOME) " +
                "ORDER BY SORT";
            public const string SELECT_EXPLAINLIST = " " +
                "SELECT * " +
                "FROM T_BASE_HOME_PAGE " +
                "WHERE  PAGE_TYPE = '5' " +
                "ORDER BY SORT ASC";
            public const string SELECT_ACTIVELIST = " " +
                "SELECT * " +
                "FROM T_BASE_HOME_PAGE " +
                "WHERE  PAGE_TYPE = '6' " +
                "ORDER BY SORT ASC";
            public const string SELECT_CATEGORYLIST = " " +
                "SELECT T1.*,(SELECT NAME FROM T_BUSS_GOODS_CATEGORY T2 WHERE T2.ID =T1.PARENTID ) PARENTNAME ," +
                "(SELECT COUNT(*) FROM T_BUSS_GOODS T3 WHERE T3.CATEGORY2 = T1.ID AND T3.IF_USE = 1) GOODSCOUNT " +
                "FROM T_BUSS_GOODS_CATEGORY T1 " +
                "WHERE LEVEL = 2 AND FLAG ='1' " +
                "ORDER BY PARENTID,DISPLAYORDER";
            public const string SELECT_CATEGORY_GOODS_BY_SHOW_ID = ""
                + "SELECT * "
                + "FROM T_BUSS_GOODS "
                + "WHERE CATEGORY2 = {0} "
                + "AND IF_USE = 1";
            public const string SELECT_ACTIVECHECKLIST_BY_ACTIVETYPE = ""
                + "SELECT A.*,C.*,S.STORE_NAME " +
                "FROM T_BUSS_ACTIVE A,T_BUSS_ACTIVE_CHECK C ,T_BASE_STORE S " +
                "WHERE A.ACTIVE_ID = C.ACTIVE_ID " +
                "AND A.ACTIVE_STORE = S.STORE_ID " +
                "AND A.ACTIVE_TYPE='1' " +
                "AND (A.ACTIVE_STATE ='1' OR A.ACTIVE_STATE ='-1') " +
                "ORDER BY A.ACTIVE_STATE DESC, A.ACTIVE_ID DESC LIMIT 10";
            public const string SELECT_ACTIVECONSUMELIST_BY_ACTIVETYPE = ""
                + "SELECT A.*,C.*,S.STORE_NAME " +
                "FROM T_BUSS_ACTIVE A,T_BUSS_ACTIVE_CONSUME C ,T_BASE_STORE S  " +
                "WHERE A.ACTIVE_ID = C.ACTIVE_ID " +
                "AND A.ACTIVE_STORE = S.STORE_ID " +
                "AND A.ACTIVE_TYPE='0' " +
                "AND (A.ACTIVE_STATE ='1' OR A.ACTIVE_STATE ='-1') " +
                "ORDER BY A.ACTIVE_STATE DESC, A.ACTIVE_ID DESC LIMIT 10";
            public const string SELECT_QBUYLIST_BY_STATE = ""
                + "SELECT A.*,Q.*,S.STORE_NAME " +
                "FROM T_BUSS_ACTIVE A ,T_BUSS_ACTIVE_QBUY Q,T_BASE_STORE S " +
                "WHERE A.ACTIVE_ID = Q.ACTIVE_ID " +
                "AND A.ACTIVE_STORE = S.STORE_ID " +
                "AND A.ACTIVE_TYPE='2' " +
                "AND A.ACTIVE_STATE='1' " +
                "ORDER BY ACTIVE_TIME_TO DESC,A.ACTIVE_ID DESC";
            public const string SELECT_QBUYGOODSLIST_BY_ACTIVEQBUYID = "" +
                "SELECT G.GOODS_IMG, G.GOODS_NAME,QG.*,G.IF_USE,G.GOODS_STOCK  " +
                "FROM T_BUSS_GOODS G ,T_BUSS_ACTIVE_QBUY_GOODS QG  " +
                "WHERE G.GOODS_ID = QG.GOODS_ID  " +
                "AND QG.ACTIVE_QBUY_ID = '{0}'";

        }
    }
}
