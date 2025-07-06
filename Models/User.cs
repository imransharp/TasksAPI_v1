using System.ComponentModel.DataAnnotations;

namespace TasksAPI.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public List<TaskItem> Tasks { get; set; } = new();

    public string PasswordSalt { get; set; } = string.Empty;
}
