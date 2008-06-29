using System;
using System.Collections.Generic;
using System.Text;

namespace ERDesigner
{
    //Miêu tả FK vẽ lên Panel
    class FKShape
    {
        public string fkName;
        public TableShape tableReference;
        public List<string> parentColumn = new List<string>();
        public List<string> childColumn = new List<string>();
        public FKShape()
        {
            tableReference = new TableShape();
            fkName = "";
        }   
    }
}
