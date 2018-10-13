using ACBC.Common;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ACBC.Controllers
{
    [Produces("application/json")]
    [Route(Global.ROUTE_PX + "/[controller]/[action]")]
    [EnableCors("AllowSameDomain")]
    public class WxController : Controller
    {
        [HttpPost]
        public ActionResult Open([FromBody]OpenApi openApi)
        {
            if (openApi == null)
                return Json(new ResultsJson(new Message(CodeMessage.PostNull, "PostNull"), null));
            return Json(Global.BUSS.BussResults(this, openApi));
        }

        [HttpPost]
        public ActionResult Mall([FromBody]MallApi mallApi)
        {
            if (mallApi == null)
                return Json(new ResultsJson(new Message(CodeMessage.PostNull, "PostNull"), null));
            return Json(Global.BUSS.BussResults(this, mallApi));
        }

        [HttpPost]
        public ActionResult Order([FromBody]OrderApi orderApi)
        {
            if (orderApi == null)
                return Json(new ResultsJson(new Message(CodeMessage.PostNull, "PostNull"), null));
            return Json(Global.BUSS.BussResults(this, orderApi));
        }

        //Todo 个人中心（心值；订单状态合计）
        //Todo 预生成订单（立即兑换、购物车结算；其中购物车结算带cartId；返回PreOrderId）
        //Todo 生成订单（PreOrderId；返回orderCode；返回后小程序发起“待支付订单支付”）
        //Todo 待支付订单支付（check订单状态）
        //Todo 获取单订单明细


    }
}