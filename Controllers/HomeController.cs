using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wedding_Planner.Models;

namespace Wedding_Planner.Controllers
{
    public class HomeController : Controller
    {

        private MyContext dbContext;

        // here we can "inject" our context service into the constructor
        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            ViewModel viewM = new ViewModel();
            return View();
        }

        [HttpPost("Register")]
        public IActionResult Register(User NewUser)
        {
            if (ModelState.IsValid)
            {
                if (dbContext.User.Any(u => u.Email == NewUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already Exist!");
                    return View("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                NewUser.Password = Hasher.HashPassword(NewUser, NewUser.Password);
                System.Console.WriteLine("**********" + NewUser + "**" + NewUser.Password + "***" + Hasher.ToString());
                dbContext.User.Add(NewUser);
                dbContext.SaveChanges();

                HttpContext.Session.SetInt32("id", NewUser.UserId);
                int? logUser = HttpContext.Session.GetInt32("id");
                ViewBag.logUser = logUser;
                List<Wedding> Allw = dbContext.Wedding.Include(p => p.User).Include(a => a.Assoc_Wedding).ToList();
                System.Console.WriteLine("********** " + Allw + " *******");
                foreach (var a in Allw)
                {
                    if (a.date < DateTime.Now)
                    {
                        Wedding deleteW = dbContext.Wedding.FirstOrDefault(w => w.WeddingId == a.WeddingId);
                        System.Console.WriteLine("*************************" + deleteW.UserId);
                        dbContext.Remove(deleteW);
                        dbContext.SaveChanges();
                    }
                }
                return RedirectToAction("Privacy");
            }
            else
            {
                return View("Index");
            }
        }

        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("LoginProcess")]
        public IActionResult LoginProcess(ViewModel userSubmission)
        {
            if (ModelState.IsValid)
            {
                // If inital ModelState is valid, query for a user with provided email
                var userInDb = dbContext.User.FirstOrDefault(u => u.Email == userSubmission.LoginUser.Email);
                // If no user exists with provided email
                if (userInDb == null)
                {
                    // Add an error to ModelState and return to View!
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Index");
                }
                // Initialize hasher object
                var hasher = new PasswordHasher<LoginUser>();
                // verify provided password against hash stored in db
                // var result = hasher.VerifyHashedPassword (userSubmission, userInDb.Password, userSubmission.Password);
                // System.Console.WriteLine ("*********" + result);
                // result can be compared to 0 for failure
                if (hasher.VerifyHashedPassword(userSubmission.LoginUser, userInDb.Password, userSubmission.LoginUser.Password) == PasswordVerificationResult.Failed)
                {
                    ModelState.AddModelError("Email", "Invalid email/password");
                    return View("Index");
                }
                else
                {
                    HttpContext.Session.SetInt32("id", userInDb.UserId);
                    int? logUser = HttpContext.Session.GetInt32("id");

                    ViewBag.logUser = logUser;

                    List<Wedding> Allw = dbContext.Wedding.Include(p => p.User).Include(a => a.Assoc_Wedding).ToList();

                    System.Console.WriteLine("********** " + Allw + " *******");
                    foreach (var a in Allw)
                    {
                        if (a.date < DateTime.Now)
                        {
                            Wedding deleteW = dbContext.Wedding.FirstOrDefault(w => w.WeddingId == a.WeddingId);
                            System.Console.WriteLine("*************************" + deleteW.UserId);
                            dbContext.Remove(deleteW);
                            dbContext.SaveChanges();
                        }
                    }
                    return RedirectToAction("Privacy");
                }

            }
            else { return View("Index"); }
        }

        [HttpGet("Logout")]
        public IActionResult Logout()
        {
            int? logUser = HttpContext.Session.GetInt32("id");
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [HttpGet("Success")]
        public IActionResult Privacy()
        {
            int? logUser = HttpContext.Session.GetInt32("id");
            if (logUser == null)
            {
                return View("Index");
            }
            ViewBag.logUser = logUser;
            User loguser = dbContext.User.SingleOrDefault(User => User.UserId == logUser);
            ViewBag.loguserr = loguser;
            // ViewBag.logUser = logUser;
            ViewModel viewmodel = new ViewModel();
            viewmodel.NewUser = loguser;

            // List<Wedding> AllWedding = dbContext.Wedding.ToList ();
            // viewmodel.AllWeddings = AllWedding;

            List<Wedding> Allw = dbContext.Wedding.Include(p => p.User).Include(a => a.Assoc_Wedding).ToList();
            viewmodel.AllWeddings = Allw;

            System.Console.WriteLine("********** " + Allw + " *******");
            foreach (var a in Allw)
            {
                if (a.date < DateTime.Now)
                {
                    Wedding deleteW = dbContext.Wedding.FirstOrDefault(w => w.WeddingId == a.WeddingId);
                    System.Console.WriteLine("*************************" + deleteW.UserId);
                    dbContext.Remove(deleteW);
                    dbContext.SaveChanges();
                }
            }
            // List<Wedding> Allw = dbContext.Wedding.Include (p => p.Assoc_Wedding).ToList ();
            // viewmodel.AllWeddings = Allw;

            // User UserAssoc = dbContext.User.Include(a => a.Assoc_User).ThenInclude(p=>p.);
            return View(Allw);
        }

        [HttpGet("show/{WId}")]
        public IActionResult Show(int WId)
        {
            int? logUser = HttpContext.Session.GetInt32("id");
            if (logUser == null)
            {
                return View("Index");
            }
            User loginUser = dbContext.User.FirstOrDefault(u => u.UserId == logUser);
            // ViewModel viewmodel = new ViewModel ();
            // viewmodel.NewUser = loginUser;

            // Wedding CurrWedding = dbContext.Wedding.FirstOrDefault (w => w.WeddingId == WId);
            // viewmodel.NewWedding = CurrWedding;

            Wedding Allguests = dbContext.Wedding.Include(a => a.Assoc_Wedding).ThenInclude(a => a.User).FirstOrDefault(w => w.WeddingId == WId);
            ViewBag.Allguests = Allguests;

            return View(Allguests);
        }

        [HttpGet("Success/NewWedding")]
        public IActionResult NewWedding()
        {
            return View("NewWedding");
        }

        [HttpPost("Success/NewWedding")]
        public IActionResult NewWedding(Wedding submission)
        {
            if (ModelState.IsValid)
            {
                int? logUser = HttpContext.Session.GetInt32("id");

                submission.UserId = (int)logUser;
                dbContext.Add(submission);
                dbContext.SaveChanges();
                return RedirectToAction("Privacy");
            }
            else
            {
                return View("NewWedding");
            }
        }

        [HttpGet("AddUserToWedding/{WId}")]
        public IActionResult AddUserToWedding(int WId)
        {
            int? LoginUser = HttpContext.Session.GetInt32("id");
            System.Console.WriteLine("*********************************************************************");
            System.Console.WriteLine("***" + LoginUser + " ****** " + WId + "************************");
            User LogUser = dbContext.User.FirstOrDefault(User => User.UserId == LoginUser);
            Wedding thisWedding = dbContext.Wedding.FirstOrDefault(w => w.WeddingId == WId);
            Association NewGuest = new Association();
            NewGuest.UserId = LogUser.UserId;
            NewGuest.WeddingId = thisWedding.WeddingId;
            dbContext.association.Add(NewGuest);
            dbContext.SaveChanges();
            return RedirectToAction("Privacy");
        }

        [HttpGet("RemoveUserFromWedding/{WId}")]
        public IActionResult RemoveUserFromWedding(int WId)
        {
            int? LoginUser = HttpContext.Session.GetInt32("id");
            User LogUser = dbContext.User.FirstOrDefault(u => u.UserId == LoginUser);
            Association this_Wedding = dbContext.association.FirstOrDefault(u => u.WeddingId == WId && u.UserId == (int)LoginUser);
            dbContext.Remove(this_Wedding);
            dbContext.SaveChanges();
            return RedirectToAction("Privacy");
        }

        [HttpGet("Delete/{Wid}")]
        public IActionResult Delete(int WId)
        {
            int? logUser = HttpContext.Session.GetInt32("id");
            if (logUser == null)
            {
                return View("Index");
            }
            Wedding deleteW = dbContext.Wedding.FirstOrDefault(w => w.WeddingId == WId);
            System.Console.WriteLine("*************************" + deleteW.UserId);
            if (deleteW.UserId == logUser)
            {

                dbContext.Remove(deleteW);
                dbContext.SaveChanges();
            }
            else
            {
                return View("Privacy");
            }
            return RedirectToAction("Privacy");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}