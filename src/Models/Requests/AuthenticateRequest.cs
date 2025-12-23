namespace OdidoAlerter.Models.Requests;

public class AuthenticateBasic
{
    public string UserID { get; set; } = string.Empty;
    public string ClientPasswd { get; set; } = string.Empty;
}

public class AuthenticateDevice
{
    public string PhysicalDeviceID { get; set; } = string.Empty;
    public string TerminalID { get; set; } = string.Empty;
    public string DeviceModel { get; set; } = string.Empty;
}

public class AuthenticateRequest
{
    public AuthenticateBasic? AuthenticateBasic { get; set; }
    public AuthenticateDevice? AuthenticateDevice { get; set; }
}