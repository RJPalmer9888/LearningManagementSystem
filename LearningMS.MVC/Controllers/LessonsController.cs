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

namespace LearningMS.MVC.Controllers
{
    public class LessonsController : Controller
    {
        private LearningMSEntities db = new LearningMSEntities();

        // GET: Lessons
        public ActionResult Index()
        {
            var lessons = db.Lessons.Include(l => l.Cours);
            return View(lessons.ToList());
        }

        // GET: Lessons/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {
                return HttpNotFound();
            }
            return View(lesson);
        }

        // GET: Lessons/Create
        public ActionResult Create()
        {
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName");
            return View();
        }

        // POST: Lessons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LessonId,LessonTitle,CourseId,Introduction,VideoURL,PdfFilename,LessonPhoto,IsActive")] Lesson lesson, HttpPostedFileBase lessonPhoto, HttpPostedFileBase lessonPdf)
        {
            if (ModelState.IsValid)
            {
                #region File Upload
                string file = "NoImage.png";

                if (lessonPhoto != null)
                {
                    file = lessonPhoto.FileName;
                    string ext = file.Substring(file.LastIndexOf('.'));
                    string[] goodExts = { ".jpeg", ".jpg", ".png", ".gif" };

                    //Check that the uploaded file is in our list of acceptable exts and file size <= 4mb max from ASP.NET
                    if (goodExts.Contains(ext.ToLower()) && lessonPhoto.ContentLength <= 4194303)
                    {
                        //Create a new file name (using a GUID)
                        file = Guid.NewGuid() + ext;

                        #region Resize Image
                        string savePath = Server.MapPath("~/imgstore/lessons/");

                        Image convertedImage = Image.FromStream(lessonPhoto.InputStream);

                        int maxImageSize = 500;

                        int maxThumbSize = 100;

                        ImageUtility.ResizeImage(savePath, file, convertedImage, maxImageSize, maxThumbSize);
                        #endregion
                    }

                    //no matter what, update the PhotoUrl witht he value of the file variable

                }
                lesson.LessonPhoto = file;
                #endregion
                #region File Upload
                string pdffile = null;

                if (lessonPdf != null)
                {
                    pdffile = lessonPdf.FileName;
                    string ext = pdffile.Substring(pdffile.LastIndexOf('.'));
                    string[] goodExts = { ".pdf" };

                    //Check that the uploaded file is in our list of acceptable exts and file size <= 4mb max from ASP.NET
                    if (goodExts.Contains(ext.ToLower()) && lessonPdf.ContentLength <= 4194303)
                    {
                        //Create a new file name (using a GUID)
                        pdffile = Guid.NewGuid() + ext;

                        string savePath = Server.MapPath("~/pdfstore/lessons/");
                        lessonPdf.SaveAs(savePath + pdffile);

                    }

                    //no matter what, update the PhotoUrl witht he value of the file variable

                }
                lesson.PdfFilename = pdffile;
                #endregion
                if (lesson.VideoURL != null)
                {
                    if (lesson.VideoURL.Contains("/watch")) { lesson.VideoURL = lesson.VideoURL.Replace("/watch?v=", "/embed/"); }
                }

                db.Lessons.Add(lesson);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", lesson.CourseId);
            return View(lesson);
        }

        // GET: Lessons/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", lesson.CourseId);
            return View(lesson);
        }

        // POST: Lessons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LessonId,LessonTitle,CourseId,Introduction,VideoURL,PdfFilename,LessonPhoto,IsActive")] Lesson lesson, HttpPostedFileBase newLessonPhoto, HttpPostedFileBase lessonPdf)
        {
            if (ModelState.IsValid)
            {
                #region File Upload
                string file = "NoImage.png";
                if (lesson.LessonPhoto != "NoImage.png" && lesson.LessonPhoto != null)
                {
                    file = lesson.LessonPhoto;
                }

                if (newLessonPhoto != null)
                {
                    file = newLessonPhoto.FileName;
                    string ext = file.Substring(file.LastIndexOf('.'));
                    string[] goodExts = { ".jpeg", ".jpg", ".png", ".gif" };

                    //Check that the uploaded file is in our list of acceptable exts and file size <= 4mb max from ASP.NET
                    if (goodExts.Contains(ext.ToLower()) && newLessonPhoto.ContentLength <= 4194303)
                    {
                        //Create a new file name (using a GUID)
                        file = Guid.NewGuid() + ext;

                        #region Resize Image
                        string savePath = Server.MapPath("~/imgstore/lessons/");

                        Image convertedImage = Image.FromStream(newLessonPhoto.InputStream);

                        int maxImageSize = 500;

                        int maxThumbSize = 100;

                        ImageUtility.ResizeImage(savePath, file, convertedImage, maxImageSize, maxThumbSize);
                        #endregion
                    }

                    //no matter what, update the PhotoUrl witht he value of the file variable

                }
                lesson.LessonPhoto = file;
                #endregion

                #region File Upload
                string pdffile = null;
                if (lesson.PdfFilename != null)
                {
                    pdffile = lesson.PdfFilename;

                }

                if (lessonPdf != null)
                {
                    pdffile = lessonPdf.FileName;
                    string ext = pdffile.Substring(pdffile.LastIndexOf('.'));
                    string[] goodExts = { ".pdf" };

                    //Check that the uploaded file is in our list of acceptable exts and file size <= 4mb max from ASP.NET
                    if (goodExts.Contains(ext.ToLower()) && lessonPdf.ContentLength <= 4194303)
                    {
                        //Create a new file name (using a GUID)
                        pdffile = Guid.NewGuid() + ext;

                        string savePath = Server.MapPath("~/pdfstore/lessons/");
                        lessonPdf.SaveAs(savePath + pdffile);

                    }

                    //no matter what, update the PhotoUrl witht he value of the file variable

                }
                lesson.PdfFilename = pdffile;
                #endregion

                if (lesson.VideoURL != null)
                {
                    if (lesson.VideoURL.Contains("/watch")) { lesson.VideoURL = lesson.VideoURL.Replace("/watch?v=", "/embed/"); }
                }
                db.Entry(lesson).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", lesson.CourseId);
            return View(lesson);
        }

        // GET: Lessons/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {
                return HttpNotFound();
            }
            return View(lesson);
        }

        // POST: Lessons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Lesson lesson = db.Lessons.Find(id);
            db.Lessons.Remove(lesson);
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
