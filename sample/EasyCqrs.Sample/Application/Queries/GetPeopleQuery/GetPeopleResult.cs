﻿namespace EasyCqrs.Sample.Application.Queries.GetPeopleQuery;

public class GetPeopleResult
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public int Age { get; set; }
}