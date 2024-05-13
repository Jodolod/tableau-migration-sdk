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

using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Engine.Migrators;
using Tableau.Migration.Engine.Pipelines;

namespace Tableau.Migration.Engine.Actions
{
    /// <summary>
    /// Default <see cref="IMigrateContentAction{TContent}"/> implementation.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public class MigrateContentAction<TContent> : IMigrateContentAction<TContent>
        where TContent : class, IContentReference
    {
        private readonly IContentMigrator<TContent> _contentMigrator;

        /// <summary>
        /// Creates a new <see cref="MigrateContentAction{TContent}"/> object.
        /// </summary>
        /// <param name="pipeline">A pipeline to use to get the content migrator.</param>
        public MigrateContentAction(IMigrationPipeline pipeline)
        {
            _contentMigrator = pipeline.GetMigrator<TContent>();
        }

        /// <inheritdoc />
        public async Task<IMigrationActionResult> ExecuteAsync(CancellationToken cancel)
        {
            var migrateResult = await _contentMigrator.MigrateAsync(cancel).ConfigureAwait(false);

            return MigrationActionResult.FromResult(migrateResult);
        }
    }
}
