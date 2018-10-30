using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ACBC.Common;
using ACBC.Dao;

namespace ACBC.Buss
{
    public class MemberBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.MemberApi;
        }

        public object Do_GetMemberInfo(BaseApi baseApi)
        {
            MemberDao memberDao = new MemberDao();
            string memberId = Utils.GetMemberID(baseApi.token);
            return memberDao.GetMemberInfo(memberId);

        }

        public object Do_GetStoreMemberList(BaseApi baseApi)
        {
            MemberDao memberDao = new MemberDao();
            string memberId = Utils.GetMemberID(baseApi.token);
            List<MemberStore> list = memberDao.GetMemberStoreListByMemberId(memberId);
            return list;

        }

        
    }
}
