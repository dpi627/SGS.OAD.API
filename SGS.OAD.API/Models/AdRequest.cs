#nullable disable

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SGS.OAD.API.Models;

public class AdRequest
{
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    [DefaultValue("APAC")]
    public string Domain { get; set; } = "APAC";
}
