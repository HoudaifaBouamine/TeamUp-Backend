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
        string Id,
        string Email,
        string DisplayName,
        string Handler,
        float Rate,
        string? ProfilePicture
    );

    record UserReadDetailsDto
    (
        string Id,
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
        string? FirstName,
        string? LastName,
        string DisplayName,
        string Handler,
        string? FullAddress,
        string? ProfilePicture
    );
