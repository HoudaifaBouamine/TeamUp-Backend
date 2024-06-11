using System.Net.Http.Headers;
using System.Text;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using Serilog;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace TeamUp.Features.Notification;

public static class FireBaseNotification
{
    private const string ProjectId = "teamup-2cp";
    private const string FirebaseUrl = $"https://fcm.googleapis.com/v1/projects/{ProjectId}/messages:send";

    public static async Task<bool> SendMessageAsync(string clientAccessToken,string notificationTitle,string notificationBody,object notificationData)
    {
        var credential = GoogleCredential.FromJson(JsonSerializer.Serialize(ServiceAccount.Key))
            .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");
        
        var accessToken = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var message = new
        {
            message = new
            {
                token = clientAccessToken,
                notification = new
                {
                    title = notificationTitle,
                    body = notificationBody
                },
                data = notificationData
            }
        };

        var jsonMessage = JsonConvert.SerializeObject(message);
        var httpContent = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(FirebaseUrl, httpContent);

        var isSuccess = true;
            
        if (response.IsSuccessStatusCode)
        {
            Log.Information("Notification sent successfully.");
        }
        else
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            Log.Error($"Error sending notification: {errorMessage}");
            isSuccess = false;
        }

        return isSuccess;
    }
}
