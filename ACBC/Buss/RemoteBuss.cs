using ACBC.Common;
using ACBC.Dao;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ACBC.Buss
{
    public class RemoteBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.RemoteApi;
        }

        public object Do_AddMemberInfo(BaseApi baseApi)
        {
            AddMemberInfoParam addMemberInfoParam = JsonConvert.DeserializeObject<AddMemberInfoParam>(baseApi.param.ToString());
            if (addMemberInfoParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            RemoteDao remoteDao = new RemoteDao();
            if(!remoteDao.GetStoreMemberByCode(baseApi.code, addMemberInfoParam.phone))
            {
                throw new ApiException(CodeMessage.RemoteStoreMemberExist, "RemoteStoreMemberExist");
            }

            if(!remoteDao.AddRemoteStoreMember(baseApi.code, addMemberInfoParam.phone, addMemberInfoParam.cardCode, addMemberInfoParam.point))
            {
                throw new ApiException(CodeMessage.AddRemoteStoreMemberError, "AddRemoteStoreMemberError");
            }

            return "";
        }
    }
}
