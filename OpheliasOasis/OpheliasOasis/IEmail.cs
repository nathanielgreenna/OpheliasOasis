using System;
using System.Collections.Generic;

namespace OpheliasOasis
{
	interface IEmail
	{
		List<string> GetRecipients();
		string GetHeaderText();
		string GetBodyText();
		List<string> GetAttachments();
	}
}
