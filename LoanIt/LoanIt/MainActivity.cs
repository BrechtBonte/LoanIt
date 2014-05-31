using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.IO;
using LoanIt.Core;

namespace LoanIt
{
	[Activity(Label = "LoanIt", MainLauncher = true)]
	public class MainActivity : Activity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			this.CheckDatabaseFile();

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			Repository repository = Repository.GetInstance();

			Loan[] loans = repository.GetAllLoans();
			Console.WriteLine("Current Loans:");
			for (int i = 0; i < loans.Length; i++) {
				if (i == loans.Length - 1) {
					Console.Write("\\- ");
				} else {
					Console.Write("|- ");
				}
				Console.WriteLine(loans[i].ToString());
			}
			Console.WriteLine("");

			Person[] people = repository.GetAllPeople();
			Console.WriteLine("Current people:");
			for (int i = 0; i < people.Length; i++) {
				if (i == people.Length - 1) {
					Console.Write("\\- ");
				} else {
					Console.Write("|- ");
				}
				Console.WriteLine(people[i].ToString());
			}
			Console.WriteLine("");

			Console.WriteLine(String.Format("total: {0}", repository.GetTotalAmount()));

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button>(Resource.Id.myButton);

			button.Click += delegate {
				Console.WriteLine("clicked");
				Person testPerson = repository.GetPersonByName("testName");
				if (testPerson == null) {
					testPerson = new Person("testName");
					repository.InsertObject(testPerson);
					Console.WriteLine(String.Format("Added person {0}", testPerson.ToString()));
				} else {
					Console.WriteLine(String.Format("found person {0}", testPerson.ToString()));
				}
				Loan loan = new Loan(1, true, testPerson, "");
				repository.InsertObject(loan);
				Console.WriteLine(String.Format("Added loan {0}", loan.ToString()));
				UpdateLoanCount();
			};

			UpdateLoanCount();
		}

		protected void UpdateLoanCount()
		{
			TextView totalLoans = FindViewById<TextView>(Resource.Id.textView1);
			totalLoans.SetText(
				Repository.GetInstance().GetTotalAmount().ToString(),
				TextView.BufferType.Normal
			);
		}

		protected void CheckDatabaseFile()
		{
			string dbPath = Repository.GetDbPath();
			if (!File.Exists(dbPath))
			{
				using (BinaryReader br = new BinaryReader(Assets.Open(Repository.DB_NAME)))
				{
					using (BinaryWriter bw = new BinaryWriter(new FileStream(dbPath, FileMode.Create)))
					{
						byte[] buffer = new byte[2048];
						int len = 0;
						while ((len = br.Read(buffer, 0, buffer.Length)) > 0)
						{
							bw.Write (buffer, 0, len);
						}
					}
				}
			}
		}
	}
}
