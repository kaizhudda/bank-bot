﻿using Microsoft.WindowsAzure.MobileServices;
using BankBot.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankBot
{
    public class AzureManager
    {

        private static AzureManager instance;
        private MobileServiceClient client;
        private IMobileServiceTable<Timeline> timelineTable;

        private AzureManager()
        {
            this.client = new MobileServiceClient("http://moodtimeline1001.azurewebsites.net");
            this.timelineTable = this.client.GetTable<Timeline>();
        }

        public MobileServiceClient AzureClient
        {
            get { return client; }
        }

        public static AzureManager AzureManagerInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AzureManager();
                }

                return instance;
            }
        }

        public async Task AddTimeline(Timeline timeline)
        {
            await this.timelineTable.InsertAsync(timeline);
        }

        public async Task<List<Timeline>> GetTimelines()
        {
            return await this.timelineTable.ToListAsync();
        }

        public async Task DeleteTimeline(Timeline timeline)
        {
            await this.timelineTable.DeleteAsync(timeline);
        }

        public async Task UpdateTimeline(Timeline timeline)
        {
            await this.timelineTable.UpdateAsync(timeline);
        }
    }
}
