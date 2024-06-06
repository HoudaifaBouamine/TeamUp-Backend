using System.Net.Http.Headers;
using System.Text;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;

namespace Features;

public class FireBaseNotification
{    
    public static async Task<bool> SendMessageAsync(string clientAccessToken,string notificationTitle,string notificationBody,object notificationData)
    {
        string projectId = "teamup-2cp";
        string firebaseUrl = $"https://fcm.googleapis.com/v1/projects/{projectId}/messages:send";

        // Load the service account key JSON file
        GoogleCredential credential;
        await using (var stream = new FileStream("Features/Notification/ServiceAccount.json", FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream)
                .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");
        }

        // Create an authorized HTTP client
        var accessToken = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
        using (var httpClient = new HttpClient())
        {
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

            var response = await httpClient.PostAsync(firebaseUrl, httpContent);

            var isSucess = true;
            
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Message sent successfully.");
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error sending message: {errorMessage}");
                isSucess = false;
            }

            return isSucess;
        }
    }
}
