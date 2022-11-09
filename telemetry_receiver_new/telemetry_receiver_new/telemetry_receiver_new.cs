using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.Json;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Azure.Messaging.EventHubs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace telemetry_receiver
{
    public class DumbbellData
    {
        public float[,] data;

        public void populate(float[] ax, float[] ay, float[] az, float[] gx, float[] gy, float[] gz, float[] go)
        {
            this.data = new float[10,7];
            for(int i = 0; i < 10; ++i)
            {
                this.data[i, 0] = ax[i];
                this.data[i, 1] = ay[i];
                this.data[i, 2] = az[i];
                this.data[i, 3] = gx[i];
                this.data[i, 4] = gy[i];
                this.data[i, 5] = gz[i];
                this.data[i, 6] = go[i];
            }
        }
    }


    public class telemetry_receiver
    {
        static string predictorUrl = "http://ec2-52-77-242-91.ap-southeast-1.compute.amazonaws.com:80/predict";
        static string fallDetectorUrl = "http://ec2-54-169-87-226.ap-southeast-1.compute.amazonaws.com:80/predict";
        static string emergencyUrl = "https://api.telegram.org/bot5767852164:AAGjjY1I5_mUF4k-NAjeGrjV8irYvC1nPAQ/sendMessage?chat_id=-664384504&parse_mode=html&text=EMERGENCY!! Your loved one has fallen. <b>Please</b> check on them immediately. \n";


        private static HttpClient client = new HttpClient();

        [FunctionName("telemetry_receiver")]
        public static void Run([IoTHubTrigger("boydapp",
            Connection = "ConnectionString")] EventData message, ILogger log)
        {
            string messageString = message.Body.ToString();
            string[] messageStringTokens = messageString.Split('^');
            string sender = messageStringTokens[0];
            string[] dataStrings = messageStringTokens[1].Split(' ');

            string predictionRequest = "";

            if(sender == "dumbbell")
            {
                var response = client.PostAsJsonAsync(predictorUrl, "")
                    .GetAwaiter().GetResult();
                string responseString = response.Content.ToString();
            }
            else
            {

            }

            log.LogInformation($"C# IoT Hub trigger function processed a " +
                $"message: {Encoding.UTF8.GetString(message.Body.ToArray())}");

            var result = client.GetStringAsync(emergencyUrl)
                .GetAwaiter().GetResult();

            log.LogInformation(result);
        }
    }
}
