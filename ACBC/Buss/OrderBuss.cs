using ACBC.Common;
using ACBC.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ACBC.Buss
{
    public class OrderBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.OrderApi;
        }

        public object Do_GetOrderList(BaseApi baseApi)
        {
            OrderDao orderDao = new OrderDao();
            string openId = Utils.GetOpenID(baseApi.token);
            return orderDao.GetOrderList(openId);
        }
    }
}
