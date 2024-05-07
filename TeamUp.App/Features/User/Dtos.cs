using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Storage;

namespace Users;

record GetUsersListResponse
(
    int TotalCount,
    int PageNumber,
    int PageSize,
    bool IsPrevPageExist,
    bool IsNextPageExist,
    IEnumerable<UserReadDto> Users
);

record UserReadDto
(
    Guid Id,
    string Email,
    string DisplayName,
    string Handler,
    float Rate,
    string? ProfilePicture
);

record UserReadDetailsDto
(
    Guid Id,
    string Email,
    string? FirstName,
    string? LastName,
    string DisplayName,
    string Handler,
    string? FullAddress,
    float Rate,
    string? ProfilePicture
);

record UserUpdateRequestDto
(
    [Required,MinLength(2)] string FirstName,
    [Required,MinLength(2)] string LastName,
    [Required,MinLength(5)] string DisplayName,
    [Required] string Handler,
    [Required] string FullAddress,
    [Required,Url] string ProfilePicture
);
