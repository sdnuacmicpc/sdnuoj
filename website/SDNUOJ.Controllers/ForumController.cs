using System;
using System.Collections.Generic;
using System.Web.Mvc;

using SDNUOJ.Controllers.Attributes;
using SDNUOJ.Controllers.Core;
using SDNUOJ.Controllers.Exception;
using SDNUOJ.Entity;
using SDNUOJ.Utilities;
using SDNUOJ.Utilities.Web;

namespace SDNUOJ.Controllers
{
    [Function(PageType.MainForum)]
    public class ForumController : BaseController
    {
        /// <summary>
        /// 讨论版列表页面
        /// </summary>
        /// <param name="id">页面索引</param>
        /// <returns>操作后的结果</returns>
        [OutputCache(CacheProfile = "MinimumPageCache", VaryByParam = "id")]
        public ActionResult Main(Int32 id = 1)
        {
            PagedList<TreeNode<ForumPostEntity>> list = ForumPostManager.GetPostTreeList(id, String.Empty);

            ViewBag.PageCount = list.PageCount;
            ViewBag.PageIndex = id;

            return View("Main", list);
        }

        /// <summary>
        /// 讨论版列表页面
        /// </summary>
        /// <param name="id">页面索引</param>
        /// <param name="pid">题目ID</param>
        /// <returns>操作后的结果</returns>
        [OutputCache(CacheProfile = "MinimumPageCache", VaryByParam = "pid;id")]
        public ActionResult Problem(String pid, Int32 id = 1)
        {
            if (String.IsNullOrEmpty(pid))
            {
                throw new InvalidRequstException(RequestType.Problem);
            }

            PagedList<TreeNode<ForumPostEntity>> list = ForumPostManager.GetPostTreeList(id, pid);

            ViewBag.PageCount = list.PageCount;
            ViewBag.PageIndex = id;
            ViewBag.ProblemID = pid;

            return View("Main", list);
        }

        /// <summary>
        /// 讨论版发布页面
        /// </summary>
        /// <param name="pid">题目ID</param>
        /// <returns>操作后的结果</returns>
        [Authorize]
        public ActionResult New(String pid = "")
        {
            ViewBag.ProblemID = pid;

            return View();
        }

        /// <summary>
        /// 讨论版发布
        /// </summary>
        /// <param name="form">Form集合</param>
        /// <returns>操作后的结果</returns>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult New(FormCollection form)
        {
            ForumTopicEntity topic = new ForumTopicEntity()
            {
                Title = form["title"]
            };
            String pid = form["pid"];

            if (!ForumTopicManager.InsertForumTopic(topic, String.Empty, pid, form["content"]))
            {
                throw new OperationFailedException("Failed to post your topic!");
            }

            if (String.IsNullOrEmpty(pid))
            {
                return RedirectToAction("Main", "Forum");
            }
            else
            {
                return RedirectToAction("Problem", "Forum", new { pid = pid });
            }
        }

        /// <summary>
        /// 讨论版回复页面
        /// </summary>
        /// <param name="id">帖子ID</param>
        /// <param name="tid">主题ID</param>
        /// <returns>操作后的结果</returns>
        public ActionResult Reply(Int32 id = -1, String tid = "")
        {
            ForumPostEntity post = (String.IsNullOrEmpty(tid) ? ForumPostManager.GetForumPost(id) : ForumPostManager.GetForumPostByTopicID(tid));
            ForumTopicEntity topic = ForumTopicManager.GetForumTopic(post.TopicID);

            if (topic.Type == ForumTopicType.Contest)
            {
                throw new InvalidInputException("This topic is not in the main disscus board!");
            }

            post.RelativeType = (topic.Type == ForumTopicType.Problem ? topic.Type : ForumTopicType.Default);
            post.RelativeID = (topic.Type == ForumTopicType.Problem ? topic.RelativeID : -1);

            List<TreeNode<ForumPostEntity>> listTreeNode = ForumPostManager.GetPostTreeList(topic, post.PostID);

            ViewBag.IsLocked = topic.IsLocked;

            return View(new Tuple<ForumPostEntity, List<TreeNode<ForumPostEntity>>>(post, listTreeNode));
        }

        /// <summary>
        /// 讨论版回复
        /// </summary>
        /// <param name="id">帖子ID</param>
        /// <param name="form">Form集合</param>
        /// <returns>操作后的结果</returns>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Reply(Int32 id, FormCollection form)
        {
            ForumPostEntity post = ForumPostManager.GetForumPost(id);
            ForumTopicEntity topic = ForumTopicManager.GetForumTopic(post.TopicID);

            if (topic.Type == ForumTopicType.Contest)
            {
                throw new InvalidInputException("This topic is not in the main disscus board!");
            }

            ForumPostEntity reply = new ForumPostEntity()
            {
                Title = form["title"],
                Content = form["content"]
            };

            String link = Url.Action("Reply", "Forum", new { id = post.PostID });

            if (!ForumPostManager.InsertForumPost(reply, topic, post, link))
            {
                throw new OperationFailedException("Failed to post your reply!");
            }

            return RedirectToAction("Reply", "Forum", new { id = post.PostID });
        }
    }
}