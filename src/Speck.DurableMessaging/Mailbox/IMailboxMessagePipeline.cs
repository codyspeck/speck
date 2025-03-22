namespace Speck.DurableMessaging.Mailbox;

internal interface IMailboxMessagePipeline
{
    Task SendAsync(MailboxMessageContext context);
}
