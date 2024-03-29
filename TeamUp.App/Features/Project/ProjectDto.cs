﻿using Models;
using System.ComponentModel.DataAnnotations;
using Users;

namespace Features.Projects;

public class ProjectCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
}

public class ProjectReadDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; } = null;
    public required int UsersCount { get; set; }
    public List<ProjecUserShortDto> UsersSample { get; set; } = [];
}

public record ProjecUserShortDto
(
    string Id,
    string ProfilePicture
);


public class ProjectDetailsReadDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; } = null;
    public required int UsersCount { get; set; }
    public List<ProjecUserLongDto> Users { get; set; } = [];
}

public record ProjecUserLongDto
(
    string Id,
    string DisplayName,
    string Handler,
    string ProfilePicture,
    bool IsMentor
);



    
