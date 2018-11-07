using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACBC.Common;
using ACBC.Dao;
using Newtonsoft.Json;
using Senparc.Weixin.WxOpen.Containers;

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

        public object Do_GetMemberStoreList(BaseApi baseApi)
        {
            MemberDao memberDao = new MemberDao();
            string memberId = Utils.GetMemberID(baseApi.token);
            List<MemberStore> list = memberDao.GetMemberStoreListByMemberId(memberId);
            return list;

        }

        public object Do_SetDefaultMemberStore(BaseApi baseApi)
        {
            SetDefaultMemberStoreParam setDefaultMemberStoreParam = JsonConvert.DeserializeObject<SetDefaultMemberStoreParam>(baseApi.param.ToString());
            if (setDefaultMemberStoreParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            MemberDao memberDao = new MemberDao();
            string memberId = Utils.GetMemberID(baseApi.token);
            if (!memberDao.SetDefaultMemberStore(memberId, setDefaultMemberStoreParam.storeId))
            {
                throw new ApiException(CodeMessage.SetDefaultMemberStoreError, "SetDefaultMemberStoreError");
            }
            return "";

        }

        public object Do_CheckCode(BaseApi baseApi)
        {
            CheckCodeParam checkCodeParam = JsonConvert.DeserializeObject<CheckCodeParam>(baseApi.param.ToString());
            if (checkCodeParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            string code = new Random().Next(999999).ToString().PadLeft(6, '0');
            SessionBag sessionBag = SessionContainer.GetSession(baseApi.token);
            SessionUser sessionUser = JsonConvert.DeserializeObject<SessionUser>(sessionBag.Name);
            if (sessionUser == null)
            {
                throw new ApiException(CodeMessage.InvalidToken, "InvalidToken");
            }
            sessionUser.checkCode = code;
            sessionUser.checkPhone = checkCodeParam.phone;
            sessionBag.Name = JsonConvert.SerializeObject(sessionUser);
            SessionContainer.Update(sessionBag.Key, sessionBag);
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(Global.SMS_CODE_URL, Global.SMS_CODE, Global.SMS_TPL, checkCodeParam.phone, code);
            string url = builder.ToString();
            string res = Utils.GetHttp(url);

            SmsCodeRes smsCodeRes = JsonConvert.DeserializeObject<SmsCodeRes>(res);
            if (smsCodeRes == null || smsCodeRes.error_code != 0)
            {
                throw new ApiException(CodeMessage.SmsCodeError, (smsCodeRes == null ? "SmsCodeError" : smsCodeRes.reason));
            }

            return "";
        }

        public object Do_BindMemberStore(BaseApi baseApi)
        {
            BindStoreParam bindStoreParam = JsonConvert.DeserializeObject<BindStoreParam>(baseApi.param.ToString());
            if (bindStoreParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            MemberDao memberDao = new MemberDao();
            string memberId = Utils.GetMemberID(baseApi.token);

            //check phone code
            SessionBag sessionBag = SessionContainer.GetSession(baseApi.token);
            SessionUser sessionUser = JsonConvert.DeserializeObject<SessionUser>(sessionBag.Name);
            if (sessionUser == null)
            {
                throw new ApiException(CodeMessage.InvalidToken, "InvalidToken");
            }
            if(sessionUser.checkCode != bindStoreParam.checkCode ||
                sessionUser.checkPhone != bindStoreParam.phone)
            {
                throw new ApiException(CodeMessage.InvalidCheckCode, "InvalidCheckCode");
            }
            sessionUser.checkCode = "";
            sessionUser.checkPhone = "";
            sessionBag.Name = JsonConvert.SerializeObject(sessionUser);
            SessionContainer.Update(sessionBag.Key, sessionBag);
            //check exist store member
            List<MemberStore> memberStoreList = memberDao.GetMemberStoreListByMemberId(memberId);
            MemberStore memberStore = memberStoreList.Find
                    (
                        item => item.storeId.Equals(bindStoreParam.storeId)
                    );
            if(memberStore != null)
            {
                throw new ApiException(CodeMessage.StoreMemberExist, "StoreMemberExist");
            }
            //get remote member
            RemoteStoreMember remoteStoreMember = memberDao.GetRemoteStoreMember(bindStoreParam.storeId, bindStoreParam.phone);
            if (remoteStoreMember == null)
            {
                throw new ApiException(CodeMessage.RemoteStoreMemberNotExist, "RemoteStoreMemberNotExist");
            }
            //get need default
            bool setDefault = memberStoreList.Count == 0;
            //bind remote member
            if (!memberDao.BindBindMemberStore(memberId, remoteStoreMember, setDefault))
            {
                throw new ApiException(CodeMessage.BindStoreMemberError, "BindStoreMemberError");
            }
            return "";
        }
    }
}
