
namespace InternetDatabase.EntityDTO.Deposit;

public class MessageDTO
{

    public required string Message { get; set; }

    public bool IsUser { get; set; } = false;

    public DateTime DateTime { get; set; }

    public string? Tag { get; set; }

    public InternetDatabase.EntityDB.FileInfo? File { get; set; }
}
