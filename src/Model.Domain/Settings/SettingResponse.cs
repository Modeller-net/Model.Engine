namespace Model.Domain;

public record SettingResponse(Settings Instance, FileInfo Name, bool Success, string Message);