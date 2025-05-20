using Application.Common.CommandDispatcher;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ObjectMapper;
using QueryContracts.QueryDispatching;

public static class EndPointBase
{
    public static class Command
    {
        public static class WithRequest<TRequest>
        {
            public abstract class WithResponse<TResponse> : EndpointBase
            {
                public abstract Task<ActionResult<TResponse>> HandleAsync(TRequest request, [FromServices] ICommandDispatcher dispatcher);
            }
            public abstract class WithoutResponse : EndpointBase
            {
                public abstract Task<ActionResult> HandleAsync(TRequest request, [FromServices] ICommandDispatcher dispatcher);
            }
            public abstract class AndResult<TResult> : EndpointBase
                where TResult : IResult
            {
                public abstract Task<TResult> HandleAsync(TRequest request);
            }
            public abstract class AndResults<TResult1, TResult2> : EndpointBase
                where TResult1 : IResult
                where TResult2 : IResult
            {
                public abstract Task<Results<TResult1, TResult2>> HandleAsync(TRequest request, [FromServices] ICommandDispatcher dispatcher);
            }
            public abstract class AndResults<TResult1, TResult2, TResult3> : EndpointBase
                where TResult1 : IResult
                where TResult2 : IResult
                where TResult3 : IResult
            {
                public abstract Task<Results<TResult1, TResult2, TResult3>> HandleAsync();
            }
        
        }
        public static class WithoutRequest
        {
            public abstract class WithResponse<TResponse> : EndpointBase
            {
                public abstract Task<ActionResult<TResponse>> HandleAsync([FromServices] ICommandDispatcher dispatcher);
            }
            public abstract class WithoutResponse : EndpointBase
            {
                public abstract Task<ActionResult> HandleAsync([FromServices] ICommandDispatcher dispatcher);
            }
        }
    }

    public static class Query
    {
        public static class WithRequest<TRequest>
        {
            public abstract class WithResponse<TResponse> : EndpointBase
            {
                public abstract Task<ActionResult<TResponse>> HandleAsync(TRequest request, [FromServices] IQueryDispatcher dispatcher);
            }
            public abstract class WithoutResponse : EndpointBase
            {
                public abstract Task<ActionResult> HandleAsync(TRequest request, [FromServices] IQueryDispatcher dispatcher);
            }
            public abstract class AndResult<TResult> : EndpointBase
                where TResult : IResult
            {
                public abstract Task<TResult> HandleAsync(TRequest request);
            }
            public abstract class AndResults<TResult1, TResult2> : EndpointBase
                where TResult1 : IResult
                where TResult2 : IResult
            {
                public abstract Task<Results<TResult1, TResult2>> HandleAsync(TRequest request);
            }
            public abstract class AndResults<TResult1, TResult2, TResult3> : EndpointBase
                where TResult1 : IResult
                where TResult2 : IResult
                where TResult3 : IResult
            {
                public abstract Task<Results<TResult1, TResult2, TResult3>> HandleAsync();
            }
        
        }
        public static class WithoutRequest
        {
            public abstract class WithResponse<TResponse> : EndpointBase
            {
                public abstract Task<ActionResult<TResponse>> HandleAsync([FromServices] ICommandDispatcher dispatcher);
            }
            public abstract class WithoutResponse : EndpointBase
            {
                public abstract Task<ActionResult> HandleAsync([FromServices] ICommandDispatcher dispatcher);
            }
        }
    }
}
[ApiController, Route("api")]
public abstract class EndpointBase : ControllerBase;