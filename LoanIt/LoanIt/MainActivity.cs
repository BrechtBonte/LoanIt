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
using LoanIt.Helpers;
using Android.Text;
using Java.Lang;
using Android.Views.InputMethods;

namespace LoanIt
{
	[Activity(Label = "LoanIt", MainLauncher = true)]
	public class MainActivity : Activity
	{
		private int addBalance = 0;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			this.CheckDatabaseFile();

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			UpdateLoanCount();
			InitAddForm();
		}

		protected void InitAddForm()
		{
			Button  loanLower = FindViewById<Button>(Resource.Id.loanLowerButton),
					loanHigher = FindViewById<Button>(Resource.Id.loanHigherButton),
					loanSwitch = FindViewById<Button>(Resource.Id.loanSwitchButton);
			loanLower.Click += delegate {
				CheckFocus();
				addBalance -= 100;
				UploadLoanAddInput();
			};
			loanHigher.Click += delegate {
				CheckFocus();
				addBalance += 100;
				UploadLoanAddInput();
			};
			loanSwitch.Click += delegate {
				CheckFocus();
				addBalance *= -1;
				UploadLoanAddInput();
			};

			EditText loanInput = FindViewById<EditText>(Resource.Id.loanAddInput);
			loanInput.FocusChange += delegate(object sender, View.FocusChangeEventArgs e) {
				if (e.HasFocus) {
					loanInput.SetText("", TextView.BufferType.Editable);
				} else {
					LoadInputValue(loanInput);
				}
			};

			UploadLoanAddInput();
		}

		protected void UploadLoanAddInput()
		{
			EditText loanInput = FindViewById<EditText>(Resource.Id.loanAddInput);
			loanInput.SetText(NumberFormatter.FormatBalance(addBalance), TextView.BufferType.Editable);
			loanInput.SetTextColor(addBalance == 0 ? Color.Orange : (addBalance < 0 ? Color.Red : Color.Green));
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
					NumberFormatter.FormatBalance(balance),
					TextView.BufferType.Normal
				);
				balanceText.SetTextColor(balance < 0 ? Color.Red : Color.Green);
			}
		}

		protected void CheckFocus()
		{
			EditText loanInput = FindViewById<EditText>(Resource.Id.loanAddInput);
			if (loanInput.IsFocused) {
				LoadInputValue(loanInput);

				InputMethodManager imm = (InputMethodManager) GetSystemService(Context.InputMethodService);
				imm.HideSoftInputFromWindow(loanInput.WindowToken, 0);

				LinearLayout loanAddLayout = FindViewById<LinearLayout>(Resource.Id.addLoanLayout);
				loanAddLayout.RequestFocus();
			}
		}

		protected void LoadInputValue(EditText loanInput)
		{
			try {
				addBalance = NumberFormatter.GetBalanceFromFormat(loanInput.Text);
			} catch (ArgumentException) {
				addBalance = 0;
			}
			UploadLoanAddInput();
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
