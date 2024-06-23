namespace Services.DTOs;

public class UserAuthenticationRequest
{
    public string Id { get; set; }
    public string Role { get; set; }
    public string jwtToken { get; set; }
    public string Title { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; } //Email
    public string Email { get; set; } //Email
    public string Password { get; set; }
   
    //role?: Role;
    //jwtToken?: string;

}

public class UserAuthenticationResult
{
    public int UserId { get; set; }
    public string JwtToken { get; set; }
    public int ExpiresIn { get; set; }
    public bool success { get; set; }
    public UserAuthenticationResult() { }
    public UserAuthenticationResult(int UserId, string JwtToken, int ExpiresIn, bool success)
    {
        this.UserId = UserId;
        this.JwtToken = JwtToken;
        this.ExpiresIn = ExpiresIn;
        this.success = success;
    }
}