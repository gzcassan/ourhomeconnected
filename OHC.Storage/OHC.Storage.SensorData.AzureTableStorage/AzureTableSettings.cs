﻿using System;
using System.Collections.Generic;
using System.Text;

namespace OHC.Storage.SensorData.AzureTableStorage
{
    public class AzureTableSettings
    {
        public string StorageAccount { get; }
        public string StorageKey { get; }
        public string TableName { get; }

        public AzureTableSettings(string storageAccount,
                                       string storageKey,
                                       string tableName)
        {
            if (string.IsNullOrEmpty(storageAccount))
                throw new ArgumentNullException("StorageAccount");

            if (string.IsNullOrEmpty(storageKey))
                throw new ArgumentNullException("StorageKey");

            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException("TableName");

            this.StorageAccount = storageAccount;
            this.StorageKey = storageKey;
            this.TableName = tableName;
        }
    }
}
