using System;
using System.Collections.Generic;
using SQLite;

namespace LoanIt.Core
{
	public class Person
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; protected set; }
		public string Name { get; set; }

		public Person() {}

		public Person(string name)
		{
			this.Name = name;
		}

		public override string ToString()
		{
			return string.Format("[Person: Id={0}, Name={1}]", Id, Name);
		}
	}
}
