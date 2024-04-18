using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Asp.Versioning;
using Bogus.DataSets;
using CommandLine;
using FluentEmail.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Models;
using Project = Models.Project;

namespace Features.Projects;

partial class ProjectsController
{
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
