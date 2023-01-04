using MVC_Core_Mid_Monthly_1268474.Models;

namespace MVC_Core_Mid_Monthly_1268474.ViewModels
{
    public class DataViewModel
    {
        public int SelectedExamID { get; set; }
        public IEnumerable<Course> Courses { get; set; } = default!;
        public IEnumerable<Module> Modules { get; set; } = default!;
        public IEnumerable<CourseModule> CourseModules { get; set; } = default!;
        public IEnumerable<ExamResult> ExamResults { get; set; } = default!;
        public IEnumerable<Exam> Exams { get; set; } = default!;
        public IEnumerable<Trainne> Trainnes { get; set; } = default!;
    }
}
