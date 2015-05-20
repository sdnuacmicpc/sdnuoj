using System;

namespace SDNUOJ.Controllers.Exception
{
    /// <summary>
    /// 请求项目类型
    /// </summary>
    public enum RequestType : byte
    {
        None                    = 0,
        User                    = 10,
        Problem                 = 20,
        ProblemCategory         = 21,
        Contest                 = 30,
        Solution                = 40,
        SolutionError           = 41,
        ForumTopic              = 50,
        ForumPost               = 51,
        News                    = 60,
        UserMail                = 70,
        UserForgetPassword      = 80,
        TopicPage               = 90,
        Resource                = 100
    }
}