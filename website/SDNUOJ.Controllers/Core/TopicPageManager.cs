using System;
using System.Web;

using SDNUOJ.Caching;
using SDNUOJ.Controllers.Exception;
using SDNUOJ.Data;
using SDNUOJ.Entity;
using SDNUOJ.Logging;
using SDNUOJ.Utilities;
using SDNUOJ.Utilities.Text.RegularExpressions;

namespace SDNUOJ.Controllers.Core
{
    /// <summary>
    /// 主题页面数据管理类
    /// </summary>
    internal static class TopicPageManager
    {
        #region 用户方法
        /// <summary>
        /// 根据ID得到一个主题页面实体
        /// </summary>
        /// <param name="name">页面名称</param>
        /// <returns>主题页面实体</returns>
        public static TopicPageEntity GetTopicPage(String name)
        {
            if (!RegexVerify.IsPageName(name))
            {
                throw new InvalidRequstException(RequestType.TopicPage);
            }

            TopicPageEntity topicpage = TopicPageCache.GetTopicPageCache(name);//读取缓存

            if (topicpage == null)
            {
                topicpage = TopicPageManager.GetReplacedContent(TopicPageRepository.Instance.GetEntity(name));
                TopicPageCache.SetTopicPageCache(topicpage);//设置缓存
            }

            if (topicpage == null)
            {
                throw new NullResponseException(RequestType.TopicPage);
            }

            if (topicpage.IsHide && !AdminManager.HasPermission(PermissionType.SuperAdministrator))
            {
                throw new NoPermissionException();
            }

            return topicpage;
        }
        #endregion

        #region 管理方法
        /// <summary>
        /// 增加一条主题页面
        /// </summary>
        /// <param name="model">对象实体</param>
        /// <returns>是否成功增加</returns>
        public static IMethodResult AdminInsertTopicPage(TopicPageEntity entity)
        {
            if (!AdminManager.HasPermission(PermissionType.SuperAdministrator))
            {
                throw new NoPermissionException();
            }

            if (!RegexVerify.IsPageName(entity.PageName))
            {
                return MethodResult.InvalidRequest(RequestType.TopicPage);
            }

            if (String.IsNullOrEmpty(entity.Title))
            {
                return MethodResult.FailedAndLog("Page title can not be NULL!");
            }

            if (String.IsNullOrEmpty(entity.Description))
            {
                return MethodResult.FailedAndLog("Page description can not be NULL!");
            }

            if (String.IsNullOrEmpty(entity.Content))
            {
                return MethodResult.FailedAndLog("Page content can not be NULL!");
            }

            entity.CreateUser = UserManager.CurrentUserName;
            entity.IsHide = true;
            entity.LastDate = DateTime.Now;

            Boolean success = TopicPageRepository.Instance.InsertEntity(entity) > 0;

            if (!success)
            {
                return MethodResult.FailedAndLog("No page was added!");
            }

            return MethodResult.SuccessAndLog("Admin add page, name = {0}", entity.PageName);
        }

        /// <summary>
        /// 更新一条主题页面
        /// </summary>
        /// <param name="entity">对象实体</param>
        /// <param name="oldname">旧的主题页面名</param>
        /// <returns>是否成功更新</returns>
        public static IMethodResult AdminUpdateTopicPage(TopicPageEntity entity, String oldname)
        {
            if (!AdminManager.HasPermission(PermissionType.SuperAdministrator))
            {
                throw new NoPermissionException();
            }

            if (!RegexVerify.IsPageName(entity.PageName))
            {
                return MethodResult.InvalidRequest(RequestType.TopicPage);
            }

            if (String.IsNullOrEmpty(entity.Title))
            {
                return MethodResult.FailedAndLog("Page title can not be NULL!");
            }

            if (String.IsNullOrEmpty(entity.Description))
            {
                return MethodResult.FailedAndLog("Page description can not be NULL!");
            }

            if (String.IsNullOrEmpty(entity.Content))
            {
                return MethodResult.FailedAndLog("Page content can not be NULL!");
            }

            entity.LastDate = DateTime.Now;

            Boolean success = TopicPageRepository.Instance.UpdateEntity(entity, oldname) > 0;

            if (!success)
            {
                return MethodResult.FailedAndLog("No page was updated!");
            }

            TopicPageCache.RemoveTopicPageCache(oldname);//更新缓存

            return MethodResult.SuccessAndLog("Admin update page, name = {0}{1}", entity.PageName, 
                (!String.Equals(oldname, entity.PageName, StringComparison.OrdinalIgnoreCase) ? ", previous = " + oldname : ""));
        }

        /// <summary>
        /// 更新主题页面隐藏状态
        /// </summary>
        /// <param name="id">主题页面名称</param>
        /// <param name="isHide">隐藏状态</param>
        /// <returns>是否成功更新</returns>
        public static IMethodResult AdminUpdateTopicPageIsHide(String name, Boolean isHide)
        {
            if (!AdminManager.HasPermission(PermissionType.SuperAdministrator))
            {
                throw new NoPermissionException();
            }

            Boolean success = TopicPageRepository.Instance.UpdateEntityIsHide(name, isHide) > 0;

            if (!success)
            {
                return MethodResult.FailedAndLog("No page was {0}!", isHide ? "hided" : "unhided");
            }

            TopicPageCache.RemoveTopicPageCache(name);//删除缓存

            return MethodResult.SuccessAndLog("Admin {1} page, name = {0}", name, isHide ? "hide" : "unhide");
        }

        /// <summary>
        /// 删除指定ID的主题页面
        /// </summary>
        /// <param name="ids">逗号分隔的主题页面ID</param>
        /// <returns>是否成功删除</returns>
        public static IMethodResult AdminDeleteTopicPages(String names)
        {
            if (!AdminManager.HasPermission(PermissionType.SuperAdministrator))
            {
                throw new NoPermissionException();
            }

            Boolean success = TopicPageRepository.Instance.DeleteEntities(names) > 0;

            if (!success)
            {
                return MethodResult.FailedAndLog("No page was deleted!");
            }

            names.ForEachInIDs(',', name => 
            {
                TopicPageCache.RemoveTopicPageCache(name);//删除缓存
            });

            return MethodResult.SuccessAndLog("Admin delete page, name = {0}", names);
        }

        /// <summary>
        /// 根据ID得到一个主题页面实体
        /// </summary>
        /// <param name="id">主题页面ID</param>
        /// <returns>主题页面实体</returns>
        public static IMethodResult AdminGetTopicPage(String name)
        {
            if (!AdminManager.HasPermission(PermissionType.SuperAdministrator))
            {
                throw new NoPermissionException();
            }

            if (!RegexVerify.IsPageName(name))
            {
                return MethodResult.InvalidRequest(RequestType.TopicPage);
            }

            TopicPageEntity entity = TopicPageRepository.Instance.GetEntity(name);

            if (entity == null)
            {
                return MethodResult.NotExist(RequestType.TopicPage);
            }

            return MethodResult.Success(entity);
        }

        /// <summary>
        /// 获取主题页面列表
        /// </summary>
        /// <param name="pageIndex">页面索引</param>
        /// <returns>主题页面列表</returns>
        public static PagedList<TopicPageEntity> AdminGetTopicPageList(Int32 pageIndex)
        {
            if (!AdminManager.HasPermission(PermissionType.SuperAdministrator))
            {
                throw new NoPermissionException();
            }

            Int32 pageSize = AdminManager.ADMIN_LIST_PAGE_SIZE;
            Int32 recordCount = TopicPageManager.AdminCountTopicPages();

            return TopicPageRepository.Instance
                .GetEntities(pageIndex, pageSize, recordCount)
                .ToPagedList(pageSize, recordCount);
        }

        /// <summary>
        /// 获取主题页面总数(无缓存)
        /// </summary>
        /// <returns>主题页面总数</returns>
        private static Int32 AdminCountTopicPages()
        {
            return TopicPageRepository.Instance.CountEntities();
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 获取替换标签的内容
        /// </summary>
        /// <param name="topicpage">原始内容</param>
        /// <returns>替换标签的内容</returns>
        private static TopicPageEntity GetReplacedContent(TopicPageEntity entity)
        {
            if (entity == null)
            {
                return null;
            }

            entity.Content = entity.Content.Replace("{$Title}", entity.Title);
            entity.Content = entity.Content.Replace("{$Description}", entity.Description);
            entity.Content = entity.Content.Replace("{$CreateUser}", entity.CreateUser);
            entity.Content = entity.Content.Replace("{$LastDate}", entity.LastDate.ToString("yyyy-MM-dd HH:mm:ss"));
            entity.Content = entity.Content.Replace("{$Now}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            return entity;
        }
        #endregion
    }
}