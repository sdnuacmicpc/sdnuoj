using System;
using System.Collections.Generic;
using System.Web;

using SDNUOJ.Caching;
using SDNUOJ.Controllers.Exception;
using SDNUOJ.Data;
using SDNUOJ.Entity;
using SDNUOJ.Utilities;
using SDNUOJ.Utilities.Text.RegularExpressions;

namespace SDNUOJ.Controllers.Core
{
    /// <summary>
    /// 公告数据管理类
    /// </summary>
    internal static class NewsManager
    {
        #region 常量
        /// <summary>
        /// 首页显示的公告数量
        /// </summary>
        private const Int32 HOMEPAGE_PAGE_SIZE = 5;

        /// <summary>
        /// 更多公告的公告数量
        /// </summary>
        private const Int32 NEWSLIST_PAGE_SIZE = 20;
        #endregion

        #region 用户方法
        /// <summary>
        /// 根据ID得到一个公告实体
        /// </summary>
        /// <param name="id">公告ID</param>
        /// <returns>公告实体</returns>
        public static NewsEntity GetNews(Int32 id)
        {
            if (id <= 0)
            {
                throw new InvalidRequstException(RequestType.News);
            }

            NewsEntity entity = NewsCache.GetNewsCache(id);//读取缓存

            if (entity == null)
            {
                entity = NewsRepository.Instance.GetEntity(id);
                NewsCache.SetNewsCache(entity);//设置缓存
            }

            if (entity == null)
            {
                throw new NullResponseException(RequestType.News);
            }

            return entity;
        }

        /// <summary>
        /// 获取默认公告（不存在时返回null）
        /// </summary>
        /// <returns>公告实体（可能为null）</returns>
        public static NewsEntity GetDefaultNews()
        {
            try
            {
                return NewsManager.GetNews(NewsRepository.DEFAULTID);
            }
            catch (NullResponseException)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取最近公告
        /// </summary>
        /// <param name="count">获取公告数</param>
        /// <returns>最近公告</returns>
        public static List<NewsEntity> GetLastestNewsList()
        {
            List<NewsEntity> list = NewsCache.GetLastestNewsListCache();//获取缓存

            if (list == null)
            {
                list = NewsRepository.Instance.GetLastEntities(HOMEPAGE_PAGE_SIZE);

                NewsCache.SetLastestNewsListCache(list);//设置缓存
            }

            return list;
        }

        /// <summary>
        /// 获取公告列表
        /// </summary>
        /// <param name="pageIndex">页面索引</param>
        /// <returns>公告列表</returns>
        public static PagedList<NewsEntity> GetNewsList(Int32 pageIndex)
        {
            Int32 pageSize = NewsManager.NEWSLIST_PAGE_SIZE;
            Int32 recordCount = NewsManager.CountNews();

            return NewsRepository.Instance
                .GetEntities(pageIndex, pageSize, recordCount, false)
                .ToPagedList(pageSize, recordCount);
        }

        /// <summary>
        /// 获取公告总数(有缓存)
        /// </summary>
        /// <returns>公告总数</returns>
        private static Int32 CountNews()
        {
            Int32 recordCount = NewsCache.GetNewsCountCache();//获取缓存

            if (recordCount < 0)
            {
                recordCount = NewsRepository.Instance.CountEntities() - 1;
                NewsCache.SetNewsCountCache(recordCount);//设置缓存
            }

            return recordCount;
        }
        #endregion

        #region 管理方法
        /// <summary>
        /// 增加一条公告
        /// </summary>
        /// <param name="entity">对象实体</param>
        /// <returns>是否成功增加</returns>
        public static IMethodResult AdminInsertNews(NewsEntity entity)
        {
            if (!AdminManager.HasPermission(PermissionType.NewsManage))
            {
                throw new NoPermissionException();
            }

            if (String.IsNullOrEmpty(entity.Title))
            {
                return MethodResult.FailedAndLog("News title can not be NULL!");
            }

            if (String.IsNullOrEmpty(entity.Description))
            {
                return MethodResult.FailedAndLog("News content can not be NULL!");
            }

            entity.PublishDate = DateTime.Now;

            Boolean success = NewsRepository.Instance.InsertEntity(entity) > 0;

            if (!success)
            {
                return MethodResult.FailedAndLog("No news was added!");
            }

            NewsCache.RemoveLastestNewsListCache();//删除缓存
            NewsCache.RemoveNewsCountCache();//删除缓存

            return MethodResult.SuccessAndLog("Admin add news, title = {0}", entity.Title);
        }

        /// <summary>
        /// 更新一条公告
        /// </summary>
        /// <param name="entity">对象实体</param>
        /// <returns>是否成功更新</returns>
        public static IMethodResult AdminUpdateNews(NewsEntity entity)
        {
            if (!AdminManager.HasPermission(PermissionType.NewsManage))
            {
                throw new NoPermissionException();
            }

            if (String.IsNullOrEmpty(entity.Title))
            {
                return MethodResult.FailedAndLog("News title can not be NULL!");
            }

            if (String.IsNullOrEmpty(entity.Description))
            {
                return MethodResult.FailedAndLog("News content can not be NULL!");
            }

            entity.PublishDate = DateTime.Now;

            Boolean success = NewsRepository.Instance.UpdateEntity(entity) > 0;

            if (!success)
            {
                return MethodResult.FailedAndLog("No news was updated!");
            }

            NewsCache.SetNewsCache(entity);//更新缓存

            if (entity.AnnounceID != NewsRepository.DEFAULTID)
            {
                NewsCache.RemoveLastestNewsListCache();//删除缓存
            }

            return MethodResult.SuccessAndLog("Admin update news, id = {0}", entity.AnnounceID.ToString());
        }

        /// <summary>
        /// 删除指定ID的公告
        /// </summary>
        /// <param name="ids">逗号分隔的公告ID</param>
        /// <returns>是否成功删除</returns>
        public static IMethodResult AdminDeleteNews(String ids)
        {
            if (!AdminManager.HasPermission(PermissionType.NewsManage))
            {
                throw new NoPermissionException();
            }

            if (!RegexVerify.IsNumericIDs(ids))
            {
                return MethodResult.InvalidRequst(RequestType.News);
            }

            String[] arrids = ids.Split(',');
            String defaultID = NewsRepository.DEFAULTID.ToString();

            for (Int32 i = 0; i < arrids.Length; i++)
            {
                if (String.Equals(arrids[i], defaultID))
                {
                    return MethodResult.FailedAndLog("Can not delete the default news!");
                }
            }

            Boolean success = NewsRepository.Instance.DeleteEntities(ids) > 0;

            if (!success)
            {
                return MethodResult.FailedAndLog("No news was deleted!");
            }

            ids.ForEachInIDs(',', id => 
            {
                NewsCache.RemoveNewsCache(id);//删除缓存
            });
            NewsCache.RemoveLastestNewsListCache();//删除缓存
            NewsCache.RemoveNewsCountCache();//删除缓存

            return MethodResult.SuccessAndLog("Admin delete news, id = {0}", ids);
        }

        /// <summary>
        /// 根据ID得到一个公告实体
        /// </summary>
        /// <param name="id">公告ID</param>
        /// <returns>公告实体</returns>
        public static NewsEntity AdminGetNews(Int32 id)
        {
            if (!AdminManager.HasPermission(PermissionType.NewsManage))
            {
                throw new NoPermissionException();
            }

            if (id <= 0)
            {
                throw new InvalidRequstException(RequestType.News);
            }

            NewsEntity entity = NewsRepository.Instance.GetEntity(id);

            if (entity == null)
            {
                throw new NullResponseException(RequestType.News);
            }

            return entity;
        }

        /// <summary>
        /// 获取公告列表
        /// </summary>
        /// <param name="pageIndex">页面索引</param>
        /// <returns>公告列表</returns>
        public static PagedList<NewsEntity> AdminGetNewsList(Int32 pageIndex)
        {
            if (!AdminManager.HasPermission(PermissionType.NewsManage))
            {
                throw new NoPermissionException();
            }

            Int32 pageSize = AdminManager.ADMIN_LIST_PAGE_SIZE;
            Int32 recordCount = NewsManager.AdminCountNews();

            return NewsRepository.Instance
                .GetEntities(pageIndex, pageSize, recordCount, true)
                .ToPagedList(pageSize, recordCount);
        }

        /// <summary>
        /// 获取公告总数(无缓存)
        /// </summary>
        /// <returns>公告总数</returns>
        private static Int32 AdminCountNews()
        {
            return NewsRepository.Instance.CountEntities();
        }
        #endregion
    }
}