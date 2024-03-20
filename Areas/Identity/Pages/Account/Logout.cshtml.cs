// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ASPIdentityApp.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using log4net;

namespace ASPIdentityApp.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        private static readonly ILog _log = LogManager.GetLogger(typeof(LogoutModel));

        public LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            try
            {
                await _signInManager.SignOutAsync();
                _log.Info("User logged out.");
                _logger.LogInformation("User logged out.");

                if (returnUrl != null)
                {
                    return LocalRedirect(returnUrl);
                }
                else
                {
                    return RedirectToPage();
                }
            }
            catch (Exception ex )
            {
                _log.Error("An error occurred while processing the logout request.");
                _logger.LogError(ex, "An error occurred while processing the logout request.");
                return RedirectToPage("/Home/Error");
            }
        }

    }
}
