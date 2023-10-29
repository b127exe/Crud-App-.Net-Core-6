using CrudASPCore.Data;
using CrudASPCore.Models;
using Microsoft.AspNetCore.Mvc;

namespace CrudASPCore.Controllers.UserImage
{
    public class UserController : Controller
    {
        private readonly MartDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private string newImageName;

        public UserController(MartDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            this._context = context;
            this._webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(User user)
        {
            try
            {
                var uploadFile = UploadImage(user);
                var data = new User()
                {
                    Name = user.Name,
                    Address = user.Address,
                    Age = user.Age,
                    ImageName = uploadFile
                };
                _context.Users.Add(data);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return View();
        }

        public IActionResult Delete(int id)
        {
            if (id == 0 || _context.Users == null)
            {
                return NotFound();
            }
            var data = _context.Users.Where(m => m.Id == id).SingleOrDefault();
            if (data != null)
            {
                // Image Delete 
                string deleteImageFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/user-images/");
                string currentImage = Path.Combine(Directory.GetCurrentDirectory(), deleteImageFolder, data.ImageName);
                if (currentImage != null)
                {
                    if (System.IO.File.Exists(currentImage))
                    {
                        System.IO.File.Delete(currentImage);
                    }
                }

                _context.Users.Remove(data);
            }
            _context.SaveChanges();
            return RedirectToAction("Index", "User");

        }

        public IActionResult Edit(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }
            var user = _context.Users.Where(m => m.Id == id).FirstOrDefault();
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int? id, User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            // add new Image
            if (user.ImageFile != null)
            {
                this.newImageName = UploadImage(user);

                // delete old image 
                string deleteImageFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/user-images/");
                string currentImage = Path.Combine(Directory.GetCurrentDirectory(), deleteImageFolder, user.ImageName);
                if (currentImage != null)
                {
                    if (System.IO.File.Exists(currentImage))
                    {
                        System.IO.File.Delete(currentImage);
                    }
                }
            }
            else
            {
                this.newImageName = user.ImageName;
            }
            var data = new User()
            {
                Id = user.Id,
                Name = user.Name,
                Address = user.Address,
                Age = user.Age,
                ImageName = this.newImageName
            };

            _context.Users.Update(data);
            _context.SaveChanges();
            return RedirectToAction("Index", "User");
        }

        private string UploadImage(User user)
        {
            string uniqueFileName = string.Empty;
            if (user.ImageFile != null)
            {
                string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/user-images/");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + user.ImageFile.FileName;
                string filePath = Path.Combine(uploadFolder, uniqueFileName);
                using (var filestream = new FileStream(filePath, FileMode.Create))
                {
                    user.ImageFile.CopyTo(filestream);
                }

            }
            return uniqueFileName;
        }

    }
}
