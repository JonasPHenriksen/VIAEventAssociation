namespace ObjectMapper;

public interface IMappingConfig<TInput, TOutput>
    where TOutput : class
    where TInput : class
{
    TOutput Map(TInput input);
}