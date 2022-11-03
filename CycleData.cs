//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace CCPS.Function
{
    /// <summary>
    /// Class to parse and model a single row of device data found in the datasets.
    /// </summary>
    public class CycleData
    {
        private readonly Dictionary<string, object> columns = new Dictionary<string, object>();

        /// <summary>
        ///  Serializes the CycleData into a message in JSON format to be sent to IoT Hub
        /// </summary>
        /// <returns>JSON serialized message to send to IoT Hub</returns>
        public string Message => JsonConvert.SerializeObject(columns);

        /// <summary>
        /// Public constructor takes an array of string representing a row of 
        /// data from a data set file
        /// </summary>
        /// <param name="cycleDataRow">An array of string representing a row of data from a dataset</param>
        public CycleData(string[] cycleDataRow)
        {
            if (cycleDataRow.Length != 7)
            {
                throw new ArgumentOutOfRangeException(nameof(cycleDataRow), $"Expected 7 columns, but got {cycleDataRow.Length}");
            }

            //if (!int.TryParse(cycleDataRow[1], out int cycle))
            //{
            //    throw new ApplicationException("The cycle time is invalid");
            //}

            // columns.Add("CycleTime", cycle);

            for (int i = 0; i < cycleDataRow.Length; i++)
            {
                if(i == 0)
                {
                    string columnValue = cycleDataRow[i];
                    columns.Add("DeviceID", columnValue);
                }
                else if(i == 1)
                {
                    float.TryParse(cycleDataRow[i], out float columnValue);
                    columns.Add("DeviceMeasure", columnValue);
                }
                else if(i == 2)
                {
                    string columnValue = cycleDataRow[i];
                    columns.Add("DeviceName", columnValue);
                }
                else if(i == 3)
                {
                    string columnValue = cycleDataRow[i];
                    columns.Add("UnitOfMeasurement", columnValue);
                }
                else if(i == 4)
                {
                    string columnValue = cycleDataRow[i];
                    columns.Add("RoomID", columnValue);
                }
                else if(i == 5)
                {
                    string columnValue = cycleDataRow[i];
                    columns.Add("Site", columnValue);
                }
                else if(i == 6)
                {
                    string columnValue = cycleDataRow[i];
                    var timestampOrg = DateTime.Parse(columnValue);
                    string timestampNew = timestampOrg.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
                    columns.Add("Timestamp", timestampNew);
                    columns.Add("OriginalTimestamp", columnValue);
                }
                else
                {
                    string columnValue = cycleDataRow[i];
                    columns.Add($"OperationalSetting{i - 1}", columnValue);
                }

            }
        }
    }
}