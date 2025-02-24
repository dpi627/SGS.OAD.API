#nullable disable

namespace SGS.OAD.API.Models;

public class AdValidModel
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Domain { get; set; } = "APAC";
}
