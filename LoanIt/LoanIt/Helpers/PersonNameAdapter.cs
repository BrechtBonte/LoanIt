using System;
using LoanIt.Core;
using Android.Widget;
using Android.Content;
using Android.Views;
using Android.Graphics;
using LoanIt.Helpers;
using System.Collections.Generic;

namespace LoanIt
{
	public class PersonNameAdapter : ArrayAdapter<string>
	{
		private Dictionary<string, int> peopleBalances;

		public PersonNameAdapter(Context context, string[] peopleNames, Dictionary<string, int> peopleBalances) : base (context, Resource.Layout.Loan, peopleNames)
		{
			this.peopleBalances = peopleBalances;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			LayoutInflater inflator = (LayoutInflater)this.Context.GetSystemService(Context.LayoutInflaterService);
			View loanView = inflator.Inflate(Resource.Layout.PersonNameItem, parent, false);

			TextView personNameField = loanView.FindViewById<TextView>(Resource.Id.personName);
			TextView balanceText = loanView.FindViewById<TextView>(Resource.Id.personBalance);

			string personName = this.GetItem(position);
			personNameField.SetText(personName, TextView.BufferType.Normal);

			if (this.peopleBalances.ContainsKey(personName)) {
				int balance = this.peopleBalances[personName];

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

			return loanView;
		}
	}
}
