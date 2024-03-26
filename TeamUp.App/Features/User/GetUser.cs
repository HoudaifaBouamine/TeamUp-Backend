using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Utils;

namespace Users;

partial class UserEndpoints
{
    async Task<Results<Ok<UserReadDetailsDto>, NotFound<ErrorResponse>, BadRequest<ErrorResponse>>> 
        GetUser(
        [FromRoute] Guid Id,
        [FromServices] AppDbContext db
    )
    {

        User? user = await db.Users.FirstOrDefaultAsync(u=>u.Id == Id.ToString());

        if(user is null)
        {
            return TypedResults.NotFound(new ErrorResponse("User is not found"));
        }

        var userReadResult = new UserReadDetailsDto(
            Id    : user.Id,
            Email : user.Email!,
            DisplayName : user.DisplayName,
            Handler: user.Handler,
            Rate: user.Rate,
            ProfilePicture: user.ProfilePicture!,
            FirstName: user.FirstName,
            LastName: user.LastName,
            FullAddress: user.FullAddress);

        return TypedResults.Ok(userReadResult);
    }
}

