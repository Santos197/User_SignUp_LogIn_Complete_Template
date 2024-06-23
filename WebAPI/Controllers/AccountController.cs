using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using Castle.Core.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Models.DataModels;
using Models.Helpers;
using Services.DTOs;
using Services.Extensions;
using Services.Helpers.MailService;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace WebAPI.Controllers;

[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class AccountController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IMailService _emailSender; //IEmailSender _emailSender;
    private readonly IOptions<AppSettings> _options;
    private readonly IAuth _auth;
    private readonly IAccountLogic _accountLogic;
    public AccountController(UserManager<IdentityUser> userManager, IMailService emailSender,IOptions<AppSettings> options,IAuth auth,IAccountLogic accountLogic)
    {
        this._userManager= userManager;
        this._emailSender = emailSender;
        this._options= options;
        this._auth = auth;
        this._accountLogic = accountLogic;
    }
    /// <summary>
    /// The only official way to get an access token for this API
    /// </summary>
    /// <remarks>
    /// For security reasons, the response doesn't confirm the username existence if the password is wrong.
    /// The Username and Password combination must be valid
    /// </remarks>
    /// <param name="request">User credentials</param>
    /// <response code="404">Invalid username or password</response>
    /// <response code="202">Login successful</response>
    [ProducesResponseType(202, Type = typeof(UserAuthenticationResult))]
    [ProducesResponseType(404, Type = null)]
    [HttpPost("Login")]  //Token
    public IActionResult Login([FromBody] UserAuthenticationRequest request)  //async Task
    {
        try
        {
            //var user = await _userManager.FindByEmailAsync(request.Email);
            //if (user == null)
            //{
            //    // User not found
            //    //return BadRequest("Invalid login attempt.");
            //    return StatusCode(StatusCodes.Status202Accepted, new { message = "Invalid login attempt..", success = false });
            //}

            //// Check if the email is confirmed
            //if (! await _userManager.IsEmailConfirmedAsync(user))
            //{
            //    // Email is not confirmed
            //    return StatusCode(StatusCodes.Status202Accepted,new { message = "Please confirm your email before logging in.",success=false });
            //}

            var res =  _auth.Authenticate(request);
            if (res.token == null)
                //return Unauthorized(new { message = "Username or password is incorrect"});
                return StatusCode(StatusCodes.Status202Accepted,
                        new { message = "Username or password is incorrect", success = false }); //UserAuthenticationResult(res.userId, res.token, options.Value.TokenExpirationMinutes));

            return StatusCode(StatusCodes.Status202Accepted,
                new UserAuthenticationResult(res.userId, res.token, _options.Value.TokenExpirationMinutes, true)); ;
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Creates a new token with the same credientials of the exisiting one
    /// </summary>
    /// <remarks>
    /// Use this to bypass the expiration date of the token without requiring the user to re-enter their passwords or dangerously save the username and password locally
    /// </remarks>
    /// <response code="200">New token</response>
    [Authorize]
    [ProducesResponseType(200, Type = typeof(UserAuthenticationResult))]
    [HttpPost("RefreshToken")]
    public IActionResult RefreshToken()
    {
        var userId = User.GetId();
        if (userId != 0)
        {
          var res = _auth.GenerateToken(userId);
            return Ok(new UserAuthenticationResult(res.userId, res.token, _options.Value.TokenExpirationMinutes, true));
        }
        else
            return Ok();
    }

    /// <summary>
    /// Registers the user into the database
    /// </summary>
    /// <param name="request">User credentials</param>
    /// <response code="200">Operation successful</response>
    /// <response code="409">Username already exists</response>
    [ProducesResponseType(200, Type = null)]
    [ProducesResponseType(409, Type = null)]
    [HttpPost("Register")]
    public IActionResult Register([FromBody] UserAuthenticationRequest request)  //async Task
    {
        var user = new IdentityUser { UserName = request.UserName, Email = request.Email };
        if (_accountLogic.Register(request, "User"))
        {
            //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //var callbackUrl = Url.Action(nameof(ConfirmEmail), "Account", new { userId = user.Id, code }, protocol: HttpContext.Request.Scheme);
            //_emailSender.SendEmail(request.Email, "Confirm your email",$"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>.","santosh07mca21@kcc.edu.np");

            return Ok(new { success = true });
        }

        return StatusCode(StatusCodes.Status202Accepted,
         new { message = "User already exists", success = false }); //UserAuthenticationResult(res.userId, res.token, options.Value.TokenExpirationMinutes));
        
        //return StatusCode(StatusCodes.Status409Conflict);
    }

    /// <summary>
    /// Logs-out user from all devices
    /// </summary>
    /// <remarks>
    /// <b style="color:red">WARNING</b>: This will deny access to all current tokens; user will be logged-out from <b>ALL</b> devices and each device will have to log-in again
    /// if you want to log out the user from a single client, just get rid of the token that your client stores
    /// </remarks>
    /// <response code="200">User logged-out successfully</response>
    [Authorize]
    [ProducesResponseType(200, Type = null)]
    [HttpPost("Logout")]
    public IActionResult Logout()
    {
        _auth.Logout(User.GetId());
        return Ok();
    }

    [HttpGet("ConfirmEmail")]
    public async Task<IActionResult> ConfirmEmail(string userId, string code)
    {
        if (userId == null || code == null)
        {
            return BadRequest("Error confirming your email.");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return BadRequest("User not found.");
        }

        var result = await _userManager.ConfirmEmailAsync(user, code);
        return result.Succeeded ? Ok("Email confirmed successfully!") : BadRequest("Error confirming your email.");
    }
}
