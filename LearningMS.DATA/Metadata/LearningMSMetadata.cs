using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningMS.DATA/*.Metadata*/
{
    #region Course Metadata
    public class CourseMetadata
    {
        //public string CourseId { get; set; }

        [DisplayFormat(NullDisplayText = "[-N/A-]")]
        [UIHint("MultilineText")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(200, ErrorMessage = "*Value must be 200 characters or less")]
        [Display(Name = "Course Name")]
        public string CourseName { get; set; }

        [StringLength(500, ErrorMessage = "*Value must be 500 characters or less")]
        [Display(Name = "Course Description")]
        public string CourseDescription { get; set; }

        [StringLength(200, ErrorMessage = "*Value must be 200 characters or less")]
        [Display(Name = "Course Image")]
        public string CoursePhoto { get; set; }

        public bool IsActive { get; set; }
    }

    [MetadataType(typeof(CourseMetadata))]
    public partial class Course { }
    #endregion
}
