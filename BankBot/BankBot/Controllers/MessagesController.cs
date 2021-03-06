﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using BankBot.Models;
using System.Collections.Generic;
using BankBot.DataModels;
using Microsoft.Bot.Builder.Dialogs;

namespace BankBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            { 
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                // setup state 
                StateClient stateClient = activity.GetStateClient();
                // grab user data
                BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);
                // Get/Set users property data
                string endOutput = "Hello, welcome to DJI Bank";
                bool isFound = true;
                bool isExchangeRate = false;
                bool isBankNumber = false;
                bool isOpeningHours = false;
                bool isExchangeRateFull = false;
                bool isBankInfo = false;
                bool isHelp = false;
                bool isRating = false;
                bool isRated = false;
                //string[] currencyList = { };
                ExchangeLUIS StLUIS = await GetEntityFromLUIS(activity.Text);
                if (StLUIS.intents.Count() > 0)
                 {
                    switch (StLUIS.intents[0].intent)
                    {
                        case "ExchangeRate":
                            endOutput = await GetExchange(StLUIS.entities[0].entity);
                            isExchangeRate = true;
                            break;
                        case "ExchangeRate2":
                            endOutput = await GetExchange(StLUIS.entities[0].entity);
                            isExchangeRate = true;
                            break;
                        case "ExchangeRateUSD":
                            endOutput = await GetExchange(StLUIS.entities[0].entity);
                            isExchangeRate = true;
                            break;
                        case "ExchangeRateUSD2":
                            endOutput = await GetExchange(StLUIS.entities[0].entity);
                            isExchangeRate = true;
                            break;
                        case "ExchangeRateEUR":
                            endOutput = await GetExchange(StLUIS.entities[0].entity);
                            isExchangeRate = true;
                            break;
                        case "ExchangeRateEUR2":
                            endOutput = await GetExchange(StLUIS.entities[0].entity);
                            isExchangeRate = true;
                            break;
                        case "ExchangeRateGBP":
                            endOutput = await GetExchange(StLUIS.entities[0].entity);
                            isExchangeRate = true;
                            break;
                        case "ExchangeRateGBP2":
                            endOutput = await GetExchange(StLUIS.entities[0].entity);
                            isExchangeRate = true;
                            break;
                        case "GetBankNumber":
                            endOutput = GetPhoneNumber();
                            isBankNumber = true; 
                            break;
                        case "Timings":
                            endOutput = GetBankHours(StLUIS.entities[0].type);
                            isOpeningHours = true;
                            break;
                        case "Timings2":
                            endOutput = GetBankHours(StLUIS.entities[0].type);
                            isOpeningHours = true;
                            break;
                        case "AllExchangeRatesNow":
                            //currenyList = GetExchangeRates(StLUIS.entities[0].type);
                            isExchangeRateFull = true;
                            break;
                        case "LearnMoreAboutJDI":
                            //currenyList = GetExchangeRates(StLUIS.entities[0].type);
                            isBankInfo = true;
                            break;
                        case "HelpCommand":
                            //endOutput = await GetExchange(StLUIS.entities[0].entity);
                            isHelp = true;
                            break;
                        case "Ratings":
                            //endOutput = "No problem! Please rate the bank out of 5.";
                            isRating = true;
                            break;
                        default:
                            isFound = false;
                            //endOutput = "Sorry, I am not getting you...";
                            break;
                    }
                    if (isFound == true && isExchangeRate == true)
                    {
                        Activity replyToConversation = activity.CreateReply("");
                        replyToConversation.Recipient = activity.From;
                        replyToConversation.Type = "message";
                        replyToConversation.Attachments = new List<Attachment>();
                        //replyToConversation.AttachmentLayout = "carousel";
                        //List<CardImage> cardImages = new List<CardImage>();
                        //cardImages.Add(new CardImage(url: "https://s14.postimg.org/hzleh2doh/15209121_1868667360019432_1788343028_n.jpg"));
                        HeroCard plCard = new HeroCard()
                        {
                            Title = endOutput,
                            Subtitle = "will give you 1.00 NZD",
                            //Images = cardImages
                        };
                        Attachment plAttachment = plCard.ToAttachment();
                        replyToConversation.Attachments.Add(plAttachment);
                        var reply = await connector.Conversations.SendToConversationAsync(replyToConversation);
                        return Request.CreateResponse(HttpStatusCode.OK);
                    } else if (isFound == true && isBankNumber == true) {
                        Activity replyToConversation = activity.CreateReply("");
                        replyToConversation.Recipient = activity.From;
                        replyToConversation.Type = "message";
                        replyToConversation.Attachments = new List<Attachment>();
                        //replyToConversation.AttachmentLayout = "carousel";
                        List<CardImage> cardImages = new List<CardImage>();
                        cardImages.Add(new CardImage(url: "https://s14.postimg.org/hzleh2doh/15209121_1868667360019432_1788343028_n.jpg"));
                        List<CardAction> cardButtons = new List<CardAction>();
                        CardAction plButton = new CardAction()
                        {
                            Value = endOutput,
                            Type = "call",
                            Title = "Call us"
                        };
                        cardButtons.Add(plButton);
                        HeroCard plCard = new HeroCard()
                        {
                            Title = "JDI Bank!",
                            Images = cardImages,
                            Buttons = cardButtons
                        };
                        Attachment plAttachment = plCard.ToAttachment();
                        replyToConversation.Attachments.Add(plAttachment);
                        var reply = await connector.Conversations.SendToConversationAsync(replyToConversation);
                        return Request.CreateResponse(HttpStatusCode.OK);
                    } else if (isFound == true && isOpeningHours == true)
                    {
                        // return repy to user 
                        Activity openHourReply = activity.CreateReply(endOutput);
                        await connector.Conversations.ReplyToActivityAsync(openHourReply);
                        return Request.CreateResponse(HttpStatusCode.OK);
                    } else if (isFound == true && isExchangeRateFull == true)
                    {
                        string AUDValue = await GetExchange("AUD");
                        string USDValue = await GetExchange("USD");
                        string EURValue = await GetExchange("EUR");
                        string GBPValue = await GetExchange("GBP");
                        Activity replyToConversation = activity.CreateReply("");
                        replyToConversation.Recipient = activity.From;
                        replyToConversation.Type = "message";
                        replyToConversation.Attachments = new List<Attachment>();
                        replyToConversation.AttachmentLayout = "carousel";
                        //List<CardImage> cardImages = new List<CardImage>();
                        //cardImages.Add(new CardImage(url: "https://cdn2.iconfinder.com/data/icons/ios-7-style-metro-ui-icons/512/MetroUI_iCloud.png"));
                        HeroCard plCard = new HeroCard()
                        {
                            Title = AUDValue,
                            Subtitle = "will give you 1.00 NZD",
                            //Images = cardImages
                        };
                        Attachment plAttachment = plCard.ToAttachment();
                        HeroCard plCard2 = new HeroCard()
                        {
                            Title = USDValue,
                            Subtitle = "will give you 1.00 NZD",
                            //Images = cardImages
                        };
                        Attachment plAttachment2 = plCard2.ToAttachment();
                        HeroCard plCard3 = new HeroCard()
                        {
                            Title = EURValue,
                            Subtitle = "will give you 1.00 NZD",
                            //Images = cardImages
                        };
                        Attachment plAttachment3 = plCard3.ToAttachment();
                        HeroCard plCard4 = new HeroCard()
                        {
                            Title = GBPValue,
                            Subtitle = "will give you 1.00 NZD",
                            //Images = cardImages
                        };
                        Attachment plAttachment4 = plCard4.ToAttachment();
                        List<CardAction> cardButtons = new List<CardAction>();
                        CardAction plButton = new CardAction()
                        {
                            Value = "https://xe.com",
                            Type = "openUrl",
                            Title = "Convert more currencies now!"
                        };
                        cardButtons.Add(plButton);
                        HeroCard plCard5 = new HeroCard()
                        {
                            //Title = "Converter",
                            //Subtitle = "will give you 1.00 NZD",
                            Buttons = cardButtons
                        };
                        Attachment plAttachment5 = plCard5.ToAttachment();
                        replyToConversation.Attachments.Add(plAttachment);
                        replyToConversation.Attachments.Add(plAttachment2);
                        replyToConversation.Attachments.Add(plAttachment3);
                        replyToConversation.Attachments.Add(plAttachment4);
                        replyToConversation.Attachments.Add(plAttachment5);
                        var reply = await connector.Conversations.SendToConversationAsync(replyToConversation);
                        return Request.CreateResponse(HttpStatusCode.OK);
                    } else if (isFound == true && isBankInfo == true)
                    {
                        Activity replyToConversation = activity.CreateReply("JDI Bank information");
                        replyToConversation.Recipient = activity.From;
                        replyToConversation.Type = "message";
                        replyToConversation.Attachments = new List<Attachment>();
                        List<CardImage> cardImages = new List<CardImage>();
                        cardImages.Add(new CardImage(url: "https://s14.postimg.org/hzleh2doh/15209121_1868667360019432_1788343028_n.jpg"));
                        List<CardAction> cardButtons = new List<CardAction>();
                        CardAction plButton = new CardAction()
                        {
                            Value = "https://anz.co.nz",
                            Type = "openUrl",
                            Title = "JDI Bank Website"
                        };
                        cardButtons.Add(plButton);
                        ThumbnailCard plCard = new ThumbnailCard()
                        {
                            Title = "Visit JDI Bank",
                            Subtitle = "online banking",
                            Images = cardImages,
                            Buttons = cardButtons
                        };
                        Attachment plAttachment = plCard.ToAttachment();
                        replyToConversation.Attachments.Add(plAttachment);
                        await connector.Conversations.SendToConversationAsync(replyToConversation);

                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else if (isFound == true && isHelp == true)
                    {
                        Activity replyToConversation = activity.CreateReply("");
                        replyToConversation.Recipient = activity.From;
                        replyToConversation.Type = "message";
                        replyToConversation.Attachments = new List<Attachment>();
                        replyToConversation.AttachmentLayout = "carousel";
                        List<CardImage> cardImages = new List<CardImage>();
                        cardImages.Add(new CardImage(url: "https://s11.postimg.org/5y1rtv75v/Help_Exchange_Rates.png"));
                        List<CardImage> cardImages1 = new List<CardImage>();
                        cardImages1.Add(new CardImage(url: "https://s15.postimg.org/r29lqgpsb/Help_Opening_About_Number.png"));
                        List<CardImage> cardImages2 = new List<CardImage>();
                        cardImages2.Add(new CardImage(url: "https://s17.postimg.org/5epvmzvrz/Help_Ratings.png"));
                        //cardImages.Add(new CardImage(url: "https://s11.postimg.org/5y1rtv75v/Help_Exchange_Rates.png"));
                        HeroCard plCard = new HeroCard()
                        {
                            //Title = "Help Commands",
                            //Subtitle = "will give you 1.00 NZD",
                            Images = cardImages
                        };
                        Attachment plAttachment = plCard.ToAttachment();
                        HeroCard plCard1 = new HeroCard()
                        {
                            //Title = "Help Commands",
                            //Subtitle = "will give you 1.00 NZD",
                            Images = cardImages1
                        };
                        Attachment plAttachment1 = plCard1.ToAttachment();
                        HeroCard plCard2 = new HeroCard()
                        {
                            //Title = "Help Commands",
                            //Subtitle = "will give you 1.00 NZD",
                            Images = cardImages2
                        };
                        Attachment plAttachment2 = plCard2.ToAttachment();
                        replyToConversation.Attachments.Add(plAttachment);
                        replyToConversation.Attachments.Add(plAttachment1);
                        replyToConversation.Attachments.Add(plAttachment2);
                        var reply = await connector.Conversations.SendToConversationAsync(replyToConversation);
                        return Request.CreateResponse(HttpStatusCode.OK);
                    } else if (isFound == true && isRating == true) {
                        Activity replyToConversation = activity.CreateReply("No problem!");
                        replyToConversation.Recipient = activity.From;
                        replyToConversation.Type = "message";
                        replyToConversation.Attachments = new List<Attachment>();
                        List<CardImage> cardImages = new List<CardImage>();
                        cardImages.Add(new CardImage(url: "https://s14.postimg.org/hzleh2doh/15209121_1868667360019432_1788343028_n.jpg"));
                        List<CardAction> cardButtons = new List<CardAction>();
                        CardAction plButton4 = new CardAction()
                        {
                            Value = "0",
                            Type = "imBack",
                            Title = "0"
                        };
                        cardButtons.Add(plButton4);
                        CardAction plButton = new CardAction()
                        {
                            Value = "1",
                            Type = "imBack",
                            Title = "1"
                        };
                        cardButtons.Add(plButton);
                        CardAction plButton0 = new CardAction()
                        {
                            Value = "2",
                            Type = "imBack",
                            Title = "2"
                        };
                        cardButtons.Add(plButton0);
                        CardAction plButton1 = new CardAction()
                        {
                            Value = "3",
                            Type = "imBack",
                            Title = "3"
                        };
                        cardButtons.Add(plButton1);
                        CardAction plButton2 = new CardAction()
                        {
                            Value = "4",
                            Type = "imBack",
                            Title = "4"
                        };
                        cardButtons.Add(plButton2);
                        CardAction plButton3 = new CardAction()
                        {
                            Value = "5",
                            Type = "imBack",
                            Title = "5"
                        };
                        cardButtons.Add(plButton3);
                        HeroCard plCard = new HeroCard()
                        {
                            Title = "Rate the bank!",
                            Subtitle = "1 being bad, 5 being amazing",
                            Images = cardImages,
                            Buttons = cardButtons
                        };
                        Attachment plAttachment = plCard.ToAttachment();
                        replyToConversation.Attachments.Add(plAttachment);
                        await connector.Conversations.SendToConversationAsync(replyToConversation);

                        return Request.CreateResponse(HttpStatusCode.OK);

                    } else if (isFound == true && isRated == true)
                    {
                        Activity infoReply1 = activity.CreateReply(endOutput);
                        await connector.Conversations.ReplyToActivityAsync(infoReply1);
                        return Request.CreateResponse(HttpStatusCode.OK);

                    }
                }
                else
                {
                    endOutput = "Sorry, I am not getting you...";
                }

               

                // calc something for us to return
                if (userData.GetProperty<bool>("SentGreeting"))
                {
                    endOutput = "Hello " + activity.From.Name + ", thanks for visiting us again! Type 'help' for list of commands you can ask me!";
                } else
                {
                    userData.SetProperty<bool>("SentGreeting", true);
                    await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                }

                var userMessage = activity.Text;
                bool isBankRequest = true; 


                if (userMessage.ToLower().Contains("clear"))
                {
                    endOutput = "User data cleared";
                    await stateClient.BotState.DeleteStateForUserAsync(activity.ChannelId, activity.From.Id);
                    isBankRequest = false; 
                }
                // Show exchange rates with base currency of NZD
                if (userMessage.ToLower().Contains("convertssss"))
                {
                }
                // Card to show more information about bot
                if (userMessage.ToLower().Contains("jdi bank"))
                {
                    Activity replyToConversation = activity.CreateReply("JDI Bank information");
                    replyToConversation.Recipient = activity.From;
                    replyToConversation.Type = "message";
                    replyToConversation.Attachments = new List<Attachment>();
                    List<CardImage> cardImages = new List<CardImage>();
                    cardImages.Add(new CardImage(url: "https://s14.postimg.org/hzleh2doh/15209121_1868667360019432_1788343028_n.jpg"));
                    List<CardAction> cardButtons = new List<CardAction>();
                    CardAction plButton = new CardAction()
                    {
                        Value = "https://anz.co.nz",
                        Type = "openUrl",
                        Title = "JDI Bank Website"
                    };
                    cardButtons.Add(plButton);
                    ThumbnailCard plCard = new ThumbnailCard()
                    {
                        Title = "Visit JDI Bank",
                        Subtitle = "online banking",
                        Images = cardImages,
                        Buttons = cardButtons
                    };
                    Attachment plAttachment = plCard.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment);
                    await connector.Conversations.SendToConversationAsync(replyToConversation);

                    return Request.CreateResponse(HttpStatusCode.OK);

                }

                if (userMessage.ToLower().Equals("get ratings"))
                {
                    List<Timeline> timelines = await AzureManager.AzureManagerInstance.GetTimelines();
                    endOutput = "";
                    int numOfRatings = 0;
                    double sumOfRatings = 0; 
                    foreach (Timeline t in timelines)
                    {
                        numOfRatings++;
                        sumOfRatings += t.Rating; 
                        endOutput += "[" + t.Date + "] " + t.Name+ ", Rating " + t.Rating + "\n\n";
                    }
                    double sum = sumOfRatings / numOfRatings;
                    isBankRequest = false;
                    endOutput += "Avg Rating: " + Math.Round(sum, 2);

                }

                if (userMessage.ToLower().Equals("0") || userMessage.ToLower().Equals("1") || userMessage.ToLower().Equals("2") || userMessage.ToLower().Equals("3") || userMessage.ToLower().Equals("4") || userMessage.ToLower().Equals("5"))
                {
                    Timeline timeline = new Timeline()
                    {
                        Name = activity.From.Name,
                        Rating = Int32.Parse(userMessage),
                        Date = DateTime.Now
                    };

                    await AzureManager.AzureManagerInstance.AddTimeline(timeline);

                    isBankRequest = false;

                    endOutput = "Your Rating has been added! Thanks " + timeline.Name;
                }

                if (userMessage.ToLower().Contains("update ratings to"))
                {
                    var newRating = userMessage.Split(' ');
                    List<Timeline> timelines = await AzureManager.AzureManagerInstance.GetTimelines();
                    string userName = activity.From.Name;
                    bool isUpdated = false; 
                    foreach (Timeline t in timelines)
                    {
                        
                        if (t.Name.Equals(userName)) 
                        {
                            t.Rating = Int32.Parse(newRating[3]);
                            await AzureManager.AzureManagerInstance.UpdateTimeline(t);
                            endOutput = "Your NEW Rating has been added! Thanks " + activity.From.Name;
                            isUpdated = true; 
                            break;
                        }
                    }

                    
                    if (isUpdated == false)
                    {
                        isBankRequest = false;
                        endOutput = "Sorry we are unable to update your ratings! Make sure you have correctly typed a rating!";
                    }
                    
                    
                }

                if (userMessage.ToLower().Contains("delete my rating"))
                {
                    var newRating = userMessage.Split(' ');
                    List<Timeline> timelines = await AzureManager.AzureManagerInstance.GetTimelines();
                    string userName = activity.From.Name;
                    bool isUpdated = false;
                    foreach (Timeline t in timelines)
                    {

                        if (t.Name.Equals(userName))
                        {
                            await AzureManager.AzureManagerInstance.DeleteTimeline(t);
                            endOutput = "Your Rating has been deleted! Thanks " + activity.From.Name;
                            isUpdated = true;
                            break;
                        }
                    }


                    if (isUpdated == false)
                    {
                        isBankRequest = false;
                        endOutput = "Sorry we are unable to delete your ratings! Make sure you have correctly typed the command!";
                    }


                }

                if (endOutput == "Hello, welcome to DJI Bank")
                {
                    Activity replyToConversation = activity.CreateReply("");
                    replyToConversation.Recipient = activity.From;
                    replyToConversation.Type = "message";
                    replyToConversation.Attachments = new List<Attachment>();
                    replyToConversation.AttachmentLayout = "carousel";
                    List<CardImage> cardImages = new List<CardImage>();
                    cardImages.Add(new CardImage(url: "https://s14.postimg.org/hzleh2doh/15209121_1868667360019432_1788343028_n.jpg"));
                    List<CardAction> cardButtons = new List<CardAction>();
                    CardAction plButton = new CardAction()
                    {
                        Value = "tel:123456789",
                        Type = "call",
                        Title = "Call us"
                    };
                    cardButtons.Add(plButton);
                    HeroCard plCard = new HeroCard()
                    {
                        Title = "Welcome to JDI Bank!",
                        Subtitle = "type 'JDI bank' for more info!",
                        Images = cardImages,
                        Buttons = cardButtons
                    };
                    Attachment plAttachment = plCard.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment);
                    HeroCard plCard2 = new HeroCard()
                    {
                        Title = "To see a list of commands",
                        Subtitle = "type 'help'",
                        Images = cardImages
                    };
                    Attachment plAttachment2 = plCard2.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment2);
                    var reply = await connector.Conversations.SendToConversationAsync(replyToConversation);
                    return Request.CreateResponse(HttpStatusCode.OK);
                }

                // return repy to user 
                Activity infoReply = activity.CreateReply(endOutput);
                await connector.Conversations.ReplyToActivityAsync(infoReply);

                // check if they typed account balance
                
                if (userMessage == "account balance")
                {
                    Activity reply1 = activity.CreateReply($"Your account balance is: $1,300,000.00");
                    await connector.Conversations.ReplyToActivityAsync(reply1);
                }

            }
            else
            {

            }
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
        // LUIS Integration
        private static async Task<ExchangeLUIS> GetEntityFromLUIS(string Query)
        {
            Query = Uri.EscapeDataString(Query);
            ExchangeLUIS Data = new ExchangeLUIS();
            using (HttpClient client = new HttpClient())
            {
                string RequestURI = "https://api.projectoxford.ai/luis/v2.0/apps/22fc874e-00f2-4c74-b9d8-5edab286bb0a?subscription-key=e394d52977564fa3b44142e95719fcb6&q=" + Query + "&verbose=true";
                HttpResponseMessage msg = await client.GetAsync(RequestURI);

                if (msg.IsSuccessStatusCode)
                {
                    var JsonDataResponse = await msg.Content.ReadAsStringAsync();
                    Data = JsonConvert.DeserializeObject<ExchangeLUIS>(JsonDataResponse);
                }
            }
            return Data;
        }

        private async Task<string> GetExchange(string ExchangeSymbol)
        {
            // API call
            Currency.RootObject rootObject;
            HttpClient client = new HttpClient();
            string x = await client.GetStringAsync(new Uri("http://api.fixer.io/latest?base=NZD"));
            rootObject = JsonConvert.DeserializeObject<Currency.RootObject>(x);
            double AUD = rootObject.rates.AUD;
            double EUR = rootObject.rates.EUR;
            double CAD = rootObject.rates.CAD;
            double USD = rootObject.rates.USD;
            double INR = rootObject.rates.INR;
            double JPY = rootObject.rates.JPY;
            double GBP = rootObject.rates.GBP;
            double SGD = rootObject.rates.SGD;

            //var currencyType = userMessage.Split(' '); // e.g. AUD
            var toRate = ExchangeSymbol.ToUpper();
            var endOutput = "";
            if (toRate == "AUD")
            {
                endOutput = toRate + ": " + AUD;
            }
            if (toRate == "EUR")
            {
                endOutput = toRate + ": " + EUR;
            }
            if (toRate == "CAD")
            {
                endOutput = toRate + ": " + CAD;
            }
            if (toRate == "USD")
            {
                endOutput = toRate + ": " + USD;
            }
            if (toRate == "INR")
            {
                endOutput = toRate + ": " + INR;
            }
            if (toRate == "JPY")
            {
                endOutput = toRate + ": " + JPY;
            }
            if (toRate == "GBP" || toRate == "POUNDS")
            {
                endOutput = "GBP" + ": " + GBP;
            }
            if (toRate == "SGD")
            {
                endOutput = toRate + ": " + SGD;
            }
            return endOutput;
        }

        // Get Banks Phone Number
        private string GetPhoneNumber()
        {
            var PhoneNumber = 123456789;
            return "tel:" + PhoneNumber;
        }

        // Get Banks Hours
        private string GetBankHours(string entityType)
        {
            var opening = "9:00AM - 5:00PM (Monday to Sunday)";
            if (entityType == "OpenHours")
            {
                return opening;
            }
            return "incorrect";

        }

    }
}