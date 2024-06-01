using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Asp.Versioning;
using AutoMapper;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Serilog;
using Users;
using Utils;

namespace Features;

[Tags("Files Group")]
[ApiVersion(1)]
[Route("api/v{v:apiVersion}/files")]
[ApiController]
public class FileEndpoints(AppDbContext db, UserManager<User> userManager) : ControllerBase
{
    [HttpPost("user-picture")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PictureCreationDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> UploadUserImage(IFormFile image)
    {
        var picture = await FileManager.HandleImageAsync(image);
        if (picture is null) return BadRequest(new ErrorResponse("Image not valid"));

        var user = await userManager.GetUserAsync(User);

        if (user is null) return Unauthorized(new ErrorResponse("User not found, this should not happen"));

        var userPicture = new UserPicture()
        {
            Id = Guid.NewGuid(),
            Purpos = PicturePurpos.UserProfilePicture,
            PictureDataId = picture.Id,
            UserId = user.Id,
        };

        db.Pictures.Add(picture);
        db.UserPictures.Add(userPicture);
        await db.SaveChangesAsync();
        user.ProfilePicture = $"{HttpContext.Request.Host.Value}/api/v1/files/pictures/{picture.Id}";

        if (!user.ProfilePicture.StartsWith("localhost"))
            user.ProfilePicture = $"https://{user.ProfilePicture}";

        await userManager.UpdateAsync(user);

        return Ok(new PictureCreationDto(picture.Id,$"{HttpContext.Request.Host.Value}/api/v1/files/pictures/{picture.Id}"));
    }

    record PictureCreationDto(Guid PictureId, string PictureUrl);
    
    [HttpGet("pictures/{id:guid}")]
    public async Task<IActionResult> GetPictures([FromRoute] Guid id)
    {
        var picture = await db.Pictures.FirstOrDefaultAsync(p => p.Id == id);

        if (picture is null) return NotFound();
        
        var stream = new MemoryStream(picture.Bytes);

        return File(stream, picture.ContentType);
    }
}

public static class FileManager
{
    public static async Task<Picture?> HandleImageAsync(IFormFile file)
    {
        if (file.Length <= 0)
            return null;

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        
        // Upload the file if less than 4 MB  
        if (memoryStream.Length >= 2097152 * 2)
            return null;
    
        return new Picture()
            {
                Id = Guid.NewGuid(),
                Bytes = memoryStream.ToArray(),
                FileExtension = Path.GetExtension(file.FileName),
                ContentType = file.ContentType,
                Size = file.Length,
                FileName = file.FileName
            };
    }
}
public class Picture
{
    [Key]  
    public Guid Id { get; set; }  
    public byte[] Bytes { get; set; }  
    public string FileExtension { get; set; }  
    public string ContentType { get; set; }  
    public decimal Size { get; set; }

    public string FileName { get; set; }
}

public class UserPicture
{
    [Key] 
    public Guid Id { get; set; }
    public Guid PictureDataId { get; set; }
    public Guid UserId { get; set; }
    public PicturePurpos Purpos;
}

public enum PicturePurpos
{
    UserProfilePicture
}