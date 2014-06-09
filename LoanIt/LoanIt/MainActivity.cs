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
using System.Collections.Generic;

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
			UpdateRecentLoans();
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

			Button addLoanButton = FindViewById<Button>(Resource.Id.addLoanButton);
			AutoCompleteTextView nameInput = FindViewById<AutoCompleteTextView>(Resource.Id.personNameInput);
			TextView notesInput = FindViewById<TextView>(Resource.Id.loanNotesInput);
			addLoanButton.Click += delegate {
				Color errorCollor = new Color(255, 0, 0, 100);
				bool allOk = true;
				if (addBalance == 0) {
					loanInput.SetBackgroundColor(errorCollor);
					allOk = false;
				} else {
					loanInput.SetBackgroundColor(Color.Transparent);
				}

				if (nameInput.Text.Length == 0) {
					nameInput.SetBackgroundColor(errorCollor);
					allOk = false;
				} else {
					nameInput.SetBackgroundColor(Color.Transparent);
				}

				if (allOk) {
					Repository repository = Repository.GetInstance();
					Person person = repository.FindPersonByName(nameInput.Text);
					if (person == null) {
						person = new Person(nameInput.Text);
						repository.InsertObject(person);
					}
					Loan loan = new Loan(
						(int)System.Math.Abs(addBalance),
						addBalance > 0,
						person,
						notesInput.Text
					);
					repository.InsertObject(loan);

					UpdateLoanCount();
					UpdateAutocompleteNames();
					UpdateRecentLoans();

					addBalance = 0;
					UploadLoanAddInput();
					nameInput.SetText("", TextView.BufferType.Editable);
					notesInput.SetText("", TextView.BufferType.Editable);
				}
			};

			UpdateAutocompleteNames();
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
				balanceText.SetTextColor(Color.Orange);
			} else {
				balanceText.SetText(
					NumberFormatter.FormatBalance(balance),
					TextView.BufferType.Normal
				);
				balanceText.SetTextColor(balance < 0 ? Color.Red : Color.Green);
			}
		}

		protected void UpdateRecentLoans()
		{
			ListView recentLoansList = FindViewById<ListView>(Resource.Id.recentLoansList);
			LoanAdapter loanAdapter = new LoanAdapter(this, Repository.GetInstance().GetRecentLoans(3));
			recentLoansList.Adapter = loanAdapter;
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

		protected void UpdateAutocompleteNames()
		{
			Repository repository = Repository.GetInstance();

			AutoCompleteTextView nameInput = FindViewById<AutoCompleteTextView>(Resource.Id.personNameInput);
			Dictionary<string, int> peopleBalances = repository.GetPersonBalances();
			string[] peopleNames = new string[peopleBalances.Count];
			peopleBalances.Keys.CopyTo(peopleNames, 0);
			nameInput.Adapter = new PersonNameAdapter(this, peopleNames, peopleBalances);
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
