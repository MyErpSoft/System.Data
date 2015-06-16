using System.Data.Query;

namespace System.Data.Drivers {

    internal class AutoSelectSqlBuilder : BasicAutoSqlBuilder {

        public AutoSelectSqlBuilder(AutoSelectInfo info) {
            this._selectInfo = info;
        }
        
        private readonly AutoSelectInfo _selectInfo;
        public AutoSelectInfo SelectInfo {
            get { return _selectInfo; }
        }

        public virtual string Build() {
            /*
            SELECT t1.Id f1,t1.Name f2 FROM Customers t1
            */
            AppendSelect();
            AppendFrom();
            AppendWhere();

            return Sql.ToString();
        }

        protected virtual void AppendWhere() {
            //_sql.Append(" WHERE ");
        }

        protected virtual void AppendSelect() {
            Sql.Append("SELECT ");
            foreach (var field in _selectInfo.SelectFields) {
                AppendField(field);
            }
        }


        protected virtual void AppendFrom() {
#if DEBUG
            Sql.AppendLine();
#endif
            //FROM SalesOrder t1 LEFT JOIN Customers t2 ON t1.f1 = t2.f30
            Sql.Append(" FROM ");
            AppendTable(_selectInfo.RootTableNode);

            AppendRelations(_selectInfo.RootTableNode);
        }
    }
}
