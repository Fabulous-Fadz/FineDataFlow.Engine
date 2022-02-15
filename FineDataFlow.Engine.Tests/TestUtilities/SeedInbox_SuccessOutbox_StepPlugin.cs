using FineDataFlow.Engine.Abstractions;
using System;
using System.Threading.Tasks;

[StepPlugin]
public static class SeedInbox_SuccessOutbox_StepPlugin
{
	[SuccessOutbox]
	public static Action<Row> Success;

	[Parameter]
	public static string Parameter1;

	[Initialize]
	public static async Task InitializeAsync()
	{
		//...
		await Task.CompletedTask;
	}

	[SeedInbox]
	public static void SeedInboxAsync()
	{
		Success(new Row());
		//await Task.CompletedTask;
	}

	[Destroy]
	public static async Task DestroyAsync()
	{
		//...
		await Task.CompletedTask;
	}
}

