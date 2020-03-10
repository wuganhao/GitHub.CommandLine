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

namespace Oolong.CommandLineParser
{
    public abstract class CommandAbstractAttribute : Attribute {
        /// <summary>
        /// Create new instance of CommandOptionAttribute
        /// </summary>
        /// <param name="longName"></param>
        /// <param name="shortName"></param>
        /// <param name="description"></param>
        public CommandAbstractAttribute(string longName, string shortName, string description) {
            this.LongName = longName;
            this.ShortName = shortName;
            this.Description = description;
        }

        /// <summary>
        /// Long name
        /// </summary>
        public string LongName { get; }

        /// <summary>
        /// Short name
        /// </summary>
        public string ShortName { get; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; }

        public abstract bool TryConsumeArgs<T>(T instance, PropertyInfo prop, IEnumerator<string> argEnumerator);
    }

    public class RequiredAttribute: Attribute {
        public RequiredAttribute(bool required = true) {
            this.Required = required;
        }

        public bool Required { get; }
    }
}
