using CourseProject1.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CourseProject1.Models;
using CourseProject1.Models.Enums;
using CourseProject1.Data;

namespace UsersApp.Controllers
{
    public class AccountController(SignInManager<User> signInManager, UserManager<User> userManager,RoleManager<IdentityRole> _roleManager, AppDbContext _context) : Controller
    {
        List<Roles> roles = Enum.GetValues(typeof(Roles)).Cast<Roles>().ToList();
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.UserName!);
                if (user != null && await userManager.CheckPasswordAsync(user, model.Password!))
                {
                    if (user.IsActive == false)
                    {
                        ModelState.AddModelError(string.Empty, "Your account is blocked. Please contact support.");
                        return View(model);
                    }
                }
                //login
                var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);

                if (result.Succeeded)
                {
                    await userManager.UpdateAsync(user);
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid login attempt");
                return View(model);
            }
            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            foreach (var role in roles)
            {
                if (!(await _roleManager.RoleExistsAsync(role.ToString())))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role.ToString()));
                }
            }
            if (ModelState.IsValid)
            {
                User user = new()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.UserName,
                    Email = model.Email,
                    IsActive = true,
                    Collections = new List<Collection>()
                };
                var result = await userManager.CreateAsync(user, model.Password!);
                await userManager.AddToRoleAsync(user, Roles.Member.ToString());
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, false);

                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> Edit(EditVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);

                if (user == null)
                {
                    return NotFound(); // User not found
                }

                // Update user properties with the new values
                user.UserName = model.UserName;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                // Update other properties as needed

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Redirect to a success page or action
                return RedirectToAction("Index", "Home");
            }

            // If model state is invalid, return the same view with validation errors
            return View(model);
        }
    }
    }
