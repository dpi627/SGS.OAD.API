#nullable disable

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SGS.OAD.API.Models;

public class AdValidModel
{
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Password { get; set; }
    [DefaultValue("APAC")]
    public string Domain { get; set; } = "APAC";
}
