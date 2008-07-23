using System;
using System.Collections.Generic;
using System.Text;
using ERDesigner.Shape;
using System.Drawing;

namespace ERDesigner
{
    class DrawingSupport
    {
        public static int CalculateCardiDirection(ShapeBase shape1, ShapeBase shape2)
        {
            if (shape1 == null || shape2 == null)
                return 0;

            //Cạnh đặt Shape - > 1,2,3,4: Up, Right, Down, Left
            int edgeCardiPlace = -1;

            Point point1 = shape1.CenterPoint;
            Point point2 = shape2.CenterPoint;

            //Cách khác: phương trình qua 2 điểm centerRelationship và point1 (y = mx + b)
            double m = point2.Y - point1.Y;

            if (point2.X != point1.X)
                m /= point2.X - point1.X;
            else
                m = 0;

            double b = point2.Y - m * point2.X;


            if (point1.X < point2.X) //shape 2 nằm bên phải
            {
                //thế "x + width" của shape 1 vào phương trình để tính y
                //Nếu y vượt quá khoảng y của shape 2 thì đặt cardi ở up hoặc down: else đặt right
                double ytemp = m * (shape1.Location.X + shape1.Width) + b;

                if (ytemp < shape1.Location.Y) edgeCardiPlace = 1; //Up
                else if (ytemp > shape1.Location.Y + shape1.Height) edgeCardiPlace = 3; //Down

                if (ytemp >= shape1.Location.Y && ytemp <= shape1.Location.Y + shape1.Height) edgeCardiPlace = 2; //Right
            }
            else //shape 2 nằm bên trái hoặc nằm thẳng hàng
            {
                //thế "x" của shape 1 vào phương trình để tính y
                //Nếu y vượt quá khoảng y của shape 2 thì đặt cardi ở up hoặc down: else đặt left
                double ytemp = m * (shape1.Location.X) + b;

                if (ytemp < shape1.Location.Y) edgeCardiPlace = 1; //Up
                else if (ytemp > shape1.Location.Y + shape1.Height) edgeCardiPlace = 3; //Down

                if (ytemp >= shape1.Location.Y && ytemp <= shape1.Location.Y + shape1.Height) edgeCardiPlace = 4; //Left
            }
            return edgeCardiPlace;
        }
        public static void DrawCardinalitiesLine(Graphics g, CardinalityShape cardi, int EntityEdgeCardiPlace, int pos, int numCardi, Color backgroundColor)
        {
            EntityShape entity = cardi.Entity;
            RelationshipShape rel = cardi.Relationship;

            //debug
            //g.DrawLine(new Pen(Color.Red, 1), entity.CenterPoint, rel.CenterPoint);

            Point centerRelationship = rel.CenterPoint;
            Point centerEntity = entity.CenterPoint;

            Point TopLeft = entity.Location;
            Point BottomRight = new Point(entity.Location.X + entity.Width, entity.Location.Y + entity.Height);

            int stepX = pos * entity.Width / (numCardi + 1);
            int stepY = pos * entity.Height / (numCardi + 1);

            Bitmap myCardi = GetCardinalitiesShape(cardi, EntityEdgeCardiPlace, backgroundColor);

            Point EntityStartPos = getStartPos(EntityEdgeCardiPlace, (ShapeBase)entity, TopLeft, BottomRight, stepX, stepY, myCardi);
            Point EntityCardiPos = getCardiPos(EntityEdgeCardiPlace, (ShapeBase)entity, TopLeft, BottomRight, stepX, stepY, myCardi);

            if (rel.type != RelationshipType.AssociativeEntity)
            {
                g.DrawImage(myCardi, EntityCardiPos);
                g.DrawLine(ThongSo.ConectiveLinePen, EntityStartPos, centerRelationship);
            }
            else //nếu relationship là Associative Entity
            {
                Point TopLeftRel = rel.Location;
                Point BottomRightRel = new Point(rel.Location.X + rel.Width, rel.Location.Y + rel.Height);

                int AssEdgeCardiPlace = 0, index = 0;
                rel.getCardiPosition(cardi, ref AssEdgeCardiPlace, ref index);
                
                int stepXRel = (index + 1) * rel.Width / (rel.cardiplaces[AssEdgeCardiPlace - 1].Count + 1);
                int stepYRel = (index + 1) * rel.Height / (rel.cardiplaces[AssEdgeCardiPlace - 1].Count + 1);

                //cardi 1, 1
                CardinalityShape cardi1 = new CardinalityShape();
                cardi1.setValue(1, 1);

                //lấy cardi của đầu bên kia Relationship
                CardinalityShape cardi2 = new CardinalityShape();
                foreach (CardinalityShape card in cardi.Relationship.cardinalities)
                {
                    if (card != cardi)
                        cardi2 = card;
                }

                Bitmap CardiAtEn = GetCardinalitiesShape(cardi1, EntityEdgeCardiPlace, backgroundColor);
                Bitmap CardiAtRel = GetCardinalitiesShape(cardi2, AssEdgeCardiPlace, backgroundColor);

                Point AssStartPos = getStartPos(AssEdgeCardiPlace, (ShapeBase)rel, TopLeftRel, BottomRightRel, stepXRel, stepYRel, CardiAtRel);
                Point AssCardiPos = getCardiPos(AssEdgeCardiPlace, (ShapeBase)rel, TopLeftRel, BottomRightRel, stepXRel, stepYRel, CardiAtRel);

                g.DrawImage(CardiAtEn, EntityCardiPos);
                g.DrawImage(CardiAtRel, AssCardiPos);
                g.DrawLine(ThongSo.ConectiveLinePen, EntityStartPos, AssStartPos);
            }
        }
        private static Point getCardiPos(int EdgeCardiPlace, ShapeBase shape, Point TopLeft, Point BottomRight, int stepX, int stepY, Bitmap myCardi)
        {
            Point CardiPos = new Point();

            switch (EdgeCardiPlace)
            {
                case 1:
                    CardiPos.X = TopLeft.X + stepX - myCardi.Width / 2;
                    CardiPos.Y = TopLeft.Y - myCardi.Height;
                    break;
                case 2:
                    CardiPos.X = BottomRight.X;
                    CardiPos.Y = BottomRight.Y - (shape.Height - stepY) - myCardi.Height / 2;
                    break;
                case 3:
                    CardiPos.X = BottomRight.X - (shape.Width - stepX) - myCardi.Width / 2 + 1;
                    CardiPos.Y = BottomRight.Y;
                    break;
                case 4:
                    CardiPos.X = TopLeft.X - myCardi.Width;
                    CardiPos.Y = TopLeft.Y + stepY - myCardi.Height / 2 + 1;
                    break;
            }

            return CardiPos;
        }
        private static Point getStartPos(int EdgeCardiPlace, ShapeBase shape, Point TopLeft, Point BottomRight, int stepX, int stepY, Bitmap myCardi)
        {
            Point StartPos = new Point();

            switch (EdgeCardiPlace)
            {
                case 1:
                    StartPos.X = TopLeft.X + stepX;
                    StartPos.Y = TopLeft.Y - myCardi.Height;
                    break;
                case 2:
                    StartPos.X = BottomRight.X + myCardi.Width;
                    StartPos.Y = BottomRight.Y - (shape.Height - stepY);
                    break;
                case 3:
                    StartPos.X = BottomRight.X - (shape.Width - stepX);
                    StartPos.Y = BottomRight.Y + myCardi.Height;
                    break;
                case 4:
                    StartPos.X = TopLeft.X - myCardi.Width;
                    StartPos.Y = TopLeft.Y + stepY;
                    break;
            }

            return StartPos;
        }
        private static Bitmap GetCardinalitiesShape(CardinalityShape cardi, int edgeCardiPlace, Color backgroundColor)
        {
            Bitmap carshape = new Bitmap(20, 20);
            Graphics gimage = Graphics.FromImage(carshape);

            if (cardi.MaxCardinality == -1)
            {
                gimage.DrawLine(ThongSo.ConectiveLinePen, new Point(10, 8), new Point(3, 20));
                gimage.DrawLine(ThongSo.ConectiveLinePen, new Point(10, 8), new Point(17, 20));
                gimage.DrawLine(ThongSo.ConectiveLinePen, new Point(10, 0), new Point(10, 20));

                if (cardi.MinCardinality == 0)
                {
                    gimage.FillEllipse(new SolidBrush(backgroundColor), 7, 0, 6, 6);
                    gimage.DrawEllipse(ThongSo.ConectiveLinePen, 7, 0, 6, 6);
                }
                else
                    gimage.DrawLine(ThongSo.ConectiveLinePen, new Point(3, 5), new Point(17, 5));
            }
            else
            {
                gimage.DrawLine(ThongSo.ConectiveLinePen, new Point(3, 13), new Point(17, 13));
                gimage.DrawLine(ThongSo.ConectiveLinePen, new Point(10, 0), new Point(10, 20));

                if (cardi.MinCardinality == 0)
                {
                    gimage.FillEllipse(new SolidBrush(backgroundColor), 7, 3, 6, 6);
                    gimage.DrawEllipse(ThongSo.ConectiveLinePen, 7, 3, 6, 6);
                }
                else
                    gimage.DrawLine(ThongSo.ConectiveLinePen, new Point(3, 8), new Point(17, 8));
            }
            switch (edgeCardiPlace)
            {
                case 1:
                    break;
                case 2: carshape.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case 3: carshape.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case 4: carshape.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
            }
            return carshape;
        }
    }
}
