using MVC_Core_Mid_Monthly_1268474.Models;

namespace MVC_Core_Mid_Monthly_1268474.HostedServices
{
    public class DbSeederHostedService : IHostedService
    {

        IServiceProvider serviceProvider;
        public DbSeederHostedService(
            IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;

        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (IServiceScope scope = serviceProvider.CreateScope())
            {

                var db = scope.ServiceProvider.GetRequiredService<CourseDbContext>();

                await SeedDbAsync(db);

            }
        }
        public async Task SeedDbAsync(CourseDbContext db)
        {
            await db.Database.EnsureCreatedAsync();
            if (!db.Courses.Any())
            {
                var c = new Course { BatchName = "CS/ACSL/R-50", CourseName = "CS", CourseDesc = "Full IT Related Course", CourseDuration = "1 year", StartDate = new DateTime(2021, 01, 05), EndDate = new DateTime(2022, 02, 06), Available = true };
                var e = new Exam { ExamName = "Monthly", ExamFee=00.00M};
                var t = new Trainne { TrainneName = " Nur Sakib", TraineeAddress = "Mirpur-10", Email = "nursakib47@gmail.com", IsRunning = true, BirthDate = new DateTime(1997, 01, 03), Picture = "1.jpg" };
                c.Trainnes.Add(t);
                
                var ex = new ExamResult { Result = Result.pass, Exam = e, Trainne = t };

                await db.Courses.AddAsync(c);
                await db.Exams.AddAsync(e);
                await db.ExamResults.AddAsync(ex);

                await db.SaveChangesAsync();
            }

        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
