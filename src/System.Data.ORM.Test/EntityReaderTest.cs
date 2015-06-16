using System.Data.Metadata.DataEntities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Data.ORM.Test {
    [TestClass]
    public class EntityReaderTest {
        [TestMethod]
        public void TestRead() {
            /*
            Customers 表
                CREATE TABLE[dbo].[Customers] (
                    [Id]   INT NOT NULL,
                    [Name] NVARCHAR(50) NULL
                );
            */
            var dt = typeof(Customer).GetEntityType();
            var selector = new MySelector(dt);
            selector.PropertyFieldMaps.Add(new Metadata.Mapping.PropertyFieldPair(0, dt.GetProperty("Id")));
            selector.PropertyFieldMaps.Add(new Metadata.Mapping.PropertyFieldPair(1, dt.GetProperty("Name")));

            using (var con = TestEnv.GetSqlConnection()) {
                var cmd = con.CreateCommand();
                cmd.CommandText = "SELECT Id,Name FROM dbo.Customers";
                using (var reader = cmd.ExecuteReader(CommandBehavior.SingleResult)) {

                    EntityReader entityReader = new EntityReader(reader, new Metadata.Mapping.EntitySelector[] { selector });
                    do {
                        Customer c = (Customer)entityReader.Read();
                        if (c != null) {
                            Assert.IsTrue(c.Id != 0);
                            Assert.IsTrue(c.Name != null);
                        }
                        else {
                            break;
                        }
                    } while (true);
                }
            }
        }
    }

    internal sealed class MySelector : Metadata.Mapping.EntitySelector {
        public MySelector(IEntityType dt) {
            this._entityType = dt;
        }

        private IEntityType _entityType;
        public override bool TryCreateEntity(object[] values, out object entity) {
            entity = _entityType.CreateInstance();
            return true;
        }
    }

    public class Customer {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
