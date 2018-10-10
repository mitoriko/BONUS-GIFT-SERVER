﻿using ACBC.Buss;
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
                    ShowDay showDay = new ShowDay
                    {
                        showId = dr["SHOW_ID"].ToString(),
                        showImg = dr["SHOW_IMG"].ToString(),
                        showTitle = dr["SHOW_TITLE"].ToString(),
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
        }
    }
}