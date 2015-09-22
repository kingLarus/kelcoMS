using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Linq;
using System.Web.Security;
using System.Data.Entity.Validation;

namespace LoginSystem.Controllers
{
    public class UserController : Controller
    {


        // GET: User
        public ActionResult Index()
        {
            return View();
        }
            [HttpGet]
        public ActionResult LogIn() { 
        
        return View();

        }
            [HttpPost]
            public ActionResult LogIn(LoginSystem.Models.UserModel user)
            {
               

                if(ModelState.IsValid)
                {
                    if (isValid(user.Email, user.Password))
                    {

                        FormsAuthentication.SetAuthCookie(user.Email, false);
                        return RedirectToAction("Index", "Home");
                    }
                    else {
                        ModelState.AddModelError("","Log in Data is incorrect!");
                    
                    }

                }

                return View(user);
            }

            [HttpGet]
            public ActionResult Registration()
            {
                return View();
            
            }

            [HttpPost]
            public ActionResult Registration(LoginSystem.Models.UserModel user)
            {


                if (ModelState.IsValid)
                {
                    var db = new MainDbEntities();
                    try
                    {
                        var crypto = new SimpleCrypto.PBKDF2();
                        var encrpPass = crypto.Compute(user.Password);
                        var sysUser = db.SystemUsers.Create();

                        sysUser.Email = user.Email;
                        sysUser.Password = encrpPass;
                        sysUser.PasswordSalt = crypto.Salt;
                        sysUser.UserId = "2";
                        db.Configuration.ValidateOnSaveEnabled = false;
                        db.SystemUsers.Add(sysUser);
                        db.SaveChanges();

                        return RedirectToAction("Index", "Home");


                    }
                 

                    finally { }


                }
                else {
                    ModelState.AddModelError("","Log in data is incorrect");
                
                }

                return View(user);

            }

            public ActionResult LogOut()
            {
                return View();
            
            }
            private bool isValid(string email, string password)
            {
                var crypto = new SimpleCrypto.PBKDF2();
            bool isValid = false;

            var db = new MainDbEntities();
                 try
                {
                
                    var user = db.SystemUsers.FirstOrDefault(u =>u.Email == email);

                     if(user != null)
                     {
                         if(user.Password == crypto.Compute(password, user.PasswordSalt))
                         {
                             isValid = true;
                         
                         }
                     }
                }
                finally{
                 }

                return isValid;
            }
    }
}