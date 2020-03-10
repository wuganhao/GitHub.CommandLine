/**********************************************************************************************************************
 * Copyright Moody's Analytics. All Rights Reserved.
 *
 * This software is the confidential and proprietary information of
 * Moody's Analytics. ("Confidential Information"). You shall not
 * disclose such Confidential Information and shall use it only in
 * accordance with the terms of the license agreement you entered
 * into with Moody's Analytics.
 *********************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Oolong.CommandLineParser {
    public class CommandSwitchAttribute : CommandAbstractAttribute {

        /// <summary>
        /// Create new instance of CommandOptionAttribute
        /// </summary>
        /// <param name="longName"></param>
        /// <param name="shortName"></param>
        /// <param name="description"></param>
        public CommandSwitchAttribute(string longName, string shortName, string description) :
            base(longName, shortName, description) {
        }


        public override bool TryConsumeArgs<T>(T instance, PropertyInfo prop, IEnumerator<string> argEnumerator) {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            if (instance == null)
                throw new ArgumentNullException(nameof(argEnumerator));

            string argName = argEnumerator.Current;

            if (argName != $"--{this.LongName}" && argName != $"-{this.ShortName}") {
                return false;
            }

            prop.SetValue(instance, Convert.ChangeType(true, prop.PropertyType));
            return true;
        }
    }
}
