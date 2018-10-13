using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ACBC.Common
{
    /// <summary>
    /// 返回信息对照
    /// </summary>
    public enum CodeMessage
    {
        OK = 0,
        PostNull = -1,

        AppIDError = 201,
        SignError = 202,

        NotFound = 404,
        InnerError = 500,

        SenparcCode = 1000,

        PaymentError = 3000,
        PaymentTotalPriceZero=3001,
        PaymentMsgError = 3002,

        InvalidToken = 4000,
        InvalidMethod = 4001,
        InvalidParam = 4002,
        InterfaceRole = 4003,
        InterfaceValueError = 4004,
        InterfaceDBError = 4005,
        NeedLogin = 4006,

        MemberExist = 10001,
        MemberRegError = 10002,

        HomeInitError = 10101,
        InvalidGoods = 10102,
        NotEnoughGoods = 10103,
        UpdateCartError = 10104,
        DeleteCartError = 10105,
        BindStoreFirst = 10106,
        InvalidPreOrderId = 10107,
    }
}
