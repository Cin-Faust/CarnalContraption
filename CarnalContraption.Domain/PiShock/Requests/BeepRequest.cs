namespace CarnalContraption.Domain.PiShock.Requests;

public record BeepRequest(Duration Duration) : Request(Operations.Beep);