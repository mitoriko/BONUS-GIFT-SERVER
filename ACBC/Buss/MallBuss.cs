using ACBC.Common;
using ACBC.Dao;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ACBC.Buss
{
    public class MallBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.MallApi;
        }

        public object Do_GetHome(BaseApi baseApi)
        {
            Home home = Utils.GetCache<Home>();
            if(home == null)
            {
                MallDao mallDao = new MallDao();
                HomeInfo homeInfo = mallDao.GetHome();
                if (homeInfo == null)
                {
                    throw new ApiException(CodeMessage.HomeInitError, "HomeInitError");
                }
                List<HomeList> list = mallDao.GetHomeList(homeInfo.homeId);
                home = new Home();
                home.homeInfo = homeInfo;
                home.list = list;

                Utils.SetCache(home);
            }
            
            return home;
        }

        public object Do_GetShowDay(BaseApi baseApi)
        {
            ShowDayList list = Utils.GetCache<ShowDayList>();
            if(list == null)
            {
                MallDao mallDao = new MallDao();
                list = mallDao.GetShowDay();
                Utils.SetCache(list);
            }
            return list;
        }

        public object Do_GetShowDayGoodsList(BaseApi baseApi)
        {
            GetShowDayGoodsListParam getShowDayGoodsListParam = JsonConvert.DeserializeObject<GetShowDayGoodsListParam>(baseApi.param.ToString());
            if (getShowDayGoodsListParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            ShowDayGoodsList list = Utils.GetCache<ShowDayGoodsList>(getShowDayGoodsListParam);
            if (list == null)
            {
                MallDao mallDao = new MallDao();
                list = mallDao.GetShowDayGoodsList(getShowDayGoodsListParam.showId);
                list.Unique = getShowDayGoodsListParam.GetUnique();
                Utils.SetCache(list);
            }
            return list;
        }
    }
}
