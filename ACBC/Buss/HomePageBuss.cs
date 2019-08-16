                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      using ACBC.Common;
using ACBC.Dao;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ACBC.Buss
{
    public class HomePageBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.HomePageApi;
        }

        public object Do_GetHomePage(BaseApi baseApi)
        {
            HomePageDao homePageDao = new HomePageDao();
            string memberId = Utils.GetMemberID(baseApi.token);
            var list = homePageDao.getHomePage();
            return list;
        }
        public object Do_GetExplainList(BaseApi baseApi)
        {
            HomePageDao homePageDao = new HomePageDao();
            var list = homePageDao.getExplainList();
            return list;
        }
        public object Do_GetActiveList(BaseApi baseApi)
        {
            HomePageDao homePageDao = new HomePageDao();
            var list = homePageDao.getActiveList();
            return list;
        }
        public object Do_GetCategoryList(BaseApi baseApi)
        {
            HomePageDao homePageDao = new HomePageDao();
            var list = homePageDao.getCategoryList();
            return list;
        }
        public object Do_GetCategoryGoodsList(BaseApi baseApi)
        {
            GetShowDayGoodsListParam getShowDayGoodsListParam = JsonConvert.DeserializeObject<GetShowDayGoodsListParam>(baseApi.param.ToString());
            if (getShowDayGoodsListParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            ShowDayGoodsList list = Utils.GetCache<ShowDayGoodsList>(getShowDayGoodsListParam);
            if (list == null)
            {
                HomePageDao homePageDao = new HomePageDao();
                list = homePageDao.GetShowDayGoodsList(getShowDayGoodsListParam.showId);
                list.Unique = getShowDayGoodsListParam.GetUnique();
                Utils.SetCache(list);
            }
            return list;
        }

        public object Do_GetActiveCheck(BaseApi baseApi)
        {
            HomePageDao homePageDao = new HomePageDao();
            var list = homePageDao.getActiveCheckListByActiveType();
            return list;
        }

        public object Do_GetActiveConsume(BaseApi baseApi)
        {
            HomePageDao homePageDao = new HomePageDao();
            var list = homePageDao.getActiveConsumeListByActiveType();
            return list;
        }
        public object Do_GetQBuyList(BaseApi baseApi)
        {
            HomePageDao homePageDao = new HomePageDao();
            var list = homePageDao.getQBuyList();
            return list;
        }
        public object Do_GetQBuyGoodsList(BaseApi baseApi)
        {
            QbuyParam qbuyParam = JsonConvert.DeserializeObject<QbuyParam>(baseApi.param.ToString());
            if (qbuyParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            HomePageDao homePageDao = new HomePageDao();
            var list = homePageDao.GetQbuyGoodsList(qbuyParam.qbuyCode);
            return list;
        }
    }
}
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 