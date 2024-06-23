using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Services.Extensions;

public static class ClaimsPrincipalExtensions
{
    private static bool Verify(ClaimsPrincipal principal)
    {
        //principal.Claims.Any();
        if (!principal.Claims.Any())
            return false;
        else
            return true;//throw new UnauthorizedAccessException();
    }

    public static int GetId(this ClaimsPrincipal principal)
    {
        if (Verify(principal))
        {
            return principal.Claims.Where(claim => claim.Type == "Id").Select(claim => int.Parse(claim.Value)).Single();
        }
        return 0;
    }

    public static int GetTenantId(this ClaimsPrincipal principal)
    {
        Verify(principal);
        return principal.Claims.Where(claim => claim.Type == "TenantId")
            .Select(claim => int.Parse(claim.Value))
            .Single();
    }

    public static DateTime GetTokenDateIssued(this ClaimsPrincipal principal)
    {
        Verify(principal);
        return principal.Claims.Where(claim => claim.Type == "DateIssued")
            .Select(claim => DateTime.Parse(claim.Value))
            .Single();
    }

    public static IEnumerable<string> GetRoles(this ClaimsPrincipal principal)
    {
        Verify(principal);
        return principal.Claims.Where(claim => claim.Type == ClaimTypes.Role).Select(claim => claim.Value);
    }

    public static IEnumerable<string> GetPermissions(this ClaimsPrincipal principal)
    {
        Verify(principal);
        return principal.Claims.Where(claim => claim.Type == "Permission").Select(claim => claim.Value);
    }
}