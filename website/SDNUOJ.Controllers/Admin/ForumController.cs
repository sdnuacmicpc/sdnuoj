using System;
using System.Web.Mvc;

using SDNUOJ.Controllers.Core;
using SDNUOJ.Entity;
using SDNUOJ.Utilities;

namespace SDNUOJ.Areas.Admin.Controllers
{
    [Authorize(Roles = "ForumManage")]
    public class ForumController : AdminBaseController
    {
        /// <summary>
        /// 论坛主题列表页面
        /// </summary>
        /// <param name="id">页面索引</param>
        /// <param name="ftids">主题ID列表</param>
        /// <param name="username">发帖人用户名</param>
        /// <param name="title">主题名包含</param>
        /// <param name="type">主题类型</param>
        /// <param name="relativeid">相关ID</param>
        /// <param name="islocked">是否锁定</param>
        /// <param name="ishide">是否隐藏</param>
        /// <param name="startdate">注册时间范围起始</param>
        /// <param name="enddate">注册时间范围结束</param>
        /// <returns>操作后的结果</returns>
        public ActionResult TopicList(Int32 id = 1,
            String ftids = "", String username = "", String title = "", String type = "", String relativeid = "", String islocked = "", String ishide = "",
            String startdate = "", String enddate = "")
        {
            PagedList<ForumTopicEntity> list = ForumTopicManager.AdminGetForumTopicList(id,
                ftids, username, title, type, relativeid, islocked, ishide, startdate, enddate);

            ViewBag.PageSize = list.PageSize;
            ViewBag.RecordCount = list.RecordCount;
            ViewBag.PageCount = list.PageCount;
            ViewBag.PageIndex = id;

            ViewBag.TopicIDs = ftids;
            ViewBag.UserName = username;
            ViewBag.Title = title;
            ViewBag.Type = type;
            ViewBag.RelativeID = relativeid;
            ViewBag.IsLocked = islocked;
            ViewBag.IsHide = ishide;
            ViewBag.StartDate = startdate;
            ViewBag.EndDate = enddate;

            return View(list);
        }

        /// <summary>
        /// 主题锁定
        /// </summary>
        /// <param name="ids">主题ID</param>
        /// <returns>操作后的结果</returns>
        public ActionResult TopicLock(String ids)
        {
            return ResultToJson(ForumTopicManager.AdminUpdateForumTopicIsLocked, ids, true);
        }

        /// <summary>
        /// 主题解锁
        /// </summary>
        /// <param name="ids">主题ID</param>
        /// <returns>操作后的结果</returns>
        public ActionResult TopicUnlock(String ids)
        {
            return ResultToJson(ForumTopicManager.AdminUpdateForumTopicIsLocked, ids, false);
        }

        /// <summary>
        /// 主题隐藏
        /// </summary>
        /// <param name="ids">主题ID</param>
        /// <returns>操作后的结果</returns>
        public ActionResult TopicHide(String ids)
        {
            return ResultToJson(ForumTopicManager.AdminUpdateForumTopicIsHide, ids, true);
        }

        /// <summary>
        /// 主题显示
        /// </summary>
        /// <param name="ids">主题ID</param>
        /// <returns>操作后的结果</returns>
        public ActionResult TopicShow(String ids)
        {
            return ResultToJson(ForumTopicManager.AdminUpdateForumTopicIsHide, ids, false);
        }

        /// <summary>
        /// 论坛帖子列表页面
        /// </summary>
        /// <param name="id">页面索引</param>
        /// <param name="fpids">帖子ID列表</param>
        /// <param name="ftids">主题ID列表</param>
        /// <param name="username">发帖人用户名</param>
        /// <param name="title">主题名包含</param>
        /// <param name="content">帖子内容包含</param>
        /// <param name="ishide">是否隐藏</param>
        /// <param name="startdate">注册时间范围起始</param>
        /// <param name="enddate">注册时间范围结束</param>
        /// <param name="postip">发帖IP包含</param>
        /// <returns>操作后的结果</returns>
        public ActionResult PostList(Int32 id = 1,
            String fpids = "", String ftids = "", String username = "", String title = "", String content = "", String ishide = "",
            String startdate = "", String enddate = "", String postip = "")
        {
            PagedList<ForumPostEntity> list = ForumPostManager.AdminGetForumPostList(id,
                fpids, ftids, username, title, content, ishide, startdate, enddate, postip);

            ViewBag.PageSize = list.PageSize;
            ViewBag.RecordCount = list.RecordCount;
            ViewBag.PageCount = list.PageCount;
            ViewBag.PageIndex = id;

            ViewBag.PostIDs = fpids;
            ViewBag.TopicIDs = ftids;
            ViewBag.UserName = username;
            ViewBag.Title = title;
            ViewBag.Content = content;
            ViewBag.IsHide = ishide;
            ViewBag.StartDate = startdate;
            ViewBag.EndDate = enddate;
            ViewBag.PostIP = postip;

            return View(list);
        }

        /// <summary>
        /// 帖子隐藏
        /// </summary>
        /// <param name="ids">帖子ID</param>
        /// <returns>操作后的结果</returns>
        public ActionResult PostHide(String ids)
        {
            return ResultToJson(ForumPostManager.AdminUpdateForumPostIsHide, ids, true);
        }

        /// <summary>
        /// 帖子显示
        /// </summary>
        /// <param name="ids">帖子ID</param>
        /// <returns>操作后的结果</returns>
        public ActionResult PostShow(String ids)
        {
            return ResultToJson(ForumPostManager.AdminUpdateForumPostIsHide, ids, false);
        }
    }
}