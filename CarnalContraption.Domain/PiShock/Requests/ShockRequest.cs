namespace CarnalContraption.Domain.PiShock.Requests;

public record ShockRequest(Duration Duration, Intensity Intensity) : Request(Operations.Shock);