using System;
using System.Text;
using System.Text.RegularExpressions;

namespace LoanIt.Helpers
{
	public static class NumberFormatter
	{
		public static string FormatBalance(int balance)
		{
			if (balance == 0) {
				return "0.00";
			}

			return String.Format("{0}{1:N2}", balance < 0 ? '-' : '+', Math.Abs(balance / 100.0));
		}

		public static int GetBalanceFromFormat(string formatted)
		{
			decimal result;
			if (!decimal.TryParse(formatted, out result)) {
				throw new ArgumentException(String.Format("'{0}' is not a recognized decimal"));
			}
			return (int)Math.Round(result * 100);
		}
	}
}
