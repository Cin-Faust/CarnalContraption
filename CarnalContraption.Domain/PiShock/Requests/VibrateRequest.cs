namespace CarnalContraption.Domain.PiShock.Requests;

public record VibrateRequest(Duration Duration, Intensity Intensity) : Request(Operations.Vibrate);