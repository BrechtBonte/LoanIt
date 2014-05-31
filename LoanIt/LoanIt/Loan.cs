using System;
using SQLite;

namespace LoanIt.Core
{
	public class Loan
	{
		[PrimaryKey, AutoIncrement]
		public int		Id { get; protected set; }
		public int		Amount { get; private set; }
		public bool		IsOwedToUser { get; private set; }
		public int		PersonId { get; private set; }
		public string 	Notes { get; set; }
		public DateTime	DateAdded { get; private set; }

		public Loan() {}

		public Loan(int amount, bool isOwedToUser, Person person, string notes)
		{
			this.Amount = amount;
			this.IsOwedToUser = isOwedToUser;
			this.PersonId = person.Id;
			this.DateAdded = DateTime.Now;
			this.Notes = notes;
		}

		public override string ToString()
		{
			return string.Format("[Loan: Id={0}, Amount={1}, IsOwedToUser={2}, PersonId={3}, Notes={4}, DateAdded={5}]", Id, Amount, IsOwedToUser, PersonId, Notes, DateAdded);
		}
	}
}
