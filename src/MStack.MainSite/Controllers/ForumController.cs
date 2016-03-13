using MStack.Core.Repositories;
using MStack.Infrastructure.Entities;
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
        public MStackRepository<Guid> DataContext { get; set; }

        public ForumController()
        {
            this.DataContext = new MStackRepository<Guid>(NHSessionFactory.OpenSession());
        }
        /// <summary>
        /// 论坛模块首页
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 帖子列表部分页
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
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
            var viewModel = new Topic() { PublishDateTime=DateTime.Now };
            return View(viewModel);
        }

        [HttpPost, ActionName("Publish")]
        public ActionResult PublishPost(FormCollection collection)
        {
            var model = new Topic();
            this.TryUpdateModel<Topic>(model);
            var userName = User.Identity.Name;
            var user = this.DataContext.GetQuery<User>().SingleOrDefault(x => x.UserName == userName);
            model.Author = new Author { AuthorId = user.Id, AuthorName = userName };
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            //DataContext.SaveOrUpdate<Topic>(model);
            
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