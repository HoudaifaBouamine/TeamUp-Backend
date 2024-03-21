using AutoMapper;
using Carter;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using TeamUp_Backend;

namespace Users;

partial class UserEndpoints
{
    async Task<Results<Ok<UserReadDto>, NotFound<ErrorResponse>, BadRequest<ErrorResponse>>> 
        GetUser(
        [FromQuery] string? Email,
        [FromQuery] Guid? Id,
        [FromServices] AppDbContext db
    )
    {
        if(Id is null && Email is null) return TypedResults.BadRequest(new ErrorResponse("You must send either the 'Id' or the 'Email' query parameter"));

        if(Id is not null && Email is not null) return TypedResults.BadRequest(new ErrorResponse("You must send ONLY ONE query parameter either the 'Id' or the 'Email', NOT BOTH"));

        User? user = null;

        if(Id is not null)
        {
            user = await db.Users.FirstOrDefaultAsync(u=>u.Id == Id.ToString());
        }
        else if(Email is not null)
        {
            user = await db.Users.FirstOrDefaultAsync(u=>u.Email == Email);
        }

        if(user is null)
        {
            return TypedResults.NotFound(new ErrorResponse("User is not found"));
        }

        var userReadResult = new UserReadDto(
            Id    : user.Id,
            Email : user.Email!,
            DisplayName : user.DisplayName,
            Handler: user.Handler,
            Rate: user.Rate,
            ProfilePicture: user.ProfilePicture!);

        return TypedResults.Ok(userReadResult);
    }
}

