using ACBC.Common;
using ACBC.Dao;
using Newtonsoft.Json;
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

        public object Do_GetCart(BaseApi baseApi)
        {
            OrderDao orderDao = new OrderDao();
            string memberId = Utils.GetMemberID(baseApi.token);
            var list = orderDao.GetCartList(memberId);
            int totalNum = 0;
            int totalPrice = 0;
            foreach (CartGoods cartGoods in list)
            {
                if(cartGoods.cartChecked)
                {
                    totalPrice += cartGoods.goodsPrice * cartGoods.goodsNum;
                    totalNum += cartGoods.goodsNum;
                }
            }

            return new { totalPrice, totalNum, list };
        }
    }
}
