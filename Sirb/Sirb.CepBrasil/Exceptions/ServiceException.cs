﻿using System;

namespace Sirb.CepBrasil.Exceptions
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S3925:\"ISerializable\" should be implemented correctly", Justification = "<Pending>")]
	public sealed class ServiceException : Exception
	{
		public ServiceException(string message) : base(message)
		{
		}

		public ServiceException(string message, Exception innerException) : base(message, innerException)
		{
		}

		/// <summary>
		/// Throws ServiceException when condition are met.
		/// </summary>
		/// <param name="condition">Condition for exception</param>
		/// <param name="message">Exception message</param>
		/// <param name="innerException">Inner exception</param>
		public static void When(bool condition, string message, Exception innerException = null)
		{
			if (condition)
				throw new ServiceException(message, innerException);
		}
	}
}