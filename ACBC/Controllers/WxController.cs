using ACBC.Common;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Senparc.Weixin.MP.TenPayLib;

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

        [HttpPost]
        public ActionResult Member([FromBody]MemberApi memberApi)
        {
            if (memberApi == null)
                return Json(new ResultsJson(new Message(CodeMessage.PostNull, "PostNull"), null));
            return Json(Global.BUSS.BussResults(this, memberApi));
        }

        //Todo 待支付订单支付（check订单状态；获取余额并验证；处理库存；变更状态）
    }
}