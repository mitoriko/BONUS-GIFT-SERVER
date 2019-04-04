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
    }
}
