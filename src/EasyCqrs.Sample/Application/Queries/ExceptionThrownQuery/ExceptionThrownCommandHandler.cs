using EasyCqrs.Queries;

namespace EasyCqrs.Sample.Application.Queries.ExceptionThrownQuery;

public class ExceptionThrownQueryHandler : IQueryHandler<ExceptionThrownQueryInput, QueryResult<int>>
{
    public Task<QueryResult<int>> Handle(ExceptionThrownQueryInput request, CancellationToken cancellationToken)
    {
        var divisor = 0;
        _ = 420 / divisor;

        return Task.FromResult(new QueryResult<int>());
    }
}

