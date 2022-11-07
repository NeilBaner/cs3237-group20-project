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
using System;

namespace telemetry_receiver
{
    public class DumbbellData
    {
        public float[,] data;

        public void populate(float[] ax, float[] ay, float[] az, float[] gx, float[] gy, float[] gz, float[] go)
        {
            this.data = new float[10, 7];
            for (int i = 0; i < 10; ++i)
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
        private const string V = "dumbbell";
        private const string predictorUrl = "http://ec2-52-77-242-91.ap-southeast-1.compute.amazonaws.com:80/predict";
        private const string fallDetectorUrl = "http://ec2-54-169-87-226.ap-southeast-1.compute.amazonaws.com:80/predict";
        private const string emergencyUrl = "https://api.telegram.org/bot5767852164:AAGjjY1I5_mUF4k-NAjeGrjV8irYvC1nPAQ/sendMessage?chat_id=-664384504&parse_mode=html&text=EMERGENCY!! Your loved one has fallen. <b>Please</b> check on them immediately. \n";
        private const string dbTableName = "[dbo].[exercise_table]";
        private const string fallResponseString = "";

        private static HttpClient client = new HttpClient();

        [FunctionName("telemetry_receiver")]
        public static void Run([IoTHubTrigger("boydapp",
            Connection = "ConnectionString")] EventData message, ILogger log)
        {
            string messageString = message.Body.ToString();
            string[] messageStringTokens = messageString.Split('^');
            string sender = messageStringTokens[0];
            string[] dataStrings = messageStringTokens[1].Split(' ');

            //TODO: parse the output of the wemos mqtt whatever
            //TODO: make sure the output is formatted correctly

            var sqlConnectionString = Environment.GetEnvironmentVariable("sqldb_connection");

            //TODO: build the POST request

            string predictionRequest = "";

            if (sender == V)
            {
                var response = client.PostAsJsonAsync(predictorUrl, "")
                    .GetAwaiter().GetResult();
                string responseString = response.Content.ToString();
                //TODO: Make sure response is being read correctly
                string category = "idle";
                switch (responseString)
                {
                    case "1":
                        category = "bicep_curl";
                        break;
                    case "2":
                        category = "shoulder_press";
                        break;
                    case "3":
                        category = "left_shoulder_lateral_raise";
                        break;
                    case "4":
                        category = "right_shoulder_lateral_raise";
                        break;
                    case "5":
                        category = "shoulder_front_raise";
                        break;
                    case "6":
                        category = "left_hand_tricep_extension";
                        break;
                    case "7":
                        category = "right_hand_tricep_extension";
                        break;
                    case "8":
                        category = "forward_lunge";
                        break;
                    case "9":
                        category = "dumbbell_squat";
                        break;
                    default:
                        category = "idle";
                        break;
                }

                string query = $"UPDATE {dbTableName} SET [{category}] = [{category}] + 1"; 

                using (SqlConnection conn = new SqlConnection(sqlConnectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        // Execute the command and log the # rows affected.
                        var rows = cmd.ExecuteNonQueryAsync().GetAwaiter().GetResult();
                        log.LogInformation($"{rows} rows were updated");
                    }
                }
            }
            else
            {
                var response = client.PostAsJsonAsync(fallDetectorUrl, "")
                    .GetAwaiter().GetResult();
                string responseString = response.Content.ToString();
                if (responseString == fallResponseString)
                {
                    var result = client.GetStringAsync(emergencyUrl)
                .GetAwaiter().GetResult();
                    log.LogInformation(result);
                }
            }

            log.LogInformation($"C# IoT Hub trigger function processed a " +
                $"message: {Encoding.UTF8.GetString(message.Body.ToArray())}");




        }
    }
}
