using MStack.Core.Repositories;
using MStack.Infrastructure.Entities;
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
            var viewModel = DataContext.GetQuery<Topic>().ToList<Topic>();
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
        public ActionResult PublishPost(FormCollection collection)
        {
            var model = new Topic();
            try
            {
                using (var tran = DataContext.Session.BeginTransaction())
                {
                    this.TryUpdateModel<Topic>(model);
                    model.PublishDateTime = DateTime.Now;
                    model.Author = DataContext.GetQuery<User>().SingleOrDefault(x => x.UserName == User.Identity.Name);
                    DataContext.Insert<Topic>(model);
                    tran.Commit();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                return View(model);
            }
        }

        /// <summary>
        /// 编辑Topic页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditTopic(Guid id)
        {
            var model = DataContext.Get<Topic>(x => x.Id == id);
            return View(model);
        }

        /// <summary>
        /// 编辑Topic提交Action
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, ActionName("EditTopic")]
        public ActionResult EditTopicPost(Topic model)
        {
            using (var tran = DataContext.Session.BeginTransaction())
            {
                model.Author = DataContext.GetQuery<User>().SingleOrDefault(x => x.UserName == User.Identity.Name);
                DataContext.SaveOrUpdate<Topic>(model);
                tran.Commit();
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult Comment(Models.Comment comment)
        {
            return View();
        }
    }
}