using System;
using System.Web.Mvc;

using SDNUOJ.Controllers.Attributes;
using SDNUOJ.Controllers.Status;
using SDNUOJ.Utilities.Drawing;

namespace SDNUOJ.Controllers
{
    public class UtilityController : Controller
    {
        /// <summary>
        /// 验证码图片
        /// </summary>
        /// <returns>验证码图片</returns>
        [NoClientCache]
        public ActionResult CheckCode()
        {
            CheckCode checkCode = new CheckCode();
            CheckCodeStatus.SetCheckCode(checkCode.CodeText);
            Byte[] data = checkCode.GetBitmapData();

            return File(data, "image/jpeg");
        }
    }
}