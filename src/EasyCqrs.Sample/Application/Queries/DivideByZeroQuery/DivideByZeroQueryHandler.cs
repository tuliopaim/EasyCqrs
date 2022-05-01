using EasyCqrs.Queries;

namespace EasyCqrs.Sample.Application.Queries.DivideByZeroQuery;

public class DivideByZeroQueryHandler : IQueryHandler<DivideByZeroQueryInput, QueryResult<int>>
{
    public Task<QueryResult<int>> Handle(DivideByZeroQueryInput request, CancellationToken cancellationToken)
    {
        var divisor = 0;
        _ = 420 / divisor;

        return Task.FromResult(new QueryResult<int>());
    }
}

