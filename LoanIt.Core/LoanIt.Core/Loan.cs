using System;

namespace LoanIt.Core
{
	public class Loan
	{
		protected int amount;
		protected bool isOwedToUser;
		protected Person person;

		public Loan(int amount, bool isOwedToUser, Person person)
		{
			this.amount = amount;
			this.isOwedToUser = isOwedToUser;
			this.person = person;
		}
	}
}
