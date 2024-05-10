﻿//
//  Copyright (c) 2024, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the "License") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System;
using System.Net.Http;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api.Rest
{
    /// <summary>
    /// Class representing an error from a Tableau REST API
    /// </summary>
    public class RestException : Exception
    {
        /// <summary>
        /// Gets the request URI from Tableau API.
        /// </summary>
        public readonly HttpMethod? HttpMethod;

        /// <summary>
        /// Gets the request URI from Tableau API.
        /// </summary>
        public readonly Uri? RequestUri;

        /// <summary>
        /// Gets the error code from Tableau API.
        /// </summary>
        public readonly string? Code;

        /// <summary>
        /// Gets the error detail from Tableau API.
        /// </summary>
        public readonly string? Detail;

        /// <summary>
        /// Gets the error summary from Tableau API.
        /// </summary>
        public readonly string? Summary;

        /// <summary>
        /// Creates a new <see cref="RestException"/> instance.
        /// </summary>
        /// <param name="httpMethod">The http method that generated the current error.</param>
        /// <param name="requestUri">The request URI that generated the current error.</param>
        /// <param name="error">The <see cref="Error"/> returned from the Tableau API.</param>
        /// <param name="sharedResourcesLocalizer">A string localizer.</param>
        public RestException(
            HttpMethod? httpMethod,
            Uri? requestUri,
            Error error,
            ISharedResourcesLocalizer sharedResourcesLocalizer)
            : base(FormatError(httpMethod, requestUri, error, sharedResourcesLocalizer))
        {
            HttpMethod = httpMethod;
            RequestUri = requestUri;
            Code = error.Code;
            Detail = error.Detail;
            Summary = error.Summary;
        }

        private static string FormatError(
            HttpMethod? httpMethod,
            Uri? requestUri,
            Error error,
            ISharedResourcesLocalizer sharedResourcesLocalizer)
        {
            var nullValue = $"<{sharedResourcesLocalizer[SharedResourceKeys.NullValue]}>";

            return string.Format(
                sharedResourcesLocalizer[SharedResourceKeys.RestExceptionContent],
                httpMethod,
                requestUri,
                error.Code ?? nullValue,
                error.Summary ?? nullValue,
                error.Detail ?? nullValue);
        }
    }
}
