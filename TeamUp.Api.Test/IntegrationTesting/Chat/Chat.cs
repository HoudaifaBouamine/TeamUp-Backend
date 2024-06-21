

using FluentAssertions;
using Microsoft.AspNetCore.SignalR.Client;
using TeamUp.Test.IntegrationTesting;

public class ChatTests
{
    [Fact]
    public async Task sendMessage_should_send_the_user_and_message_to_all_clients()
    {
        // Arrange
        var application = new ApplicationFactory();
        var client = application.CreateClient();
        var server = application.Server;

        var connection = await StartConnectionAsync(server.CreateHandler(), "chat");

        connection.Closed += async error =>
        {
            await Task.Delay(new Random().Next(0, 5) * 1000);
            await connection.StartAsync();
        };

        // Act
        string user = null;
        string message = null;
        connection.On<string, string>("OnReceiveMessage", (u, m) =>
        {
            user = u;
            message = m;
        });
        await connection.InvokeAsync("SendMessage", "super_user", "Hello World!!");

        //Assert
        user.Should().Be("super_user");
        message.Should().Be("Hello World!!");
    }


    public static async Task<HubConnection> StartConnectionAsync(HttpMessageHandler handler, string hubName)
    {
        var hubConnection = new HubConnectionBuilder()
            .WithUrl ($"ws://localhost/api/v1/{hubName}", o =>
            {
                o.HttpMessageHandlerFactory = _ => handler;
            })
            .Build();

        await hubConnection.StartAsync();

        return hubConnection;
    }
}
