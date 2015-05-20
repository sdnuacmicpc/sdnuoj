using System;

namespace SDNUOJ.Entity
{
    /// <summary>
    /// 竞赛状态
    /// </summary>
    public enum ContestStatus : byte
    {
        Pending         = 0,
        Registering     = 1,
        Running         = 2,
        Ended           = 3
    }
}