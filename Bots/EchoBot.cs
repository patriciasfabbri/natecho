// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.BotBuilderSamples.OmniChannel;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class EchoBot : ActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
           if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                // Replace with your own message
                //System.Diagnostics.Debug.WriteLine("message");
                IActivity replyActivity = MessageFactory.Text($"Message");

                // Replace with your own condition for bot escalation
                if (turnContext.Activity.Text.Equals("escalate", StringComparison.InvariantCultureIgnoreCase))

                {
                    replyActivity = MessageFactory.Text($"TROCANDO PARA UM AGENTE");
                    //System.Diagnostics.Debug.WriteLine("escalate");
                    Dictionary<string, object> contextVars = new Dictionary<string, object>() { { "BotHandoffTopic", "troca" } };
                    OmnichannelBotClient.AddEscalationContext(replyActivity, contextVars);
                }
                // Replace with your own condition for bot end conversation
                else if (turnContext.Activity.Text.Equals("endconversation", StringComparison.InvariantCultureIgnoreCase))
                {
                    OmnichannelBotClient.AddEndConversationContext(replyActivity);
                }
                // Call method BridgeBotMessage for every response that needs to be delivered to the customer.
                else
                {
                    OmnichannelBotClient.BridgeBotMessage(replyActivity);
                }

                await turnContext.SendActivityAsync(replyActivity, cancellationToken);
            }
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    //Set the bridge mode for every message that needs to be delivered to customer
                    OmnichannelBotClient.BridgeBotMessage(turnContext.Activity);
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Welcome to Echo Bot."), cancellationToken);
                }
            }
        }
    }
}
