using System;
using System.Collections.Generic;
using System.Web;

using SDNUOJ.Caching;
using SDNUOJ.Controllers.Exception;
using SDNUOJ.Controllers.Logging;
using SDNUOJ.Data;
using SDNUOJ.Entity;
using SDNUOJ.Utilities;
using SDNUOJ.Utilities.Text.RegularExpressions;

namespace SDNUOJ.Controllers.Core
{
    /// <summary>
    /// 题目类型种类数据管理类
    /// </summary>
    internal static class ProblemCategoryManager
    {
        #region 用户方法
        ///// <summary>
        ///// 根据ID得到一个题目类型种类实体
        ///// </summary>
        ///// <param name="id">题目类型种类ID</param>
        ///// <returns>题目类型种类实体</returns>
        //public static ProblemCategoryEntity GetProblemCategory(Int32 id)
        //{
        //    if (id <= 0)
        //    {
        //        throw new InvalidRequstException(RequestType.ProblemCategory);
        //    }

        //    List<ProblemCategoryEntity> list = ProblemCategoryManager.GetProblemCategoryList();

        //    if (list != null && list.Count > 0)
        //    {
        //        for (Int32 i = 0; i < list.Count; i++)
        //        {
        //            if (list[i].TypeID == id) return list[i];
        //        }
        //    }

        //    throw new NullResponseException(RequestType.ProblemCategory);
        //}

        /// <summary>
        /// 获取所有题目类型种类
        /// </summary>
        /// <returns>所有题目类型种类</returns>
        public static List<ProblemCategoryEntity> GetProblemCategoryList()
        {
            List<ProblemCategoryEntity> list = ProblemCategoryCache.GetProblemCategoryListCache();//获取缓存

            if (list == null)
            {
                list = ProblemCategoryRepository.Instance.GetAllEntities();
                ProblemCategoryCache.SetProblemCategoryListCache(list);//设置缓存
            }

            return list;
        }
        #endregion

        #region 管理方法
        /// <summary>
        /// 增加一条题目类型种类
        /// </summary>
        /// <param name="model">题目类型种类实体</param>
        /// <returns>是否成功增加</returns>
        public static Boolean AdminInsertProblemCategory(ProblemCategoryEntity model)
        {
            if (!AdminManager.HasPermission(PermissionType.ProblemManage))
            {
                throw new NoPermissionException();
            }

            if (String.IsNullOrEmpty(model.Title))
            {
                throw new InvalidInputException("Problem category title cannot be NULL!");
            }

            Boolean success = ProblemCategoryRepository.Instance.InsertEntity(model) > 0;

            if (success)
            {
                LogManager.LogOperation(HttpContext.Current, UserManager.CurrentUserName, String.Format("Admin Insert Problem Category, Title = \"{0}\"", model.Title));
                ProblemCategoryCache.RemoveProblemCategoryListCache();//删除缓存
            }

            return success;
        }

        /// <summary>
        /// 更新一条题目类型种类
        /// </summary>
        /// <param name="model">题目类型种类实体</param>
        /// <returns>是否成功更新</returns>
        public static Boolean AdminUpdateProblemCategory(ProblemCategoryEntity model)
        {
            if (!AdminManager.HasPermission(PermissionType.ProblemManage))
            {
                throw new NoPermissionException();
            }

            if (String.IsNullOrEmpty(model.Title))
            {
                throw new InvalidInputException("Problem category title cannot be NULL!");
            }

            Boolean success = ProblemCategoryRepository.Instance.UpdateEntity(model) > 0;

            if (success)
            {
                LogManager.LogOperation(HttpContext.Current, UserManager.CurrentUserName, String.Format("Admin Update Problem Category, ID = {0}", model.TypeID));
                ProblemCategoryCache.RemoveProblemCategoryListCache();//删除缓存
            }

            return success;
        }

        /// <summary>
        /// 删除指定ID的题目类型种类
        /// </summary>
        /// <param name="id">题目类型种类ID</param>
        /// <returns>是否成功删除</returns>
        public static Boolean AdminDeleteProblemCategory(Int32 id)
        {
            if (!AdminManager.HasPermission(PermissionType.ProblemManage))
            {
                throw new NoPermissionException();
            }
            
            if (id <= 0)
            {
                throw new InvalidRequstException(RequestType.ProblemCategory);
            }

            if (ProblemCategoryItemRepository.Instance.CountEntities(id) > 0)
            {
                throw new OperationFailedException("This category still has some problems, please remove these problem from this category first!");
            }

            Boolean success = ProblemCategoryRepository.Instance.DeleteEntity(id) > 0;

            if (success)
            {
                LogManager.LogOperation(HttpContext.Current, UserManager.CurrentUserName, String.Format("Admin Delete Problem Category, ID = {0}", id));
                ProblemCategoryCache.RemoveProblemCategoryListCache();//删除缓存
            }

            return success;
        }

        /// <summary>
        /// 根据ID得到题目类型种类实体
        /// </summary>
        /// <param name="id">题目类型种类ID</param>
        /// <returns>题目类型种类实体</returns>
        public static ProblemCategoryEntity AdminGetProblemCategory(Int32 id)
        {
            if (!AdminManager.HasPermission(PermissionType.ProblemManage))
            {
                throw new NoPermissionException();
            }

            if (id <= 0)
            {
                throw new InvalidRequstException(RequestType.ProblemCategory);
            }

            ProblemCategoryEntity entity = ProblemCategoryRepository.Instance.GetEntity(id);

            if (entity == null)
            {
                throw new NullResponseException(RequestType.ProblemCategory);
            }

            return entity;
        }

        /// <summary>
        /// 获取题目类型种类列表
        /// </summary>
        /// <param name="pageIndex">页面索引</param>
        /// <returns>题目类型种类列表</returns>
        public static PagedList<ProblemCategoryEntity> AdminGetProblemCategoryList(Int32 pageIndex)
        {
            if (!AdminManager.HasPermission(PermissionType.ProblemManage))
            {
                throw new NoPermissionException();
            }

            Int32 pageSize = AdminManager.ADMIN_LIST_PAGE_SIZE;
            Int32 recordCount = ProblemCategoryManager.AdminCountProblemCategoryList();

            return ProblemCategoryRepository.Instance
                .GetEntities(pageIndex, pageSize, recordCount)
                .ToPagedList(pageSize, recordCount);
        }

        /// <summary>
        /// 获取题目类型种类总数
        /// </summary>
        /// <returns>题目类型种类总数</returns>
        private static Int32 AdminCountProblemCategoryList()
        {
            return ProblemCategoryRepository.Instance.CountEntities();
        }
        #endregion
    }
}