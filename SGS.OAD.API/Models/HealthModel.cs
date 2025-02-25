#nullable disable

namespace SGS.OAD.API.Models;

public class HealthModel
{
    public string Status { get; set; }
    public Checks Checks { get; set; }
}

public class Checks
{
    public string Name { get; set; }
    public string Status { get; set; }
    public string Duration { get; set; }
    public string Description { get; set; }
}
