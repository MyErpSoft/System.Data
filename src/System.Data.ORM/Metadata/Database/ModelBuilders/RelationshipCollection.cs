using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Metadata.Database.ModelBuilders {

    public class RelationshipCollection : MetadataCollection<string, Relationship> {

        protected override string GetKeyForItem(Relationship item) {
            return item.Name;
        }
    }
}
