﻿using EasyCqrs.Results;
using EasyCqrs.Sample.Repositories;

namespace EasyCqrs.Sample.Application.Queries.GetPersonByIdQuery;

public class GetPersonByIdQueryHandler : IQueryHandler<GetPersonByIdQuery, GetPersonByIdQueryItem>
{
    private readonly IPersonRepository _personRepository;

    public GetPersonByIdQueryHandler(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }
    
    public async Task<Result<GetPersonByIdQueryItem>> Handle(GetPersonByIdQuery request, CancellationToken cancellationToken)
    {
        var person = _personRepository.GetPeople().FirstOrDefault(x => x.Id == request.Id);
        
        var personResult = person is null
            ? null
            : new GetPersonByIdQueryItem(person.Id, person.Name, person.Age);

        return personResult;
    }
}