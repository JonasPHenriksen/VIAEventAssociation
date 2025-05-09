using ObjectMapper;

namespace WebAPI.Endpoints.VeaEvents;

public class SingleEventMapping : IMappingConfig<MyInput, MyOutput>
{
    public MyOutput Map(MyInput input)
        => new($"{input.FirstName} {input.LastName}");
}

public record MyInput(string FirstName, string LastName);
public record MyOutput(string FullName);