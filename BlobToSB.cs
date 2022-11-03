using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json.Linq;

namespace CCPS.Function
{
    public static class BlobToSB
    {
        [FunctionName("BlobToSB")]
        // [return: ServiceBus("vdqqueue", Connection = "ServiceBusConnectionSender")]
        public static async Task Run([BlobTrigger("sensorsdata/{name}", Connection = "ccpsattachmentsstorage")] Stream myBlob, string name, ILogger log)
        {

            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
            using (var reader = new StreamReader(myBlob))
            {

                //Read the first line of the CSV file and break into header values
                var line = reader.ReadLine();

                log.LogInformation("headers: " + line);
                var headers = line.Split(',');

                //Define the sleep interval in milliseconds, use 0 if you don't want to wait, or remove the sleep command below
                // int delayTime = 0;

                //Read the rest of the file
                while (!reader.EndOfStream)
                {
                    //Create an empty string for our JSON
                    string outputJSON = "";
                    outputJSON = outputJSON + "{\n";

                    //Read our lines one by one and split into values
                    line = reader.ReadLine();
                    log.LogInformation("Line: " + line);
                    // var values = line.Split(',');
                    string[] sensorsDataColumns = line.TrimEnd().Split(',');

                    await using (ServiceBusClient client = new ServiceBusClient(Environment.GetEnvironmentVariable("SERVICE_BUS_CONNECTION")))
                    {
                        // Create sender for the queue
                        ServiceBusSender sender = client.CreateSender(Environment.GetEnvironmentVariable("QUEUE_NAME"));

                        // Create a new message
                        var cycleData = new CycleData(sensorsDataColumns);
                        ServiceBusMessage message = new ServiceBusMessage(cycleData.Message);

                        var messageJson = JObject.Parse(cycleData.Message);
                        // message.ScheduledEnqueueTime = DateTime.Now.AddMinutes(2);
                        string messageTimestamp = messageJson["Timestamp"].ToString();
                        var enqueueTimeEst = DateTime.Parse(messageTimestamp);
                        TimeZoneInfo est = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                        var enqueueTimeUtc = TimeZoneInfo.ConvertTimeToUtc(enqueueTimeEst, est);
                        message.ScheduledEnqueueTime = enqueueTimeUtc;
                        log.LogInformation("Scheduled Enqueue Time: " + enqueueTimeUtc.ToString("yyyy-MM-dd HH:mm:ss"));

                        await sender.SendMessageAsync(message);
                        log.LogInformation("Message sent: " + cycleData.Message);
                    }

                    // System.Threading.Thread.Sleep(delayTime);
                }
            }

        }

    }
}
