using System.ComponentModel.DataAnnotations;

namespace WealthManager.Modules.Identity.Application.DTOs;

public record RegisterRequest
(
    [Required(ErrorMessage = "Email can not be empty")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    string Email,

    [Required]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    string Password,

    [Required]
    string FullName
);