using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Core_Mid_Monthly_1268474.ViewModels;
using MVC_Core_Mid_Monthly_1268474.Models;

namespace MVC_Core_Mid_Monthly_1268474.Controllers
{
    public class MainController : Controller
    {
        CourseDbContext db;
        IWebHostEnvironment env;
        public MainController(CourseDbContext db, IWebHostEnvironment env)
        {
            this.db = db;
            this.env = env;
        }

        public async Task<IActionResult> Index()
        {
            var id = 0;
            if (db.Trainnes.Any())
            {
                id = db.Trainnes.ToList()[0].TrainneID;
            }

            DataViewModel data = new DataViewModel();
            data.SelectedExamID = id;
            data.Courses = await db.Courses.ToListAsync();

            data.ExamResults = await db.ExamResults.Where(ti => ti.TrainneID == id).ToListAsync();
            data.Exams = await db.Exams.ToListAsync();
            data.Trainnes = await db.Trainnes.ToListAsync();
            return View(data);
        }
        #region child actions
        public async Task<IActionResult> GetSelectedExamResults(int id)
        {


            var ExamResults = await db.ExamResults.Include(x => x.Trainne).Include(x => x.Exam).Where(ti => ti.ExamID == id).ToListAsync();

            return PartialView("_ExamResultTable", ExamResults);
        }

        //create cust
        public IActionResult CreateCourse()
        {
            return PartialView("_CreateCourse");
        }
        [HttpPost]
        public async Task<IActionResult> CreateCourse(Course c)
        {
            if (ModelState.IsValid)
            {
                await db.Courses.AddAsync(c);
                await db.SaveChangesAsync();
                return Json(c);
            }
            return BadRequest("Unexpected error");
        }

        //edit cust
        public async Task<IActionResult> EditCourse(int id)
        {
            var data = await db.Courses.FirstOrDefaultAsync(c => c.CourseID == id);
            return PartialView("_EditCourse", data);
        }
        [HttpPost]
        public async Task<IActionResult> EditCourse(Course c)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Json(c);
            }
            return BadRequest("Unexpected error");
        }

        //delete cust
        [HttpPost]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            if (!await db.Trainnes.AnyAsync(o => o.CourseID == id))
            {
                var o = new Course { CourseID = id };
                db.Entry(o).State = EntityState.Deleted;
                try
                {
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
                return Json(new { success = true, message = "Data deleted" });
            }
            return Json(new { success = false, message = "Cannot delete, item has related child." });
        }

        ///add trainee <summary>
        public async Task<IActionResult> CreateTrainne(int id)
        {
            var data = await db.Trainnes.FirstOrDefaultAsync(c => c.TrainneID == id);
            ViewData["Courses"] = await db.Courses.ToListAsync();
            return PartialView("_CreateTrainne", data);
        }
        [HttpPost]
        public async Task<IActionResult> CreateTrainne(TrainneInputModel p)
        {

            if (ModelState.IsValid)
            {
                var trainne = new Trainne
                {
                    TrainneName = p.TrainneName,
                    TraineeAddress = p.TraineeAddress,
                    Email = p.Email,
                    BirthDate = p.BirthDate,
                    IsRunning = p.IsRunning,
                    CourseID = p.CourseID,
                    TrainneID = p.TrainneID
                };
                string fileName = Guid.NewGuid() + Path.GetExtension(p.Picture.FileName);
                string savePath = Path.Combine(this.env.WebRootPath, "Pictures", fileName);
                var fs = new FileStream(savePath, FileMode.Create);
                p.Picture.CopyTo(fs);
                fs.Close();
                trainne.Picture = fileName;
                await db.Trainnes.AddAsync(trainne);
                await db.SaveChangesAsync();
                var x = Gettrainee(trainne.TrainneID);
                return Json(trainne);


            }
            ViewData["Trainnes"] = await db.Trainnes.ToListAsync();
            ViewData["Courses"] = await db.Courses.ToListAsync();
            return BadRequest("Falied to insert product");
        }
        private Trainne? Gettrainee(int id)
        {
            return db.Trainnes.Include(x => x.Course).FirstOrDefault(x => x.TrainneID == id);
        }
        //Edit trainne
        public async Task<IActionResult> EditTrainne(int id)
        {
            ViewData["Courses"] = await db.Courses.ToListAsync();
            var data = await db.Trainnes.FirstAsync(x => x.TrainneID == id);
            ViewData["CurrentPic"] = data.Picture;
            return PartialView("_EditTrainne", new TrainneEditModel { TrainneID = data.TrainneID, TrainneName = data.TrainneName, TraineeAddress = data.TraineeAddress, Email = data.Email, BirthDate = data.BirthDate, IsRunning = data.IsRunning, CourseID = data.CourseID });
        }
        [HttpPost]
        public async Task<IActionResult> EditTrainne(TrainneEditModel p)
        {
            if (ModelState.IsValid)
            {
                var trainne = await db.Trainnes.FirstAsync(x => x.TrainneID == p.TrainneID);
                trainne.TrainneName = p.TrainneName;
                trainne.TraineeAddress = p.TraineeAddress;
                trainne.Email = p.Email;
                trainne.BirthDate = p.BirthDate;
                trainne.IsRunning = p.IsRunning;
                trainne.CourseID = p.CourseID;
                if (p.Picture != null)
                {
                    string fileName = Guid.NewGuid() + Path.GetExtension(p.Picture.FileName);
                    string savePath = Path.Combine(this.env.WebRootPath, "Pictures", fileName);
                    var fs = new FileStream(savePath, FileMode.Create);
                    p.Picture.CopyTo(fs);
                    fs.Close();
                    trainne.Picture = fileName;
                }


                await db.SaveChangesAsync();
                var x = Gettrainee(trainne.TrainneID);
                return Json(x);
            }
            ViewData["Courses"] = await db.Courses.ToListAsync();
            return BadRequest();
        }
        //delete tranie
        public async Task<IActionResult> DeleteTrainne(int id)
        {
            if (!await db.ExamResults.AnyAsync(o => o.TrainneID == id))
            {
                var o = new Trainne { TrainneID = id };
                db.Entry(o).State = EntityState.Deleted;
                try
                {
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
                return Json(new { success = true, message = "Data deleted" });
            }
            return Json(new { success = false, message = "Cannot delete, item has related child." });
        }

        //add exam
        public async Task<IActionResult> CreateExam()
        {
            ViewData["Trainnes"] = await db.Trainnes.ToListAsync();
            return PartialView("_CreateExam");
        }
        [HttpPost]
        public async Task<IActionResult> CreateExam(Exam o, int[] TrainneID, Result[] Result)
        {
            if (ModelState.IsValid)
            {
                for (var i = 0; i < TrainneID.Length; i++)
                {
                    o.ExamResults.Add(new ExamResult { TrainneID = TrainneID[i], Result = Result[i] });
                }
                await db.Exams.AddAsync(o);

                await db.SaveChangesAsync();


                var ord = await GetExam(o.ExamID);
                return Json(ord);
            }
            return BadRequest();
        }
        //eidt exam
        public async Task<IActionResult> EditExam(int id)
        {
            ViewData["Trainnes"] = await db.Trainnes.ToListAsync();
            var data = await db.Exams
                .Include(x => x.ExamResults).ThenInclude(x => x.Trainne)
                .FirstOrDefaultAsync(x => x.ExamID == id);
            return PartialView("_EditExam", data);
        }
        [HttpPost]
        public async Task<IActionResult> EditExam(Exam o)
        {
            if (ModelState.IsValid)
            {
                var existing = await db.Exams.FirstAsync(x => x.ExamID == o.ExamID);
                existing.ExamName = o.ExamName;
                existing.ExamFee = o.ExamFee;
                try
                {
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return Json(existing);
            }
            return BadRequest();
        }
        private async Task<Exam?> GetExam(int id)
        {
            var o = await db.Exams.FirstOrDefaultAsync(x => x.ExamID == id);
            return o;
        }

        //delete exam
        [HttpPost]
        public async Task<IActionResult> DeleteExam(int id)
        {
            var o = new Exam { ExamID = id };
            db.Entry(o).State = EntityState.Deleted;
            await db.SaveChangesAsync();
            return Json(new { success = true, message = "Data deleted" });
        }
        public async Task<IActionResult> CreateItem()
        {
            ViewData["Trainnes"] = await db.Trainnes.ToListAsync();
            return PartialView("_CreateItem");
        }

        //add examresult
        public async Task<IActionResult> CreateExamResult(int id)
        {
            ViewData["ExamID"] = id;
            ViewData["Trainnes"] = await db.Trainnes.ToListAsync();
            return PartialView("_CreateExamResult");
        }
        [HttpPost]
        public async Task<IActionResult> CreateExamResult(ExamResult oi)
        {
            if (ModelState.IsValid)
            {
                await db.ExamResults.AddAsync(oi);
                await db.SaveChangesAsync();
                var o = await GetOrderItem(oi.ExamID, oi.TrainneID);
                return Json(o);
            }
            return BadRequest();
        }
        //edit exam result
        public async Task<IActionResult> EditExamResult(int oid, int pid)
        {
            ViewData["Trainnes"] = await db.Trainnes.ToListAsync();
            ViewData["EaxmID"] = oid;
            var oi = await db.ExamResults.FirstAsync(x => x.ExamID == oid && x.TrainneID == pid);
            return PartialView("_EditExamResult", oi);
        }
        [HttpPost]
        public async Task<IActionResult> EditExamResult(ExamResult oi)
        {
            if (ModelState.IsValid)
            {
                db.Entry(oi).State = EntityState.Modified;
                await db.SaveChangesAsync();
                var o = await GetOrderItem(oi.ExamID, oi.TrainneID);
                return Json(o);
            }
            return BadRequest();
        }

        //delete exam result

        [HttpPost]
        public async Task<IActionResult> DeleteExamResult([FromQuery] int oid, [FromQuery] int pid)
        {

            var o = new ExamResult { TrainneID = pid, ExamID = oid };
            db.Entry(o).State = EntityState.Deleted;

            await db.SaveChangesAsync();

            return Json(new { success = true, message = "Data deleted" });

        }
        private async Task<ExamResult> GetOrderItem(int oid, int pid)
        {
            var oi = await db.ExamResults
                .Include(o => o.Exam)
                .Include(o => o.Trainne)
                .FirstAsync(x => x.ExamID == oid && x.TrainneID == pid);
            return oi;
        }
        #endregion
    }
}