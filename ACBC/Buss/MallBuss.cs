using ACBC.Common;
using ACBC.Dao;
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
            MallDao mallDao = new MallDao();

            Home home = mallDao.GetHome();
            if(home == null)
            {
                throw new ApiException(CodeMessage.HomeInitError, "HomeInitError");
            }
            List<HomeList> list = mallDao.GetHomeList(home.homeId);

            return new { home, list };
        }

        public object Do_GetShowDay(BaseApi baseApi)
        {
            var list = Utils.GetCache<ShowDayList>(Global.ROUTE_PX + "/ShowDayList");
            if(list == null)
            {
                MallDao mallDao = new MallDao();
                list = mallDao.GetShowDay();
                Utils.SetCache(Global.ROUTE_PX + "/ShowDayList", list, 1, 0, 0);
            }
            return list;
        }
    }
}
