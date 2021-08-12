using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningMS.DATA/*.Metadata*/
{
    #region Course Metadata
    public class CourseMetadata
    {
        //public string CourseId { get; set; }


        [Required(ErrorMessage = "Name is required")]
        [StringLength(200, ErrorMessage = "*Value must be 200 characters or less")]
        [Display(Name = "Course Name")]
        public string CourseName { get; set; }

        [StringLength(500, ErrorMessage = "*Value must be 500 characters or less")]
        [Display(Name = "Course Description")]
        [DisplayFormat(NullDisplayText = "[-N/A-]")]
        public string CourseDescription { get; set; }

        [StringLength(200, ErrorMessage = "*Value must be 200 characters or less")]
        [Display(Name = "Course Image")]
        [DisplayFormat(NullDisplayText = "[-N/A-]")]
        public string CoursePhoto { get; set; }

        [Required(ErrorMessage = "Active is required")]
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }

    [MetadataType(typeof(CourseMetadata))]
    public partial class Course
    {
        public bool Completion { get; set; }

        public void checkCompletion(int course, string user, LearningMSEntities db)
        {
            //Lessons where courseid == courseID;
            var lessons = db.Lessons.Where(l => l.CourseId == course);
            //lessonviews where courseid == courseId && userid == userid
            var lessionViews = db.LessonViews.Where(lv => lv.Lesson.CourseId == course && lv.UserId == user && lv.DateViewed.Year == DateTime.Now.Year);
            var courseCompletions = db.CourseCompletions.Where(lv => lv.UserId == user && lv.CourseId == course).FirstOrDefault();
            if (lessons.Count() == lessionViews.Count())
            {
                Completion = true;
                if (courseCompletions == null)
                {
                    CourseCompletion newCC = new CourseCompletion();
                    newCC.DateCompleted = DateTime.Now;
                    newCC.CourseId = course;
                    newCC.UserId = user;
                    db.CourseCompletions.Add(newCC);
                }
                else
                {
                    CourseCompletion courseCompletion = courseCompletions;
                    courseCompletion.DateCompleted = DateTime.Now;
                    db.Entry(courseCompletion).State = EntityState.Modified;

                }
                db.SaveChanges();
            }
            else
            {
                Completion = false;
            }
            return;
        }
    }
    #endregion

    #region Course Completion Metadata
    public class CourseCompletionMetadata
    {

        //public int CourseCompletionId { get; set; }

        [Required(ErrorMessage = "A User is required")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "A Course is required")]
        public int CourseId { get; set; }

        [Display(Name = "Completed")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "A Completion Date is required")]
        public System.DateTime DateCompleted { get; set; }
    }

    [MetadataType(typeof(CourseCompletionMetadata))]
    public partial class CourseCompletion
    {


    }
    #endregion

    #region Lesson Metadata
    public class LessonMetadata
    {
        //public int LessonId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "*Value must be 200 characters or less")]
        [Display(Name = "Lesson Title")]
        public string LessonTitle { get; set; }

        [Required(ErrorMessage = "A Course is required")]
        public int CourseId { get; set; }

        [StringLength(300, ErrorMessage = "*Value must be 300 characters or less")]
        [Display(Name = "Lesson Description")]
        [DisplayFormat(NullDisplayText = "[-N/A-]")]
        public string Introduction { get; set; }

        [StringLength(250, ErrorMessage = "*Value must be 250 characters or less")]
        [Display(Name = "Lesson Video")]
        [DisplayFormat(NullDisplayText = "[-N/A-]")]
        public string VideoURL { get; set; }

        [StringLength(200, ErrorMessage = "*Value must be 200 characters or less")]
        [Display(Name = "Lesson PDF")]
        [DisplayFormat(NullDisplayText = "[-N/A-]")]
        public string PdfFilename { get; set; }

        [StringLength(200, ErrorMessage = "*Value must be 200 characters or less")]
        [Display(Name = "Lesson Image")]
        [DisplayFormat(NullDisplayText = "[-N/A-]")]
        public string LessonPhoto { get; set; }

        [Required(ErrorMessage = "Active is required")]
        public bool IsActive { get; set; }
    }

    [MetadataType(typeof(LessonMetadata))]
    public partial class Lesson
    {
        public bool Completion { get; set; }


    }
    #endregion

    #region Lesson View Metadata
    public class LessonViewMetadata
    {
        //public int LessonViewedId { get; set; }

        [Required(ErrorMessage = "A User is required")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "A Lesson is required")]
        public int LessonId { get; set; }

        [Display(Name = "Viewed")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "A View Date is required")]
        public System.DateTime DateViewed { get; set; }
    }

    [MetadataType(typeof(LessonViewMetadata))]
    public partial class LessonView { }
    #endregion


}
