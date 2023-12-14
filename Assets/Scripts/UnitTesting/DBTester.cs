/*using Mono.Data.Sqlite;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Data;

// Important note: We are NOT testing any calls to an external DB as this would be integration testing.
// As such, any tests involving queries will use in-memory DBs (ephemeral, only last for the connection lifetime).
// We can however test to see if a DB "connection" can be made, which for SQLite is just making a local DB file (if not in-memory).

[TestFixture] // Indicates our tester (class below) has explicit setup/teardown functionality and contains the test methods.
public class DBTester {
    private DBManager _dbManager;

    // Setup called before each test, teardown after.
    [SetUp]
    public void Setup() {
        _dbManager = DBManager.GetInstance(); // Get a handle on the singleton instance. Note this has not yet opened any connection.
        _dbManager.OpenDBConnection(DBName: ":memory:");
        _dbManager.ExecuteParamQueryNonReader($@"CREATE TABLE IF NOT EXISTS Mock (TestID INTEGER PRIMARY KEY NOT NULL, SomeVal INT NOT NULL);");
        _dbManager.ExecuteParamQueryNonReader($@"INSERT INTO Mock (SomeVal) VALUES (4), (5), (6);");
    }

    [TearDown]
    public void Teardown() {
        _dbManager.CloseDBConnection(); // Close any open connection.
    }

    // Tests.
    // Connection Start.
    [Category("UnitLevel")]
    [Test]
    public void OpenDBConnectionToFileSuccess() {
        _dbManager.OpenDBConnection(DBName: "TestDB.db");

        // Connection should exist AND be open.
        ClassicAssert.NotNull(_dbManager.Connection);
        ClassicAssert.AreEqual(ConnectionState.Open, _dbManager.Connection.State);
    }

    [Category("UnitLevel")]
    [Test]
    public void OpenDBConnectionToFileFailureInvalidCharInName() {
        // Should throw SQL-specific exception.
        SqliteException ex = Assert.Throws<SqliteException>(() => _dbManager.OpenDBConnection(DBName: "TestDB*.db"));
        TestContext.WriteLine($"{ex.Message} ({ex.ErrorCode})");
    }

    [Category("UnitLevel")]
    [Test]
    public void OpenDBConnectionToFileFailureNoName() {
        // Should throw argument-level exception.
        ArgumentException ex = Assert.Throws<ArgumentException>(() => _dbManager.OpenDBConnection(DBName: ""));
        TestContext.WriteLine($"{ex.Message}");
    }

    [Category("UnitLevel")]
    [Test]
    public void ExecuteQuerySuccess() {
        Assert.DoesNotThrow(
            () => _dbManager.ExecuteParamQueryReader($@"SELECT * FROM Mock;")
        );
    }

    [Category("UnitLevel")]
    [Test]
    public void ExecuteQueryFailureSyntaxError() {
        SqliteException ex = Assert.Throws<SqliteException>(
            () => _dbManager.ExecuteParamQueryNonReader($@"Hello;")
        );

        TestContext.WriteLine($"{ex.Message}");
    }

    [Category("UnitLevel")]
    [Test]
    public void ExecuteQueryFailureLogicError() {
        SqliteException ex = Assert.Throws<SqliteException>(() =>
        _dbManager.ExecuteParamQueryReader($@"SELECT NonExistentColumn FROM Mock;"));

        TestContext.WriteLine($"{ex.Message}");
    }

    [Category("UnitLevel")]
    [Test]
    public void ExecuteQueryFailureEmptyQueryString() {
        ArgumentException ex = Assert.Throws<ArgumentException>(
            () => _dbManager.ExecuteParamQueryReader("")
        );

        TestContext.WriteLine($"{ex.Message}");
    }

    [Category("UnitLevel")]
    [Test]
    public void CloseDBConnectionSuccess() {
        Assert.DoesNotThrow(() => _dbManager.CloseDBConnection());
    }

    [Category("UnitLevel")]
    [Test]
    public void CloseDBConnectionSuccessRepeatedClosureCalls() {
        Assert.DoesNotThrow(() => {
            _dbManager.CloseDBConnection();
            _dbManager.CloseDBConnection();
        });
    }


    // CRUD.
    [Category("SideEffectLevel")]
    [Test]
    public void CREATETableInDBSuccess() {
        // To check, https://renenyffenegger.ch/notes/development/databases/SQLite/internals/schema-objects/sqlite_master/index
        // Note that CREATE does not return number of affected rows (makes sense), thus we need to query the DB to find the table.
        string tableName = "Test";
        _dbManager.ExecuteParamQueryNonReader(
            $@"CREATE TABLE IF NOT EXISTS {tableName} (TestID INTEGER PRIMARY KEY NOT NULL, TestName VARCHAR(10) NOT NULL);"
        );

        ClassicAssert.IsTrue(CheckIfTableExists(tableName));
    }

    private bool CheckIfTableExists(string tableName) {
        using SqliteDataReader reader = _dbManager.ExecuteParamQueryReader(
            $@"SELECT Name FROM sqlite_master WHERE Type = 'table' AND Name = '{tableName}';"
        );
        return reader.Read();
    }

    [Category("SideEffectLevel")]
    [Test]
    public void INSERTIntoTableSuccess() {
        int recordsAffected = _dbManager.ExecuteParamQueryNonReader(
            sql: $@"INSERT INTO Mock (SomeVal) VALUES (@firstVal), (@secondVal);",
            sqlParams: 
                new Dictionary<string, object> {
                    { "@firstVal", "1" },
                    { "@secondVal", "2" },
                }
        );

        ClassicAssert.IsTrue(recordsAffected == 2); // Inserted two rows, should be 2.
        TestContext.WriteLine($"Records affected: {recordsAffected}");
    }

    [Category("SideEffectLevel")]
    [Test]
    public void SELECTFromTableSuccess() {
        using SqliteDataReader reader = _dbManager.ExecuteParamQueryReader($@"SELECT * FROM Mock;");

        ClassicAssert.IsTrue(reader.FieldCount == 2); // Columns returned (should be 2).

        int rowCount = 0; // Helper to track number of rows returned.

        while (reader.Read()) {
            rowCount++;
            
            // Access specific column data by indexing (can also use column name).
            int id = reader.GetInt32(0);
            int val = reader.GetInt32(1);

            TestContext.WriteLine($"TestID: {id}, SomeVal: {val}");
        }

        ClassicAssert.IsTrue(rowCount == 3);
    }

    [Category("SideEffectLevel")]
    [Test]
    public void UpdateThreeRecordsSuccess() {
        int recordsAffected = _dbManager.ExecuteParamQueryNonReader(
            $@"UPDATE Mock SET SomeVal = 20 WHERE SomeVal != 20;"
        );

        ClassicAssert.IsTrue(recordsAffected == 3);
    }

    [Category("SideEffectLevel")]
    [Test]
    public void UpdateNoRecordsWhenConditionFiltersNoRows() {
        int recordsAffected = _dbManager.ExecuteParamQueryNonReader(
            $@"UPDATE Mock SET SomeVal = 20 WHERE SomeVal = 50;"
        );

        ClassicAssert.IsTrue(recordsAffected == 0);
    }

    [Category("SideEffectLevel")]
    [Test]
    public void Delete1RecordSuccess() {
        int recordsAffected = _dbManager.ExecuteParamQueryNonReader(
            $@"DELETE FROM Mock WHERE SomeVal = 4;"
        );

        ClassicAssert.IsTrue(recordsAffected == 1);
    }

    [Category("SideEffectLevel")]
    [Test]
    public void DeleteAllRecordsFromTableSuccess() {
        int recordsAffected = _dbManager.ExecuteParamQueryNonReader(
            $@"DELETE FROM Mock;"
        );

        ClassicAssert.IsTrue(recordsAffected == 3);
    }

    [Category("SideEffectLevel")]
    [Test]
    public void DropTableSuccess() {
        _dbManager.ExecuteParamQueryNonReader(
            $@"DROP TABLE Mock;"
        );

        ClassicAssert.IsFalse(CheckIfTableExists("Mock"));
    }

    // Nothing to drop DB, don't need for now.
}
*/