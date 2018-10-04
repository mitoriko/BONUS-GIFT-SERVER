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
    }
}
