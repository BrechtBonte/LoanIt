using System;
using LoanIt.Helpers;
using LoanIt.Core;
using Android.Views;
using Android.Graphics;
using Android.Widget;
using Android.Content;

namespace LoanIt
{
	public class LoanAdapter : ArrayAdapter<Loan>
	{
		private Loan[] loans;

		public LoanAdapter(Context context, Loan[] loans) : base (context, Resource.Layout.Loan, loans)
		{
			this.loans = loans;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			LayoutInflater inflator = (LayoutInflater)this.Context.GetSystemService(Context.LayoutInflaterService);
			View loanView = inflator.Inflate(Resource.Layout.Loan, parent, false);

			TextView personName = loanView.FindViewById<TextView>(Resource.Id.loanPersonName);
			TextView loanAmount = loanView.FindViewById<TextView>(Resource.Id.loanAmountText);
			TextView loanNotes = loanView.FindViewById<TextView>(Resource.Id.loanNotes);
			TextView dateAddedText = loanView.FindViewById<TextView>(Resource.Id.dateAddedText);

			Loan loan = loans[position];
			Person person = Repository.GetInstance().GetPersonById(loan.PersonId);

			personName.SetText(person.Name, TextView.BufferType.Normal);
			loanNotes.SetText(loan.Notes, TextView.BufferType.Normal);
			dateAddedText.SetText(loan.DateAdded.ToShortDateString(), TextView.BufferType.Normal);

			loanAmount.SetText(
				NumberFormatter.FormatBalance(loan.Amount),
				TextView.BufferType.Normal
			);
			loanAmount.SetTextColor(loan.Amount < 0 ? Color.Red : Color.Green);

			return loanView;
		}
	}
}
