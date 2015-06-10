using System;
using System.Collections.Generic;

using SDNUOJ.Caching;
using SDNUOJ.Configuration;
using SDNUOJ.Controllers.Exception;
using SDNUOJ.Controllers.Status;
using SDNUOJ.Data;
using SDNUOJ.Entity;
using SDNUOJ.Utilities;
using SDNUOJ.Utilities.Text;
using SDNUOJ.Utilities.Text.RegularExpressions;

namespace SDNUOJ.Controllers.Core
{
    /// <summary>
    /// 论坛帖子数据管理类
    /// </summary>
    /// <remarks>
    /// HtmlEncode : 帖子标题 / 保存在数据库中
    /// HtmlEncode : 帖子内容 / 保存在数据库中
    /// </remarks>
    internal static class ForumPostManager
    {
        #region 用户方法
        /// <summary>
        /// 增加一条回帖
        /// </summary>
        /// <param name="post">帖子实体</param>
        /// <param name="topic">主题实体</param>
        /// <param name="parentPost">回复的帖子实体</param>
        /// <param name="postip">发布者IP</param>
        /// <param name="link">当前页面地址</param>
        /// <returns>是否成功增加</returns>
        public static Boolean InsertForumPost(ForumPostEntity post, ForumTopicEntity topic, ForumPostEntity parentPost, String postip, String link)
        {
            if (!UserManager.IsUserLogined)
            {
                throw new UserUnLoginException();
            }

            if (String.IsNullOrEmpty(post.Title))
            {
                throw new InvalidInputException("Reply title can not be NULL!");
            }

            if (post.Title.Length > ForumPostRepository.TITLE_MAXLEN)
            {
                throw new InvalidInputException("Reply title is too long!");
            }

            if (!KeywordsFilterManager.IsForumPostContentLegal(post.Title))
            {
                throw new InvalidInputException("Reply title can not contain illegal keywords!");
            }

            if (String.IsNullOrEmpty(post.Content) || post.Content.Length < ForumPostRepository.POST_MINLEN)
            {
                throw new InvalidInputException("Reply content is too short!");
            }

            if (post.Content.Length > ForumPostRepository.POST_MAXLEN)
            {
                throw new InvalidInputException("Reply content is too long!");
            }

            if (!KeywordsFilterManager.IsForumPostContentLegal(post.Content))
            {
                throw new InvalidInputException("Reply content can not contain illegal keywords!");
            }

            if (parentPost.Deepth + 1 < ForumPostRepository.DEEPTH_MIN)
            {
                throw new InvalidInputException("Reply deepth is INVALID!");
            }

            if (parentPost.Deepth + 1 > ForumPostRepository.DEEPTH_MAX)
            {
                throw new InvalidInputException("Reply deepth is too deep!");
            }

            if (topic.IsLocked)
            {
                throw new NoPermissionException("You have no privilege to reply the post!");
            }

            if (!UserSubmitStatus.CheckLastSubmitForumPostTime(UserManager.CurrentUserName))
            {
                throw new InvalidInputException(String.Format("You can not submit post more than twice in {0} seconds!", ConfigurationManager.SubmitInterval.ToString()));
            }

            post.TopicID = parentPost.TopicID;
            post.Title = HtmlEncoder.HtmlEncode(post.Title);
            post.Content = HtmlEncoder.HtmlEncode(post.Content);
            post.UserName = UserManager.CurrentUserName;
            post.Deepth = parentPost.Deepth + 1;
            post.ParentPostID = parentPost.PostID;
            post.PostDate = DateTime.Now;
            post.PostIP = postip;

            Boolean success = ForumPostRepository.Instance.InsertEntity(post) > 0;

            if (success && !String.Equals(parentPost.UserName, post.UserName))
            {
                if (ConfigurationManager.ReplyPostMailNotification)
                {
                    try
                    {
                        UserMailEntity mail = new UserMailEntity();
                        String url = ConfigurationManager.DomainUrl + ((link[0] == '/') ? link.Substring(1) : link);

                        mail.FromUserName = ConfigurationManager.SystemAccount;
                        mail.ToUserName = parentPost.UserName;
                        mail.Title = "Your post has new reply!";
                        mail.Content =
                            String.Format("Your post \"{0}\" has new reply, <br/>", parentPost.Title) +
                            String.Format("Please visit <a href=\"{0}\" target=\"_blank\">{0}</a>", url);

                        UserMailManager.InternalSendUserMail(mail);
                    }
                    catch { }
                }
            }

            return success;
        }

        /// <summary>
        /// 根据ID得到一个帖子实体
        /// </summary>
        /// <param name="id">帖子ID</param>
        /// <returns>帖子实体</returns>
        public static ForumPostEntity GetForumPost(Int32 id)
        {
            if (id <= 0)
            {
                throw new InvalidRequstException(RequestType.ForumPost);
            }

            ForumPostEntity entity = ForumPostCache.GetForumPostCache(id);

            if (entity == null)
            {
                entity = ForumPostRepository.Instance.GetEntity(id);
                ForumPostCache.SetForumPostCache(entity);
            }

            if (entity == null)
            {
                throw new NullResponseException(RequestType.ForumPost);
            }

            if (entity.IsHide && !AdminManager.HasPermission(PermissionType.ForumManage))
            {
                throw new NoPermissionException("You have no privilege to view the post!");
            }

            return entity;
        }

        /// <summary>
        /// 根据主题ID得到一个帖子实体
        /// </summary>
        /// <param name="topicID">主题ID</param>
        /// <returns>帖子实体</returns>
        public static ForumPostEntity GetForumPostByTopicID(String topicID)
        {
            Int32 tid = 0;
            Int32.TryParse(topicID, out tid);

            if (tid <= 0)
            {
                throw new InvalidRequstException(RequestType.ForumTopic);
            }

            ForumPostEntity entity = ForumPostRepository.Instance.GetEntityByTopicID(tid);

            if (entity == null)
            {
                throw new NullResponseException(RequestType.ForumPost);
            }

            if (entity.IsHide && !AdminManager.HasPermission(PermissionType.ForumManage))
            {
                throw new NoPermissionException("You have no privilege to view the post!");
            }

            return entity;
        }

        /// <summary>
        /// 获取帖子列表
        /// </summary>
        /// <param name="topics">主题列表</param>
        /// <param name="includeHide">是否包含隐藏主题</param>
        /// <returns>帖子列表</returns>
        public static List<ForumPostEntity> GetForumPostList(IList<ForumTopicEntity> topics, Boolean includeHide)
        {
            return ForumPostRepository.Instance.GetEntities(topics, includeHide);
        }

        /// <summary>
        /// 获取帖子列表
        /// </summary>
        /// <param name="topic">主题实体</param>
        /// <param name="includeHide">是否包含隐藏主题</param>
        /// <returns>帖子列表</returns>
        public static List<ForumPostEntity> GetForumPostList(ForumTopicEntity topic, Boolean includeHide)
        {
            List<ForumTopicEntity> topics = new List<ForumTopicEntity>() { topic };
            return GetForumPostList(topics, includeHide);
        }

        /// <summary>
        /// 创建树形列表
        /// </summary>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pid">题目ID</param>
        /// <returns>树形列表</returns>
        public static PagedList<TreeNode<ForumPostEntity>> GetPostTreeList(Int32 pageIndex, String pid)
        {
            PagedList<ForumTopicEntity> listTopic = ForumTopicManager.GetForumTopicList(pageIndex, String.Empty, pid);
            List<ForumPostEntity> listPost = ForumPostManager.GetForumPostList(listTopic, false);

            if (listPost == null || listPost.Count == 0)
            {
                return PagedList<TreeNode<ForumPostEntity>>.Empty;
            }

            //将主题帖放入树节点数组
            List<TreeNode<ForumPostEntity>> listTreeNode = new List<TreeNode<ForumPostEntity>>();
            List<ForumPostEntity> listRoot = new List<ForumPostEntity>();

            //找出所有根节点
            for (Int32 i = 0; i < listPost.Count; i++)
            {
                if (listPost[i].Deepth == 0)
                {
                    listRoot.Add(listPost[i]);
                    listPost.RemoveAt(i--);
                }
            }

            //将Topic的Type和RelativeID写入Post中
            for (Int32 i = 0; i < listTopic.Count; i++)
            {
                for (Int32 j = 0; j < listRoot.Count; j++)
                {
                    if (listRoot[j].TopicID == listTopic[i].TopicID)
                    {
                        if (listTopic[i].Type != ForumTopicType.Default)
                        {
                            listRoot[j].RelativeType = listTopic[i].Type;
                            listRoot[j].RelativeID = listTopic[i].RelativeID;
                        }

                        TreeNode<ForumPostEntity> node = new TreeNode<ForumPostEntity>(listRoot[j].PostID.ToString(), listRoot[j]);
                        listTreeNode.Add(node);
                        listRoot.RemoveAt(j);

                        break;
                    }
                }
            }

            //根据树节点数组创建树
            if (listTreeNode.Count > 0)
            {
                for (Int32 i = 0; i < listTreeNode.Count; i++)
                {
                    ForumPostManager.CreateTree(listTreeNode[i], listPost);
                }

                return listTreeNode.ToPagedList(listTopic.PageSize, listTopic.RecordCount);
            }
            else
            {
                return PagedList<TreeNode<ForumPostEntity>>.Empty;
            }
        }

        /// <summary>
        /// 创建树形列表
        /// </summary>
        /// <param name="topic">主题帖</param>
        /// <param name="postID">帖子ID</param>
        /// <returns>树形列表</returns>
        public static List<TreeNode<ForumPostEntity>> GetPostTreeList(ForumTopicEntity topic, Int32 postID)
        {
            List<ForumPostEntity> listPost = ForumPostManager.GetForumPostList(topic, false);

            if (listPost == null || listPost.Count == 0)
            {
                return new List<TreeNode<ForumPostEntity>>();
            }

            //将主题帖放入树节点数组
            List<TreeNode<ForumPostEntity>> listTreeNode = new List<TreeNode<ForumPostEntity>>();

            //将父节点是帖子ID的节点放入树节点数组
            for (Int32 i = 0; i < listPost.Count; i++)
            {
                if (listPost[i].ParentPostID == postID)
                {
                    TreeNode<ForumPostEntity> node = new TreeNode<ForumPostEntity>(listPost[i].PostID.ToString(), listPost[i]);
                    listTreeNode.Add(node);
                    listPost.RemoveAt(i--);
                }
            }

            //根据树节点数组创建树
            if (listTreeNode.Count > 0)
            {
                for (Int32 i = 0; i < listTreeNode.Count; i++)
                {
                    ForumPostManager.CreateTree(listTreeNode[i], listPost);
                }

                return listTreeNode;
            }
            else
            {
                return new List<TreeNode<ForumPostEntity>>();
            }
        }

        /// <summary>
        /// 递归创建树
        /// </summary>
        /// <param name="parent">父节点</param>
        /// <param name="listPost">帖子数组</param>
        private static void CreateTree(TreeNode<ForumPostEntity> parent, List<ForumPostEntity> listPost)
        {
            for (Int32 i = 0; i < listPost.Count; i++)
            {
                if (String.Equals(listPost[i].ParentPostID.ToString(), parent.Value))
                {
                    TreeNode<ForumPostEntity> node = new TreeNode<ForumPostEntity>(listPost[i].PostID.ToString(), listPost[i]);
                    parent.AddNote(node);
                    listPost.RemoveAt(i--);

                    ForumPostManager.CreateTree(node, listPost);
                }
            }
        }
        #endregion

        #region 管理方法
        /// <summary>
        /// 更新帖子隐藏状态
        /// </summary>
        /// <param name="ids">帖子ID列表</param>
        /// <param name="isHide">是否隐藏</param>
        /// <returns>是否成功更新</returns>
        public static IMethodResult AdminUpdateForumPostIsHide(String ids, Boolean isHide)
        {
            if (!AdminManager.HasPermission(PermissionType.ForumManage))
            {
                throw new NoPermissionException();
            }

            if (!RegexVerify.IsNumericIDs(ids))
            {
                return MethodResult.InvalidRequest(RequestType.ForumPost);
            }

            Boolean success = ForumPostRepository.Instance.UpdateEntityIsHide(ids, isHide) > 0;

            if (!success)
            {
                return MethodResult.FailedAndLog("No forum post was {0}!", isHide ? "hided" : "unhided");
            }

            ids.ForEachInIDs(',', id =>
            {
                ForumPostCache.RemoveForumPostCache(id);//删除缓存
            });

            return MethodResult.SuccessAndLog("Admin {1} forum post, id = {0}", ids, isHide ? "hide" : "unhide");
        }

        /// <summary>
        /// 获取帖子列表
        /// </summary>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="fpids">帖子ID列表</param>
        /// <param name="ftids">主题ID列表</param>
        /// <param name="username">发帖用户名</param>
        /// <param name="title">帖子标题</param>
        /// <param name="content">帖子内容</param>
        /// <param name="isHide">是否隐藏</param>
        /// <param name="startDate">发帖开始时间</param>
        /// <param name="endDate">发帖结束时间</param>
        /// <param name="postip">发帖IP</param>
        /// <returns>帖子列表</returns>
        public static PagedList<ForumPostEntity> AdminGetForumPostList(Int32 pageIndex, String fpids, String ftids, String username, String title, String content, String isHide, String startDate, String endDate, String postip)
        {
            if (!AdminManager.HasPermission(PermissionType.ForumManage))
            {
                throw new NoPermissionException();
            }

            Int32 pageSize = AdminManager.ADMIN_LIST_PAGE_SIZE;
            Int32 recordCount = ForumPostManager.AdminCountForumPosts(fpids, ftids, username, title, content, isHide, startDate, endDate, postip);

            DateTime dtStart = DateTime.MinValue, dtEnd = DateTime.MinValue;

            fpids = fpids.SearchOptimized();
            ftids = ftids.SearchOptimized();

            if (!String.IsNullOrEmpty(fpids) && !RegexVerify.IsNumericIDs(fpids))
            {
                throw new InvalidRequstException(RequestType.ForumPost);
            }

            if (!String.IsNullOrEmpty(ftids) && !RegexVerify.IsNumericIDs(ftids))
            {
                throw new InvalidRequstException(RequestType.ForumTopic);
            }

            return ForumPostRepository.Instance
                .GetEntities(pageIndex, pageSize, recordCount,
                    fpids, ftids, username, title, content,
                    (!String.IsNullOrEmpty(isHide) ? "1".Equals(isHide, StringComparison.OrdinalIgnoreCase) : new Nullable<Boolean>()),
                    (!String.IsNullOrEmpty(startDate) && DateTime.TryParse(startDate, out dtStart) ? dtStart : new Nullable<DateTime>()),
                    (!String.IsNullOrEmpty(endDate) && DateTime.TryParse(endDate, out dtEnd) ? dtEnd : new Nullable<DateTime>()), postip)
                    .ToPagedList(pageSize, recordCount);
        }

        /// <summary>
        /// 获取帖子总数(无缓存)
        /// </summary>
        /// <param name="fpids">帖子ID列表</param>
        /// <param name="ftids">主题ID列表</param>
        /// <param name="username">发帖用户名</param>
        /// <param name="title">帖子标题</param>
        /// <param name="content">帖子内容</param>
        /// <param name="isHide">是否隐藏</param>
        /// <param name="startDate">发帖开始时间</param>
        /// <param name="endDate">发帖结束时间</param>
        /// <param name="postip">发帖IP</param>
        /// <returns>帖子总数</returns>
        private static Int32 AdminCountForumPosts(String fpids, String ftids, String username, String title, String content, String isHide, String startDate, String endDate, String postip)
        {
            DateTime dtStart = DateTime.MinValue, dtEnd = DateTime.MinValue;

            fpids = fpids.SearchOptimized();
            ftids = ftids.SearchOptimized();

            if (!String.IsNullOrEmpty(fpids) && !RegexVerify.IsNumericIDs(fpids))
            {
                throw new InvalidRequstException(RequestType.ForumPost);
            }

            if (!String.IsNullOrEmpty(ftids) && !RegexVerify.IsNumericIDs(ftids))
            {
                throw new InvalidRequstException(RequestType.ForumTopic);
            }

            return ForumPostRepository.Instance
                    .CountEntities(fpids, ftids, username, title, content,
                    (!String.IsNullOrEmpty(isHide) ? "1".Equals(isHide, StringComparison.OrdinalIgnoreCase) : new Nullable<Boolean>()),
                    (!String.IsNullOrEmpty(startDate) && DateTime.TryParse(startDate, out dtStart) ? dtStart : new Nullable<DateTime>()),
                    (!String.IsNullOrEmpty(endDate) && DateTime.TryParse(endDate, out dtEnd) ? dtEnd : new Nullable<DateTime>()), postip);
        }
        #endregion
    }
}