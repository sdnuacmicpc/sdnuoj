using System;
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
        public static Boolean AdminInsertTopicPage(TopicPageEntity entity)
        {
            if (!AdminManager.HasPermission(PermissionType.SuperAdministrator))
            {
                throw new NoPermissionException();
            }

            if (!RegexVerify.IsPageName(entity.PageName))
            {
                throw new InvalidRequstException(RequestType.TopicPage);
            }

            if (String.IsNullOrEmpty(entity.Title))
            {
                throw new InvalidInputException("Page title can not be NULL!");
            }

            if (String.IsNullOrEmpty(entity.Description))
            {
                throw new InvalidInputException("Page description can not be NULL!");
            }

            if (String.IsNullOrEmpty(entity.Content))
            {
                throw new InvalidInputException("Page content can not be NULL!");
            }

            entity.CreateUser = UserManager.CurrentUserName;
            entity.IsHide = true;
            entity.LastDate = DateTime.Now;

            Boolean success = TopicPageRepository.Instance.InsertEntity(entity) > 0;

            if (success)
            {
                LogManager.LogOperation(HttpContext.Current, UserManager.CurrentUserName, String.Format("Admin Insert TopicPage, Page Name = \"{0}\"", entity.PageName));
            }

            return success;
        }

        /// <summary>
        /// 更新一条主题页面
        /// </summary>
        /// <param name="entity">对象实体</param>
        /// <param name="oldname">旧的主题页面名</param>
        /// <returns>是否成功更新</returns>
        public static Boolean AdminUpdateTopicPage(TopicPageEntity entity, String oldname)
        {
            if (!AdminManager.HasPermission(PermissionType.SuperAdministrator))
            {
                throw new NoPermissionException();
            }

            if (!RegexVerify.IsPageName(entity.PageName))
            {
                throw new InvalidRequstException(RequestType.TopicPage);
            }

            if (String.IsNullOrEmpty(entity.Title))
            {
                throw new InvalidInputException("Page title can not be NULL!");
            }

            if (String.IsNullOrEmpty(entity.Description))
            {
                throw new InvalidInputException("Page description can not be NULL!");
            }

            if (String.IsNullOrEmpty(entity.Content))
            {
                throw new InvalidInputException("Page content can not be NULL!");
            }

            entity.LastDate = DateTime.Now;

            Boolean success = TopicPageRepository.Instance.UpdateEntity(entity, oldname) > 0;

            if (success)
            {
                LogManager.LogOperation(HttpContext.Current, UserManager.CurrentUserName, String.Format("Admin Update TopicPage, Page Name = {0}", entity.PageName));
                TopicPageCache.RemoveTopicPageCache(oldname);//更新缓存
            }

            return success;
        }

        /// <summary>
        /// 更新主题页面隐藏状态
        /// </summary>
        /// <param name="id">主题页面名称</param>
        /// <param name="isHide">隐藏状态</param>
        /// <returns>是否成功更新</returns>
        public static Boolean AdminUpdateTopicPageIsHide(String name, Boolean isHide)
        {
            if (!AdminManager.HasPermission(PermissionType.SuperAdministrator))
            {
                throw new NoPermissionException();
            }

            Boolean success = TopicPageRepository.Instance.UpdateEntityIsHide(name, isHide) > 0;

            if (success)
            {
                LogManager.LogOperation(HttpContext.Current, UserManager.CurrentUserName, String.Format("Admin {0} TopicPage, Page Name = {1}", (isHide ? "Hide" : "Unhide"), name));
                TopicPageCache.RemoveTopicPageCache(name);//删除缓存
            }

            return success;
        }

        /// <summary>
        /// 删除指定ID的主题页面
        /// </summary>
        /// <param name="ids">逗号分隔的主题页面ID</param>
        /// <returns>是否成功删除</returns>
        public static Boolean AdminDeleteTopicPages(String names)
        {
            if (!AdminManager.HasPermission(PermissionType.SuperAdministrator))
            {
                throw new NoPermissionException();
            }

            String[] arrnames = names.Split(',');
            Boolean success = TopicPageRepository.Instance.DeleteEntities(names) > 0;

            if (success)
            {
                LogManager.LogOperation(HttpContext.Current, UserManager.CurrentUserName, String.Format("Admin Delete TopicPage, Names in ({0})", names));

                for (Int32 i = 0; i < arrnames.Length; i++)
                {
                    if (!String.IsNullOrEmpty(arrnames[i]))
                    {
                        TopicPageCache.RemoveTopicPageCache(arrnames[i]);//删除缓存
                    }
                }
            }

            return success;
        }

        /// <summary>
        /// 根据ID得到一个主题页面实体
        /// </summary>
        /// <param name="id">主题页面ID</param>
        /// <returns>主题页面实体</returns>
        public static TopicPageEntity AdminGetTopicPage(String name)
        {
            if (!AdminManager.HasPermission(PermissionType.SuperAdministrator))
            {
                throw new NoPermissionException();
            }

            if (!RegexVerify.IsPageName(name))
            {
                throw new InvalidRequstException(RequestType.TopicPage);
            }

            TopicPageEntity entity = TopicPageRepository.Instance.GetEntity(name);

            if (entity == null)
            {
                throw new NullResponseException(RequestType.TopicPage);
            }

            return entity;
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