using FineDataFlow.Engine.Abstractions;
using System;
using System.Threading.Tasks;

[StepPlugin]
public static class StreamInbox_SuccessOutbox_StepPlugin
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

	[RowStreamInbox]
	public static async void StreamInbox(Row row)
	{
		Success(row);
		await Task.CompletedTask;
	}

	[Destroy]
	public static async void DestroyAsync()
	{
		//...
		await Task.CompletedTask;
	}
}

