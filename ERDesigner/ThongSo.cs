using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ERDesigner
{
    static class ThongSo
    {
        public static int ShapeW = 100;
        public static int ShapeH = 50;
        public static Pen JPen = new Pen(Color.Black, 1);
        public static Brush JBrush = Brushes.Black;
        public static Font JFont = new Font("Arial", 8);
        public static Pen ConectiveLinePen = new Pen(Color.Black, 1);
        public static Pen getDashPen()
        {
            Pen daspen = (Pen)JPen.Clone();
            
            //daspen.DashStyle = DashStyle.Custom;
            //daspen.DashPattern = new float[] { 10, 10, 10 };
            daspen.DashStyle = DashStyle.Dash;
            return daspen;
        }
    }
    static class EntityType
    {
        public const string Strong = "Strong Entity";
        public const string Weak = "Weak Entity";
    }
    static class AttributeType
    {
        public const string Simple = "Simple Attribute";
        public const string Key = "Key Attribute";
        public const string MultiValued = "Multivalued Attribute";
        public const string Derived = "Derived Attribute";
        public const string Child = "Child Attribute";
    }
    static class RelationshipType
    {
        public const string Normal = "Normal Relationship";
        public const string Identifier = "Identify Relationship";
        public const string AssociativeEntity = "Associative Entity";
    }
    static class ModelType
    {
        public const string Conceptual = "ConceptualModel";
        public const string Physical = "PhysicalModel";
        public const string ConceptualExtention = ".cxl";
        public const string PhysicalExtention = ".pxl";
    }

}
