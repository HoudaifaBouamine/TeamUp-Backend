
using System.Security.Claims;
using Authentication.UserManager;
using Features.Projects;
using Features.Projects.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Moq;
using Repositories;
using Utils;
namespace TeamUp.Test.ProjectFeature;

public class ProjectControllerTests
{

    [Fact]
    public async void TestGetAllProjects()
    {
        // Arrange
        
        var rep = new Mock<IProjectRepository>();

        rep.Setup(rep=>rep.GetListWithFiltersAsync(
            It.IsAny<int?>(),
            It.IsAny<int?>(),
            It.IsAny<string>(),null,null,null))
                .ReturnsAsync(new GetProjectsListResponse(
                    0,
                    1,
                    0,
                    false,
                    false,
                    Array.Empty<ProjectReadDto>()));





        var cont = new ProjectsController(rep.Object);
        
        // Act

        var response = await cont.GetProjectsAsync(null,null,null);
        
        // Assert

        var okresponse  = Assert.IsType<OkObjectResult>(response);
        System.Console.WriteLine("ok response " + okresponse?.Value);
        
        var result = Assert.IsType<GetProjectsListResponse>(okresponse.Value);
        Assert.Empty(result.Projects);
    }

    [Fact]
    public async void CreateProject_WithValidInput_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var rep = new Mock<IProjectRepository>();

        rep.Setup(r=>r.CreateAsync(
            It.IsAny<ProjectCreateDto>(),
            It.IsAny<User>()))
                .ReturnsAsync(1);;

        var store = new Mock<IUserStore<User>>();
        // var userManagerMock = new UserManagerMock(store.Object);
        var userManagerMock = new Mock<CustomUserManager>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        userManagerMock.Setup(um=>um.GetUserAsync(
            It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new User
                {
                    Id = "guid",
                    ProfilePicture= "url",
                    EmailConfirmed = true
                });

        var projectDto = new ProjectCreateDto
        (
            "name",
            "description",
            default
        );

        var cont = new ProjectsController(rep.Object);

        // Act

        var response = await cont.CreateProjectAsync(projectDto,userManagerMock.Object);

        // Assert

        var createdResponse = Assert.IsType<CreatedAtActionResult>(response);
        var result = Assert.IsType<ProjectReadDto>(createdResponse.Value);
        Assert.Equal("name",result.Name);
    }

    [Fact]
    public async void CreateProject_WithValidInputButEmailNotVerifed_ShouldReturnsBadRequest()
    {
        // Arrange
        var rep = new Mock<IProjectRepository>();

        rep.Setup(r=>r.CreateAsync(
            It.IsAny<ProjectCreateDto>(),
            It.IsAny<User>()))
                .ReturnsAsync(1);;

        var store = new Mock<IUserStore<User>>();
        // var userManagerMock = new UserManagerMock(store.Object);
        var userManagerMock = new Mock<CustomUserManager>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        userManagerMock.Setup(um=>um.GetUserAsync(
            It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new User
                {
                    Id = "guid",
                    ProfilePicture= "url",
                    EmailConfirmed = false
                });

        var projectDto = new ProjectCreateDto
        (
            "name",
            "description",
            default
        );

        var cont = new ProjectsController(rep.Object);

        // Act

        var response = await cont.CreateProjectAsync(projectDto,userManagerMock.Object);

        // Assert

        var createdResponse = Assert.IsType<BadRequestObjectResult>(response);
        var result = Assert.IsType<ErrorResponse>(createdResponse.Value);
        Assert.Equal("User email is not confirmed",result.Error);
    }

    [Fact]
    public async void CreateProject_WithValidInputButUserNotFound_ShouldReturnsBadRequest()
    {
        // Arrange
        var rep = new Mock<IProjectRepository>();

        rep.Setup(r=>r.CreateAsync(
            It.IsAny<ProjectCreateDto>(),
            It.IsAny<User>()))
                .ReturnsAsync(1);;

        var store = new Mock<IUserStore<User>>();
        // var userManagerMock = new UserManagerMock(store.Object);
        var userManagerMock = new Mock<CustomUserManager>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        userManagerMock.Setup(um=>um.GetUserAsync(
            It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync((User?)null);

        var projectDto = new ProjectCreateDto
        (
            "name",
            "description",
            default
        );

        var cont = new ProjectsController(rep.Object);

        // Act

        var response = await cont.CreateProjectAsync(projectDto,userManagerMock.Object);

        // Assert

        var createdResponse = Assert.IsType<BadRequestObjectResult>(response);
        var result = Assert.IsType<ErrorResponse>(createdResponse.Value);
        Assert.Equal("User account does not exist any more",result.Error);
    }
}
