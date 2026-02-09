namespace Backend.Services;

public class UuidGeneratorService
{
    public string NewUuid()
    {
        Guid newGuid = Guid.NewGuid();
        return newGuid.ToString();
    }
}
