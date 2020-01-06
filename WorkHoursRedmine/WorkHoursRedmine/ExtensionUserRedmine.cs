using Redmine.Net.Api.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinRedminePlaning
{
    static class ExtensionUserRedmine
    {
        public static bool IsCustomFieldEqual(this User user, string value)
        {
            bool isCustomFieldEqual = false;

            foreach (var customField in user.CustomFields)
            {
                if (customField.Name.Contains(value))
                {
                    string info = customField.Values[0].Info;
                    if (info.ToLower().Contains("1"))
                        isCustomFieldEqual = true;
                }
            }

            return isCustomFieldEqual;
        }
    }
}
