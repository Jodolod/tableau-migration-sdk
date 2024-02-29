﻿// Copyright (c) 2023, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the ""License"") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an ""AS IS"" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System.IO;
using Tableau.Migration.Content.Files;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content.Files
{
    public class TemporaryDirectoryContentFileStoreTests
    {
        public abstract class TemporaryDirectoryContentFileStoreTest : DirectoryContentFileStoreTestBase<TemporaryDirectoryContentFileStore>
        { }

        public class Ctor : TemporaryDirectoryContentFileStoreTest
        {
            [Fact]
            public void MakesRandomSubDirectory()
            {
                var fileStore = CreateFileStore();

                var baseStorePath = GetBaseStorePath(fileStore);

                var subDir = Path.GetRelativePath(RootPath, baseStorePath);
                Assert.NotEmpty(subDir);
            }
        }
    }
}
