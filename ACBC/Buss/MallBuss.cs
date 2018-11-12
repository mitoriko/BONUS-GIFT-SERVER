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

        public object Do_GetGoods(BaseApi baseApi)
        {
            GetGoodsParam getGoodsParam = JsonConvert.DeserializeObject<GetGoodsParam>(baseApi.param.ToString());
            if (getGoodsParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            Goods goods = Utils.GetCache<Goods>(getGoodsParam);
            if (goods == null)
            {
                MallDao mallDao = new MallDao();
                goods = mallDao.GetGoodsByGoodsId(getGoodsParam.goodsId);
                goods.Unique = getGoodsParam.GetUnique();
                Utils.SetCache(goods, 0, 1, 0);
            }

            return goods;
        }

        public object Do_GetStoreList(BaseApi baseApi)
        {
            StoreList storeList = Utils.GetCache<StoreList>();
            if(storeList == null)
            {
                MallDao mallDao = new MallDao();
                string memberId = Utils.GetMemberID(baseApi.token);
                storeList = new StoreList();
                storeList.storeList = mallDao.GetStoreList();
                Utils.SetCache(storeList);
            }
            return storeList;

        }

        public object Do_GetStoreInfo(BaseApi baseApi)
        {
            GetStoreInfoParam getStoreInfoParam = JsonConvert.DeserializeObject<GetStoreInfoParam>(baseApi.param.ToString());
            if (getStoreInfoParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            StoreInfo storeInfo = Utils.GetCache<StoreInfo>(getStoreInfoParam);
            if(storeInfo == null)
            {
                MallDao mallDao = new MallDao();
                storeInfo = mallDao.GetStoreInfo(getStoreInfoParam.storeId);
                if (storeInfo == null)
                {
                    throw new ApiException(CodeMessage.InvalidStore, "InvalidStore");
                }
                storeInfo.Unique = getStoreInfoParam.GetUnique();
                Utils.SetCache(storeInfo);
            }
            return storeInfo;

        }
    }
}
