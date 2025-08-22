using CourseWork.Models;
using CourseWork.Models.ViewModels;
using CourseWork.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CourseWork.Controllers
{
    [Authorize]
    public class UserController : Controller
    {

        private readonly UserManager<User> _userManager;
        private readonly DropboxService _dropboxService;


        public UserController(UserManager<User> userManager, DropboxService dropboxService)
        {
            _userManager = userManager;
            _dropboxService = dropboxService;
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new UserViewModel
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhotoUrl = user.PhotoUrl,
                Email = user.Email,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePhoto(IFormFile newPhoto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            if (newPhoto == null || newPhoto.Length == 0)
            {
                TempData["ErrorMessage"] = "File not selected";
                return RedirectToAction(nameof(Profile));
            }

            string[] format = new[] {".jpg", ".jpeg", ".png", ".gif", ".webp", ".jfif"};
            string fileExtension = Path.GetExtension(newPhoto.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(fileExtension) || !format.Contains(fileExtension))
            {
                TempData["ErrorMessage"] = "Can upload only: JPG, JPEG, PNG, GIF, WEBP, JFIF";
                return RedirectToAction(nameof(Profile));
            }

            const int maxFileSize = 5 * 1024 * 1024; 
            if (newPhoto.Length > maxFileSize)
            {
                TempData["ErrorMessage"] = "The file size must not exceed 5 MB.";
                return RedirectToAction(nameof(Profile));
            }

            try
            {

                if (!string.IsNullOrEmpty(user.DropboxPath))
                {
                    await _dropboxService.DeleteFileAsync(user.DropboxPath);
                }


                using var stream = newPhoto.OpenReadStream();
                string url = await _dropboxService.UploadUserAvatarAsync(
                    stream,
                    newPhoto.FileName,
                    user.Id);

               
                string newPath = $"/avatars/{user.Id}_avatar_{newPhoto.FileName}";

                user.DropboxPath = newPath;
                user.PhotoUrl = url;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    TempData["ErrorMessage"] = "Error save profile";
                    return RedirectToAction(nameof(Profile));
                }

                TempData["SuccessMessage"] = "Avatar update successful";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
            }

            return RedirectToAction(nameof(Profile));
        }


        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new EditViewModel
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            user.UserName = model.UserName;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            TempData["SuccessMessage"] = "Profile update successful!";
            return RedirectToAction(nameof(Profile));
        }
    }
}
