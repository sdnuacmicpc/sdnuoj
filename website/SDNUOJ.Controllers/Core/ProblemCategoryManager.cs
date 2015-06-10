using System;
using System.Collections.Generic;

using SDNUOJ.Caching;
using SDNUOJ.Controllers.Exception;
using SDNUOJ.Data;
using SDNUOJ.Entity;
using SDNUOJ.Utilities;

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
        /// <param name="entity">题目类型种类实体</param>
        /// <returns>是否成功增加</returns>
        public static IMethodResult AdminInsertProblemCategory(ProblemCategoryEntity entity)
        {
            if (!AdminManager.HasPermission(PermissionType.ProblemManage))
            {
                throw new NoPermissionException();
            }

            if (String.IsNullOrEmpty(entity.Title))
            {
                return MethodResult.FailedAndLog("Problem category title cannot be NULL!");
            }

            Boolean success = ProblemCategoryRepository.Instance.InsertEntity(entity) > 0;

            if (!success)
            {
                return MethodResult.FailedAndLog("No problem category was added!");
            }

            ProblemCategoryCache.RemoveProblemCategoryListCache();//删除缓存

            return MethodResult.SuccessAndLog("Admin add problem category, title = {0}", entity.Title);
        }

        /// <summary>
        /// 更新一条题目类型种类
        /// </summary>
        /// <param name="entity">题目类型种类实体</param>
        /// <returns>是否成功更新</returns>
        public static IMethodResult AdminUpdateProblemCategory(ProblemCategoryEntity entity)
        {
            if (!AdminManager.HasPermission(PermissionType.ProblemManage))
            {
                throw new NoPermissionException();
            }

            if (entity.TypeID <= 0)
            {
                return MethodResult.InvalidRequest(RequestType.ProblemCategory);
            }

            if (String.IsNullOrEmpty(entity.Title))
            {
                return MethodResult.FailedAndLog("Problem category title cannot be NULL!");
            }

            Boolean success = ProblemCategoryRepository.Instance.UpdateEntity(entity) > 0;

            if (!success)
            {
                return MethodResult.FailedAndLog("No problem category was updated!");
            }

            ProblemCategoryCache.RemoveProblemCategoryListCache();//删除缓存

            return MethodResult.SuccessAndLog("Admin update problem category, id = {0}", entity.TypeID.ToString());
        }

        /// <summary>
        /// 删除指定ID的题目类型种类
        /// </summary>
        /// <param name="id">题目类型种类ID</param>
        /// <returns>是否成功删除</returns>
        public static IMethodResult AdminDeleteProblemCategory(Int32 id)
        {
            if (!AdminManager.HasPermission(PermissionType.ProblemManage))
            {
                throw new NoPermissionException();
            }
            
            if (id <= 0)
            {
                return MethodResult.InvalidRequest(RequestType.ProblemCategory);
            }

            if (ProblemCategoryItemRepository.Instance.CountEntities(id) > 0)
            {
                return MethodResult.FailedAndLog("This category still has some problems, please remove these problem from this category first!");
            }

            Boolean success = ProblemCategoryRepository.Instance.DeleteEntity(id) > 0;

            if (!success)
            {
                return MethodResult.FailedAndLog("No problem category was deleted!");
            }

            ProblemCategoryCache.RemoveProblemCategoryListCache();//删除缓存

            return MethodResult.SuccessAndLog("Admin delete problem category, id = {0}", id.ToString());
        }

        /// <summary>
        /// 根据ID得到题目类型种类实体
        /// </summary>
        /// <param name="id">题目类型种类ID</param>
        /// <returns>题目类型种类实体</returns>
        public static IMethodResult AdminGetProblemCategory(Int32 id)
        {
            if (!AdminManager.HasPermission(PermissionType.ProblemManage))
            {
                throw new NoPermissionException();
            }

            if (id <= 0)
            {
                return MethodResult.InvalidRequest(RequestType.ProblemCategory);
            }

            ProblemCategoryEntity entity = ProblemCategoryRepository.Instance.GetEntity(id);

            if (entity == null)
            {
                return MethodResult.NotExist(RequestType.ProblemCategory);
            }

            return MethodResult.Success(entity);
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