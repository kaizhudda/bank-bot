using System;
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

                // calc something for us to return
                if (userData.GetProperty<bool>("SentGreeting"))
                {
                    endOutput = "Hello Again";
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
                if (userMessage.ToLower().Contains("convert"))
                {
                    var currencyType = userMessage.Split(' '); // e.g. AUD
                    var toRate = currencyType[1];
                    if (toRate == "AUD")
                    {
                        endOutput = currencyType[1] + ": " + AUD;
                    }
                    if (toRate == "EUR")
                    {
                        endOutput = currencyType[1] + ": " +  EUR;
                    }
                    if (toRate == "CAD")
                    {
                        endOutput = currencyType[1] + ": " + CAD;
                    }
                    if (toRate == "USD")
                    {
                        endOutput = currencyType[1] + ": " + USD;
                    }
                    if (toRate == "INR")
                    {
                        endOutput = currencyType[1] + ": " + INR;
                    }
                    if (toRate == "JPY")
                    {
                        endOutput = currencyType[1] + ": " + JPY;
                    }
                    if (toRate == "GBP")
                    {
                        endOutput = currencyType[1] + ": " + GBP;
                    }
                    if (toRate == "SGD")
                    {
                        endOutput = currencyType[1] + ": " +SGD;
                    }
                    Activity replyToConversation = activity.CreateReply("1 NZD will give you:");
                    replyToConversation.Recipient = activity.From;
                    replyToConversation.Type = "message";
                    replyToConversation.Attachments = new List<Attachment>();
                    //List<CardImage> cardImages = new List<CardImage>();
                    //cardImages.Add(new CardImage(url: "https://cdn2.iconfinder.com/data/icons/ios-7-style-metro-ui-icons/512/MetroUI_iCloud.png"));
                    HeroCard plCard = new HeroCard()
                    {
                        Title = endOutput,
                        //Subtitle = "",
                        //Images = cardImages
                    };
                    Attachment plAttachment = plCard.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment);
                    var reply = await connector.Conversations.SendToConversationAsync(replyToConversation);
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                // Card to show more information about bot
                if (userMessage.ToLower().Contains("jdi bank"))
                {
                    Activity replyToConversation = activity.CreateReply("JDI Bank information");
                    replyToConversation.Recipient = activity.From;
                    replyToConversation.Type = "message";
                    replyToConversation.Attachments = new List<Attachment>();
                    List<CardImage> cardImages = new List<CardImage>();
                    cardImages.Add(new CardImage(url: "https://cdn2.iconfinder.com/data/icons/ios-7-style-metro-ui-icons/512/MetroUI_iCloud.png"));
                    List<CardAction> cardButtons = new List<CardAction>();
                    CardAction plButton = new CardAction()
                    {
                        Value = "http://anz.co.nz",
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

                if (endOutput == "Hello, welcome to DJI Bank")
                {
                    Activity replyToConversation = activity.CreateReply("");
                    replyToConversation.Recipient = activity.From;
                    replyToConversation.Type = "message";
                    replyToConversation.Attachments = new List<Attachment>();
                    List<CardImage> cardImages = new List<CardImage>();
                    cardImages.Add(new CardImage(url: "https://cdn2.iconfinder.com/data/icons/ios-7-style-metro-ui-icons/512/MetroUI_iCloud.png"));
                    HeroCard plCard = new HeroCard()
                    {
                        Title = "Welcome to JDI Bank!",
                        Subtitle = "type 'JDI bank' for more info!",
                        Images = cardImages
                    };
                    Attachment plAttachment = plCard.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment);
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
    }
}