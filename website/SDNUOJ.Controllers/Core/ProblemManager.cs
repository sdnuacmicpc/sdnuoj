using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

using SDNUOJ.Caching;
using SDNUOJ.Configuration;
using SDNUOJ.Controllers.Core.Exchange;
using SDNUOJ.Controllers.Exception;
using SDNUOJ.Data;
using SDNUOJ.Entity;
using SDNUOJ.Utilities;
using SDNUOJ.Utilities.Security;
using SDNUOJ.Utilities.Text;
using SDNUOJ.Utilities.Text.RegularExpressions;

namespace SDNUOJ.Controllers.Core
{
    /// <summary>
    /// 问题数据管理类
    /// </summary>
    /// <remarks>
    /// HtmlEncode : 样例输入 / 调取时转换
    /// HtmlEncode : 样例输出 / 调取时转换
    /// </remarks>
    internal static class ProblemManager
    {
        #region 常量
        /// <summary>
        /// 问题页面页面大小
        /// </summary>
        private const Int32 PROBLEM_SET_PAGE_SIZE = 100;
        #endregion

        #region 内部方法
        /// <summary>
        /// 判断给定ID的题目是否存在
        /// </summary>
        /// <param name="id">题目ID</param>
        /// <returns>题目是否存在</returns>
        internal static Boolean InternalExistsProblem(Int32 id)
        {
            return (GetProblem(id) != null);
        }

        /// <summary>
        /// 根据ID得到一个题目实体
        /// </summary>
        /// <param name="id">题目ID</param>
        /// <returns>题目实体</returns>
        internal static ProblemEntity InternalGetProblemModel(Int32 id)
        {
            ProblemEntity problem = ProblemCache.GetProblemCache(id);//获取缓存

            if (problem == null)
            {
                problem = ProblemRepository.Instance.GetEntity(id);

                if (problem != null)
                {
                    problem.SampleInput = HtmlEncoder.HtmlEncode(problem.SampleInput);
                    problem.SampleOutput = HtmlEncoder.HtmlEncode(problem.SampleOutput);

                    ProblemCache.SetProblemCache(problem);//设置缓存
                }
            }

            return problem;
        }
        #endregion

        #region 权限管理
        /// <summary>
        /// 获取用户可以浏览的题目列表
        /// </summary>
        /// <param name="list">题目列表</param>
        /// <returns>用户可以浏览的题目列表</returns>
        internal static List<ProblemEntity> GetUserCanViewProblemList(List<ProblemEntity> list)
        {
            List<ProblemEntity> userProblemList = new List<ProblemEntity>();

            if (list == null || list.Count == 0)
            {
                return userProblemList;
            }

            Boolean canViewHide = AdminManager.HasPermission(PermissionType.ProblemManage);
            
            for (Int32 i = 0; i < list.Count; i++)
            {
                if (canViewHide || !list[i].IsHide)
                {
                    userProblemList.Add(list[i]);
                }
            }

            return userProblemList;
        }
        #endregion

        #region 评测机方法
        /// <summary>
        /// 根据ID得到一个题目实体
        /// </summary>
        /// <param name="id">题目ID</param>
        /// <returns>题目实体</returns>
        public static ProblemEntity GetJudgeProblem(Int32 id)
        {
            return InternalGetProblemModel(id);
        }
        #endregion

        #region 用户方法
        /// <summary>
        /// 根据ID得到一个题目实体
        /// </summary>
        /// <param name="id">题目ID</param>
        /// <returns>题目实体</returns>
        public static ProblemEntity GetProblem(Int32 id)
        {
            if (id < ConfigurationManager.ProblemSetStartID)
            {
                throw new InvalidRequstException(RequestType.Problem);
            }

            ProblemEntity problem = ProblemManager.InternalGetProblemModel(id);

            if (problem == null)
            {
                throw new NullResponseException(RequestType.Problem);
            }

            if (problem.IsHide && !AdminManager.HasPermission(PermissionType.ProblemManage))
            {
                throw new NoPermissionException("You have no privilege to view the problem!");
            }

            return problem;
        }

        /// <summary>
        /// 获取题目列表
        /// </summary>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">页面大小</param>
        /// <returns>题目列表</returns>
        public static PagedList<ProblemEntity> GetProblemSet(Int32 pageIndex)
        {
            Int32 pageSize = ProblemManager.PROBLEM_SET_PAGE_SIZE;
            Int32 recordCount = ProblemManager.CountProblemsByMaxID();

            List<ProblemEntity> list = ProblemCache.GetProblemSetCache(pageIndex);//获取缓存

            if (list == null)
            {
                list = ProblemRepository.Instance.GetEntitiesForProblemSet(pageIndex, pageSize);
                ProblemCache.SetProblemSetCache(pageIndex, list);//设置缓存
            }

            list = ProblemManager.GetUserCanViewProblemList(list);

            return list.ToPagedList(pageSize, recordCount);
        }

        /// <summary>
        /// 根据题目搜索结果获取题目列表
        /// </summary>
        /// <param name="type">搜索类别</param>
        /// <param name="content">搜索内容</param>
        /// <returns>题目列表</returns>
        public static List<ProblemEntity> GetProblemBySearch(String type, String content)
        {
            List<ProblemEntity> list = null;

            if (String.Equals(type, "category", StringComparison.OrdinalIgnoreCase))
            {
                Int32 typeID = -1;

                if (!Int32.TryParse(content, out typeID) || typeID <= 0)
                {
                    throw new InvalidRequstException(RequestType.ProblemCategory);
                }

                list = ProblemManager.GetProblemListByType(typeID);
            }
            else if (String.Equals(type, "pid", StringComparison.OrdinalIgnoreCase))
            {
                content = content.SearchOptimized();

                if (!RegexVerify.IsNumericIDs(content))
                {
                    throw new InvalidRequstException(RequestType.Problem);
                }

                list = ProblemManager.GetProblemListByID(content);
            }
            else if (String.Equals(type, "title", StringComparison.OrdinalIgnoreCase))
            {
                if (String.IsNullOrEmpty(content) || !SQLValidator.IsNonNullANDSafe(content))
                {
                    throw new InvalidInputException("Problem Title is INVALID!");
                }

                list = ProblemManager.GetProblemListByTitle(content);
            }
            else if (String.Equals(type, "source", StringComparison.OrdinalIgnoreCase))
            {
                if (String.IsNullOrEmpty(content) || !SQLValidator.IsNonNullANDSafe(content))
                {
                    throw new InvalidInputException("Problem Source is INVALID!");
                }

                list = ProblemManager.GetProblemListBySource(content);
            }

            list = GetUserCanViewProblemList(list);

            return list;
        }

        /// <summary>
        /// 获取题目列表总数(有缓存)
        /// </summary>
        /// <returns>题目列表总数</returns>
        /// <remarks>
        /// 目前只有后台首页欢迎页面在使用该数据
        /// 前台分页使用CountProblemsByMaxID
        /// </remarks>
        public static Int32 CountProblems()
        {
            Int32 recordCount = ProblemCache.GetProblemSetCountCache();//获取缓存

            if (recordCount < 0)
            {
                recordCount = ProblemRepository.Instance.CountEntities();
                ProblemCache.SetProblemSetCountCache(recordCount);//设置缓存
            }

            return recordCount;
        }

        /// <summary>
        /// 获取题目列表总数(有缓存)
        /// </summary>
        /// <returns>获取题目列表总数</returns>
        private static Int32 CountProblemsByMaxID()
        {
            Int32 maxID = ProblemCache.GetProblemIDMaxCache();//获取缓存

            if (maxID < 0)
            {
                maxID = ProblemRepository.Instance.GetMaxProblemID();
                ProblemCache.SetProblemIDMaxCache(maxID);//设置缓存
            }

            return maxID - ConfigurationManager.ProblemSetStartID + 1;
        }

        /// <summary>
        /// 根据题目ID获取题目列表
        /// </summary>
        /// <param name="ids">逗号分隔的题目ID</param>
        /// <returns>题目列表</returns>
        private static List<ProblemEntity> GetProblemListByID(String ids)
        {
            return ProblemRepository.Instance.GetEntitiesByIDs(ids);
        }

        /// <summary>
        /// 根据题目类型ID获取题目列表
        /// </summary>
        /// <param name="typeID">题目类型ID</param>
        /// <returns>题目列表</returns>
        private static List<ProblemEntity> GetProblemListByType(Int32 typeID)
        {
            return ProblemRepository.Instance.GetEntitiesByTypeID(typeID);
        }

        /// <summary>
        /// 根据题目标题获取题目列表
        /// </summary>
        /// <param name="title">题目标题</param>
        /// <returns>题目列表</returns>
        private static List<ProblemEntity> GetProblemListByTitle(String title)
        {
            return ProblemRepository.Instance.GetEntitiesByTitle(title);
        }

        /// <summary>
        /// 根据题目来源获取题目列表
        /// </summary>
        /// <param name="source">题目来源</param>
        /// <returns>题目列表</returns>
        private static List<ProblemEntity> GetProblemListBySource(String source)
        {
            return ProblemRepository.Instance.GetEntitiesBySource(source);
        }
        #endregion

        #region 管理方法
        /// <summary>
        /// 增加一条题目
        /// </summary>
        /// <param name="entity">题目实体</param>
        /// <returns>是否成功增加</returns>
        public static IMethodResult AdminInsertProblem(ProblemEntity entity)
        {
            if (!AdminManager.HasPermission(PermissionType.ProblemManage))
            {
                throw new NoPermissionException();
            }

            if (String.IsNullOrEmpty(entity.Title))
            {
                return MethodResult.FailedAndLog("Problem title cannot be NULL!");
            }

            if (String.IsNullOrEmpty(entity.Description))
            {
                return MethodResult.FailedAndLog("Problem description cannot be NULL!");
            }

            if (String.IsNullOrEmpty(entity.Input))
            {
                return MethodResult.FailedAndLog("Problem input cannot be NULL!");
            }

            if (String.IsNullOrEmpty(entity.Output))
            {
                return MethodResult.FailedAndLog("Problem output cannot be NULL!");
            }

            if (String.IsNullOrEmpty(entity.SampleInput))
            {
                return MethodResult.FailedAndLog("Problem sample input cannot be NULL!");
            }

            if (String.IsNullOrEmpty(entity.SampleOutput))
            {
                return MethodResult.FailedAndLog("Problem sample output cannot be NULL!");
            }

            if (entity.TimeLimit <= 0)
            {
                return MethodResult.FailedAndLog("Time limit must not be less or equal than zero!");
            }

            if (entity.MemoryLimit <= 0)
            {
                return MethodResult.FailedAndLog("Memory limit must not be less or equal than zero!");
            }

            entity.IsHide = true;
            entity.LastDate = DateTime.Now;
            Boolean success = ProblemRepository.Instance.InsertEntity(entity) > 0;

            if (!success)
            {
                return MethodResult.FailedAndLog("No problem was added!");
            }

            ProblemCache.IncreaseProblemSetCountCache();//更新缓存
            ProblemCache.IncreaseProblemIDMaxCache();//更新缓存
            ProblemCache.RemoveProblemSetCache(GetProblemPageIndex(entity.ProblemID));//删除缓存

            return MethodResult.SuccessAndLog("Admin add problem, title = {0}", entity.Title);
        }

        /// <summary>
        /// 更新题目信息
        /// </summary>
        /// <param name="entity">对象实体</param>
        /// <returns>是否成功更新</returns>
        public static IMethodResult AdminUpdateProblem(ProblemEntity entity)
        {
            if (!AdminManager.HasPermission(PermissionType.ProblemManage))
            {
                throw new NoPermissionException();
            }

            if (entity.ProblemID < ConfigurationManager.ProblemSetStartID)
            {
                return MethodResult.InvalidRequst(RequestType.Problem);
            }

            if (String.IsNullOrEmpty(entity.Title))
            {
                return MethodResult.FailedAndLog("Problem title cannot be NULL!");
            }

            if (String.IsNullOrEmpty(entity.Description))
            {
                return MethodResult.FailedAndLog("Problem description cannot be NULL!");
            }

            if (String.IsNullOrEmpty(entity.Input))
            {
                return MethodResult.FailedAndLog("Problem input cannot be NULL!");
            }

            if (String.IsNullOrEmpty(entity.Output))
            {
                return MethodResult.FailedAndLog("Problem output cannot be NULL!");
            }

            if (String.IsNullOrEmpty(entity.SampleInput))
            {
                return MethodResult.FailedAndLog("Problem sample input cannot be NULL!");
            }

            if (String.IsNullOrEmpty(entity.SampleOutput))
            {
                return MethodResult.FailedAndLog("Problem sample output cannot be NULL!");
            }

            if (entity.TimeLimit <= 0)
            {
                return MethodResult.FailedAndLog("Time limit must not be less or equal than zero!");
            }

            if (entity.MemoryLimit <= 0)
            {
                return MethodResult.FailedAndLog("Memory limit must not be less or equal than zero!");
            }

            entity.LastDate = DateTime.Now;
            Boolean success = ProblemRepository.Instance.UpdateEntity(entity) > 0;

            if (!success)
            {
                return MethodResult.FailedAndLog("No problem was updated!");
            }

            ProblemCache.RemoveProblemCache(entity.ProblemID);//删除缓存
            ProblemCache.RemoveProblemSetCache(GetProblemPageIndex(entity.ProblemID));

            return MethodResult.SuccessAndLog("Admin update problem, id = {0}", entity.ProblemID.ToString());
        }

        /// <summary>
        /// 导入题目（不存在时返回null）
        /// </summary>
        /// <param name="request">Http请求</param>
        /// <param name="fileType">文件类型</param>
        /// <param name="uploadType">上传方式</param>
        /// <param name="content">文件内容</param>
        /// <param name="file">上传文件</param>
        /// <returns>题目数据是否插入成功集合（全部失败时为null）</returns>
        public static IMethodResult AdminImportProblem(HttpRequestBase request, String fileType, String uploadType, String content, HttpPostedFileBase file)
        {
            if (!AdminManager.HasPermission(PermissionType.SuperAdministrator))
            {
                throw new NoPermissionException();
            }

            if (!String.Equals("1", fileType))
            {
                return MethodResult.FailedAndLog("File type is INVALID!");
            }

            if (String.Equals("1", uploadType))//从文件上传
            {
                if (file == null)
                {
                    return MethodResult.FailedAndLog("No file was uploaded!");
                }

                StreamReader sr = new StreamReader(file.InputStream);

                content = sr.ReadToEnd();
            }

            //转换题库模型
            List<ProblemEntity> problems = null;
            List<Byte[]> datas = null;
            List<Dictionary<String, Byte[]>> images = null;
            Dictionary<String, Byte[]> imagefiles = new Dictionary<String, Byte[]>();

            if (!ProblemImport.TryImportFreeProblemSet(content, out problems, out datas, out images))
            {
                return MethodResult.FailedAndLog("File content is INVALID!");
            }

            if (problems == null || problems.Count == 0)
            {
                return MethodResult.FailedAndLog("No problem was imported!");
            }

            //处理题目及图片路径
            for (Int32 i = 0; i < problems.Count; i++)
            {
                problems[i].IsHide = true;
                problems[i].LastDate = DateTime.Now;

                if (images[i] == null)
                {
                    continue;
                }

                String uploadRoot = ConfigurationManager.UploadDirectoryUrl;

                foreach (KeyValuePair<String, Byte[]> pair in images[i])
                {
                    if (pair.Value == null || !pair.Key.Contains("."))
                    {
                        continue;
                    }
                    
                    String oldUrl = pair.Key;
                    String fileNewName = MD5Encrypt.EncryptToHexString(oldUrl + DateTime.Now.ToString("yyyyMMddHHmmssffff"), true) + pair.Key.Substring(pair.Key.LastIndexOf('.'));
                    String newUrl = uploadRoot + fileNewName;

                    problems[i].Description = problems[i].Description.Replace(oldUrl, newUrl);
                    problems[i].Input = problems[i].Input.Replace(oldUrl, newUrl);
                    problems[i].Output = problems[i].Output.Replace(oldUrl, newUrl);
                    problems[i].Hint = problems[i].Hint.Replace(oldUrl, newUrl);

                    imagefiles[fileNewName] = pair.Value;
                }
            }

            //将题目插入到数据库
            List<Int32> pids = ProblemRepository.Instance.InsertEntities(problems);

            if (pids == null || pids.Count == 0)
            {
                return MethodResult.FailedAndLog("Failed to import problem!");
            }

            //保存题目数据
            Dictionary<Int32, Boolean> dataadded = new Dictionary<Int32, Boolean>();

            for (Int32 i = 0; i < pids.Count; i++)
            {
                if (pids[i] < 0)
                {
                    continue;
                }

                try
                {
                    if (datas[i] != null)
                    {
                        ProblemDataManager.InternalAdminSaveProblemData(pids[i], datas[i]);
                        dataadded[pids[i]] = true;
                    }
                }
                catch
                {
                    dataadded[pids[i]] = false;
                }

                ProblemCache.IncreaseProblemSetCountCache();//更新缓存
                ProblemCache.IncreaseProblemIDMaxCache();//更新缓存
                ProblemCache.RemoveProblemSetCache(GetProblemPageIndex(pids[i]));//删除缓存
            }

            //保存题目图片
            foreach (KeyValuePair<String, Byte[]> pair in imagefiles)
            {
                try
                {
                    UploadsManager.InternalAdminSaveUploadFile(pair.Value, pair.Key);
                }
                catch { }
            }

            return MethodResult.SuccessAndLog<Dictionary<Int32, Boolean>>(dataadded, "Admin import problem, id = {0}", String.Join(",", pids));
        }

        /// <summary>
        /// 更新题目隐藏状态
        /// </summary>
        /// <param name="ids">题目ID列表</param>
        /// <param name="isHide">隐藏状态</param>
        /// <returns>是否成功更新</returns>
        public static IMethodResult AdminUpdateProblemIsHide(String ids, Boolean isHide)
        {
            if (!AdminManager.HasPermission(PermissionType.ProblemManage))
            {
                throw new NoPermissionException();
            }

            if (!RegexVerify.IsNumericIDs(ids))
            {
                return MethodResult.InvalidRequst(RequestType.Problem);
            }

            Boolean success = ProblemRepository.Instance.UpdateEntityIsHide(ids, isHide) > 0;

            if (!success)
            {
                return MethodResult.FailedAndLog("No problem was {0}!", isHide ? "hided" : "unhided");
            }

            ids.ForEachInIDs(',', id => 
            {
                ProblemCache.RemoveProblemCache(id);//删除缓存
                ProblemCache.RemoveProblemSetCache(GetProblemPageIndex(id));//删除缓存
            });

            return MethodResult.SuccessAndLog("Admin {1} problem, id = {0}", ids, isHide ? "hide" : "unhide");
        }

        /// <summary>
        /// 更新题目提交总数
        /// </summary>
        /// <param name="problemID">题目ID</param>
        /// <returns>是否成功更新</returns>
        public static IMethodResult AdminUpdateProblemSubmitCount(Int32 problemID)
        {
            if (!AdminManager.HasPermission(PermissionType.ProblemManage))
            {
                throw new NoPermissionException();
            }

            if (problemID < ConfigurationManager.ProblemSetStartID)
            {
                return MethodResult.InvalidRequst(RequestType.Problem);
            }

            Boolean success = ProblemRepository.Instance.UpdateEntitySubmitCount(problemID) > 0;

            if (!success)
            {
                return MethodResult.FailedAndLog("No problem's submit count was recalculated!");
            }

            ProblemCache.RemoveProblemCache(problemID);//删除缓存
            ProblemCache.RemoveProblemSetCache(GetProblemPageIndex(problemID));//删除缓存

            return MethodResult.SuccessAndLog("Admin update problem's submit count, id = {0}", problemID.ToString());
        }

        /// <summary>
        /// 更新题目通过总数
        /// </summary>
        /// <param name="problemID">题目ID</param>
        /// <returns>是否成功更新</returns>
        public static IMethodResult AdminUpdateProblemSolvedCount(Int32 problemID)
        {
            if (!AdminManager.HasPermission(PermissionType.ProblemManage))
            {
                throw new NoPermissionException();
            }

            if (problemID < ConfigurationManager.ProblemSetStartID)
            {
                return MethodResult.InvalidRequst(RequestType.Problem);
            }

            Boolean success = ProblemRepository.Instance.UpdateEntitySolvedCount(problemID) > 0;

            if (!success)
            {
                return MethodResult.FailedAndLog("No problem's solved count was recalculated!");
            }

            ProblemCache.RemoveProblemCache(problemID);//删除缓存
            ProblemCache.RemoveProblemSetCache(GetProblemPageIndex(problemID));//删除缓存

            return MethodResult.SuccessAndLog("Admin update problem's solved count, id = {0}", problemID.ToString());
        }

        /// <summary>
        /// 根据ID得到一个题目实体
        /// </summary>
        /// <param name="id">题目ID</param>
        /// <returns>题目实体</returns>
        public static ProblemEntity AdminGetProblem(Int32 id)
        {
            if (!AdminManager.HasPermission(PermissionType.ProblemManage))
            {
                throw new NoPermissionException();
            }

            if (id < ConfigurationManager.ProblemSetStartID)
            {
                throw new InvalidRequstException(RequestType.Problem);
            }

            ProblemEntity problem = ProblemRepository.Instance.GetEntity(id);

            return problem;
        }

        /// <summary>
        /// 获取题目列表
        /// </summary>
        /// <param name="pageIndex">页面索引</param>
        /// <returns>题目列表</returns>
        public static PagedList<ProblemEntity> AdminGetProblemList(Int32 pageIndex)
        {
            if (!AdminManager.HasPermission(PermissionType.ProblemManage))
            {
                throw new NoPermissionException();
            }

            Int32 pageSize = AdminManager.ADMIN_LIST_PAGE_SIZE;
            Int32 recordCount = ProblemManager.AdminCountProblems();

            return ProblemRepository.Instance
                .GetEntities(pageIndex, pageSize, recordCount)
                .ToPagedList(pageSize, recordCount);
        }

        /// <summary>
        /// 获取题目总数
        /// </summary>
        /// <returns>题目总数</returns>
        private static Int32 AdminCountProblems()
        {
            return ProblemRepository.Instance.CountEntities();
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 获取指定题目属于题目列表第几页
        /// </summary>
        /// <param name="pid">题目ID</param>
        /// <returns>题目列表第几页</returns>
        private static Int32 GetProblemPageIndex(Int32 pid)
        {
            Int32 pageIndex = (pid - ConfigurationManager.ProblemSetStartID) / PROBLEM_SET_PAGE_SIZE + 1;

            return pageIndex;
        }
        #endregion
    }
}