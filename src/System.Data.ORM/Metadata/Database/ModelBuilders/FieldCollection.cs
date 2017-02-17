using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Metadata.Database.ModelBuilders {

    public class FieldCollection : MetadataCollection<string, Field> {

        protected override string GetKeyForItem(Field item) {
            return item.Name;
        }
    }
}
