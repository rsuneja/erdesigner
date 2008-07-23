using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ERDesigner
{
    public interface INotation
    {
        IMetaData getMetaData();
        void DrawConnectiveLines(Graphics g);
    }
}
