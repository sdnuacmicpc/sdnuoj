using System;

namespace SDNUOJ.Controllers
{
    /// <summary>
    /// 页面类别
    /// </summary>
    public enum PageType : int
    {
        None                = 0x0,
        Register            = 0x1,
        ForgetPassword      = 0x2,
        UserControl         = 0x10,
        UserMail            = 0x20,
        UserInfo            = 0x40,
        SourceView          = 0x100,
        MainSubmit          = 0x200,
        MainForum           = 0x400,
        MainProblem         = 0x800,
        MainRanklist        = 0x1000,
        MainStatus          = 0x2000,
        Resource            = 0x4000,
        Contest             = 0x8000
    }
}