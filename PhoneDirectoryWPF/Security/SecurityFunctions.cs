using PhoneDirectoryWPF.Containers;
using PhoneDirectoryWPF.Data;
using System;
using System.Collections.Generic;
using System.Data;

namespace PhoneDirectoryWPF.Security
{
    public static class SecurityFunctions
    {
        private static string localUsername;

        private static Dictionary<string, AccessGroup> accessGroups = new Dictionary<string, AccessGroup>();
        private static LocalUser localUser;

        public static void PopulateUserAccess()
        {
            try
            {
                localUsername = Environment.UserName;

                using (var results = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectUserByName(localUsername)))
                {
                    if (results.Rows.Count > 0)
                    {
                        DataRow r = results.Rows[0];
                        localUser = new LocalUser(
                            r[Tables.Users.UserName].ToString(),
                            r[Tables.Users.FullName].ToString(),
                            (int)r[Tables.Users.AccessLevel],
                            r[Tables.Users.Guid].ToString());
                    }
                    else
                    {
                        localUser = new LocalUser();
                    }
                }
            }
            catch (Exception ex)
            {
                localUser = new LocalUser();
                throw ex;
            }
        }

        public static void PopulateAccessGroups()
        {
            using (var results = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectSecurityTable))
            {
                foreach (DataRow row in results.Rows)
                {
                    accessGroups.Add(row[Tables.Security.SecModule].ToString(), new AccessGroup(row));
                }
            }
        }

        /// <summary>
        /// Returns true if the specified user level has access the the specified security group.
        /// </summary>
        /// <param name="groupName">Name of the security group.</param>
        public static bool CanAccess(string groupName)
        {
            return CanAccess(groupName, localUser.AccessLevel);
        }

        /// <summary>
        /// Returns true if the specified user level has access the the specified security group.
        /// </summary>
        /// <param name="groupName">Name of the security group.</param>
        /// <param name="userLevel">Access level to check against.</param>
        /// <remarks>
        /// Bitwise mask operation.
        /// The user access level will be the sum of one or more groups, whos values increment
        /// by multiple of 2. (ie. 1,2,4,8,16,32,64,...) So a user access level of 5 contains
        /// only groups 1 and 3.
        ///
        /// A mask integer with the initial value of 1 is shifted left by 1 bit on each iteration.
        /// A left bit shift increments the integer value by multiples of 2, same as the above example.
        /// Then a bit AND operation is performed against the users access level and the mask. The AND
        /// operation will result in the value of the mask only if the user access level contains
        /// the mask bit.  So if groupMask = 4 and userLevel = 5, the value of (userLevel AND groupMask)
        /// will equal 4. If levelMask = 2 the value of (userLevel AND groupMask) will equal 0.
        /// </remarks>
        public static bool CanAccess(string groupName, int userLevel)
        {
            int groupMask = 1;

            foreach (AccessGroup group in accessGroups.Values)
            {
                // If the AND operation != 0 then the userLevel contains the current groupMask bit.
                bool hasGroup = (userLevel & groupMask) != 0;

                if (hasGroup)
                {
                    // We know that the current access level contains this group bit.
                    // Now we need to see if the group name matches the specified value.
                    if (group.Name == groupName & !DBFactory.CacheMode)
                    {
                        return true;
                    }
                }

                // Bitwise Left shift.
                groupMask <<= 1;
            }
            return false;
        }

        /// <summary>
        /// Checks if current user has access to the specified group. Throws a <see cref="InvalidAccessException"/> if they do not.
        /// </summary>
        /// <param name="securityGroup"></param>
        public static void CheckForAccess(string securityGroup)
        {
            if (!CanAccess(securityGroup))
            {
                string errMessage;

                if (DBFactory.CacheMode)
                {
                    errMessage = "Extensions cannot be modified while in offline mode.";
                }
                else
                {
                    errMessage = "You do not have the required access rights for this function. Must have access to '" + securityGroup + "'.";
                }

                throw new InvalidAccessException(errMessage);
            }
        }
    }
}