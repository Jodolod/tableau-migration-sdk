# Copyright (c) 2024, Salesforce, Inc.
# SPDX-License-Identifier: Apache-2
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

# Make sure the test can find the module
import sys
from os.path import abspath
from pathlib import Path
import uuid
import pytest

from tableau_migration.migration import (
    PyMigrationManifest, 
    PyMigrationResult)
from tableau_migration.migration_engine import (
    PyMigrationPlan)

from tableau_migration.migration_engine_migrators import PyMigrator

import System
from Tableau.Migration import (
    IMigrator, 
    IMigrationManifest, 
    IMigrationPlan,
    MigrationResult,
    MigrationCompletionStatus)
from Tableau.Migration.Engine.Manifest import (
    MigrationManifest, 
    IMigrationManifestEditor)
import Moq
        

class TestPyMigrator():
    def test_init(self):
        from tableau_migration.migration_engine_migrators import PyMigrator
        PyMigrator()
    
    def test_migration_migrator_execute_without_manifest(self):
        migrator_mock = Moq.Mock[IMigrator]()
        plan_mock = Moq.Mock[IMigrationPlan]()

        migrator = PyMigrator()
        migrator._migrator = migrator_mock.Object
    
        plan = PyMigrationPlan(plan_mock.Object)

        migrator.execute(plan)

        invokedMethodNames = [methodInfo.Method.Name for methodInfo in migrator_mock.Invocations]

        assert "ExecuteAsync" in invokedMethodNames


    def test_migration_migrator_execute_with_manifest(self):
        migrator_mock = Moq.Mock[IMigrator]()
        manifest_mock = Moq.Mock[IMigrationManifest]()
        plan_mock = Moq.Mock[IMigrationPlan]()

        migrator = PyMigrator()
        migrator._migrator = migrator_mock.Object

        manifest = PyMigrationManifest(manifest_mock.Object)

        plan = PyMigrationPlan(plan_mock.Object)
    
        migrator.execute(plan, previous_manifest=manifest)

        invokedMethodNames = [methodInfo.Method.Name for methodInfo in migrator_mock.Invocations]

        assert "ExecuteAsync" in invokedMethodNames


    def test_migration_plan_builder_ctor(self):
        """
        Verify that the PyMigrationPlanBuilder can be used
        """
        from tableau_migration.migration_engine import PyMigrationPlanBuilder
        PyMigrationPlanBuilder()


class TestPyMigrationResult():
    def test_init(self):
        status = MigrationCompletionStatus.Completed
        manifest_mock = Moq.Mock[IMigrationManifestEditor]()
        result = MigrationResult(status, manifest_mock.Object)

        PyMigrationResult(result)

class TestPyMigrationManifest():
    def test_init(self):
        manifest_mock = Moq.Mock[IMigrationManifestEditor]()

        PyMigrationManifest(manifest_mock.Object)


    def test_add_error_list(self):
        manifest_mock = Moq.Mock[IMigrationManifestEditor]()
        manifest = PyMigrationManifest(manifest_mock.Object)

        errors = [System.Exception(), System.Exception()]

        manifest.add_errors(errors)
        invokedMethodNames = [methodInfo.Method.Name for methodInfo in manifest_mock.Invocations]
        assert "AddErrors" in invokedMethodNames


    def test_add_error_exception(self):
        manifest_mock = Moq.Mock[IMigrationManifestEditor]()
        manifest = PyMigrationManifest(manifest_mock.Object)

        manifest.add_errors(System.Exception())

        invokedMethodNames = [methodInfo.Method.Name for methodInfo in manifest_mock.Invocations]
        assert "AddErrors" in invokedMethodNames

    def test_add_error_throw_on_bad_type(self):
        manifest_mock = Moq.Mock[IMigrationManifestEditor]()
        manifest = PyMigrationManifest(manifest_mock.Object)
        
        with pytest.raises(Exception):
            manifest.add_errors(Exception()) # Passing a python exception which is not valid

        invokedMethodNames = [methodInfo.Method.Name for methodInfo in manifest_mock.Invocations]
        # dotnet function was not called, as there was no function to call that takes
        # a python exception
        assert not invokedMethodNames 
