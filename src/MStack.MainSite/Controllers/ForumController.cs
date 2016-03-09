using MStack.MainSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MStack.MainSite.Controllers
{
    [Authorize]
    public class ForumController : BaseController
    {
        /// <summary>
        /// 论坛模块首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 帖子列表部分页
        /// </summary>
        /// <returns></returns>
        public ActionResult TopicList()
        {
            var viewModel = new List<Topic>();
            return PartialView(viewModel);
        }

        /// <summary>
        /// 发帖页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Publish()
        {
            var viewModel = new Topic();
            return View(viewModel);
        }

        [HttpPost, ActionName("Publish")]
        public ActionResult PublishPost()
        {
            return View();
        }

        /// <summary>
        /// 编辑Topic页面
        /// </summary>
        /// <param name="topicId"></param>
        /// <returns></returns>
        public ActionResult EditTopic(Guid topicId)
        {
            return View();
        }

        /// <summary>
        /// 编辑Topic提交Action
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult EditTopicPost(Topic model)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Comment(Comment comment)
        {
            return View();
        }
    }
}