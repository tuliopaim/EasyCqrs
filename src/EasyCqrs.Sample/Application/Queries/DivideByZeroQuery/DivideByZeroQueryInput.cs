using EasyCqrs.Queries;

namespace EasyCqrs.Sample.Application.Queries.DivideByZeroQuery;

public class DivideByZeroQueryInput : IQueryInput<QueryResult<int>>
{
}
