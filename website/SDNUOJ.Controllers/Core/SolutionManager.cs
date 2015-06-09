using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web;

using SDNUOJ.Caching;
using SDNUOJ.Configuration;
using SDNUOJ.Controllers.Core.Exchange;
using SDNUOJ.Controllers.Exception;
using SDNUOJ.Controllers.Status;
using SDNUOJ.Data;
using SDNUOJ.Entity;
using SDNUOJ.Entity.Complex;
using SDNUOJ.Utilities;
using SDNUOJ.Utilities.Security;
using SDNUOJ.Utilities.Text;
using SDNUOJ.Utilities.Text.RegularExpressions;
using SDNUOJ.Utilities.Web;

namespace SDNUOJ.Controllers.Core
{
    /// <summary>
    /// 提交数据管理类
    /// </summary>
    /// <remarks>
    /// HtmlEncode : 程序代码 / 调取时转换
    /// </remarks>
    internal static class SolutionManager
    {
        #region 常量
        /// <summary>
        /// 问题页面页面大小
        /// </summary>
        private const Int32 STATUS_PAGE_SIZE = 20;
        #endregion

        #region 字段
        /// <summary>
        /// 同时只有一个Judge请求Pending
        /// </summary>
        private static Object _selectLock = new Object();

        /// <summary>
        /// 同时只有一个Judge更新结果
        /// </summary>
        private static Object _updateLock = new Object();
        #endregion

        #region 权限管理
        /// <summary>
        /// 获取是否有权限查看指定用户的源代码
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>是否有权限查看源代码</returns> 
        public static Boolean CanViewSource(String userName)
        {
            if (!UserManager.IsUserLogined)
            {
                return false;
            }

            if (String.Equals(UserManager.CurrentUserName, userName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (AdminManager.HasPermission(PermissionType.SourceView))
            {
                return true;
            }

            return false;
        }
        #endregion

        #region 后台方法
        /// <summary>
        /// 更新一条提交(更新所有评测信息)
        /// </summary>
        /// <param name="model">对象实体</param>
        /// <param name="error">编译错误信息</param>
        /// <returns>是否成功更新</returns>
        public static Boolean JudgeUpdateSolutionAllResult(SolutionEntity model, String error)
        {
            if (model == null) return false;

            lock (_updateLock)
            {
                model.JudgeTime = DateTime.Now;

                if (SolutionRepository.Instance.UpdateEntity(model, error) > 0)
                {
                    if (model.Result == ResultType.Accepted)
                    {
                        UserCache.RemoveUserTop10Cache();
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 获取最早等待评测的一个提交
        /// </summary>
        /// <param name="count">获取个数</param>
        /// <param name="languageSupport">支持语言</param>
        /// <returns>提交实体</returns>
        public static List<SolutionEntity> JudgeGetPendingSolution(Int32 count, LanguageType[] languageSupport)
        {
            lock (_selectLock)
            {
                return SolutionRepository.Instance.GetPendingEntities(count, ConfigurationManager.CheckJudgeErrorSolution, languageSupport);
            }
        }
        #endregion

        #region 用户方法
        /// <summary>
        /// 增加一条提交
        /// </summary>
        /// <param name="entity">对象实体</param>
        /// <param name="userip">用户IP</param>
        /// <returns>是否成功增加</returns>
        public static Boolean InsertSolution(SolutionEntity entity, String userip)
        {
            if (!UserManager.IsUserLogined)
            {
                throw new UserUnLoginException();
            }

            if (String.IsNullOrEmpty(entity.SourceCode) || entity.SourceCode.Length < SolutionRepository.SOURCECODE_MINLEN)
            {
                throw new InvalidInputException("Code is too short!");
            }

            if (entity.SourceCode.Length > SolutionRepository.SOURCECODE_MAXLEN)
            {
                throw new InvalidInputException("Code is too long!");
            }

            if (LanguageType.IsNull(entity.LanguageType))
            {
                throw new InvalidInputException("Language Type is INVALID!");
            }

            if (!UserSubmitStatus.CheckLastSubmitSolutionTime(UserManager.CurrentUserName))
            {
                throw new InvalidInputException(String.Format("You can not submit code more than twice in {0} seconds!", ConfigurationManager.SubmitInterval.ToString()));
            }

            ProblemEntity problem = ProblemManager.InternalGetProblemModel(entity.ProblemID);
            if (problem == null)//判断题目是否存在
            {
                throw new NullResponseException(RequestType.Problem);
            }

            if (entity.ContestID <= 0 && problem.IsHide && !AdminManager.HasPermission(PermissionType.ProblemManage))//非竞赛下判断是否有权访问题目
            {
                throw new NoPermissionException("You have no privilege to submit the problem!");
            }

            entity.UserName = UserManager.CurrentUserName;
            entity.SubmitTime = DateTime.Now;
            entity.SubmitIP = userip;

            Boolean success = SolutionRepository.Instance.InsertEntity(entity) > 0;

            if (success)
            {
                ProblemCache.UpdateProblemCacheSubmitCount(entity.ProblemID, -1);//更新缓存
            }

            return success;
        }

        /// <summary>
        /// 根据ID得到一个提交实体
        /// </summary>
        /// <param name="id">提交ID</param>
        /// <returns>提交实体</returns>
        public static SolutionEntity GetSourceCode(Int32 id)
        {
            if (id <= 0)
            {
                throw new InvalidRequstException(RequestType.Solution);
            }

            SolutionEntity solu = SolutionRepository.Instance.GetEntity(id);

            if (solu == null)
            {
                throw new NullResponseException(RequestType.Solution);
            }
            else
            {
                solu.SourceCode = HtmlEncoder.HtmlEncode(solu.SourceCode);
            }

            if (!SolutionManager.CanViewSource(solu.UserName))
            {
                throw new NoPermissionException("You have no privilege to view the code!");
            }

            return solu;
        }

        /// <summary>
        /// 获取提交列表
        /// </summary>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="cid">竞赛ID</param>
        /// <param name="pid">题目ID</param>
        /// <param name="userName">用户名</param>
        /// <param name="languageType">提交语言</param>
        /// <param name="resultType">提交结果</param>
        /// <param name="order">排序顺序</param>
        /// <returns>提交列表</returns>
        public static PagedList<SolutionEntity> GetSolutionList(Int32 pageIndex, Int32 cid, Int32 pid, String userName, String languageType, String resultType, String order)
        {
            Int32 pageSize = SolutionManager.STATUS_PAGE_SIZE;
            Int32 recordCount = 0;
            List<SolutionEntity> list = null;

            if (pid <= 0 && String.IsNullOrEmpty(userName) && String.IsNullOrEmpty(languageType) && String.IsNullOrEmpty(resultType))
            {
                recordCount = SolutionManager.CountSolutions(cid, -1, null, null, null);
                list = SolutionRepository.Instance.GetEntities(cid, -1, null, LanguageType.Null, new Nullable<ResultType>(), -1,
                    pageIndex, pageSize, recordCount);
            }
            else
            {
                if (!String.IsNullOrEmpty(userName) && (!RegexVerify.IsUserName(userName) || !SQLValidator.IsSafe(userName)))
                {
                    throw new InvalidInputException("Username is INVALID!");
                }
                if (!String.IsNullOrEmpty(languageType) && !RegexVerify.IsNumeric(languageType))
                {
                    throw new InvalidInputException("Language Type is INVALID!");
                }
                if (!String.IsNullOrEmpty(resultType) && !RegexVerify.IsNumeric(resultType))
                {
                    throw new InvalidInputException("Result Type is INVALID!");
                }

                recordCount = SolutionManager.CountSolutions(cid, pid, userName, languageType, resultType);
                list = SolutionRepository.Instance.GetEntities(
                    cid, pid, userName,
                    (String.IsNullOrEmpty(languageType) ? LanguageType.Null : LanguageType.FromLanguageID(Convert.ToByte(languageType))),
                    (String.IsNullOrEmpty(resultType) ? new Nullable<ResultType>() : (ResultType)Convert.ToByte(resultType)),
                    (String.IsNullOrEmpty(order) ? -1 : Convert.ToInt32(order)),
                    pageIndex, pageSize, recordCount);
            }

            return list.ToPagedList(pageSize, recordCount);
        }

        /// <summary>
        /// 获取指定用户名的所有AC代码打包文件
        /// </summary>
        /// <returns>所有AC代码打包文件</returns>
        public static Byte[] GetAcceptedSourceCodeList()
        {
            if (!UserManager.IsUserLogined)
            {
                throw new UserUnLoginException();
            }

            if (!ConfigurationManager.AllowDownloadSource)
            {
                throw new FunctionDisabledException("Source code download is DISABLED!");
            }

            String userName = UserManager.CurrentUserName;
            Byte[] file = SolutionCache.GetAcceptedCodesCache(userName);

            if (file == null)
            {
                List<SolutionEntity> solutions = SolutionRepository.Instance.GetEntitiesForAcceptedSourceCode(userName);
                file = SolutionCodeExport.ExportSolutionAcceptedCodeToZip(userName, ConfigurationManager.OnlineJudgeName, solutions);

                SolutionCache.SetAcceptedCodesCache(userName, file);
            }

            return file;
        }

        /// <summary>
        /// 根据用户名和提交结果获取通过提交列表
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="contestID">竞赛ID</param>
        /// <returns>题目ID列表</returns>
        public static List<Int32> GetSolvedProblemIDListByUser(String userName, Int32 contestID)
        {
            if (String.IsNullOrEmpty(userName))
            {
                return new List<Int32>();
            }

            if (!RegexVerify.IsUserName(userName))
            {
                throw new InvalidRequstException(RequestType.User);
            }

            List<Int32> lstSolved = (contestID == -1 ? SolutionCache.GetProblemIDListCache(userName, false) : null);//获取缓存

            if (lstSolved == null)
            {
                lstSolved = SolutionRepository.Instance.GetEntityIDsByUserAndResultType(userName, contestID, ResultType.Accepted);

                if (contestID == -1) SolutionCache.SetProblemIDListCache(userName, false, lstSolved);//获取缓存
            }

            return (lstSolved != null ? lstSolved : new List<Int32>());
        }

        /// <summary>
        /// 根据用户名获取未通过提交列表
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="solvedList">该用户AC题目ID的列表</param>
        /// <param name="contestID">竞赛ID</param>
        /// <returns>题目ID列表</returns>
        public static List<Int32> GetUnSolvedProblemIDListByUser(String userName, List<Int32> lstSolved, Int32 contestID)
        {
            if (String.IsNullOrEmpty(userName))
            {
                return new List<Int32>();
            }

            if (!RegexVerify.IsUserName(userName))
            {
                throw new InvalidRequstException(RequestType.User);
            }

            List<Int32> lstUnsolved = (contestID == -1 ? SolutionCache.GetProblemIDListCache(userName, true) : null);//获取缓存

            if (lstUnsolved == null)
            {
                lstUnsolved = SolutionRepository.Instance.GetEntityIDsByUserAndLessResultType(userName, contestID, ResultType.Accepted);

                if (lstUnsolved != null && lstUnsolved.Count > 0 && lstSolved != null && lstSolved.Count > 0)
                {
                    foreach (Int32 pid in lstSolved)
                    {
                        lstUnsolved.Remove(pid);
                    }
                }

                if (contestID == -1)
                {
                    SolutionCache.SetProblemIDListCache(userName, true, lstUnsolved);//设置缓存
                }
            }

            return (lstUnsolved != null ? lstUnsolved : new List<Int32>());
        }

        /// <summary>
        /// 获取当前用户提交题目列表Json（未登录或不允许获取时返回null）
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>提交题目列表（第一项为已完成的题目ID列表，第二项为未完成的题目ID列表，未登录或不允许获取时返回null）</returns>
        public static Tuple<List<Int32>, List<Int32>> GetUserSubmitList()
        {
            if (!UserManager.IsUserLogined)
            {
                return null;
            }

            if (!ConfigurationManager.AllowMainSubmitStatus)
            {
                return null;
            }

            StringBuilder result = new StringBuilder();
            String userName = UserManager.CurrentUserName;
            List<Int32> lstSolved = SolutionManager.GetSolvedProblemIDListByUser(userName, -1);
            List<Int32> lstUnSolved = SolutionManager.GetUnSolvedProblemIDListByUser(userName, lstSolved, -1);

            return new Tuple<List<Int32>, List<Int32>>(lstSolved, lstUnSolved);
        }

        /// <summary>
        /// 获取当前用户竞赛提交列表
        /// </summary>
        /// <param name="contestID">竞赛ID</param>
        /// <returns>竞赛提交列表</returns>
        public static Dictionary<Int32, Int16> GetUserContestSubmit(Int32 contestID)
        {
            Dictionary<Int32, Int16> dict = new Dictionary<Int32, Int16>();

            String userName = UserManager.CurrentUserName;
            List<Int32> lstSolved = SolutionManager.GetSolvedProblemIDListByUser(userName, contestID);
            List<Int32> lstUnSolved = SolutionManager.GetUnSolvedProblemIDListByUser(userName, lstSolved, contestID);

            if (lstUnSolved != null)
            {
                for (Int32 i = 0; i < lstUnSolved.Count; i++)
                {
                    dict[lstUnSolved[i]] = -1;
                }
            }

            if (lstSolved != null)
            {
                for (Int32 i = 0; i < lstSolved.Count; i++)
                {
                    dict[lstSolved[i]] = 1;
                }
            }

            return dict;
        }

        /// <summary>
        /// 获取题目统计信息
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <param name="pid">题目ID</param>
        /// <returns>题目统计信息实体</returns>
        public static ProblemStatistic GetProblemStatistic(Int32 cid, Int32 pid)
        {
            //此处不能验证cid，因为普通Problem的Statistic也经由此方法

            if (pid < ConfigurationManager.ProblemSetStartID)
            {
                throw new InvalidRequstException(RequestType.Problem);
            }

            //验证pid有效性
            if (cid < 0)//非竞赛题目
            {
                ProblemEntity entity = ProblemManager.GetProblem(pid);
                pid = entity.ProblemID;
            }
            else//竞赛题目
            {
                ProblemEntity entity = ContestProblemManager.GetProblem(cid, pid);
                //竞赛题目传入的pid是ContestProblemID
            }

            ProblemStatistic statistic = SolutionCache.GetProblemStatisticCache(cid, pid);

            if (statistic == null)
            {
                statistic = SolutionRepository.Instance.GetProblemStatistic(cid, pid);//一定有返回值
            }

            return statistic;
        }

        /// <summary>
        /// 获取竞赛题目统计信息
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <returns>竞赛题目统计信息实体列表</returns>
        public static IDictionary<Int32, ContestProblemStatistic> GetContestStatistic(Int32 cid)
        {
            if (cid <= 0)
            {
                throw new InvalidRequstException(RequestType.Contest);
            }

            IDictionary<Int32, ContestProblemStatistic> statistics = SolutionCache.GetContestStatisticCache(cid);

            if (statistics == null)
            {
                statistics = SolutionRepository.Instance.GetContestStatistic(cid);
                SolutionCache.SetContestStatisticCache(cid, statistics);
            }

            return statistics;
        }

        /// <summary>
        /// 获取提交总数
        /// </summary>
        /// <param name="cid">竞赛ID</param>
        /// <param name="pid">题目ID</param>
        /// <param name="userName">用户名</param>
        /// <param name="languageType">提交语言</param>
        /// <param name="resultType">提交结果</param>
        /// <returns>提交总数</returns>
        private static Int32 CountSolutions(Int32 cid, Int32 pid, String userName, String languageType, String resultType)
        {
            if (pid <= 0 && String.IsNullOrEmpty(userName) && String.IsNullOrEmpty(languageType) && String.IsNullOrEmpty(resultType))
            {
                Int32 recordCount = SolutionCache.GetSolutionCountCache(cid);//获取缓存

                if (recordCount < 0)
                {
                    recordCount = SolutionRepository.Instance.CountEntities(cid, -1, null, LanguageType.Null, new Nullable<ResultType>());
                    SolutionCache.SetSolutionCountCache(cid, recordCount);//设置缓存
                }

                return recordCount;
            }
            else
            {
                Byte langType, resType;
                Byte.TryParse(languageType, out langType);
                Byte.TryParse(resultType, out resType);

                return SolutionRepository.Instance.CountEntities(
                    cid, pid, userName,
                    (String.IsNullOrEmpty(languageType) ? LanguageType.Null : LanguageType.FromLanguageID(langType)),
                    (String.IsNullOrEmpty(resultType) ? new Nullable<ResultType>() : (ResultType)resType));
            }
        }

        /// <summary>
        /// 获取所有提交总数(有缓存)
        /// </summary>
        /// <returns>提交总数</returns>
        public static Int32 CountSolutions()
        {
            return CountSolutions(-1, -1, null, null, null);
        }
        #endregion

        #region 管理方法
        /// <summary>
        /// 获取提交统计信息
        /// </summary>
        /// <param name="dateTime">所在月日期</param>
        /// <param name="ACOnly">是否仅有AC题目</param>
        /// <returns>提交统计信息</returns>
        public static IDictionary<Int32, Int32> AdminGetMonthlySubmitStatus(DateTime dateTime, Boolean ACOnly)
        {
            if (!AdminManager.HasPermission(PermissionType.Administrator))
            {
                throw new NoPermissionException();
            }

            DateTime dayStart = dateTime.AddDays(1 - dateTime.Day);
            return SolutionRepository.Instance.GetSubmitStatus(dayStart, dayStart.AddMonths(1), ACOnly);
        }

        /// <summary>
        /// 重测提交
        /// </summary>
        /// <param name="sids">提交ID集合</param>
        /// <param name="cid">竞赛ID</param>
        /// <param name="pid">题目ID</param>
        /// <param name="name">用户名</param>
        /// <param name="lang">提交语言</param>
        /// <param name="type">提交结果</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns>是否成功重测</returns>
        public static IMethodResult AdminRejudgeSolution(String sids, String cid, String pid, String name, String lang, String type, String startDate, String endDate)
        {
            if (!AdminManager.HasPermission(PermissionType.SolutionManage))
            {
                throw new NoPermissionException();
            }

            String solutionIDs = String.Empty;
            String userName = String.Empty;
            Int32 problemID = -1, contestID = -1;
            LanguageType languageType = LanguageType.Null;
            ResultType? resultType = new Nullable<ResultType>();
            DateTime? dtStart = null, dtEnd = null;
            Boolean noCondition = SolutionManager.AdminGetSolutionParams(sids, cid, pid, name, lang, type, startDate, endDate, out solutionIDs, out problemID, out contestID, out userName, out languageType, out resultType, out dtStart, out dtEnd);

            if (noCondition)
            {
                return MethodResult.FailedAndLog("You must set at least one condition!");
            }

            Int32 result = SolutionRepository.Instance.UpdateEntityToRejudge(solutionIDs, contestID, problemID, userName, languageType, resultType, dtStart, dtEnd);

            if (result <= 0)
            {
                return MethodResult.FailedAndLog("No solution was rejudged!");
            }

            return MethodResult.SuccessAndLog<Int32>(result, "Admin rejudge solution, sid = {0}, cid = {2}, pid = {1}, name = {3}, lang = {4}, type = {5}, start = {6}, end = {7}", solutionIDs.ToString(), contestID.ToString(), problemID.ToString(), name, languageType.ToString(), resultType.ToString(), dtStart.ToString(), dtEnd.ToString());
        }

        /// <summary>
        /// 获取提交列表
        /// </summary>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="sids">提交ID集合</param>
        /// <param name="cid">竞赛ID</param>
        /// <param name="pid">题目ID</param>
        /// <param name="name">用户名</param>
        /// <param name="lang">提交语言</param>
        /// <param name="type">提交结果</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="order">排序顺序</param>
        /// <returns>提交列表</returns>
        public static PagedList<SolutionEntity> AdminGetSolutionList(Int32 pageIndex, String sids, String cid, String pid, String name, String lang, String type, String startDate, String endDate, String order)
        {
            if (!AdminManager.HasPermission(PermissionType.SolutionManage))
            {
                throw new NoPermissionException();
            }

            Int32 pageSize = AdminManager.ADMIN_LIST_PAGE_SIZE;
            Int32 recordCount = SolutionManager.AdminCountSolutionList(sids, cid, pid, name, lang, type, startDate, endDate);
            
            String solutionIDs = String.Empty;
            String userName = String.Empty;
            Int32 problemID = -1, contestID = -1;
            LanguageType languageType = LanguageType.Null;
            ResultType? resultType = new Nullable<ResultType>();
            DateTime? dtStart = null, dtEnd = null;

            SolutionManager.AdminGetSolutionParams(sids, cid, pid, name, lang, type, startDate, endDate, out solutionIDs, out problemID, out contestID, out userName, out languageType, out resultType, out dtStart, out dtEnd);

            return SolutionRepository.Instance
                .GetEntities(solutionIDs, contestID, problemID, userName, languageType, resultType, dtStart, dtEnd,
                    (String.IsNullOrEmpty(order) ? -1 : Convert.ToInt32(order)),
                    pageIndex, pageSize, recordCount)
                .ToPagedList(pageSize, recordCount);
        }

        /// <summary>
        /// 获取提交总数(无缓存)
        /// </summary>
        /// <param name="sids">提交ID集合</param>
        /// <param name="cid">竞赛ID</param>
        /// <param name="pid">题目ID</param>
        /// <param name="name">用户名</param>
        /// <param name="lang">提交语言</param>
        /// <param name="type">提交结果</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="order">排序顺序</param>
        /// <returns>提交总数</returns>
        private static Int32 AdminCountSolutionList(String sids, String cid, String pid, String name, String lang, String type, String startDate, String endDate)
        {
            String solutionIDs = String.Empty;
            String userName = String.Empty;
            Int32 contestID = -1, problemID = -1;
            LanguageType languageType = LanguageType.Null;
            ResultType? resultType = new Nullable<ResultType>();
            DateTime? dtStart = null, dtEnd = null;

            SolutionManager.AdminGetSolutionParams(sids, cid, pid, name, lang, type, startDate, endDate, out solutionIDs, out problemID, out contestID, out userName, out languageType, out resultType, out dtStart, out dtEnd);

            return SolutionRepository.Instance.CountEntities(solutionIDs, contestID, problemID, userName, languageType, resultType, dtStart, dtEnd);
        }

        private static Boolean AdminGetSolutionParams(String sids, String cid, String pid, String name, String lang, String type, String startDate, String endDate,
            out String solutionIDs, out Int32 problemID, out Int32 contestID, out String userName, out LanguageType languageType, out ResultType? resultType, out DateTime? dtStart, out DateTime? dtEnd)
        {
            Boolean noCondition = true;
            String dateFormat = "yyyy-MM-dd HH:mm:ss";

            solutionIDs = String.Empty;
            contestID = -1;
            problemID = -1;
            userName = String.Empty;
            languageType = LanguageType.Null;
            resultType = new Nullable<ResultType>();
            dtStart = null;
            dtEnd = null;

            if (!String.IsNullOrEmpty(sids))
            {
                solutionIDs = sids.SearchOptimized();

                if (!RegexVerify.IsNumericIDs(solutionIDs))
                {
                    throw new InvalidRequstException(RequestType.Solution);
                }

                noCondition = false;
            }

            if (!String.IsNullOrEmpty(cid))
            {
                if (!Int32.TryParse(cid, out contestID))
                {
                    throw new InvalidRequstException(RequestType.Contest);
                }

                if (contestID < ContestRepository.NONECONTEST)
                {
                    throw new InvalidRequstException(RequestType.Contest);
                }

                noCondition = false;
            }

            if (!String.IsNullOrEmpty(pid))
            {
                if (!Int32.TryParse(pid, out problemID))
                {
                    throw new InvalidRequstException(RequestType.Problem);
                }

                if (problemID < ConfigurationManager.ProblemSetStartID)
                {
                    throw new InvalidRequstException(RequestType.Problem);
                }

                noCondition = false;
            }

            if (!String.IsNullOrEmpty(name))
            {
                if (!RegexVerify.IsUserName(name))
                {
                    throw new InvalidRequstException(RequestType.User);
                }

                userName = name;
                noCondition = false;
            }

            if (!String.IsNullOrEmpty(lang))
            {
                Byte langType = Byte.MaxValue;

                if (!Byte.TryParse(lang, out langType))
                {
                    throw new InvalidInputException("Language Type is INVALID!");
                }

                languageType = LanguageType.FromLanguageID(langType);
                noCondition = false;
            }

            if (!String.IsNullOrEmpty(type))
            {
                Byte resType = Byte.MaxValue;

                if (!Byte.TryParse(type, out resType))
                {
                    throw new InvalidInputException("Result Type is INVALID!");
                }

                resultType = (ResultType)resType;
                noCondition = false;
            }

            if (!String.IsNullOrEmpty(startDate))
            {
                DateTime temp;
                if (!DateTime.TryParseExact(startDate, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out temp))
                {
                    throw new InvalidInputException("Datetime is INVALID!");
                }

                dtStart = temp;
                noCondition = false;
            }

            if (!String.IsNullOrEmpty(endDate))
            {
                DateTime temp;
                if (!DateTime.TryParseExact(endDate, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out temp))
                {
                    throw new InvalidInputException("Datetime is INVALID!");
                }

                dtEnd = temp;

                if (dtStart.HasValue && dtStart.Value >= dtEnd.Value)
                {
                    throw new InvalidInputException("Start date CANNOT be later than end date!");
                }

                noCondition = false;
            }

            return noCondition;
        }
        #endregion
    }
}