public static class EndpointesConfuguration
{
    public static void MapEndpoints(this IEndpointRouteBuilder app)
    {
        
        app.MapGet("/test",()=>
        {
            return "test V1";
        })
        .MapToApiVersion(1);
        
        app.MapGet("/test",()=>
        {
            return "test V2";
        })
        .MapToApiVersion(2);

    }

}