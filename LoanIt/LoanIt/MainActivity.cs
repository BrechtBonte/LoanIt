using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;
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

			// Get our button from the layout resource,
			// and attach an event to it
			/*Button button = FindViewById<Button>(Resource.Id.myButton);

			button.Click += delegate {
				Person testPerson = repository.GetPersonByName("testName");
				if (testPerson == null) {
					testPerson = new Person("testName");
					repository.InsertObject(testPerson);
				}
				Loan loan = new Loan(1, true, testPerson, "");
				repository.InsertObject(loan);
				UpdateLoanCount();
			};*/

			UpdateLoanCount();
		}

		protected void UpdateLoanCount()
		{
			TextView balanceText = FindViewById<TextView>(Resource.Id.balanceText);
			int balance = Repository.GetInstance().GetTotalAmount();

			if (balance == 0) {
				balanceText.SetText(Resource.String.balanceEven, TextView.BufferType.Normal);
				balanceText.SetTextColor(Color.Magenta);
			} else {
				balanceText.SetText(
					String.Format("{0} € {1}", balance < 0 ? '-' : '+', Math.Abs(balance / 100.0)),
					TextView.BufferType.Normal
				);
				balanceText.SetTextColor(balance < 0 ? Color.Red : Color.Green);
			}
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
