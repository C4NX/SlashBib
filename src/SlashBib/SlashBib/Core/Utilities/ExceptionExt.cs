using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlashBib.Core.Utilities
{
    internal static class ExceptionExt
    {
        internal static IEnumerable<Exception> GetInnerExceptions(this Exception ex)
        {
            if (ex == null)
                throw new ArgumentNullException(nameof(ex));

            var innerException = ex;
            while (innerException != null)
            {
                innerException = innerException.InnerException;
                if (innerException != null)
                    yield return innerException;
            }
        }
    }
}
