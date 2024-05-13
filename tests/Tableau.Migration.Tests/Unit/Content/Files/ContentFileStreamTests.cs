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
using System.IO;
using System.Threading.Tasks;
using Tableau.Migration.Content.Files;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content.Files
{
    public class ContentFileStreamTests
    {
        public class TestStream : MemoryStream
        {
            public bool Disposed { get; private set; }

            protected override void Dispose(bool disposing)
            {
                Disposed = true;
                base.Dispose(disposing);
            }

            public override ValueTask DisposeAsync()
            {
                Disposed = true;
                GC.SuppressFinalize(this);
                return base.DisposeAsync();
            }
        }

        public class Ctor
        {
            [Fact]
            public async void Initializes()
            {
                var stream = new MemoryStream();
                await using var fileStream = new ContentFileStream(stream);

                Assert.Same(stream, fileStream.Content);
            }
        }

        public class DisposeAsync
        {
            [Fact]
            public async void DisposesStream()
            {
                var stream = new TestStream();
                var fileStream = new ContentFileStream(stream);

                await fileStream.DisposeAsync();

                Assert.True(stream.Disposed);
            }
        }
    }
}
