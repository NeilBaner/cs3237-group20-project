<<<<<<< HEAD
﻿using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

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

        private const string dumbbellString = "dumbbell";
        private const string predictorUrl = "http://ec2-52-77-242-91.ap-southeast-1.compute.amazonaws.com:80/predict";
        private const string fallDetectorUrl = "http://ec2-54-169-87-226.ap-southeast-1.compute.amazonaws.com:80/predict";
        private const string emergencyUrl = "https://api.telegram.org/bot5767852164:AAGjjY1I5_mUF4k-NAjeGrjV8irYvC1nPAQ/sendMessage?chat_id=-664384504&parse_mode=html&text=EMERGENCY!! Your loved one has fallen. <b>Please</b> check on them immediately. \n";
        private const string dbTableName = "[dbo].[exercise_table]";
        private const string fallResponseString = "fall";

        private static readonly HttpClient client = new HttpClient();

        [FunctionName("telemetry_receiver")]
        public static void Run([IoTHubTrigger("boydapp",
            Connection = "ConnectionString")] EventData message, ILogger log)
        {
            string messageString = message.Body.ToString();
            string[] messageStringTokens = messageString.Split('^');
            string sender = messageStringTokens[0];
            string[] dataStrings = messageStringTokens[1].Split(' ');

            
            //TODO: make sure the output is formatted correctly

            var sqlConnectionString = Environment.GetEnvironmentVariable("sqldb_connection");

            //TODO: build the POST request

            if (sender == dumbbellString)
            {
                DumbbellData dumbbellData = new DumbbellData();
                StringContent predictionRequest = new StringContent(JsonSerializer.Serialize(dumbbellData));
                var response = client.PostAsync(predictorUrl, predictionRequest)
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
=======
﻿using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

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
using System;

namespace telemetry_receiver
{
    public class DumbbellData
    {
        public double[,] data;

        public void populate(double[] ax, double[] ay, double[] az, double[] gx, double[] gy, double[] gz, double[] go)
        {
            this.data = new double[10, 7];
            for (int i = 0; i < 10; ++i)
            {
                this.data[i, 0] = gx[i];
                this.data[i, 1] = gy[i];
                this.data[i, 2] = gz[i];
                this.data[i, 3] = ax[i];
                this.data[i, 4] = ay[i];
                this.data[i, 5] = az[i];
                this.data[i, 6] = go[i];
            }
        }
    }

    public class FallData
    {
        public double[,] data;
        public void populate(double ax, double ay, double az, double gx, double gy, double gz, double go)
        {
            data = new double[1,7];
            data[0, 0] = gx;
            data[0, 1] = gy;
            data[0, 2] = gz;
            data[0, 3] = ax;
            data[0, 4] = ay;
            data[0, 5] = az;
            data[0, 6] = go;
        }
    }


    public class telemetry_receiver
    {
        private const string dumbbellString = "dumbbell";
        private const string predictorUrl = "http://ec2-52-77-242-91.ap-southeast-1.compute.amazonaws.com:80/predict";
        private const string fallDetectorUrl = "http://ec2-54-169-87-226.ap-southeast-1.compute.amazonaws.com:80/predict";
        private const string emergencyUrl = "https://api.telegram.org/bot5767852164:AAGjjY1I5_mUF4k-NAjeGrjV8irYvC1nPAQ/sendMessage?chat_id=615729062&parse_mode=html&text=EMERGENCY!! Your loved one has fallen. <b>Please</b> check on them immediately.";
        private const string dbTableName = "[dbo].[exercise_table]";
        private const string fallResponseString = "['Fall']";

        private static readonly HttpClient client = new HttpClient();

        [FunctionName("telemetry_receiver")]
        public static void Run([IoTHubTrigger("boydapp",
            Connection = "ConnectionString")] EventData message, ILogger log)
        {

            string messageString = Encoding.UTF8.GetString(message.Body.ToArray());

            log.LogInformation($"C# IoT Hub trigger function processed a " +
                $"message: {messageString}");
            string[] messageStringTokens = messageString.Split('^');
            string sender = messageStringTokens[0];

            log.LogInformation($"Received from {sender}");
            string[] dataStrings = messageStringTokens[1].Split(' ');
            log.LogInformation($"Received {dataStrings.Length}");

            var sqlConnectionString = Environment.GetEnvironmentVariable("sqldb_connection");

            //TODO: build the POST request

            if (sender == dumbbellString)
            {
                DumbbellData dumbbellData = new DumbbellData();
                double[] ax, ay, az, gx, gy, gz, go;
                ax = new double[10];
                ay = new double[10];
                az = new double[10];
                gx = new double[10];
                gy = new double[10];
                gz = new double[10];
                go = new double[10];
                for (int i = 0; i < 70; ++i)
                {
                    if (i < 10)
                    {
                        ax[i] = Convert.ToDouble(dataStrings[i]);
                    }
                    else if (i < 20)
                    {
                        ay[i - 10] = Convert.ToDouble(dataStrings[i]);
                    }
                    else if (i < 30)
                    {
                        az[i - 20] = Convert.ToDouble(dataStrings[i]);
                    }
                    else if (i < 40)
                    {
                        gx[i - 30] = Convert.ToDouble(dataStrings[i]);
                    }
                    else if (i < 50)
                    {
                        gy[i - 40] = Convert.ToDouble(dataStrings[i]);
                    }
                    else if (i < 60)
                    {
                        gz[i - 50] = Convert.ToDouble(dataStrings[i]);
                    }
                    else
                    {
                        go[i - 60] = Convert.ToDouble(dataStrings[i]);
                    }
                }
                dumbbellData.populate(ax, ay, az, gx, gy, gz, go);
                //string jsonString = JsonSerializer.Serialize(dumbbellData);
                //jsonString = "{\"data\": [[";
                //for (int i = 0; i < 70; i += 7)
                //{
                //    for (int j = 0; j < 7; ++j)
                //    {
                //        jsonString += dataStrings[i + j];
                //        if (j < 6)
                //        {
                //            jsonString += ", ";
                //        }
                //    }
                //    if (i < 63)
                //    {
                //        jsonString += "], [";
                //    }
                //}
                //jsonString += "]] }";
                //log.LogInformation($"Sending JSON: {jsonString}");
                //StringContent predictionRequest = new StringContent(jsonString);
                var response = client.PostAsJsonAsync(predictorUrl, dumbbellData)
                    .GetAwaiter().GetResult();
                string responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                log.LogInformation($"Received response {responseString} from EC2");
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
                FallData fallData = new FallData();
                fallData.populate(
                    Convert.ToDouble(dataStrings[0]),
                    Convert.ToDouble(dataStrings[1]),
                    Convert.ToDouble(dataStrings[2]),
                    Convert.ToDouble(dataStrings[3]),
                    Convert.ToDouble(dataStrings[4]),
                    Convert.ToDouble(dataStrings[5]),
                    Convert.ToDouble(dataStrings[6])
                    );

                var response = client.PostAsJsonAsync(fallDetectorUrl, fallData)
                    .GetAwaiter().GetResult();
                string responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                log.LogInformation(responseString);
                if (responseString == fallResponseString)
                {
                    var result = client.GetStringAsync(emergencyUrl)
                .GetAwaiter().GetResult();
                    log.LogInformation(result);
                }
            }


        }
    }
}
>>>>>>> 89f0d2d (final)
