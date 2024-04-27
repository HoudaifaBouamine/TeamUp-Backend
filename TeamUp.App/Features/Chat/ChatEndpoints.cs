using System.ComponentModel.DataAnnotations;
using EmailServices;
using Carter;

namespace Chatting;

public partial class ChatEndpoints : ICarterModule 
{

    public void AddRoutes(IEndpointRouteBuilder app)
    {

        var chatGroup = app.MapGroup("chat").WithTags("Chat Group");
        
    }

}