using System.Collections.Generic;
using uHome.Annotations;
using System.ComponentModel.DataAnnotations;

namespace uHome.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [LocalizedRequired]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [LocalizedRequired]
        public string Provider { get; set; }

        [LocalizedRequired]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [LocalizedRequired]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [LocalizedRequired]
        [Display(Name = "Email", ResourceType = typeof(Resources.Resources))]
        [EmailAddress]
        public string Email { get; set; }

        [LocalizedRequired]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(Resources.Resources))]
        public string Password { get; set; }

        [Display(Name = "RememberMe", ResourceType = typeof(Resources.Resources))]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [LocalizedRequired]
        [EmailAddress(ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "InvalidEmail",
            ErrorMessage = null)]
        [Display(Name = "Email", ResourceType = typeof(Resources.Resources))]
        public string Email { get; set; }

        [LocalizedRequired]
        [StringLength(100,
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "PasswordMinimumLength",
            MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(Resources.Resources))]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword", ResourceType = typeof(Resources.Resources))]
        [Compare("Password",
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "PasswordNotMatch")]
        public string ConfirmPassword { get; set; }

        public string RoleName { get; set; }

        public RegisterViewModel()
        {
        }

        public RegisterViewModel(string RoleName)
        {
            this.RoleName = RoleName;
        }
    }

    public class ResetPasswordViewModel
    {
        [LocalizedRequired]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [LocalizedRequired]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [LocalizedRequired]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
