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

using System.Linq;
using Microsoft.CodeAnalysis;

namespace Tableau.Migration.PythonGenerator.Writers
{
    internal sealed class PythonPropertyTestWriter : PythonMemberWriter, IPythonPropertyTestWriter
    {
        public PythonPropertyTestWriter()
        { }

        public void Write(IndentingStringBuilder builder, PythonType type, PythonProperty property)
        {
            if (property.Getter)
            {
                using (var getterBuilder = builder.AppendLineAndIndent($"def test_{property.Name}_getter(self):"))
                {
                    BuildGetterTestBody(type, property, getterBuilder);
                }

                builder.AppendLine();
            }

            if (property.Setter)
            {
                using (var getterBuilder = builder.AppendLineAndIndent($"def test_{property.Name}_setter(self):"))
                {
                    BuildSetterTestBody(type, property, getterBuilder);
                }

                builder.AppendLine();
            }

        }

        private static void BuildSetterTestBody(PythonType type, PythonProperty property, IndentingStringBuilder getterBuilder)
        {
            var dotnetObj = "dotnet";
            var pyObj = "py";

            getterBuilder.AppendLine($"{dotnetObj} = self.create({type.DotNetType.Name})");
            getterBuilder.AppendLine($"{pyObj} = {type.Name}({dotnetObj})");

            AddSetterAsserts(getterBuilder, dotnetObj, pyObj, property);
        }

        private static void AddSetterAsserts(IndentingStringBuilder builder, string dotnetObj, string pyObj, PythonProperty property)
        {
            string dotnetPropValue = $"{dotnetObj}.{property.DotNetProperty.Name}";
            var pythonPropValue = $"{pyObj}.{property.Name}";

            var typeRef = property.Type;

            switch (typeRef.ConversionMode)
            {
                case ConversionMode.WrapImmutableCollection:
                case ConversionMode.WrapMutableCollection:
                case ConversionMode.WrapArray:
                    {
                        builder.AppendLine($"assert len({dotnetPropValue}) != 0");
                        builder.AppendLine($"assert len({pythonPropValue}) == len({dotnetPropValue})");
                        builder.AppendLine();

                        builder.AppendLine("# create test data");

                        var dotnetType = property.DotNetProperty.Type;
                        
                        var element = dotnetType switch
                        {
                            INamedTypeSymbol dotnetNameType => dotnetNameType.TypeArguments.First().Name,
                            IArrayTypeSymbol dotnetArrayType => dotnetArrayType.ElementType.Name,
                            _ => throw new System.InvalidOperationException($"{dotnetType} is not supported."),
                        };

                        builder.AppendLine($"dotnetCollection = DotnetList[{element}]()");

                        for (var i = 1; i < 3; i++)
                        {
                            builder = builder.AppendLine($"dotnetCollection.Add(self.create({element}))");
                        }

                        var collectionWrapExp = ToPythonType(typeRef, $"dotnetCollection");
                        builder.AppendLine($"testCollection = {collectionWrapExp}");
                        builder.AppendLine();

                        builder.AppendLine("# set property to new test value");
                        builder.AppendLine($"{pythonPropValue} = testCollection");
                        builder.AppendLine();

                        builder.AppendLine("# assert value");
                        builder.AppendLine($"assert len({pythonPropValue}) == len(testCollection)");

                        break;
                    }
                default:
                    {
                        builder.AppendLine();

                        builder.AppendLine("# create test data");

                        var dotnetType = (INamedTypeSymbol)property.DotNetProperty.Type;

                        if (!dotnetType.IsGenericType)
                        {
                            builder.AppendLine($"testValue = self.create({dotnetType.Name})");
                        }
                        else
                        {
                            var args = string.Join(", ", dotnetType.TypeArguments.Select(x => x.Name));
                            builder.AppendLine($"testValue = self.create({dotnetType.OriginalDefinition.Name}[{args}])");
                        }

                        builder.AppendLine();

                        var wrapExp = ToPythonType(typeRef, "testValue");
                        builder.AppendLine("# set property to new test value");
                        builder.AppendLine($"{pythonPropValue} = {wrapExp}");
                        builder.AppendLine();

                        builder.AppendLine("# assert value");
                        builder.AppendLine($"assert {pythonPropValue} == {wrapExp}");
                        break;
                    }
            }
        }

        private static void BuildGetterTestBody(PythonType type, PythonProperty property, IndentingStringBuilder getterBuilder)
        {
            var dotnetObj = "dotnet";
            var pyObj = "py";

            getterBuilder.AppendLine($"{dotnetObj} = self.create({type.DotNetType.Name})");
            getterBuilder.AppendLine($"{pyObj} = {type.Name}({dotnetObj})");

            AddGetterAsserts(getterBuilder, dotnetObj, pyObj, property);
        }

        private static void AddGetterAsserts(IndentingStringBuilder builder, string dotnetObj, string pyObj, PythonProperty property)
        {
            string dotnetPropValue = $"{dotnetObj}.{property.DotNetProperty.Name}";
            var pythonPropValue = $"{pyObj}.{property.Name}";

            var typeRef = property.Type;

            switch (typeRef.ConversionMode)
            {
                case ConversionMode.WrapImmutableCollection:
                case ConversionMode.WrapMutableCollection:
                case ConversionMode.WrapArray:
                    builder.AppendLine($"assert len({dotnetPropValue}) != 0");
                    builder.AppendLine($"assert len({pythonPropValue}) == len({dotnetPropValue})");
                    break;
                case ConversionMode.Enum:
                    var enumWrapExp = ToPythonType(typeRef, dotnetPropValue);
                    builder.AppendLine($"assert {pythonPropValue}.value == ({enumWrapExp}).value");
                    break;
                case ConversionMode.Wrap:
                case ConversionMode.WrapSerialized:
                case ConversionMode.WrapGeneric:
                case ConversionMode.Direct:
                default:
                    var wrapExp = ToPythonType(typeRef, dotnetPropValue);
                    builder.AppendLine($"assert {pythonPropValue} == {wrapExp}");
                    break;
            }
        }
    }
}
