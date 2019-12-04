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

        public object Do_AddPointRecord(BaseApi baseApi)
        {
            AddPointRecordParam addPointRecordParam = JsonConvert.DeserializeObject<AddPointRecordParam>(baseApi.param.ToString());
            if (addPointRecordParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            RemoteDao remoteDao = new RemoteDao();
            if (!remoteDao.AddPointCommit(baseApi.code, addPointRecordParam.phone, addPointRecordParam.point))
            {
                throw new ApiException(CodeMessage.AddPointCommitError, "AddPointCommitError");
            }

            return "";
        }

        public object Do_GetPointCommitList(BaseApi baseApi)
        {
            RemoteDao remoteDao = new RemoteDao();
            return remoteDao.GetPointCommitList(baseApi.code);
        }

        public object Do_UpdatePointCommit(BaseApi baseApi)
        {
            UpdatePointCommitParam updatePointCommitParam = JsonConvert.DeserializeObject<UpdatePointCommitParam>(baseApi.param.ToString());
            if (updatePointCommitParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            RemoteDao remoteDao = new RemoteDao();
            if (!remoteDao.UpdatePointCommit(updatePointCommitParam.pointCommitId))
            {
                throw new ApiException(CodeMessage.UpdatePointCommitError, "UpdatePointCommitError");
            }

            return "";
        }

        public object Do_CommitBy3rdUser(BaseApi baseApi)
        {
            CommitBy3rdUserParam commitBy3rdUserParam = JsonConvert.DeserializeObject<CommitBy3rdUserParam>(baseApi.param.ToString());
            if (commitBy3rdUserParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            string returnStr = "";
            RemoteDao remoteDao = new RemoteDao();
            Store store = remoteDao.GetStoreByStoreId(baseApi.code);
            if (store == null)
            {
                throw new ApiException(CodeMessage.InvalidStoreCode, "InvalidStoreCode");
            }
            
            if (commitBy3rdUserParam.phone == null)
            {
                throw new ApiException(CodeMessage.StorePhoneExist, "StorePhoneExist");

            }
            Member member = null;
            if (remoteDao.GetMemberStoreById(baseApi.code, commitBy3rdUserParam.phone))
            {
                MemberRegParam memberRegParam = new MemberRegParam
                {
                    avatarUrl = commitBy3rdUserParam.avatarUrl,
                    gender = commitBy3rdUserParam.gender,
                    nickName = commitBy3rdUserParam.nickName
                };
                string openID = Guid.NewGuid().ToString();
                remoteDao.MemberReg(memberRegParam, openID);
                member = remoteDao.GetMember(openID);
                if(member == null)
                {
                    throw new ApiException(CodeMessage.MemberRegError, "MemberRegError");
                }
                returnStr = openID;

                RemoteStoreMember remoteStoreMember = new RemoteStoreMember
                {
                    cardCode = commitBy3rdUserParam.phone,
                    phone = commitBy3rdUserParam.phone,
                    storeId = store.storeId
                };
                if (!remoteDao.BindMemberStore(member.memberId, remoteStoreMember, false))
                {
                    throw new ApiException(CodeMessage.BindStoreMemberError, "BindStoreMemberError");
                }
            }
            else
            {
                member = remoteDao.GetMember(baseApi.code, commitBy3rdUserParam.phone);
            }

            if (commitBy3rdUserParam.goodsId != null)
            {
                commitBy3rdUserParam.preOrderId = "APP_" + store.storeCode + "_" + commitBy3rdUserParam.preOrderId;
                
                Goods goods = remoteDao.GetGoodsByGoodsId(commitBy3rdUserParam.goodsId);
                if (goods == null)
                {
                    throw new ApiException(CodeMessage.InvalidGoods, "InvalidGoods");
                }
                if (!remoteDao.CreateOrder(
                    commitBy3rdUserParam.preOrderId,
                    commitBy3rdUserParam.state,
                    member.memberId,
                    commitBy3rdUserParam.addr,
                    commitBy3rdUserParam.goodsNum,
                    store,
                    goods
                    ))
                {
                    throw new ApiException(CodeMessage.CreateOrderError, "CreateOrderError");
                }
            }

            if (commitBy3rdUserParam.heartAdd != null)
            {
                //add heart_change and add heart in member
            }

            return returnStr;
        }
    }
}
