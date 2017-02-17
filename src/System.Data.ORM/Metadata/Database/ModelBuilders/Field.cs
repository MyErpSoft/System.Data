using System;
using System.Collections.Generic;
using System.Data.DataEntities.Dynamic;
using System.Data.Metadata.DataEntities.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Metadata.Database.ModelBuilders {

    public class Field : DynamicEntity {
        public static readonly DynamicEntityType FieldDynamicEntityType = new DynamicEntityType("Field");

        public Field() : base(FieldDynamicEntityType) {
        }

        private string _name;
        /// <summary>
        /// 返回/设置字段的名称。
        /// </summary>
        public string Name {
            get { return _name; }
            set { _name = value; }
        }


    }
}
