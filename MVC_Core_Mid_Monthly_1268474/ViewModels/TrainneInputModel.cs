using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using MVC_Core_Mid_Monthly_1268474.Models;

namespace MVC_Core_Mid_Monthly_1268474.ViewModels
{
    public class TrainneInputModel
    {
        public int TrainneID { get; set; }
        [Required, StringLength(50), Display(Name = "Trainee Name")]
        public string TrainneName { get; set; } = default!;
        [Required, StringLength(70), Display(Name = "Trainee Address")]
        public string TraineeAddress { get; set; } = default!;
        [Required, StringLength(50), DataType(DataType.EmailAddress)]
        public string Email { get; set; } = default!;
        [Required]
        public IFormFile Picture { get; set; } = default!;
        
        public bool IsRunning { get; set; }

        [Required, Column(TypeName = "date"), Display(Name = "Birth Date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime BirthDate { get; set; }
        [ForeignKey("Course")]
        public int CourseID { get; set; }
        public Course? Course { get; set; } = default!;
    }
}
