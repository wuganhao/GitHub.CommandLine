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

namespace Oolong.CommandLineParser {
    /// <summary>
    /// Command line exception
    /// </summary>
    public class CommandLineException: SystemException {

        /// <summary>
        /// Create new instance of CommandLineException
        /// </summary>
        /// <param name="message"></param>
        public CommandLineException(string message): base(message) {
        }
    }
}
