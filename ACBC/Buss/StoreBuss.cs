using ACBC.Common;
using ACBC.Dao;
using Newtonsoft.Json;
using Senparc.Weixin.WxOpen.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ACBC.Buss
{
    public class StoreBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.StoreApi;
        }

        public object Do_GetAsnAndStock(BaseApi baseApi)
        {
            OpenDao openDao = new OpenDao();
            StoreUser storeUser = openDao.GetStoreUser(Utils.GetOpenID(baseApi.token));

            StoreDao storeDao = new StoreDao();
            List<StockGoods> listStock = storeDao.GetStockGoodsList(storeUser.storeId);
            List<AsnGoods> listAsn = storeDao.GetAsnGoodsList(storeUser.storeId);

            return new { listAsn, listStock };
        }

        public object Do_GetStoreAccount(BaseApi baseApi)
        {
            OpenDao openDao = new OpenDao();
            StoreUser storeUser = openDao.GetStoreUser(Utils.GetOpenID(baseApi.token));

            StoreDao storeDao = new StoreDao();
            return storeDao.GetStoreAccount(storeUser.storeId);
        }

        public object Do_ScanOrderCode(BaseApi baseApi)
        {
            ScanOrderCodeParam scanOrderCodeParam = JsonConvert.DeserializeObject<ScanOrderCodeParam>(baseApi.param.ToString());
            if (scanOrderCodeParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            StoreGoodsCode storeGoodsCode = Utils.GetCache<StoreGoodsCode>(scanOrderCodeParam);

            if(storeGoodsCode == null)
            {
                throw new ApiException(CodeMessage.InvalidOrderCode, "InvalidOrderCode");
            }
            StoreDao storeDao = new StoreDao();
            string storeId = storeDao.GetStoreId(storeGoodsCode.order.storeCode);
            OpenDao openDao = new OpenDao();
            StoreUser storeUser = openDao.GetStoreUser(Utils.GetOpenID(baseApi.token));
            if(storeUser.storeId != storeId)
            {
                throw new ApiException(CodeMessage.NotStoreUserOrder, "NotStoreUserOrder");
            }
            Utils.DeleteCache<StoreGoodsCode>(scanOrderCodeParam);
            return storeGoodsCode.order;
        }

        public object Do_PickupOrderGoods(BaseApi baseApi)
        {
            PickupOrderGoodsParam pickupOrderGoodsParam = JsonConvert.DeserializeObject<PickupOrderGoodsParam>(baseApi.param.ToString());
            if (pickupOrderGoodsParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            OpenDao openDao = new OpenDao();
            StoreUser storeUser = openDao.GetStoreUser(Utils.GetOpenID(baseApi.token));
            StoreDao storeDao = new StoreDao();
            if(!storeDao.UpdateOrderState(pickupOrderGoodsParam.orderId, storeUser.storeUserId))
            {
                throw new ApiException(CodeMessage.PickupGoodsError, "PickupGoodsError");
            }
            WsPayStateParam wsPayStateParam = new WsPayStateParam
            {
                scanCode = pickupOrderGoodsParam.code,
            };
            WsPayState wsPayState = new WsPayState
            {
                wsType = WsType.ORDER,
                Unique = wsPayStateParam.GetUnique(),
            };
            Utils.SetCache(wsPayState, 0, 0, 10);
            return "";
        }

        public object Do_Exchange(BaseApi baseApi)
        {
            ExchangeParam exchangeParam = JsonConvert.DeserializeObject<ExchangeParam>(baseApi.param.ToString());
            if (exchangeParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            ScanExchangeCodeParam scanExchangeCodeParam = new ScanExchangeCodeParam
            {
                code = exchangeParam.code
            };

            ExchangeCode exchangeCode = Utils.GetCache<ExchangeCode>(scanExchangeCodeParam);

            if (exchangeCode == null)
            {
                throw new ApiException(CodeMessage.InvalidExchangeCode, "InvalidExchangeCode");
            }

            OpenDao openDao = new OpenDao();
            StoreUser storeUser = openDao.GetStoreUser(Utils.GetOpenID(baseApi.token));

            StoreDao storeDao = new StoreDao();
            string phone = storeDao.CheckStoreMember(storeUser.storeId, exchangeCode.memberId);
            if (phone == "")
            {
                throw new ApiException(CodeMessage.NeedStoreMember, "NeedStoreMember");
            }

            if (!storeDao.InserRemoteCommit(storeUser.storeId, phone, exchangeParam.score))
            {
                throw new ApiException(CodeMessage.ExchangeError, "ExchangeError");
            }

            Utils.DeleteCache<ExchangeCode>(scanExchangeCodeParam);
            WsPayStateParam wsPayStateParam = new WsPayStateParam
            {
                scanCode = scanExchangeCodeParam.code,
            };
            WsPayState wsPayState = new WsPayState
            {
                wsType = WsType.EXCHANGE,
                Unique = wsPayStateParam.GetUnique(),
            };
            Utils.SetCache(wsPayState, 0, 0, 10);
            return "";
        }

        public object Do_CheckAsnGoods(BaseApi baseApi)
        {
            CheckAsnGoodsParam checkAsnGoodsParam = JsonConvert.DeserializeObject<CheckAsnGoodsParam>(baseApi.param.ToString());
            if (checkAsnGoodsParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            OpenDao openDao = new OpenDao();
            StoreUser storeUser = openDao.GetStoreUser(Utils.GetOpenID(baseApi.token));
            StoreDao storeDao = new StoreDao();
            if(!storeDao.CheckAsnGoods(storeUser.storeId, checkAsnGoodsParam.goodsId, storeUser.storeUserId))
            {
                throw new ApiException(CodeMessage.CheckAsnGoodsError, "CheckAsnGoodsError");
            }

            return "";
        }

        public object Do_MemberCheckStore(BaseApi baseApi)
        {
            MemberCheckStoreParam memberCheckStoreParam = JsonConvert.DeserializeObject<MemberCheckStoreParam>(baseApi.param.ToString());
            if (memberCheckStoreParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            OpenDao openDao = new OpenDao();
            StoreUser storeUser = openDao.GetStoreUser(Utils.GetOpenID(baseApi.token));

            MemberCheckStoreCodeParam memberCheckStoreCodeParam = new MemberCheckStoreCodeParam
            {
                code = memberCheckStoreParam.code,
            };

            MemberCheckStoreCode memberCheckStoreCode = Utils.GetCache<MemberCheckStoreCode>(memberCheckStoreCodeParam);
            if (memberCheckStoreCode == null)
            {
                throw new ApiException(CodeMessage.InvalidMemberCkeckStoreCode, "InvalidMemberCkeckStoreCode");
            }

            StoreDao storeDao = new StoreDao();
            if (!storeDao.InserMemberCheckStore(storeUser.storeId, memberCheckStoreCode.memberId, memberCheckStoreParam.consume, storeUser.storeUserId))
            {
                throw new ApiException(CodeMessage.MemberCkeckStoreError, "MemberCkeckStoreError");
            }

            Utils.DeleteCache<MemberCheckStoreCode>(memberCheckStoreCodeParam);
            WsPayStateParam wsPayStateParam = new WsPayStateParam
            {
                scanCode = memberCheckStoreCodeParam.code,
            };
            WsPayState wsPayState = new WsPayState
            {
                wsType = WsType.CHECK,
                Unique = wsPayStateParam.GetUnique(),
            };
            Utils.SetCache(wsPayState, 0, 0, 10);

            return "";
        }
    }
}
