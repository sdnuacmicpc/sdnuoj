using System;

namespace SDNUOJ.Entity.Complex
{
    /// <summary>
    /// 排名信息类型
    /// </summary>
    public enum RankItemInfoType : byte
    {
        Normal = 0,
        Wrong = 1,
        Accepted = 2,
        FirstBlood = 3
    }
}