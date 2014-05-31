using System;
using System.Collections.Generic;

namespace LoanIt.Core
{
	public class Person
	{
		protected string name;
		protected List<Loan> loans;

		protected string Name { get { return this.name; } }
		protected List<Loan> Loans { get { return this.loans; } }

		public Person(string name)
		{
			this.name = name;
			this.loans = new List<Loan>();
		}

		protected void AddLoan(Loan loan) {
			this.loans.Add(loan);
		}
	}
}
