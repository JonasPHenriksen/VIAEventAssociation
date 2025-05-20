namespace UnitTests.OperationResultTest;

using Xunit;
using Microsoft.Extensions.DependencyInjection;
using ObjectMapper;

public class ObjectMapperTests
{
    private readonly IMapper _mapper;

    public ObjectMapperTests()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IMapper, ObjectMapperImpl>();
        services.AddSingleton<IMappingConfig<Source, Destination>, CustomMappingConfig>();
        _mapper = new ObjectMapperImpl(services.BuildServiceProvider());
    }

    public class ObjectMapperImpl(IServiceProvider sp) : ObjectMapper(sp);

    public class Source
    {
        public string Name { get; set; }
    }

    public class Destination
    {
        public string Name { get; set; }
    }

    public class CustomMappingConfig : IMappingConfig<Source, Destination>
    {
        public Destination Map(Source input) => new() { Name = input.Name + "_mapped" };
    }

    [Fact]
    public void Map_UsesCustomMapping()
    {
        var result = _mapper.Map<Destination>(new Source { Name = "Test" });
        Assert.Equal("Test_mapped", result.Name);
    }

    [Fact]
    public void Map_FallbackToJsonMapping()
    {
        var fallbackMapper = new ObjectMapperImpl(new ServiceCollection().BuildServiceProvider());
        var result = fallbackMapper.Map<Destination>(new Source { Name = "JsonTest" });
        Assert.Equal("JsonTest", result.Name);
    }
}
