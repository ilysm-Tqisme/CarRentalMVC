namespace CarRentalMVC.Models
{
    public class AuthViewModels
    {
        public class RegisterViewModel
        {
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Address { get; set; }
            public string Password { get; set; }
            public string ConfirmPassword { get; set; }
        }

        public class LoginViewModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class ChangePasswordViewModel
        {
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
            public string ConfirmNewPassword { get; set; }
        }
    }
}
