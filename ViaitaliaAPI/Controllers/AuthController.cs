using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViaitaliaAPI.Data;
using ViaitaliaAPI.Models;
using ViaitaliaAPI.Models.DTO;
using ViaitaliaAPI.Repositories;
using ViaitaliaAPI.Services;

namespace ViaitaliaAPI.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly ITokenRepository _tokenRepository;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly TravelAuthDBContext _authContext;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;

        public AuthController(
            UserManager<IdentityUser> userManager,
            ITokenRepository tokenRepository,
            SignInManager<IdentityUser> signInManager,
            TravelAuthDBContext authContext,
            IEmailSender emailSender,
            IConfiguration configuration)
        {
            this.userManager = userManager;
            _tokenRepository = tokenRepository;
            _signInManager = signInManager;
            _authContext = authContext;
            _emailSender = emailSender;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterRequestDto registerRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return View(registerRequestDto);
            }

            var identityUser = new IdentityUser
            {
                UserName = registerRequestDto.UserName,
                Email = registerRequestDto.UserName
            };

            var identityUserResult = await userManager.CreateAsync(identityUser, registerRequestDto.Password);
            if (identityUserResult.Succeeded)
            {
                // Every new account is Reader only — this is not client-controlled.
                var roleResult = await userManager.AddToRoleAsync(identityUser, "Reader");
                if (roleResult.Succeeded)
                {
                    TempData["SuccessMessage"] = "User registered successfully! Please login.";
                    return RedirectToAction("Login", "Auth");
                }
            }

            ModelState.AddModelError("", "Something went wrong, please try again.");
            return View(registerRequestDto);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginRequestDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return View(loginRequestDto);
            }

            var user = await userManager.FindByEmailAsync(loginRequestDto.UserName);
            if (user != null)
            {
                var isValidated = await userManager.CheckPasswordAsync(user, loginRequestDto.Password);
                if (isValidated)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    var roles = await userManager.GetRolesAsync(user);
                    var token = _tokenRepository.CreateJWTToken(user, roles.ToList());
                    TempData["Token"] = token;

                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError("", "Incorrect username or password!");
            return View(loginRequestDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["SuccessMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Login", "Auth");
        }

        // GET: Auth/RequestWriterAccess
        [Authorize]
        public async Task<IActionResult> RequestWriterAccess()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            var roles = await userManager.GetRolesAsync(user);
            if (roles.Contains("Writer"))
            {
                ViewBag.AlreadyWriter = true;
                return View();
            }

            var hasPending = await _authContext.RoleRequests
                .AnyAsync(r => r.UserId == user.Id && r.Status == "Pending");

            ViewBag.HasPendingRequest = hasPending;
            return View();
        }

        // POST: Auth/RequestWriterAccess
        [HttpPost, ActionName("RequestWriterAccess")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> RequestWriterAccessConfirmed()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            var alreadyPending = await _authContext.RoleRequests
                .AnyAsync(r => r.UserId == user.Id && r.Status == "Pending");

            if (alreadyPending)
            {
                return RedirectToAction("RequestSent");
            }

            var roleRequest = new RoleRequest
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Email = user.Email,
                RequestedRole = "Writer",
                Status = "Pending",
                Token = Guid.NewGuid().ToString("N"),
                RequestedAt = DateTime.UtcNow
            };

            _authContext.RoleRequests.Add(roleRequest);
            await _authContext.SaveChangesAsync();

            var adminEmail = _configuration["EmailSettings:AdminEmail"];
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var approveUrl = $"{baseUrl}/Auth/ApproveRoleRequest?token={roleRequest.Token}";
            var rejectUrl = $"{baseUrl}/Auth/RejectRoleRequest?token={roleRequest.Token}";

            var body = $@"
                <p><strong>{roleRequest.Email}</strong> is requesting <strong>Writer</strong> access on ViaItalia.</p>
                <p>
                    <a href='{approveUrl}' style='padding:10px 20px;background:#2a8a7b;color:white;text-decoration:none;border-radius:6px;'>Approve</a>
                    &nbsp;
                    <a href='{rejectUrl}' style='padding:10px 20px;background:#c0392b;color:white;text-decoration:none;border-radius:6px;'>Reject</a>
                </p>";

            await _emailSender.SendEmailAsync(adminEmail, "ViaItalia — Writer Access Request", body);

            return RedirectToAction("RequestSent");
        }

        [Authorize]
        public IActionResult RequestSent()
        {
            return View();
        }

        // GET: Auth/ApproveRoleRequest?token=...
        [AllowAnonymous]
        public async Task<IActionResult> ApproveRoleRequest(string token)
        {
            var request = await _authContext.RoleRequests
                .FirstOrDefaultAsync(r => r.Token == token && r.Status == "Pending");

            if (request == null)
            {
                ViewBag.Result = "This request is invalid, already handled, or has expired.";
                return View("RoleRequestResult");
            }

            var user = await userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                ViewBag.Result = "The requesting user no longer exists.";
                return View("RoleRequestResult");
            }

            await userManager.AddToRoleAsync(user, request.RequestedRole);

            request.Status = "Approved";
            request.ReviewedAt = DateTime.UtcNow;
            await _authContext.SaveChangesAsync();

            ViewBag.Result = $"{request.Email} has been granted {request.RequestedRole} access.";
            return View("RoleRequestResult");
        }

        // GET: Auth/RejectRoleRequest?token=...
        [AllowAnonymous]
        public async Task<IActionResult> RejectRoleRequest(string token)
        {
            var request = await _authContext.RoleRequests
                .FirstOrDefaultAsync(r => r.Token == token && r.Status == "Pending");

            if (request == null)
            {
                ViewBag.Result = "This request is invalid, already handled, or has expired.";
                return View("RoleRequestResult");
            }

            request.Status = "Rejected";
            request.ReviewedAt = DateTime.UtcNow;
            await _authContext.SaveChangesAsync();

            ViewBag.Result = $"Request from {request.Email} has been rejected.";
            return View("RoleRequestResult");
        }
    }
}