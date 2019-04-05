using ACBC.Common;
using ACBC.Dao;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            string memberId = Utils.GetMemberID(baseApi.token);
            return orderDao.GetOrderList(memberId);
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
                if (cartGoods.cartChecked)
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

            if (goods.goodsStock < inputCartParam.goodsNum)
            {
                throw new ApiException(CodeMessage.NotEnoughGoods, "NotEnoughGoods");
            }

            CartGoods cartGoods = orderDao.GetCartGoodsByGoodsId(memberId, inputCartParam.goodsId);

            if (cartGoods == null)
            {
                if (!orderDao.InsertCart(memberId, inputCartParam.goodsId, inputCartParam.goodsNum))
                {
                    throw new ApiException(CodeMessage.UpdateCartError, "UpdateCartError");
                }
            }
            else
            {
                if (!orderDao.UpdateAddCart(cartGoods.cartId, inputCartParam.goodsNum))
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

            if (goods == null)
            {
                throw new ApiException(CodeMessage.InvalidGoods, "InvalidGoods");
            }

            if (goods.goodsStock < updateCartParam.goodsNum)
            {
                throw new ApiException(CodeMessage.NotEnoughGoods, "NotEnoughGoods");
            }

            if (!orderDao.UpdateCart(updateCartParam.cartId, updateCartParam.goodsNum))
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

        public object Do_PreOrder(BaseApi baseApi)
        {
            List<PreOrderParam> preOrderParamList = JsonConvert.DeserializeObject<List<PreOrderParam>>(baseApi.param.ToString());
            if (preOrderParamList == null || preOrderParamList.Count == 0)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            PreOrder preOrder = new PreOrder();
            OrderDao orderDao = new OrderDao();

            string memberId = Utils.GetMemberID(baseApi.token);
            Store store = orderDao.GetStoreByMemberId(memberId);
            if (store == null)
            {
                throw new ApiException(CodeMessage.BindStoreFirst, "BindStoreFirst");
            }
            preOrder.addr = store.storeAddr;

            string[] goodsIds = new string[preOrderParamList.Count];
            for (int i = 0; i < preOrderParamList.Count; i++)
            {
                goodsIds[i] = preOrderParamList[i].goodsId;
            }
            List<Goods> goodsList = orderDao.GetGoodsByGoodsIds(goodsIds);

            int total = 0;
            List<PreOrderGoods> list = new List<PreOrderGoods>();
            foreach (Goods goods in goodsList)
            {
                var preOrderParam = preOrderParamList.Find
                    (
                        item => item.goodsId.Equals(goods.goodsId)
                    );
                if(preOrderParam == null)
                {
                    throw new ApiException(CodeMessage.InvalidGoods, "InvalidGoods");
                }
                if (preOrderParam.goodsNum <= goods.goodsStock)
                {
                    total += goods.goodsPrice * preOrderParam.goodsNum;
                    PreOrderGoods preOrderGoods = new PreOrderGoods
                    {
                        cartId = preOrderParam.cartId,
                        goodsNum = preOrderParam.goodsNum,
                        goodsId = goods.goodsId,
                        goodsImg = goods.goodsImg,
                        goodsName = goods.goodsName,
                        goodsPrice = goods.goodsPrice,
                    };
                    list.Add(preOrderGoods);
                }
                else
                {
                    throw new ApiException(CodeMessage.NotEnoughGoods, "NotEnoughGoods");
                }
            }
            preOrder.list = list;
            preOrder.total = total;
            preOrder.storeCode = store.storeCode;
            preOrder.preOrderId = Guid.NewGuid().ToString();

            Utils.SetCache(preOrder.preOrderId, preOrder, 0, 5, 0);
            return preOrder;
        }

        public object Do_PayOrder(BaseApi baseApi)
        {
            PayOrderParam payOrderParam = JsonConvert.DeserializeObject<PayOrderParam>(baseApi.param.ToString());
            if (payOrderParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            PreOrder preOrder = Utils.GetCache<PreOrder>(payOrderParam.preOrderId);
            if (preOrder == null)
            {
                throw new ApiException(CodeMessage.InvalidPreOrderId, "InvalidPreOrderId");
            }
            string memberId = Utils.GetMemberID(baseApi.token);
            string orderCode = preOrder.storeCode + memberId.PadLeft(6, '0') + DateTime.Now.ToString("yyyyMMddHHmmss");

            OrderDao orderDao = new OrderDao();
            if(orderDao.InsertOrder(memberId, orderCode, preOrder, payOrderParam.remark))
            {
                Utils.DeleteCache(payOrderParam.preOrderId);
                Order order = orderDao.GetOrderInfoByCode(orderCode);
                if(order == null)
                {
                    throw new ApiException(CodeMessage.CreateOrderError, "CreateOrderError");
                }
                return order;
            }
            else
            {
                throw new ApiException(CodeMessage.CreateOrderError, "CreateOrderError");
            }

            
        }

        public object Do_GetOrderInfo(BaseApi baseApi)
        {
            GetOrderInfoParam getOrderInfoParam = JsonConvert.DeserializeObject<GetOrderInfoParam>(baseApi.param.ToString());
            if (getOrderInfoParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            OrderDao orderDao = new OrderDao();
            return orderDao.GetOrderInfo(getOrderInfoParam.orderId);
        }

        public object Do_PayForOrder(BaseApi baseApi)
        {
            PayForOrderParam payForOrderParam = JsonConvert.DeserializeObject<PayForOrderParam>(baseApi.param.ToString());
            if (payForOrderParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            OrderDao orderDao = new OrderDao();
            MemberDao memberDao = new MemberDao();
            string memberId = Utils.GetMemberID(baseApi.token);
            Order order = orderDao.GetOrderInfo(payForOrderParam.orderId);
            if (order.state != "0")
            {
                throw new ApiException(CodeMessage.InvalidOrderState, "InvalidOrderState");
            }
            int goodsNum = order.list.Count;
            string[] goodsIds = new string[goodsNum];
            for(int i = 0; i < goodsNum; i++)
            {
                goodsIds[i] = order.list[i].goodsId;
            }
            List<Goods> goodsList = orderDao.GetGoodsByGoodsIds(goodsIds);
            foreach(Goods goods in goodsList)
            {
                var orderGoods = order.list.Find
                    (
                        item => item.goodsId.Equals(goods.goodsId)
                    );
                if (orderGoods == null)
                {
                    throw new ApiException(CodeMessage.InvalidGoods, "InvalidGoods");
                }
                if (Convert.ToInt32(orderGoods.goodsNum) > goods.goodsStock)
                {
                    throw new ApiException(CodeMessage.NotEnoughGoods, "NotEnoughGoods");
                }
            }
            MemberInfo memberInfo = memberDao.GetMemberInfo(memberId);
            if(memberInfo.heart < Convert.ToInt32(order.total))
            {
                throw new ApiException(CodeMessage.NotEnoughHearts, "NotEnoughHearts");
            }

            if(!orderDao.PayForOrder(memberId, order, memberInfo.heart))
            {
                throw new ApiException(CodeMessage.PayForOrderError, "PayForOrderError");
            }

            return "";
        }

        public object Do_GetStoreGoodsCode(BaseApi baseApi)
        {
            GetStoreGoodsCodeParam getStoreGoodsCodeParam = JsonConvert.DeserializeObject<GetStoreGoodsCodeParam>(baseApi.param.ToString());
            if (getStoreGoodsCodeParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            OrderDao orderDao = new OrderDao();
            Order order = orderDao.GetOrderInfo(getStoreGoodsCodeParam.orderId);
            if(order == null)
            {
                throw new ApiException(CodeMessage.InvalidOrderState, "InvalidOrderState");
            }
            if(order.state != "2")
            {
                throw new ApiException(CodeMessage.InvalidOrderState, "InvalidOrderState");
            }
            string code = "ORDER_" + Guid.NewGuid().ToString();
            ScanOrderCodeParam scanOrderCodeParam = new ScanOrderCodeParam
            {
                code = code,
            };

            StoreGoodsCode storeGoodsCode = new StoreGoodsCode
            {
                code = code,
                order = order,
                Unique = scanOrderCodeParam.GetUnique(),
            };

            Utils.SetCache(storeGoodsCode, 1, 0, 10);

            return code;
        }
    }
}
