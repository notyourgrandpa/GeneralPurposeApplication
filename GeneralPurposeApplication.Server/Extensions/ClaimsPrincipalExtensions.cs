﻿using System.Security.Claims;

namespace GeneralPurposeApplication.Server.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new InvalidOperationException("User Id claim not found.");
        }
    }
}
