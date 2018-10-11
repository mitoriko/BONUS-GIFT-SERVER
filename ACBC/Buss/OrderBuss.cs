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

        public object Do_InputCart(BaseApi baseApi)
        {
            InputCartParam inputCartParam = JsonConvert.DeserializeObject<InputCartParam>(baseApi.param.ToString());
            if (inputCartParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            MallDao mallDao = new MallDao();
            OrderDao orderDao = new OrderDao();
            string memberId = Utils.GetMemberID(baseApi.token);
            Goods goods = mallDao.GetGoodsByGoodsId(inputCartParam.goodsId);

            if (goods == null)
            {
                throw new ApiException(CodeMessage.InvalidGoods, "InvalidGoods");
            }

            if (goods.goodsStock <= inputCartParam.goodsNum)
            {
                throw new ApiException(CodeMessage.NotEnoughGoods, "NotEnoughGoods");
            }

            CartGoods cartGoods = orderDao.GetCartGoodsByGoodsId(memberId, inputCartParam.goodsId);

            if(cartGoods == null)
            {
                if (!orderDao.InsertCart(memberId, inputCartParam.goodsId, inputCartParam.goodsNum))
                {
                    throw new ApiException(CodeMessage.UpdateCartError, "UpdateCartError");
                }
            }
            else
            {
                if (!orderDao.UpdateCart(cartGoods.cartId, inputCartParam.goodsNum))
                {
                    throw new ApiException(CodeMessage.UpdateCartError, "UpdateCartError");
                }
            }

            return "";
        }

        public object Do_UpdateCart(BaseApi baseApi)
        {
            UpdateCartParam updateCartParam = JsonConvert.DeserializeObject<UpdateCartParam>(baseApi.param.ToString());
            if (updateCartParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            OrderDao orderDao = new OrderDao();

            Goods goods = orderDao.GetGoodsByCartId(updateCartParam.cartId);

            if(goods == null)
            {
                throw new ApiException(CodeMessage.InvalidGoods, "InvalidGoods");
            }

            if(goods.goodsStock <= updateCartParam.goodsNum)
            {
                throw new ApiException(CodeMessage.NotEnoughGoods, "NotEnoughGoods");
            }

            if(!orderDao.UpdateCart(updateCartParam.cartId, updateCartParam.goodsNum))
            {
                throw new ApiException(CodeMessage.UpdateCartError, "UpdateCartError");
            }

            return "";
        }

        public object Do_DeleteCart(BaseApi baseApi)
        {
            DeleteCartParam deleteCartParam = JsonConvert.DeserializeObject<DeleteCartParam>(baseApi.param.ToString());
            if (deleteCartParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            OrderDao orderDao = new OrderDao();
            if (!orderDao.DeleteCart(deleteCartParam.cartId))
            {
                throw new ApiException(CodeMessage.UpdateCartError, "UpdateCartError");
            }
            return "";
        }
    }
}
