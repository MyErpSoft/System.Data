using System.Data.Metadata.Database;
using System.Data.ORM.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Data.Metadata.DataEntities;

namespace System.Data.Metadata.Database.Test {

    [TestClass]
    public class DatabaseMetadataContainerTest {
        [TestMethod]
        public void TestAdd() {
            DatabaseMetadataContainer container = new DatabaseMetadataContainer();
            var table = TestUtils.CreateCustomerTable();
            container.TryAdd(table);
            Assert.AreEqual(table,container.GetTable("Customer"));

            Assert.AreEqual(container, table.Container);
            Assert.AreEqual("Id", table.GetField("Id").Name);
            Assert.AreEqual(table, table.GetField("Name").Table);
        }

        [TestMethod]
        public void TestLazyGetRelationship() {
            DatabaseMetadataContainer container = new DatabaseMetadataContainer();
            container.TableResolve += (object sender, MetadataResolveEventArgs e) => {
                if (e.Name == "Customer") {
                    container.TryAdd(TestUtils.CreateCustomerTable());
                }
                else if (e.Name == "SalesOrder") {
                    container.TryAdd(TestUtils.CreateSalesOrderTable());
                }
            };

            var salesOrderTable = container.GetTable("SalesOrder");
            Assert.IsNotNull(salesOrderTable);

            var ship = salesOrderTable.GetRelationship("Customer");
            Assert.IsNotNull(ship);

            Assert.AreEqual(salesOrderTable, ship.From);
            Assert.AreEqual("Customer", ship.To.Name);
            var endMember = ship.EndMembers.First();
            Assert.AreEqual("CustomerId", endMember.FromField.Name);
            Assert.AreEqual("Id", endMember.ToField.Name);
        }

        [TestMethod]
        public void TestLazyGetRelationshipError() {
            DatabaseMetadataContainer container = new DatabaseMetadataContainer();
            var customerTable = new Table(
                "Customer", new Field[]
                {
                new Field("Id",BuiltInTypes.Int32),
                new Field("Name",BuiltInTypes.String)
            });

            var salesOrderTable = new Table(
                "SalesOrder", new Field[]
                {
                new Field("Id",BuiltInTypes.Int32),
                new Field("Coder",BuiltInTypes.String),
                new Field("CustomerId",BuiltInTypes.Int32)
            }, new Relationship[] {
                new Relationship("Customer","CustomerError",
                new EndMember[] {
                    new EndMember("CustomerId","Id")
                }),
                new Relationship("Customer2","Customer",
                new EndMember[] {
                    new EndMember("FromError","ToError")
                })
            });

            container.TableResolve += (object sender, MetadataResolveEventArgs e) => {
                if (e.Name == "Customer") {
                    container.TryAdd(customerTable);
                }
                else if (e.Name == "SalesOrder") {
                    container.TryAdd(salesOrderTable);
                }
            };

            salesOrderTable = container.GetTable("SalesOrder");
            var ship = salesOrderTable.GetRelationship("Customer");
            Exception ex2 = null;
            try {
                Assert.AreEqual("Customer", ship.To.Name);
            }
            catch (Exception ex) {
                ex2 = ex;
            }
            Assert.IsNotNull(ex2);

            ex2 = null;
            ship = salesOrderTable.GetRelationship("Customer2");
            var endMember = ship.EndMembers.First();
            try {
                Assert.AreEqual("CustomerId", endMember.FromField.Name);
            }
            catch (Exception ex) {
                ex2 = ex;
            }
            Assert.IsNotNull(ex2);

            ex2 = null;
            try {
                Assert.AreEqual("Id", endMember.ToField.Name);
            }
            catch (Exception ex) {
                ex2 = ex;
            }
            Assert.IsNotNull(ex2);
        }
    }
}
