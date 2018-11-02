using System;
using System.Collections.Generic;
using System.Text;

namespace AwsContainerInspection
{
    public static class StringExtensions
    {
        public static string GetGuidFromEndOfArn(this string arn)
        {
            if (!string.IsNullOrWhiteSpace(arn))
            {
                int start = arn.LastIndexOf('/');
                if (start >= 0 && arn.Length > start + 1)
                {
                    return arn.Substring(start + 1);
                }
            }

            return string.Empty;
        }
    }
}
