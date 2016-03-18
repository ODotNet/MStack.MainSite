using MStack.Core.Repositories;
using MStack.Infrastructure.Entities;
using System;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using MStack.MainSite.Models;
using MStack.MainSite.WebFramework.Extensions;
using MStack.MainSite.WebFramework.Untils;
using System.IO;
using System.Drawing;

namespace MStack.MainSite.Controllers
{
    public class ManageController : BaseController
    {
        private MStackRepository<Guid> DataContext { get; set; }
        public ManageController()
        {
            DataContext = new MStackRepository<Guid>(NHSessionFactory.OpenSession());
        }
        // GET: Manage
        public ActionResult Index()
        {
            var userName = UserIdentity.Name;
            var user = DataContext.GetQuery<User>().SingleOrDefault(x => x.UserName == userName);
            var model = user.Map<User, UserModel>();
            return View(model);
        }

        public ActionResult EditUser(Guid id)
        {
            var user = DataContext.GetQuery<User>().SingleOrDefault(x => x.Id == id);
            var model = user.Map<User, UserModel>();
            return View(model);
        }
        [HttpPost]
        public ActionResult EditUser(UserModel model)
        {
            if (ModelState.IsValid)
            {
                using (var tran = DataContext.Session.BeginTransaction())
                {
                    var entity = DataContext.GetQuery<User>().SingleOrDefault(x => x.Id == model.Id);
                    entity.DisplayName = model.DisplayName;
                    //entity.Email = model.Email;
                    entity.Avatar = model.Avatar;
                    if (!string.IsNullOrEmpty(model.AvatarState))
                    {
                        var tmpArray = model.AvatarState.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => Convert.ToInt32(x))
                            .ToArray();
                        if (tmpArray.Length == 4)
                        {
                            entity.Avatar = CropAvatar(model.Avatar, tmpArray[0], tmpArray[1], tmpArray[2], tmpArray[3]);
                        }
                    }
                    entity.Company = model.Company;
                    DataContext.Update(entity);
                    tran.Commit();
                }

                return RedirectToAction("Index");
            }
            return View("Error");
        }
        public ActionResult UploadAvatar(string id)
        {
            var files = Request.Files;
            if (files.Count > 0)
            {
                var imgFile = files[0];
                var ext = Path.GetExtension(imgFile.FileName);
                var dirName = "/webupload/avatar/";
                var savePath = dirName + string.Format("avatar_tmp_{0}{1}", id, ext);
                var physicPath = Server.MapPath(savePath);
                FileUntils.CreateDirIfNotExists(Server.MapPath(dirName));
                imgFile.SaveAs(physicPath);
                return Json(new { success = true, savePath = savePath });
            }
            return Json(new { success = false, savePath = string.Empty });
        }
        private string CropAvatar(string sourcePath, int x, int y, int w, int h)
        {
            var savePath = sourcePath.Replace("tmp_", "");
            ImageUntils.CropImage(Server.MapPath(sourcePath), Server.MapPath(savePath), new Rectangle(x, y, w, h));
            FileUntils.Delete(Server.MapPath(sourcePath));
            return savePath;
        }
        protected ClaimsIdentity UserIdentity
        {
            get
            {
                return User.Identity as ClaimsIdentity;
            }
        }
    }
}