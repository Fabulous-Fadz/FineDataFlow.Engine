using FineDataFlow.Engine.Abstractions;
using System;
using System.Net.Mail;

[StepPlugin]
public class SendEmailStep
{
	private SmtpClient _smtpClient;
	
	[Parameter] public string SmtpHost;
	[Parameter] public string SmtpFromEmailAddress;

	[Parameter] public string ToEmailAddressField;
	[Parameter] public string EmailContentField;

	[SuccessOutbox] public Action<Row> Success;

	[Initialize]
	public void Initialize()
	{
		_smtpClient = new(SmtpHost);
	}

	[RowStreamInbox]
	public void OnRow(Row row)
	{
		var message = new MailMessage();
		message.From = new(SmtpFromEmailAddress);
		message.To.Add((string)row[ToEmailAddressField]);
		message.Body = (string)row[EmailContentField];
		_smtpClient.Send(message);

		Success(row);
	}

	[Destroy]
	public void Destroy()
	{
		_smtpClient.Dispose();
	}
}

