using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LearningMS.DATA;
using LearningMS.MVC.Utilities;
using Microsoft.AspNet.Identity;

namespace LearningMS.MVC.Controllers
{
    public class CoursesController : Controller
    {
        private LearningMSEntities db = new LearningMSEntities();

        // GET: Courses
        public ActionResult Index()
        {
            if (Session["Grid"] == null)
            {
                Session["Grid"] = false;
            }
            bool grid = (bool)Session["Grid"];
            if (grid == true)
            {
                var courses = db.Courses.Where(c => c.IsActive == true).ToList();
                var userId = User.Identity.GetUserId();
                foreach (var course in courses)
                {
                    course.checkCompletion(course.CourseId, userId, db);
                }
                return View("CoursesGrid", courses);
            }
            if (Request.IsAuthenticated && (User.IsInRole("Manager") || User.IsInRole("HR Admin")))
            {
                Session["grid"] = false;
                var courses = db.Courses.ToList();
                var userId = User.Identity.GetUserId();
                foreach (var course in courses)
                {
                    course.checkCompletion(course.CourseId, userId, db);
                }
                return View("CoursesAdmin", courses);
            }
            else if (Request.IsAuthenticated)
            {
                Session["grid"] = false;
                var courses = db.Courses.Where(c => c.IsActive == true).ToList();
                var userId = User.Identity.GetUserId();
                foreach (var course in courses)
                {
                    course.checkCompletion(course.CourseId, userId, db);
                }
                return View(courses);
            }
            else
            {
                Session["grid"] = false;
                var courses = db.Courses.Where(c => c.IsActive == true).ToList();
                return View("CoursesBasic", courses);
            }
        }

        [Authorize]
        public ActionResult CoursesGrid()
        {
            Session["Grid"] = true;
            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult CoursesList()
        {
            Session["Grid"] = false;
            return RedirectToAction("Index");
        }

        // GET: Courses/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }

            int currentYear = DateTime.Now.Year;
            var userId = User.Identity.GetUserId();
            var lessons = db.Lessons.Where(lv => lv.CourseId == id).OrderBy(s => s.LessonId);
            var lessonViews = db.LessonViews.Where(lv => lv.UserId == userId);
            var courseCompletions = db.CourseCompletions.Where(cc => cc.UserId == userId && cc.DateCompleted.Year == currentYear);
            var courses = db.Courses.Where(lv => lv.CourseId == id);
            course.checkCompletion((int)id, userId, db);

            foreach (var lesson in lessons)
            {
                foreach (var lessonView in lessonViews)
                {
                    if (lesson.LessonId == lessonView.LessonId)
                    {
                        lesson.Completion = true;
                    }
                }
            }
            ViewBag.Title = course.CourseName;
            ViewBag.Description = course.CourseDescription;
            var completed = course.Completion;
            ViewBag.Completed = completed;
            return View(lessons.ToList());
        }

        // GET: Courses/Create
        [Authorize(Roles = "Manager, HR Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Manager, HR Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CourseId,CourseName,CourseDescription,CoursePhoto,IsActive")] Course course, HttpPostedFileBase coursePhoto)
        {
            if (ModelState.IsValid)
            {
                #region File Upload
                string file = "NoImage.png";

                if (coursePhoto != null)
                {
                    file = coursePhoto.FileName;
                    string ext = file.Substring(file.LastIndexOf('.'));
                    string[] goodExts = { ".jpeg", ".jpg", ".png", ".gif" };

                    //Check that the uploaded file is in our list of acceptable exts and file size <= 4mb max from ASP.NET
                    if (goodExts.Contains(ext.ToLower()) && coursePhoto.ContentLength <= 4194303)
                    {
                        //Create a new file name (using a GUID)
                        file = Guid.NewGuid() + ext;

                        #region Resize Image
                        string savePath = Server.MapPath("~/imgstore/courses/");

                        Image convertedImage = Image.FromStream(coursePhoto.InputStream);

                        int maxImageSize = 500;

                        int maxThumbSize = 100;

                        ImageUtility.ResizeImage(savePath, file, convertedImage, maxImageSize, maxThumbSize);
                        #endregion
                    }

                    //no matter what, update the PhotoUrl witht he value of the file variable

                }
                course.CoursePhoto = file;
                #endregion
                db.Courses.Add(course);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(course);
        }

        // GET: Courses/Edit/5
        [Authorize(Roles = "Manager, HR Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Manager, HR Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CourseId,CourseName,CourseDescription,CoursePhoto,IsActive")] Course course, HttpPostedFileBase newCoursePhoto)
        {
            if (ModelState.IsValid)
            {
                #region File Upload
                string file = "NoImage.png";
                if (course.CoursePhoto != "NoImage.png" && course.CoursePhoto != null)
                {
                    file = course.CoursePhoto;
                }

                if (newCoursePhoto != null)
                {
                    file = newCoursePhoto.FileName;
                    string ext = file.Substring(file.LastIndexOf('.'));
                    string[] goodExts = { ".jpeg", ".jpg", ".png", ".gif" };

                    //Check that the uploaded file is in our list of acceptable exts and file size <= 4mb max from ASP.NET
                    if (goodExts.Contains(ext.ToLower()) && newCoursePhoto.ContentLength <= 4194303)
                    {
                        //Create a new file name (using a GUID)
                        file = Guid.NewGuid() + ext;

                        #region Resize Image
                        string savePath = Server.MapPath("~/imgstore/courses/");

                        Image convertedImage = Image.FromStream(newCoursePhoto.InputStream);

                        int maxImageSize = 500;

                        int maxThumbSize = 100;

                        ImageUtility.ResizeImage(savePath, file, convertedImage, maxImageSize, maxThumbSize);
                        #endregion
                    }

                    //no matter what, update the PhotoUrl witht he value of the file variable

                }
                course.CoursePhoto = file;
                #endregion
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(course);
        }

        // GET: Courses/Delete/5
        [Authorize(Roles = "HR Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Delete/5
        [Authorize(Roles = "HR Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Courses.Find(id);
            db.Courses.Remove(course);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
