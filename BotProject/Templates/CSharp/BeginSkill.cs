using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Integration.AspNet.Core.Skills;
using Microsoft.Bot.Builder.Skills;
using Newtonsoft.Json;

namespace Microsoft.Bot.Builder.ComposerBot.Json
{
    public class BeginSkill : Dialog
    {
        [JsonProperty("$kind")]
        public const string DeclarativeType = "Microsoft.BeginSkill";

        private ConversationState _conversationState;

        private SkillHttpClient _skillClient;

        private BotFrameworkSkill activeSkill;

        private string appId;

        public BeginSkill([CallerFilePath] string callerPath = "", [CallerLineNumber] int callerLine = 0)
            : base()
        {
            this.RegisterSourceLocation(callerPath, callerLine);
        }

        [JsonProperty("targetSkill")]
        public BotFrameworkSkill TargetSkill { get; set; }

        [JsonProperty("skillHostEndpoint")]
        public string SkillHostEndpoint { get; set; }

        public void SetHttpClient(SkillHttpClient skillHttpClient)
        {
            this._skillClient = skillHttpClient;
        }

        public void SetConversationState(ConversationState conversationState)
        {
            this._conversationState = conversationState;
        }

        public void SetAppId(string appId)
        {
            this.appId = appId;
        }

        public override async Task<DialogTurnResult> BeginDialogAsync(DialogContext dc, object options = null, CancellationToken cancellationToken = default)
        {
            // Try to get the active skill
            this.activeSkill = TargetSkill;

            if (this.activeSkill != null)
            {
                // Send the activity to the skill
                await SendToSkill(dc, activeSkill, cancellationToken);
            }

            await _conversationState.SaveChangesAsync(dc.Context, force: true, cancellationToken: cancellationToken);
            return await dc.EndDialogAsync();
        }

        private async Task SendToSkill(DialogContext dc, BotFrameworkSkill targetSkill, CancellationToken cancellationToken)
        {
            await _conversationState.SaveChangesAsync(dc.Context, force: true, cancellationToken: cancellationToken);

            var response = await _skillClient.PostActivityAsync(this.appId, targetSkill, new Uri(SkillHostEndpoint), dc.Context.Activity, cancellationToken);

            if (!(response.Status >= 200 && response.Status <= 299))
            {
                throw new HttpRequestException($"Error invoking the skill id: \"{targetSkill.Id}\" at \"{targetSkill.SkillEndpoint}\" (status is {response.Status}). \r\n {response.Body}");
            }
        }
    }
}
