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
using System.Collections.ObjectModel;
using System.Reflection;

namespace Oolong.CommandLineParser
{
    /// <summary>
    /// Command option attribute
    /// </summary>
    public class CommandOptionAttribute : CommandAbstractAttribute {

        /// <summary>
        /// Create new instance of CommandOptionAttribute
        /// </summary>
        /// <param name="longName"></param>
        /// <param name="shortName"></param>
        /// <param name="description"></param>
        public CommandOptionAttribute(string longName, string shortName, string description) :
            base(longName, shortName, description) {
        }

        /// <summary>
        /// Try get current argument value from command line arguments
        /// </summary>
        /// <param name="argEnumerator"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool TryGetValue(IEnumerator<string> argEnumerator, out string value) {
            string argName = argEnumerator.Current;

            if (argName == $"--{this.LongName}" || argName == $"-{this.ShortName}") {
                if (!argEnumerator.MoveNext()) {
                    throw new ArgumentException($"Missing value for {argName}");
                }

                value = argEnumerator.Current;

                return true;
            } else if (argName.StartsWith($"--{this.LongName}=") || argName.StartsWith($"-{this.ShortName}=")) {
                string[] strs = argName.Split('=');
                value = strs[1];
                return true;
            } else {
                value = null;
                return false;
            }
        }

        /// <summary>
        /// Try to consume command line argument.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="prop"></param>
        /// <param name="argEnumerator"></param>
        /// <returns></returns>
        public override bool TryConsumeArgs<T>(T instance, PropertyInfo prop, IEnumerator<string> argEnumerator) {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            if (instance == null)
                throw new ArgumentNullException(nameof(argEnumerator));

            if (!this.TryGetValue(argEnumerator, out string value)) {
                return false;
            }

            if (prop.PropertyType.IsGenericType) {
                if (prop.PropertyType == typeof(ICollection<string>)) {
                    ICollection<string> collection = prop.GetValue(instance) as ICollection<string>;
                    if (collection == null) {
                        collection = new Collection<string>();
                        prop.SetValue(instance, collection);
                    }
                    collection.Add(value);
                } else {
                    throw new InvalidOperationException($"Invalid type {prop.PropertyType}");
                }
            } else if (prop.PropertyType.IsEnum) {
                prop.SetValue(instance, Enum.Parse(prop.PropertyType, value, true));
            } else {
                prop.SetValue(instance, Convert.ChangeType(value, prop.PropertyType));
            }
            return true;
        }
    }
}
