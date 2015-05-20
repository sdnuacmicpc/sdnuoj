using System;

namespace SDNUOJ.Entity
{
    /// <summary>
    /// 权限类型
    /// </summary>
    [Flags]
    public enum PermissionType : int
    {
        None                = 0x0,      //0000 0000 0000 0000 0000 0000 0000 0000   //没有权限
        HttpJudge           = 0x1,      //0000 0000 0000 0000 0000 0000 0000 0001   //0 //独立权限 不得与其他共有
        SourceView          = 0x2,      //0000 0000 0000 0000 0000 0000 0000 0010   //1
        Administrator       = 0x80,     //0000 0000 0000 0000 0000 0000 1000 0000   //7 //基本管理员权限 所有管理员必须添加
        NewsManage          = 0x100,    //0000 0000 0000 0000 0000 0001 0000 0000   //8
        ResourceManage      = 0x200,    //0000 0000 0000 0000 0000 0010 0000 0000   //9
        ForumManage         = 0x400,    //0000 0000 0000 0000 0000 0100 0000 0000   //10
        ProblemManage       = 0x800,    //0000 0000 0000 0000 0000 1000 0000 0000   //11
        ContestManage       = 0x1000,   //0000 0000 0000 0000 0001 0000 0000 0000   //12
        SolutionManage      = 0x2000,   //0000 0000 0000 0000 0010 0000 0000 0000   //13
        SuperAdministrator  = 0x3FFFFFFE//0011 1111 1111 1111 1111 1111 1111 1110   //30 //-1 去除HttpJudge 超级管理员
    }
}