using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Identity.Api.DTOs;

public class AddUserDTO
{
    [Required]
    [EmailAddress(ErrorMessage = "Please provide a valid email address.")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Please provide a username")]
    public string UserName { get; set; } = "";

    [Required(ErrorMessage = "Please provide a password")]
    public string Password { get; set; } = "";

    [Required]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string PasswordConfirm { get; set; } = "";
}
