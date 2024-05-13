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

using Moq;
using Tableau.Migration.Engine.Endpoints.Search;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Endpoints.Search
{
    public class ManifestDestinationContentReferenceFinderFactoryTests
    {
        public class ForContentType : AutoFixtureTestBase
        {
            [Fact]
            public void ReturnsManifestDestinationFinder()
            {
                var provider = Freeze<MockServiceProvider>();

                var fac = Create<ManifestDestinationContentReferenceFinderFactory>();

                var finder = fac.ForContentType<TestContentType>();

                provider.Verify(x => x.GetService(typeof(ManifestDestinationContentReferenceFinder<TestContentType>)), Times.Once);
            }
        }
    }
}
