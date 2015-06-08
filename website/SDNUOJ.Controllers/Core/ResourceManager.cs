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
    /// 资源数据管理类
    /// </summary>
    internal static class ResourceManager
    {
        #region 用户方法
        /// <summary>
        /// 获取树形资源列表
        /// </summary>
        /// <returns>树形资源列表</returns>
        public static List<TreeNode<ResourceEntity>> GetResourcesTreeList()
        {
            if (!UserManager.IsUserLogined)
            {
                throw new UserUnLoginException();
            }

            List<ResourceEntity> listResource = ResourceManager.GetResources();

            if (listResource == null || listResource.Count == 0)
            {
                return new List<TreeNode<ResourceEntity>>();
            }

            List<TreeNode<ResourceEntity>> lstTreeNode = new List<TreeNode<ResourceEntity>>();
            TreeNode<ResourceEntity> lastNode = null;
            String lastType = String.Empty;

            for (Int32 i = 0; i < listResource.Count; i++)
            {
                if (!String.Equals(lastType, listResource[i].Type))//如果与上一个类型不同则创建新根节点
                {
                    if (lastNode != null)
                    {
                        lstTreeNode.Add(lastNode);
                    }

                    lastType = listResource[i].Type;
                    lastNode = new TreeNode<ResourceEntity>(listResource[i].Type, new ResourceEntity() { ResourceID = i, Title = lastType });
                    i--;
                }
                else
                {
                    if (lastNode != null)
                    {
                        lastNode.AddNote(new TreeNode<ResourceEntity>(listResource[i].Type, listResource[i]));
                    }
                }
            }

            if (lastNode != null)//加入最后一个根节点
            {
                lstTreeNode.Add(lastNode);
            }

            return lstTreeNode;
        }

        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <returns>实体列表</returns>
        private static List<ResourceEntity> GetResources()
        {
            List<ResourceEntity> list = ResourceCache.GetResourceListCache();//读取缓存

            if (list == null)
            {
                list = ResourceRepository.Instance.GetAllEntities();
                ResourceCache.SetResourceListCache(list);//设置缓存
            }

            return list;
        }
        #endregion

        #region 管理方法
        /// <summary>
        /// 增加一条数据
        /// </summary>
        /// <param name="entity">对象实体</param>
        /// <returns>是否成功增加</returns>
        public static IMethodResult AdminInsertResource(ResourceEntity entity)
        {
            if (!AdminManager.HasPermission(PermissionType.ResourceManage))
            {
                throw new NoPermissionException();
            }

            if (String.IsNullOrEmpty(entity.Title))
            {
                return MethodResult.FailedAndLog("Resource title can not be NULL!");
            }

            if (String.IsNullOrEmpty(entity.Url))
            {
                return MethodResult.FailedAndLog("Resource url can not be NULL!");
            }

            if (String.IsNullOrEmpty(entity.Type))
            {
                return MethodResult.FailedAndLog("Resource type can not be NULL!");
            }

            Boolean success = ResourceRepository.Instance.InsertEntity(entity) > 0;

            if (!success)
            {
                return MethodResult.FailedAndLog("No resource was added!");
            }

            ResourceCache.RemoveResourceListCache();//删除缓存

            return MethodResult.SuccessAndLog("Admin add resource, title = {0}", entity.Title);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        /// <param name="entity">对象实体</param>
        /// <returns>是否成功更新</returns>
        public static IMethodResult AdminUpdateResource(ResourceEntity entity)
        {
            if (!AdminManager.HasPermission(PermissionType.ResourceManage))
            {
                throw new NoPermissionException();
            }

            if (String.IsNullOrEmpty(entity.Title))
            {
                return MethodResult.FailedAndLog("Resource title can not be NULL!");
            }

            if (String.IsNullOrEmpty(entity.Url))
            {
                return MethodResult.FailedAndLog("Resource url can not be NULL!");
            }

            if (String.IsNullOrEmpty(entity.Type))
            {
                return MethodResult.FailedAndLog("Resource type can not be NULL!");
            }

            Boolean success = ResourceRepository.Instance.UpdateEntity(entity) > 0;

            if (!success)
            {
                return MethodResult.FailedAndLog("No resource was updated!");
            }

            ResourceCache.RemoveResourceListCache();//删除缓存

            return MethodResult.SuccessAndLog("Admin update resource, id = {0}", entity.ResourceID.ToString());
        }

        /// <summary>
        /// 删除指定ID的数据
        /// </summary>
        /// <param name="ids">逗号分隔的实体ID</param>
        /// <returns>是否成功删除</returns>
        public static IMethodResult AdminDeleteResources(String ids)
        {
            if (!AdminManager.HasPermission(PermissionType.ResourceManage))
            {
                throw new NoPermissionException();
            }

            Boolean success = ResourceRepository.Instance.DeleteEntities(ids) > 0;

            if (!success)
            {
                return MethodResult.FailedAndLog("No resource was deleted!");
            }

            ResourceCache.RemoveResourceListCache();//删除缓存

            return MethodResult.SuccessAndLog("Admin delete resource, id = {0}", ids);
        }

        /// <summary>
        /// 根据ID得到一个对象实体
        /// </summary>
        /// <param name="resourceID">实体ID</param>
        /// <returns>对象实体</returns>
        public static ResourceEntity AdminGetResource(Int32 id)
        {
            if (!AdminManager.HasPermission(PermissionType.ResourceManage))
            {
                throw new NoPermissionException();
            }

            if (id <= 0)
            {
                throw new InvalidRequstException(RequestType.Resource);
            }

            ResourceEntity entity = ResourceRepository.Instance.GetEntity(id);

            if (entity == null)
            {
                throw new InvalidRequstException(RequestType.Resource);
            }

            return entity;
        }

        /// <summary>
        /// 根据ID得到对象实体列表
        /// </summary>
        /// <param name="pageIndex">页面索引</param>
        /// <returns>对象实体列表</returns>
        public static PagedList<ResourceEntity> AdminGetResourceList(Int32 pageIndex)
        {
            if (!AdminManager.HasPermission(PermissionType.ResourceManage))
            {
                throw new NoPermissionException();
            }

            Int32 pageSize = AdminManager.ADMIN_LIST_PAGE_SIZE;
            Int32 recordCount = ResourceManager.AdminCountResources();

            return ResourceRepository.Instance
                .GetEntities(pageIndex, pageSize, recordCount)
                .ToPagedList(pageSize, recordCount);
        }

        /// <summary>
        /// 获取实体总数
        /// </summary>
        /// <returns>实体总数</returns>
        private static Int32 AdminCountResources()
        {
            return ResourceRepository.Instance.CountEntities();
        }
        #endregion
    }
}