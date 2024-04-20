using System.Linq.Expressions;
using Asp.Versioning;
using Bogus.DataSets;
using FluentEmail.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Models;
using Project = Models.Project;

namespace Features.Projects;
partial class ProjectsController
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK,Type = typeof(GetProjectsListResponse))]
    public async Task<IActionResult> GetProjectsAsync(
        [FromQuery] int? PageSize,
        [FromQuery] int? PageNumber,
        [FromQuery] string? SearchPattern)
    {
        var projects = await _projectRepository
            .GetListWithSearchAndPaginationAsync(PageSize, PageNumber, SearchPattern);

        return Ok(projects);
    }

        [HttpGet]
        [ApiVersion(2)]
        [ProducesResponseType(StatusCodes.Status200OK,Type = typeof(GetProjectsListResponse))]
        public async Task<IActionResult> GetProjectsV2Async(
            [FromQuery] int? PageSize,
            [FromQuery] int? PageNumber,
            [FromQuery] string? SearchPattern,
            [FromQuery] string[] TeamSizes
        )
        {

            var projects = await _projectRepository
                .GetListWithFiltersAsync(PageSize, PageNumber, SearchPattern, TeamSizes,null,null);

            return Ok(projects);
        }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK,Type = typeof(ProjectDetailsReadDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProject(int id)
    {
        var project = await _projectRepository.GetDetailsAsync(id);
        if (project == null)
        {
            return NotFound();
        }
        return Ok(project);
    }
}
