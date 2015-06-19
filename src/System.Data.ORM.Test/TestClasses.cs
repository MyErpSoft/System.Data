using System.Data.Metadata.Database;
using System.Data.Metadata.DataEntities;

namespace System.Data.ORM.Test {

    public class Customer {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public static class TestUtils {

        public static Table CreateCustomerTable() {
            return new Table(
                "Customer", new Field[]
                {
                new Field("Id",BuiltInTypes.Int32),
                new Field("Name",BuiltInTypes.String)
            });
        }

        public static Table CreateSalesOrderTable() {

            return new Table(
                "SalesOrder", new Field[]
                {
                new Field("Id",BuiltInTypes.Int32),
                new Field("Coder",BuiltInTypes.String),
                new Field("CustomerId",BuiltInTypes.Int32)
            }, new Relationship[] {
                new Relationship("Customer","Customer",
                new EndMember[] {
                    new EndMember("CustomerId","Id")
                })
            });
        }
    }
}
