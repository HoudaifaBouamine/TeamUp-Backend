using System.Security.Claims;
using AutoMapper;
using Carter;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using TeamUp_Backend;

namespace Users;

partial class UserEndpoints
{
    async Task<Results<Ok<UserReadDetailsDto>, NotFound<ErrorResponse>, BadRequest<ErrorResponse>>> 
        UpdateUser(
            [FromBody] UserUpdateRequestDto userUpdate,
            ClaimsPrincipal currentUser,
            [FromServices] UserManager<User> userManager)
    {
        var user = await userManager.GetUserAsync(currentUser);
        
        if(user is null) return TypedResults.BadRequest(new ErrorResponse("User not found, this should not happen because this endpoint require authenticated user"));

        user.FirstName = userUpdate.FirstName ?? user.FirstName;
        user.LastName = userUpdate.LastName ?? user.LastName;
        user.DisplayName = userUpdate.DisplayName;
        user.Handler = userUpdate.Handler;
        user.FullAddress = userUpdate.FullAddress;
        user.ProfilePicture = userUpdate.ProfilePicture!;

        var result = await userManager.UpdateAsync(user);
        
        if (result.Succeeded)
        {
            // Return the updated user details
            var userReadDetailsDto = new UserReadDetailsDto
            (
                Id : user.Id,
                Email : user.Email!,
                FirstName : user.FirstName,
                LastName : user.LastName,
                DisplayName : user.DisplayName,
                Handler : user.Handler,
                FullAddress : user.FullAddress,
                ProfilePicture : user.ProfilePicture,
                Rate: user.Rate
            );

            return TypedResults.Ok(userReadDetailsDto);
        }
        else
        {
            return TypedResults.BadRequest(new ErrorResponse(result.Errors.First().Description));
        }
    }
}

