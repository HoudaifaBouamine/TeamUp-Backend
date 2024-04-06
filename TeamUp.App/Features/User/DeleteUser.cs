using CommandLine;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Utils;

namespace Users;

partial class UserEndpoints
{
    async Task<IResult> DeleteUserAsync
    (
        [FromServices] AppDbContext db,
        [FromRoute] Guid Id
    )
    {
        var deletedRowsCount = await db.Users.Where(u=>u.Id == Id.ToString()).ExecuteDeleteAsync();

        if(deletedRowsCount == 0)
            return Results.NotFound(new ErrorResponse($"User with is {Id} is not found"));
            
        return Results.Ok();
    }
}

