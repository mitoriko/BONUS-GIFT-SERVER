using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ACBC.Buss
{
    #region Sys

    public class BussCache
    {
        private string unique = "";
        public string Unique
        {
            get
            {
                return unique;
            }
            set
            {
                unique = value;
            }
        }
    }

    public class BussParam
    {
        public string GetUnique()
        {
            string needMd5 = "";
            string md5S = "";
            foreach (FieldInfo f in this.GetType().GetFields())
            {
                needMd5 += f.Name;
                needMd5 += f.GetValue(this).ToString();
            }
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(needMd5));
                var strResult = BitConverter.ToString(result);
                md5S = strResult.Replace("-", "");
            }
            return md5S;
        }
    }

    public class SessionUser
    {
        public string openid;
        public string checkPhone;
        public string checkCode;
        public string userType;
        public string memberId;
    }

    public class SmsCodeRes
    {
        public int error_code;
        public string reason;
    }

    public class WsPayState
    {
        public string userId;
        public string scanCode;
    }

    public class ExchangeRes
    {
        public string reason;
        public ExchangeResult result;
        public int error_code;
    }

    public class ExchangeResult
    {
        public string update;
        public List<string[]> list;
    }

    public enum ScanCodeType
    {
        Shop,
        User,
        Null,
    }

    #endregion

    #region Params

    public class LoginParam
    {
        public string code;
    }

    public class MemberRegParam
    {
        public string avatarUrl;
        public string city;
        public string country;
        public string gender;
        public string language;
        public string nickName;
        public string province;
    }

    public class GetShowDayGoodsListParam : BussParam
    {
        public string showId;

    }

    public class GetGoodsParam : BussParam
    {
        public string goodsId;

    }

    #endregion

    #region DaoObjs

    public class Member
    {
        public string memberName;
        public string memberId;
        public string openid;
        public string memberImg;
        public string memberPhone;
        public string memberSex;
        public string scanCode;
    }

    public class Home : BussCache
    {
        public HomeInfo homeInfo;
        public List<HomeList> list;
    }

    public class HomeInfo
    {
        public string homeId;
        public string backgroundImg;
        public string titleMaster;
        public string titleSlaver;
        public string titleTagImg;
    }

    public class HomeList
    {
        public string img;
        public string title;
        public string price;
        public string praise;
        public string urlType;
        public string urlValue;
    }

    public class ShowDayList : BussCache
    {
        public List<int> monthList;
        public Dictionary<int, List<ShowDay>> dayList;
    }

    public class ShowDay
    {
        public string showId;
        public string showImg;
        public string showTitle;
    }

    public class ShowDayGoods
    {
        public string goodsId;
        public string goodsImg;
        public string goodsTitle;
    }

    public class ShowDayGoodsList : BussCache
    {
        public List<ShowDayGoods> list;
    }

    public class OrderList
    {
        public List<Order> unPayList = new List<Order>();
        public List<Order> payList = new List<Order>();
        public List<Order> inShopList = new List<Order>();
        public List<Order> doneList = new List<Order>();
    }

    public class Order
    {
        public string orderId;
        public string orderCode;
        public string total;
        public string state;
        public string orderTime;
        public List<OrderGoods> list = new List<OrderGoods>();
    }

    public class OrderGoods
    {
        public string goodsId;
        public string goodsImg;
        public string goodsName;
        public string price;
        public string num;
    }

    public class Goods : BussCache
    {
        public string goodsId;
        public string goodsName;
        public string goodsImg;
        public string goodsDesc;
        public int goodsPrice;
        public int goodsStock;
        public int sales;
        public List<GoodsImg> mainImgs = new List<GoodsImg>();
        public List<GoodsImg> infoImgs = new List<GoodsImg>();
    }

    public class GoodsImg
    {
        public string img;
        public int imgSort;
    }

    public class CartGoods
    {
        public string goodsId;
        public string goodsName;
        public string goodsImg;
        public int goodsPrice;
        public int goodsNum;
        public bool cartChecked;
    }

    #endregion
}
