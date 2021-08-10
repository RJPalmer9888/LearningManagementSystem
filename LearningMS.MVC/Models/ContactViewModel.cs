using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations; //Brought in in order to use the built in
                                            //validations on our Model

namespace LearningMS.MVC.Models
{
    public class ContactViewModel
    {
        [Required(ErrorMessage = "Name is Required*")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public string Subject { get; set; }

        [Required(ErrorMessage = "Must provide message content*")]
        [UIHint("MultilineText")] //Provides a textarea in the UI
        public string Message { get; set; }
    }
}