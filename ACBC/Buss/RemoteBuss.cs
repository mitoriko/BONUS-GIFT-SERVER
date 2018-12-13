using ACBC.Common;
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
            return "OK";
        }
    }
}
