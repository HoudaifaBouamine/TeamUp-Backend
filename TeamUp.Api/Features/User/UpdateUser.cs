using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Utils;

namespace Users;

partial class UserEndpoints
{
    
    async Task<Results<Ok<UserReadDetails4Dto>, NotFound<ErrorResponse>, BadRequest<ErrorResponse>>> 
        UpdateUser4Async(
            [FromBody] UserUpdateRequest4Dto userUpdate,
            ClaimsPrincipal currentUser,
            [FromServices] UserManager<User> userManager,
            AppDbContext db)
    {
        var user = await userManager.GetUserAsync(currentUser);
        
        if(user is null) return TypedResults.BadRequest(new ErrorResponse("User not found, this should not happen because this endpoint require authenticated user"));

        user.Update(null,null,userUpdate.Email,userUpdate.DisplayName, userUpdate.Handler, null, userUpdate.ProfilePicture);
        
        var result = await userManager.UpdateAsync(user);
        await db.SaveChangesAsync();
        
        if (result.Succeeded)
        {
            // Return the updated user details
            var userReadDetailsDto = new UserReadDetails4Dto
            (
                Id : user.Id,
                Email : user.Email!,
                DisplayName : user.DisplayName,
                Handler : user.Handler,
                ProfilePicture : user.ProfilePicture,
                Rate: user.Rate,
                Skills:user.Skills.Select(s=>s.Name).AsEnumerable(),
                Categories: user.Skills.Select(s=>s.Name).AsEnumerable()
            );

            return TypedResults.Ok(userReadDetailsDto);
        }
        else
        {
            return TypedResults.BadRequest(new ErrorResponse(result.Errors.First().Description));
        }
    }

    async Task<Results<Ok<UserReadDetailsDto>, NotFound<ErrorResponse>, BadRequest<ErrorResponse>>> 
        UpdateUserAsync(
            [FromBody] UserUpdateRequestDto userUpdate,
            ClaimsPrincipal currentUser,
            [FromServices] UserManager<User> userManager)
    {
        var user = await userManager.GetUserAsync(currentUser);
        
        if(user is null) return TypedResults.BadRequest(new ErrorResponse("User not found, this should not happen because this endpoint require authenticated user"));

        user.Update(userUpdate.FirstName, userUpdate.LastName,null, userUpdate.DisplayName, userUpdate.Handler, userUpdate.FullAddress, userUpdate.ProfilePicture);
        
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


record UserUpdateRequest4Dto
(
    [MinLength(3)] string? DisplayName,
    [EmailAddress] string? Email,
    string? Handler,
    [Url] string? ProfilePicture
);