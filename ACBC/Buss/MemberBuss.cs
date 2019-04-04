using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
            builder.AppendFormat(Global.SMS_CODE_URL, Global.SMS_CODE, Global.SMS_TPL, code, checkCodeParam.phone);
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

            List<MemberStore> memberStoreList = memberDao.GetMemberStoreListByMemberId(memberId);
            MemberStore memberStore = memberStoreList.Find
                    (
                        item => item.storeId.Equals(bindStoreParam.storeId)
                    );
            if(memberStore != null)
            {
                throw new ApiException(CodeMessage.StoreMemberExist, "StoreMemberExist");
            }

            if(!memberDao.CheckPhone(bindStoreParam.phone, bindStoreParam.storeId))
            {
                throw new ApiException(CodeMessage.StorePhoneExist, "StorePhoneExist");
            }

            RemoteStoreMember remoteStoreMember = memberDao.GetRemoteStoreMember(bindStoreParam.storeId, bindStoreParam.phone);
            if (remoteStoreMember == null)
            {
                throw new ApiException(CodeMessage.RemoteStoreMemberNotExist, "RemoteStoreMemberNotExist");
            }
            
            bool setDefault = memberStoreList.Count == 0;
            
            if (!memberDao.BindMemberStore(memberId, remoteStoreMember, setDefault))
            {
                throw new ApiException(CodeMessage.BindStoreMemberError, "BindStoreMemberError");
            }
            return "";
        }

        public object Do_GetRemoteStore(BaseApi baseApi)
        {
            MemberDao memberDao = new MemberDao();
            string memberId = Utils.GetMemberID(baseApi.token);
            MemberInfo memberInfo = memberDao.GetMemberInfo(memberId);
            List<RemoteStoreMember> list = memberDao.GetRemoteStoreMemberList(memberId);
            return new { memberInfo.heart, list };

        }

        public object Do_GetRemoteStoreInfo(BaseApi baseApi)
        {
            GetRemoteStoreInfoParam getRemoteStoreInfoParam = JsonConvert.DeserializeObject<GetRemoteStoreInfoParam>(baseApi.param.ToString());
            if (getRemoteStoreInfoParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            MemberDao memberDao = new MemberDao();
            string memberId = Utils.GetMemberID(baseApi.token);
            RemoteStoreMember remoteStoreMember = memberDao.GetRemoteStoreMember(getRemoteStoreInfoParam.storeMemberId);
            if (remoteStoreMember == null)
            {
                throw new ApiException(CodeMessage.RemoteStoreMemberNotExist, "RemoteStoreMemberNotExist");
            }
            List<RemotePointCommit> remotePointCommitList = memberDao.GetRemotePointCommitList(remoteStoreMember);
            if(remotePointCommitList.Count > 0)
            {
                if (!memberDao.HandleCommitPoint(remoteStoreMember, remotePointCommitList))
                {
                    throw new ApiException(CodeMessage.HandleCommitPointError, "HandleCommitPointError");
                }
            }
            remoteStoreMember = memberDao.GetRemoteStoreMember(getRemoteStoreInfoParam.storeMemberId);
            if (remoteStoreMember == null)
            {
                throw new ApiException(CodeMessage.RemoteStoreMemberNotExist, "RemoteStoreMemberNotExist");
            }
            int todayChangeHeart = memberDao.GetTodayChangeHeart(memberId, remoteStoreMember.storeId);
            remoteStoreMember.todayExchange = todayChangeHeart;
            return remoteStoreMember;
        }

        public object Do_ExchangeHeart(BaseApi baseApi)
        {
            ExchangeHeartParam exchangeHeartParam = JsonConvert.DeserializeObject<ExchangeHeartParam>(baseApi.param.ToString());
            if (exchangeHeartParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            MemberDao memberDao = new MemberDao();
            string memberId = Utils.GetMemberID(baseApi.token);
            RemoteStoreMember remoteStoreMember = memberDao.GetRemoteStoreMember(exchangeHeartParam.storeMemberId);
            if (remoteStoreMember == null)
            {
                throw new ApiException(CodeMessage.RemoteStoreMemberNotExist, "RemoteStoreMemberNotExist");
            }
            MemberInfo memberInfo = memberDao.GetMemberInfo(memberId);

            int heart = exchangeHeartParam.point / remoteStoreMember.storeRate;
            exchangeHeartParam.point = exchangeHeartParam.point - (exchangeHeartParam.point % remoteStoreMember.storeRate);

            int todayChangeHeart = memberDao.GetTodayChangeHeart(memberId, remoteStoreMember.storeId);
            if(todayChangeHeart + heart > remoteStoreMember.exchangeLimit)
            {
                throw new ApiException(CodeMessage.OverTheStoreLimit, "OverTheStoreLimit");
            }
            if (!memberDao.AddCommitPoint(memberId, remoteStoreMember, exchangeHeartParam.point, heart, memberInfo.heart))
            {
                throw new ApiException(CodeMessage.ExchangeHeartError, "ExchangeHeartError");
            }
            return "";
        }

        public object Do_ReloadScanCode(BaseApi baseApi)
        {
            MemberDao memberDao = new MemberDao();
            string memberId = Utils.GetMemberID(baseApi.token);
            string scanCode = "";
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));
                var strResult = BitConverter.ToString(result);
                scanCode = strResult.Replace("-", "");
            }
            if(!memberDao.UpdateScanCode(memberId, scanCode))
            {
                throw new ApiException(CodeMessage.UpdateScanCodeError, "UpdateScanCodeError");
            }

            return "ORDER_" + scanCode;
        }
    }
}
