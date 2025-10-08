using Hotel_Managment.DTOs;
using Hotel_Managment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Hotel_Managment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")] // مهم: يسمح بس للأدمن
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        //Get all users
        
        [HttpGet("Admins")]
        public async Task<IActionResult> GetAdmins()
        {
            var users = await _userManager.GetUsersInRoleAsync("Admin");

            var result = users.Select(u => new
            {
                u.Id,
                u.UserName,
                u.Email,
                u.PhoneNumber,
                u.Country,
                u.CreatedAt
            });

            return Ok(result);
        }

        [HttpGet("Owners")]
        public async Task<IActionResult> GetOwners()
        {
            var users = await _userManager.GetUsersInRoleAsync("HotelOwner");

            var result = users.Select(u => new
            {
                u.Id,
                u.UserName,
                u.Email,
                u.PhoneNumber,
                u.Country,
                u.CreatedAt
            });

            return Ok(result);
        }

        [HttpGet("Customers")]
        public async Task<IActionResult> GetCustomers()
        {
            var users = await _userManager.GetUsersInRoleAsync("Customer");

            var result = users.Select(u => new
            {
                u.Id,
                u.UserName,
                u.Email,
                u.PhoneNumber,
                u.Country,
                u.CreatedAt
            });

            return Ok(result);
        }


        // Get user by id
        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.PhoneNumber,
                user.Country,
                user.CreatedAt
            });
        }

        // Delete user
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "User deleted successfully" });
        }

        // Change role of a user
        [HttpPost("users/{id}/set-role")]
        public async Task<IActionResult> SetUserRole(string id, [FromBody] string role)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            var result = await _userManager.AddToRoleAsync(user, role);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = $"User role updated to {role}" });
        }


        // Register a new Admin (or any role)
        [HttpPost("register-user")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterDto model, [FromQuery] string role = "Admin")
        {
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return BadRequest(new { message = "Email is already taken" });
            }

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                Country = model.Country ?? "Unknown",
                PhoneNumber = model.Phone,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(user, role);

            return Ok(new { message = $"{role} registered successfully!" });
        }
    }
}
