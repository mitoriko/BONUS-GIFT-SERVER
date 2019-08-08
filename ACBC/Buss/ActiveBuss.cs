using ACBC.Common;
using ACBC.Dao;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ACBC.Buss
{
    public class ActiveBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.ActiveApi;
        }

        public object Do_GetQbuyList(BaseApi baseApi)
        {
            ActiveDao activeDao = new ActiveDao();
            string memberId = Utils.GetMemberID(baseApi.token);
            var list = activeDao.GetQbuyList(memberId);
            return list;
        }

        public object Do_GetQbuyGoodsList(BaseApi baseApi)
        {
            QbuyParam qbuyParam = JsonConvert.DeserializeObject<QbuyParam>(baseApi.param.ToString());
            if (qbuyParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            ActiveDao activeDao = new ActiveDao();
            var list = activeDao.GetQbuyGoodsListByQbuyId(qbuyParam.qbuyCode);
            return list;
        }

        public object Do_StartQBuyGoods(BaseApi baseApi)
        {
            StartQBuyGoodsParam startQBuyGoodsParam = JsonConvert.DeserializeObject<StartQBuyGoodsParam>(baseApi.param.ToString());
            if (startQBuyGoodsParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            ActiveDao activeDao = new ActiveDao();
            var qBuyGoods = activeDao.GetQbuyGoodsByQBuyIdAndQBuyGoodsId(startQBuyGoodsParam.qBuyCode, startQBuyGoodsParam.qBuyGoodsId);

            PreOrder preOrder = new PreOrder();
            OrderDao orderDao = new OrderDao();
            MemberDao memberDao = new MemberDao();
            string memberId = Utils.GetMemberID(baseApi.token);
            Store store = orderDao.GetStoreByMemberId(memberId);
            if (store == null)
            {
                throw new ApiException(CodeMessage.BindStoreFirst, "BindStoreFirst");
            }
            preOrder.addr = store.storeAddr;

            string[] goodsIds = new string[1];
            goodsIds[0] = startQBuyGoodsParam.qBuyGoodsId;

            List<Goods> goodsList = orderDao.GetGoodsByGoodsIds(goodsIds);

            int total = 0;
            List<PreOrderGoods> list = new List<PreOrderGoods>();
            foreach (Goods goods in goodsList)
            {
                if(qBuyGoods.goodsId != goods.goodsId)
                {
                    throw new ApiException(CodeMessage.InvalidGoods, "InvalidGoods");
                }
                if (Convert.ToInt32(qBuyGoods.num) < 0)
                {
                    throw new ApiException(CodeMessage.ErrorNum, "ErrorNum");
                }
                if (Convert.ToInt32(qBuyGoods.num) <= goods.goodsStock)
                {
                    total += Convert.ToInt32(qBuyGoods.price) * Convert.ToInt32(qBuyGoods.num);
                    PreOrderGoods preOrderGoods = new PreOrderGoods
                    {
                        goodsNum = Convert.ToInt32(qBuyGoods.num),
                        goodsId = goods.goodsId,
                        goodsImg = goods.goodsImg,
                        goodsName = goods.goodsName,
                        goodsPrice = Convert.ToInt32(qBuyGoods.price),
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

            MemberInfo memberInfo = memberDao.GetMemberInfo(memberId);
            if (memberInfo.heart < Convert.ToInt32(total))
            {
                throw new ApiException(CodeMessage.NotEnoughHearts, "NotEnoughHearts");
            }

            string orderCode = preOrder.storeCode + memberId.PadLeft(6, '0') + DateTime.Now.ToString("yyyyMMddHHmmss");
            if (!activeDao.updateQBuy(startQBuyGoodsParam.qBuyCode, orderCode))
            {
                throw new ApiException(CodeMessage.PayForOrderError, "PayForOrderError");
            }
            if (!orderDao.InsertOrder(memberId, orderCode, preOrder, startQBuyGoodsParam.qBuyCode, preOrder.addr, 0))
            {
                throw new ApiException(CodeMessage.CreateOrderError, "CreateOrderError");
            }
            Order order = orderDao.GetOrderInfoByCode(orderCode);
            if (!orderDao.PayForOrder(memberId, order, memberInfo.heart))
            {
                throw new ApiException(CodeMessage.PayForOrderError, "PayForOrderError");
            }
            return "";
        }
    }
}
