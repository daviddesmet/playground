namespace Vue2API.ViewModels
{
    //using System.ComponentModel.DataAnnotations;

    public class AuthenticatorViewModel
    {
        //[Required]
        //[Display(Name = "Verification Code")]
        //[DataType(DataType.Text)]
        //[StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        public string Code { get; set; }

        public string SharedKey { get; set; }

        public string AuthenticatorUri { get; set; }
    }
}