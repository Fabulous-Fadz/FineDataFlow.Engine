using FineDataFlow.Engine.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[StepPlugin]
public static class AllRowsInbox_SuccessOutbox_StepPlugin
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

	[AllRowsInbox]
	public static async Task AllRowsInboxAsync(IEnumerable<Row> rows)
	{
		var row = new Row();
		row["Count"] = rows.Count();
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

