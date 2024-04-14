using System.Linq.Expressions;
using Asp.Versioning;
using Bogus.DataSets;
using CommandLine;
using FluentEmail.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Models;
using Project = Models.Project;

namespace Features.Projects;
partial class ProjectsController
{
    [HttpGet("{id}/AddUser/{user_id}")]
    [ProducesResponseType(StatusCodes.Status200OK,Type = typeof(ProjectDetailsReadDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddUserToProjectAsync(int id,Guid user_id,[FromQuery] bool isMentor = false)
    {
        var isAddedSuccessfuly = await _projectRepository.AddUserToProjectAsync(id, user_id,isMentor);
        if(isAddedSuccessfuly)
        {
            return Ok();
        }
        else
        {
            return BadRequest();
        }
    }
}
