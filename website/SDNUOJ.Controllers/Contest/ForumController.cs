using System;
using System.Collections.Generic;
using System.Web.Mvc;

using SDNUOJ.Controllers.Attributes;
using SDNUOJ.Controllers.Core;
using SDNUOJ.Controllers.Exception;
using SDNUOJ.Entity;
using SDNUOJ.Utilities;
using SDNUOJ.Utilities.Web;

namespace SDNUOJ.Areas.Contest.Controllers
{
    public class ForumController : ContestBaseController
    {
        /// <summary>
        /// 讨论版列表页面
        /// </summary>
        /// <param name="id">页面索引</param>
        /// <returns>操作后的结果</returns>
        [OutputCache(CacheProfile = "MinimumPageCache", VaryByParam = "id", VaryByCustom = "nm")]
        public ActionResult List(Int32 id = 1)
        {
            ContestEntity contest = ViewData["Contest"] as ContestEntity;
            PagedList<ForumTopicEntity> list = ForumTopicManager.GetForumTopicList(id, contest.ContestID.ToString(), String.Empty);

            return ViewWithPager(list, id);
        }

        /// <summary>
        /// 讨论版发布页面
        /// </summary>
        /// <returns>操作后的结果</returns>
        [Authorize]
        [ContestSubmit]
        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// 讨论版发布
        /// </summary>
        /// <param name="form">Form集合</param>
        /// <returns>操作后的结果</returns>
        [HttpPost]
        [Authorize]
        [ContestSubmit]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult New(FormCollection form)
        {
            ContestEntity contest = ViewData["Contest"] as ContestEntity;
            ForumTopicEntity topic = new ForumTopicEntity()
            {
                Title = form["title"],
            };

            String userip = this.GetCurrentUserIP();

            if (!ForumTopicManager.InsertForumTopic(topic, contest.ContestID.ToString(), String.Empty, form["content"], userip))
            {
                return RedirectToErrorMessagePage("Failed to post your topic!");
            }

            return RedirectToAction("List", "Forum", new { area = "Contest", cid = contest.ContestID });
        }

        /// <summary>
        /// 讨论版主题页面
        /// </summary>
        /// <param name="id">主题ID</param>
        /// <returns>操作后的结果</returns>
        public ActionResult Topic(Int32 id = -1)
        {
            ContestEntity contest = ViewData["Contest"] as ContestEntity;
            ForumPostEntity post = ForumPostManager.GetForumPostByTopicID(id.ToString());
            ForumTopicEntity topic = ForumTopicManager.GetForumTopic(post.TopicID);

            if (topic.Type != ForumTopicType.Contest || topic.RelativeID != contest.ContestID)
            {
                return RedirectToErrorMessagePage("This contest does not have this topic!");
            }

            List<ForumPostEntity> list = ForumPostManager.GetForumPostList(topic, false);

            return View(new Tuple<ForumTopicEntity, List<ForumPostEntity>>(topic, list));
        }

        /// <summary>
        /// 讨论版回复
        /// </summary>
        /// <param name="id">主题ID</param>
        /// <param name="form">Form集合</param>
        /// <returns>操作后的结果</returns>
        [HttpPost]
        [Authorize]
        [ContestSubmit]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Reply(String id, FormCollection form)
        {
            ContestEntity contest = ViewData["Contest"] as ContestEntity;
            ForumPostEntity post = ForumPostManager.GetForumPostByTopicID(id);
            ForumTopicEntity topic = ForumTopicManager.GetForumTopic(post.TopicID);

            if (topic.Type != ForumTopicType.Contest || topic.RelativeID != contest.ContestID)
            {
                return RedirectToErrorMessagePage("This contest does not have this topic!");
            }

            ForumPostEntity reply = new ForumPostEntity()
            {
                Title = form["title"],
                Content = form["content"]
            };

            String userip = this.GetCurrentUserIP();
            String link = Url.Action("Topic", "Forum", new { area = "Contest", cid = contest.ContestID, id = post.TopicID });

            if (!ForumPostManager.InsertForumPost(reply, topic, post, userip, link))
            {
                return RedirectToErrorMessagePage("Failed to post your reply!");
            }

            return RedirectToAction("Topic", "Forum", new { area = "Contest", cid = contest.ContestID, id = post.TopicID });
        }
    }
}