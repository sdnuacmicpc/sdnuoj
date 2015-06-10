using System;
using System.Collections.Generic;
using System.Data.Common;

using SDNUOJ.Caching;
using SDNUOJ.Controllers.Exception;
using SDNUOJ.Data;
using SDNUOJ.Entity;
using SDNUOJ.Utilities;

namespace SDNUOJ.Controllers.Core
{
    /// <summary>
    /// 竞赛问题数据管理类
    /// </summary>
    internal static class ContestProblemManager
    {
        #region 用户方法
        /// <summary>
        /// 根据ID得到一个题目实体
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <param name="pid">题目ID</param>
        /// <returns>题目实体</returns>
        public static ProblemEntity GetProblem(Int32 cid, Int32 pid)
        {
            ProblemEntity problem = ContestProblemCache.GetContestProblemCache(cid, pid);//获取缓存

            if (problem == null)
            {
                problem = ProblemRepository.Instance.GetEntityForContest(cid, pid);
                ContestProblemCache.SetContestProblemCache(cid, pid, problem);//设置缓存
            }

            if (problem == null)
            {
                throw new NullResponseException(RequestType.Problem);
            }

            return problem;
        }

        /// <summary>
        /// 获取题目列表
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <returns>题目列表</returns>
        public static List<ProblemEntity> GetProblemSet(Int32 cid)
        {
            List<ProblemEntity> list = ContestProblemCache.GetContestProblemSetCache(cid);//获取缓存

            if (list == null)
            {
                list = ProblemRepository.Instance.GetEntitiesForContest(cid);
                ContestProblemCache.SetContestProblemSetCache(cid, list);//设置缓存
            }

            return list;
        }

        /// <summary>
        /// 获取竞赛题目列表
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <returns>竞赛题目列表</returns>
        public static List<ContestProblemEntity> GetContestProblemList(Int32 cid)
        {
            List<ContestProblemEntity> list = ContestProblemCache.GetContestProblemListCache(cid);//获取缓存

            if (list == null)
            {
                list = ContestProblemRepository.Instance.GetEntities(cid);
                ContestProblemCache.SetContestProblemListCache(cid, list);//设置缓存
            }

            return list;
        }
        #endregion

        #region 管理方法
        /// <summary>
        /// 设置竞赛题目列表
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <param name="problemids">题目ID列表</param>
        /// <returns>是否成功设置</returns>
        public static IMethodResult AdminSetContestProblemList(Int32 cid, String problemids)
        {
            if (!AdminManager.HasPermission(PermissionType.ContestManage))
            {
                throw new NoPermissionException();
            }

            if (String.IsNullOrEmpty(problemids))
            {
                problemids = "";
            }

            String[] ids = problemids.Lines();
            List<Int32> problemIDs = new List<Int32>();

            for (Int32 i = 0; i < ids.Length; i++)
            {
                Int32 pid = 0;

                if (!String.IsNullOrEmpty(ids[i].Trim()) && Int32.TryParse(ids[i].Trim(), out pid))
                {
                    problemIDs.Add(pid);
                }
            }

            ContestProblemCache.RemoveContestProblemListCache(cid);//删除缓存

            try
            {
                Boolean success = ContestProblemRepository.Instance.InsertEntities(cid, problemIDs) > 0;

                if (!success)
                {
                    return MethodResult.FailedAndLog("No contest problem was updated!");
                }

                return MethodResult.SuccessAndLog("Admin add contest problem, cid = {0}, pid = {1}", cid.ToString(), String.Join(",", ids));
            }
            catch (DbException)
            {
                return MethodResult.FailedAndLog("Failed to add these problems, please check whether the problem ids are all correct.");
            }
        }

        /// <summary>
        /// 获取竞赛题目列表
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <returns>竞赛题目列表</returns>
        public static IMethodResult AdminGetContestProblemList(Int32 cid)
        {
            if (!AdminManager.HasPermission(PermissionType.ContestManage))
            {
                throw new NoPermissionException();
            }

            List<ContestProblemEntity> list = ContestProblemRepository.Instance.GetEntities(cid);

            return MethodResult.Success(list);
        }
        #endregion
    }
}