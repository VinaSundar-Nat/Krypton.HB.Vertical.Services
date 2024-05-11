using System;
using Microsoft.AspNetCore.Mvc;

namespace KR.Infrastructure.Http.Models;

public class PagingModel
{
    [FromQuery(Name ="index")]
    public string Index { get; set; }

    public int GetIndex => Index.Page(1);

    [FromQuery(Name = "limit")]
    public string Limit { get; set; }

    public int GetLimit => Index.Page(10);
}

