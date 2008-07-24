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

        public static DBMS DB_Mode;
        public static string DB_Server;
        public static string DB_UserName;
        public static string DB_Password;
        public static bool DB_GenerateScriptFile;
        public static string DB_GenerateDirect;

        public static Pen getDashPen()
        {
            Pen daspen = (Pen)JPen.Clone();
            
            //daspen.DashStyle = DashStyle.Custom;
            //daspen.DashPattern = new float[] { 10, 10, 10 };
            daspen.DashStyle = DashStyle.Dash;
            return daspen;
        }
    }
    public class EntityType
    {
        public const string Strong = "Strong Entity";
        public const string Weak = "Weak Entity";
    }
    public class AttributeType
    {
        public const string Simple = "Simple Attribute";
        public const string Key = "Key Attribute";
        public const string MultiValued = "Multivalued Attribute";
        public const string Derived = "Derived Attribute";
        public const string Child = "Child Attribute";
    }
    public class RelationshipType
    {
        public const string Normal = "Normal Relationship";
        public const string Identifier = "Identify Relationship";
        public const string AssociativeEntity = "Associative Entity";
    }
    public class ModelType
    {
        public const string Conceptual = "ConceptualModel";
        public const string Physical = "PhysicalModel";
        public const string ConceptualExtention = ".cxl";
        public const string PhysicalExtention = ".pxl";
    }
    public class SubTypeConnectorType
    {
        public const string TotalSpecialization = "Total Specialization";
        public const string PartialSpecialization = "Partial Specialization";
        public const string DisjointConstraint = "Disjoint Constraint";
        public const string OverlapConstraint = "Overlap Constraint";
    }
}
