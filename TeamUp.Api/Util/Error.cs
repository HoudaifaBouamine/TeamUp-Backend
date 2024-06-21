namespace Utils;

public record ErrorResponse(string Error);

public record ErrorResponseList(IEnumerable<string> Errors);
