using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using SQLite;

namespace Integrated_storage {
	[Activity(Label = "Integrated_storage", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity {
		int count = 1;

		protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);

			var docsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
			var pathToDatabase = System.IO.Path.Combine(docsFolder, "IS.db");

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button buttonCreate = FindViewById<Button>(Resource.Id.CreateButton);
			Button buttonAdd = FindViewById<Button>(Resource.Id.AddButton);
			Button buttonList = FindViewById<Button>(Resource.Id.ListButton);
			TextView textViewResults = FindViewById<TextView>(Resource.Id.textViewResults);

			buttonCreate.Click += async delegate {
				var result = await createDatabase(pathToDatabase);
				Toast.MakeText(Application.Context, "DB Created", ToastLength.Short).Show();

			};
			buttonAdd.Click += async delegate {
				var result = await insertUpdateData(new Person(1, "brock", "smedley"), pathToDatabase);
				Toast.MakeText(Application.Context, "DB updated", ToastLength.Short).Show();
			};
			buttonList.Click += async delegate {
				//var peopleList = new List<Person>();
				var result = await findNumberRecords(pathToDatabase);
				Toast.MakeText(Application.Context, result.ToString(), ToastLength.Short).Show();

				//type: Task<List<Person>>
				var records = getRecords(pathToDatabase);

				List<Person> people = new List<Person>();
				
				await records.ContinueWith(t => {
					foreach (var person in t.Result)
						people.Add(person);
				});

				textViewResults.Text = "";

				foreach (var p in people) {
					textViewResults.Text += p.FirstName + " " + p.LastName + " (" + p.ID.ToString() + ")" + "\n";
				}
				
			};
		}

		private async Task<string> createDatabase(string path)
		{
			try
			{
				var connection = new SQLiteAsyncConnection(path);
					 await connection.CreateTableAsync<Person>();
					 return "Database created";
			}
			catch (SQLiteException ex)
			{
				return ex.Message;
			}
		}

		private async Task<string> insertUpdateData(Person data, string path) {
			try {
				var db = new SQLiteAsyncConnection(path);
				if (await db.InsertAsync(data) != 0)
					await db.UpdateAsync(data);
				return "Single data file inserted or updated";
			}
			catch (SQLiteException ex) {
				return ex.Message;
			}
		}

		private async Task<int> findNumberRecords(string path) {
			try {
				var db = new SQLiteAsyncConnection(path);
				// this counts all records in the database, it can be slow depending on the size of the database
				var count = await db.ExecuteScalarAsync<int>("SELECT Count(*) FROM Person");

				// for a non-parameterless query
				// var count = db.ExecuteScalarAsync<int>("SELECT Count(*) FROM Person WHERE FirstName="Amy");

				return count;
			}
			catch (SQLiteException ex) {
				return -1;
			}
		}

		private async Task<List<Person>> getRecords(string path) {
			try {
				var db = new SQLiteAsyncConnection(path);
				//var records = await db.ExecuteAsync("SELECT * FROM Person");

				var query = db.Table<Person>();

				var records = await query.ToListAsync();
				
				return records;
			}
			catch {
				return null;
			}
		}
	}
}

