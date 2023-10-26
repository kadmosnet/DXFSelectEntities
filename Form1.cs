using System;
using System.Windows.Forms;
using DXFReaderNET;
using DXFReaderNET.Entities;

namespace DXFSelectEntities
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Vector2 panPointStart = Vector2.Zero;

        internal enum FunctionsEnum
        {
            None,
            ZoomWindow1,
            ZoomWindow2,
            GetEntity,
            GetEntities1,
            GetEntities2,
        }
        private FunctionsEnum CurrentFunction = FunctionsEnum.None;
        private Vector2 p1 = Vector2.Zero;
        private Vector2 p2 = Vector2.Zero;

        private void newDrawingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dxfReaderNETControl1.NewDrawing();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Width = Screen.PrimaryScreen.Bounds.Width - 40;
            this.Height = Screen.PrimaryScreen.Bounds.Height - 80;
            this.Left = 20;
            this.Top = 20;

            dxfReaderNETControl1.NewDrawing();
            dxfReaderNETControl1.CustomCursor = CustomCursorType.CrossHair;
            toolStripStatusLabel1.Text = "";
            dxfReaderNETControl1.Dock = DockStyle.Fill;
        }

        private void loadDXFFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.DefaultExt = "DXF";
            openFileDialog1.Filter = "DXF|*.dxf";
            openFileDialog1.FileName = "";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                dxfReaderNETControl1.ReadDXF(openFileDialog1.FileName);

            }
        }

        private void saveDXFFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.DefaultExt = "dxf";
            saveFileDialog1.Filter = "DXF|*.dxf";
            saveFileDialog1.FileName = "drawing.dxf";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                dxfReaderNETControl1.WriteDXF(saveFileDialog1.FileName);

            }
        }


        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void zoomExtentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dxfReaderNETControl1.ZoomExtents();
        }

        private void zoomWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentFunction = FunctionsEnum.ZoomWindow1;
            toolStripStatusLabel1.Text = "Select start point of the window";
        }

        private void aboutDXFReaderNETComponentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dxfReaderNETControl1.About();
        }



        private void dxfReaderNETControl1_MouseUp(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                switch (CurrentFunction)
                {
                    case FunctionsEnum.ZoomWindow2:

                        p2 = dxfReaderNETControl1.CurrentWCSpoint;
                        CurrentFunction = FunctionsEnum.None;
                        dxfReaderNETControl1.CustomCursor = CustomCursorType.CrossHair;

                        toolStripStatusLabel1.Text = "";
                        dxfReaderNETControl1.ZoomWindow(p1, p2);
                        break;
                    case FunctionsEnum.ZoomWindow1:
                        CurrentFunction = FunctionsEnum.ZoomWindow2;
                        toolStripStatusLabel1.Text = "Select end point of the window";
                        p1 = dxfReaderNETControl1.CurrentWCSpoint;
                        break;
                    case FunctionsEnum.GetEntity:
                        toolStripStatusLabel1.Text = "";
                        CurrentFunction = FunctionsEnum.None;

                        EntityObject ent = dxfReaderNETControl1.GetEntity(dxfReaderNETControl1.CurrentWCSpoint);
                        if (ent != null)
                        {
                            toolStripStatusLabel1.Text = EntityInfo(dxfReaderNETControl1, ent);
                            if (!dxfReaderNETControl1.DXF.SelectedEntities.Contains(ent))
                            {
                                dxfReaderNETControl1.DXF.SelectedEntities.Add(ent);

                                dxfReaderNETControl1.HighLight(ent);
                            }

                        }
                        else
                        {
                            toolStripStatusLabel1.Text = "No entity found";

                        }
                        break;
                    case FunctionsEnum.GetEntities2:
                        CurrentFunction = FunctionsEnum.None;
                        toolStripStatusLabel1.Text = "";
                        p2 = dxfReaderNETControl1.CurrentWCSpoint;

                        foreach (EntityObject entity in dxfReaderNETControl1.GetEntities(p1, p2))
                        {
                            if (!dxfReaderNETControl1.DXF.SelectedEntities.Contains(entity))
                            {
                                dxfReaderNETControl1.DXF.SelectedEntities.Add(entity);

                            }
                        }

                        toolStripStatusLabel1.Text = "Selected " + dxfReaderNETControl1.DXF.SelectedEntities.Count.ToString() + " entities";
                        dxfReaderNETControl1.HighLight(dxfReaderNETControl1.DXF.SelectedEntities);

                        break;



                    case FunctionsEnum.GetEntities1:
                        CurrentFunction = FunctionsEnum.GetEntities2;
                        toolStripStatusLabel1.Text = "Select end point of selection rectangle";
                        p1 = dxfReaderNETControl1.CurrentWCSpoint;
                        break;
                }

            }
        }

        private void dxfReaderNETControl1_MouseMove(object sender, MouseEventArgs e)
        {
            switch (CurrentFunction)
            {
                case FunctionsEnum.ZoomWindow2:
                case FunctionsEnum.GetEntities2:
                    dxfReaderNETControl1.ShowRubberBandBox(p1, dxfReaderNETControl1.CurrentWCSpoint);
                    break;
            }
            if (e.Button == MouseButtons.Middle)
            {
                dxfReaderNETControl1.Pan(dxfReaderNETControl1.CurrentWCSpoint, panPointStart);

            }
        }

        private void dxfReaderNETControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                dxfReaderNETControl1.Cursor = Cursors.Hand;
                panPointStart = dxfReaderNETControl1.CurrentWCSpoint;
            }
        }

        private void selectEntityWithAMouseClickToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentFunction = FunctionsEnum.GetEntity;
            dxfReaderNETControl1.CustomCursor = CustomCursorType.CrossHairSquare;
            toolStripStatusLabel1.Text = "Select entity";
        }

        private void selectEntitiesWithRectangleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentFunction = FunctionsEnum.GetEntities1;

            toolStripStatusLabel1.Text = "Select start point of selection rectangle";
        }


        private static string EntityInfo(DXFReaderNETControl myDXF, EntityObject ent)
        {
            string s = "";
            string colorE = ent.Color.ToString();
            if (colorE == "ByLayer") colorE = myDXF.DXF.Layers[ent.Layer.Name].Color.ToString();

            switch (colorE)
            {
                case "1":
                    colorE = "Red";
                    break;
                case "2":
                    colorE = "Yellow";
                    break;
                case "3":
                    colorE = "Green";
                    break;
                case "4":
                    colorE = "Cyan";
                    break;
                case "5":
                    colorE = "Blue";
                    break;
                case "6":
                    colorE = "Magenta";
                    break;
                case "7":
                    colorE = "White";
                    break;
            }



            string ltE = ent.Linetype.Name;
            if (ltE == "ByLayer") ltE = myDXF.DXF.Layers[ent.Layer.Name].Linetype.Name;


            s = ent.Type.ToString() + " " + ent.Handle + " - Color: " + colorE + " - Layer: " + ent.Layer.Name + " - Linetype: " + ltE;

            switch (ent.Type)
            {
                case EntityType.Line:
                    s += " - Length: " + myDXF.DXF.ToFormattedUnit(((Line)ent).Lenght);
                    s += " - Start point: (" + myDXF.DXF.ToFormattedUnit(((Line)ent).StartPoint.X) + ";" + myDXF.DXF.ToFormattedUnit(((Line)ent).StartPoint.Y) + ")";
                    s += " - End point: (" + myDXF.DXF.ToFormattedUnit(((Line)ent).EndPoint.X) + ";" + myDXF.DXF.ToFormattedUnit(((Line)ent).EndPoint.Y) + ")";

                    break;
                case EntityType.XLine:

                    s += " - Origin: (" + myDXF.DXF.ToFormattedUnit(((XLine)ent).Origin.X) + ";" + myDXF.DXF.ToFormattedUnit(((XLine)ent).Origin.Y) + ")";
                    s += " - Direction: (" + myDXF.DXF.ToFormattedUnit(((XLine)ent).Direction.X) + ";" + myDXF.DXF.ToFormattedUnit(((XLine)ent).Direction.Y) + ")";

                    break;
                case EntityType.Ray:

                    s += " - Origin: (" + myDXF.DXF.ToFormattedUnit(((Ray)ent).Origin.X) + ";" + myDXF.DXF.ToFormattedUnit(((Ray)ent).Origin.Y) + ")";
                    s += " - Direction: (" + myDXF.DXF.ToFormattedUnit(((Ray)ent).Direction.X) + ";" + myDXF.DXF.ToFormattedUnit(((Ray)ent).Direction.Y) + ")";

                    break;
                case EntityType.Arc:
                    s += " - Length: " + myDXF.DXF.ToFormattedUnit(((Arc)ent).Lenght);
                    s += " - Start point: (" + myDXF.DXF.ToFormattedUnit(((Arc)ent).StartPoint.X) + ";" + myDXF.DXF.ToFormattedUnit(((Arc)ent).StartPoint.Y) + ")";
                    s += " - End point: (" + myDXF.DXF.ToFormattedUnit(((Arc)ent).EndPoint.X) + ";" + myDXF.DXF.ToFormattedUnit(((Arc)ent).EndPoint.Y) + ")";
                    s += " - Start angle: " + myDXF.DXF.ToFormattedAngle(((Arc)ent).StartAngle * MathHelper.DegToRad);
                    s += " - End angle: " + myDXF.DXF.ToFormattedAngle(((Arc)ent).EndAngle * MathHelper.DegToRad);
                    s += " - Radius: " + myDXF.DXF.ToFormattedUnit(((Arc)ent).Radius);
                    break;
                case EntityType.Circle:
                    s += " - Length: " + myDXF.DXF.ToFormattedUnit(((Circle)ent).Lenght);
                    s += " - Area: " + myDXF.DXF.ToFormattedUnit(((Circle)ent).Area);
                    s += " - Center point: (" + myDXF.DXF.ToFormattedUnit(((Circle)ent).Center.X) + ";" + myDXF.DXF.ToFormattedUnit(((Circle)ent).Center.Y) + ")";
                    s += " - Radius: " + myDXF.DXF.ToFormattedUnit(((Circle)ent).Radius);

                    break;
                case EntityType.Ellipse:
                    s += " - Length: " + myDXF.DXF.ToFormattedUnit(((Ellipse)ent).Lenght);
                    s += " - Area: " + myDXF.DXF.ToFormattedUnit(((Ellipse)ent).Area);
                    s += " - Center point: (" + myDXF.DXF.ToFormattedUnit(((Ellipse)ent).Center.X) + ";" + myDXF.DXF.ToFormattedUnit(((Ellipse)ent).Center.Y) + ")";
                    s += " - MajorAxis: " + myDXF.DXF.ToFormattedUnit(((Ellipse)ent).MajorAxis);
                    s += " - MinorAxis: " + myDXF.DXF.ToFormattedUnit(((Ellipse)ent).MinorAxis);
                    s += " - Rotation: " + myDXF.DXF.ToFormattedAngle(((Ellipse)ent).Rotation * MathHelper.DegToRad);

                    break;
                case EntityType.LightWeightPolyline:
                    s += " - Length: " + myDXF.DXF.ToFormattedUnit(((LwPolyline)ent).Lenght);
                    if (((LwPolyline)ent).IsClosed)
                        s += " - Area: " + myDXF.DXF.ToFormattedUnit(((LwPolyline)ent).Area);

                    s += " - Vertexes #: " + ((LwPolyline)ent).Vertexes.Count.ToString();
                    break;
                case EntityType.Polyline:
                    s += " - Length: " + myDXF.DXF.ToFormattedUnit(((Polyline)ent).Lenght);
                    if (((Polyline)ent).IsClosed)
                        s += " - Area: " + myDXF.DXF.ToFormattedUnit(((Polyline)ent).Area);
                    s += " - Vertexes #: " + ((Polyline)ent).Vertexes.Count.ToString();
                    break;

                case EntityType.Insert:

                    s += " - Insert point: (" + myDXF.DXF.ToFormattedUnit(((Insert)ent).Position.X) + ";" + myDXF.DXF.ToFormattedUnit(((Insert)ent).Position.Y) + ")";
                    s += " - Block name: " + ((Insert)ent).Block.Name;

                    break;
            }


            foreach (DXFReaderNET.Objects.Group _group in myDXF.DXF.Groups)
            {
                if (_group.Entities.Contains(ent))
                {
                    s += " Group: " + _group.Name;
                    break;
                }

            }

            return s;
        }
    }
}

