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

    }
}
