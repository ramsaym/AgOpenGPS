using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace AgOpenGPS
{
    public partial class FormBoundary : Form
    {
        private readonly FormGPS mf = null;

        private bool MultipleFieldLocked = true;
        private int position = 0;
        private int oldposition = 0;
        private bool scroll = false;
        private double viewableRatio = 0;
        private double contentHeight = 0;
        private int oldY = 0;
        private double thumbHeight = 0;
        private readonly int scrollmaxheight = 0;
        private readonly int startscrollY = 0;
        private readonly int startscrollX = 0;
        private readonly int items = 0;
        private readonly int rowheight = 0;

        public FormBoundary(Form callingForm)
        {
            mf = callingForm as FormGPS;

            //winform initialization
            InitializeComponent();

            scrollmaxheight = button4.Size.Height;
            startscrollY = button4.Location.Y;
            startscrollX = button4.Location.X;
            rowheight = (int)tableLayoutPanel1.RowStyles[0].Height;
            items = (int)(tableLayoutPanel1.Height / rowheight + 0.5) - 1;

            //Column Header
            this.Text = gStr.gsStartDeleteABoundary;
            Boundary.Text = gStr.gsBounds;
            Thru.Text = gStr.gsDriveThru;
            Area.Text = gStr.gsArea;
            Around.Text = gStr.gsAround;

            //buttons
            btnDelete.Text = gStr.gsDelete;
            btnOuter.Text = gStr.gsCreate;
            btnSerialCancel.Text = gStr.gsSaveAndReturn;
            btnLoadMultiBoundaryFromGE.Text = gStr.gsLoadMulti;
            btnDeleteAll.Text = gStr.gsDeleteAll;
            btnGo.Text = gStr.gsGo;
            btnLoadBoundaryFromGE.Text = gStr.gsLoadKML;


            //Bounds.Text = gStr.gsSelectBoundary;
            //Thru.Text = gStr.gsThru;
            //Around.Text = gStr.gsLines;
            //label6.Text = gStr.gsGo_Around;
            // btnToggleDriveThru.Text = gStr.gsToggleDriveThru;         
            // btnToggleDriveAround.Text = gStr.gsToggleDriveBy;
        }

        void UpdateScroll(double pos)
        {
            contentHeight = (mf.bnd.bndArr.Count + 1) * rowheight;
            viewableRatio = tableLayoutPanel1.Size.Height / contentHeight;
            if (viewableRatio >= 1)
            {
                button4.Size = new Size(rowheight, scrollmaxheight);
                button4.Location = new Point(startscrollX, startscrollY);
            }
            else
            {
                thumbHeight = (scrollmaxheight * viewableRatio < rowheight * 2) ? rowheight * 2 : (scrollmaxheight * viewableRatio);
                button4.Size = new Size(rowheight, (int)(thumbHeight + 0.5));
                if (pos < 0)
                {
                    button4.Location = new Point(startscrollX, (int)(startscrollY + position * ((scrollmaxheight - thumbHeight) / (mf.bnd.bndArr.Count - items)) + 0.5));
                }
                else
                {
                    button4.Location = new Point(startscrollX, (int)(startscrollY + pos));
                }
            }
        }

        private void UpdateChart()
        {
            int field = 1;
            int inner = 1;

            for (int i = 0; i < mf.bnd.bndArr.Count + 1 && i < position + 9; i++)
            {
                if (i < position && i < mf.bnd.bndArr.Count)
                {
                    if (mf.bnd.bndArr[i].isOwnField)
                    {
                        //field += 1;
                    }
                    else
                    {
                        //inner += 1;
                    }
                }
                else
                {
                    Control aa = tableLayoutPanel1.GetControlFromPosition(0, i - position);
                    if (aa == null)
                    {
                        var a = new Button
                        {
                            Margin = new Padding(0),
                            Size = new Size(280, 40),
                            Name = string.Format("{0}", i - position),
                            TextAlign = ContentAlignment.MiddleCenter
                        };
                        a.Click += B_Click;
                        a.FlatStyle = FlatStyle.Flat;
                        a.FlatAppearance.BorderColor = BackColor;
                        a.FlatAppearance.MouseOverBackColor = BackColor;
                        a.FlatAppearance.MouseDownBackColor = BackColor;
                        tableLayoutPanel1.Controls.Add(a, 0, i - position);
                        aa = a;
                    }


                    if (i < mf.bnd.bndArr.Count)
                    {
                        tableLayoutPanel1.SetColumnSpan(aa, 1);

                        Control bb = tableLayoutPanel1.GetControlFromPosition(1, i - position);
                        Control cc = tableLayoutPanel1.GetControlFromPosition(2, i - position);
                        Control dd = tableLayoutPanel1.GetControlFromPosition(3, i - position);
                        Control ee = tableLayoutPanel1.GetControlFromPosition(4, i - position);

                        if (bb == null)
                        {
                            var b = new Button
                            {
                                Margin = new Padding(0),
                                Size = new System.Drawing.Size(150, 40),
                                Name = string.Format("{0}", i - position),
                                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
                            };
                            b.Click += B_Click;
                            b.FlatStyle = FlatStyle.Flat;
                            b.FlatAppearance.BorderColor = BackColor;
                            b.FlatAppearance.MouseOverBackColor = BackColor;
                            b.FlatAppearance.MouseDownBackColor = BackColor;
                            tableLayoutPanel1.Controls.Add(b, 1, i - position);
                            bb = b;

                            var c = new Button();
                            c.Margin = new Padding(0);
                            c.Size = new System.Drawing.Size(95, 40);
                            c.Name = string.Format("{0}", i - position);
                            c.TextAlign = ContentAlignment.MiddleCenter;
                            c.Click += OwnField_Click;
                            tableLayoutPanel1.Controls.Add(c, 2, i - position);
                            cc = c;

                            var d = new Button
                            {
                                Margin = new Padding(0),
                                Size = new System.Drawing.Size(160, 40),
                                Name = string.Format("{0}", i - position),
                                TextAlign = ContentAlignment.MiddleCenter
                            };
                            d.Click += DriveThru_Click;
                            tableLayoutPanel1.Controls.Add(d, 3, i - position);
                            dd = d;

                            if (ee == null)
                            {
                                var e = new Button
                                {
                                    Margin = new Padding(0),
                                    Size = new System.Drawing.Size(80, 40),
                                    Name = string.Format("{0}", i - position),
                                    TextAlign = ContentAlignment.MiddleCenter
                                };
                                e.Click += DriveAround_Click;
                                tableLayoutPanel1.Controls.Add(e, 4, i - position);
                                ee = e;
                            }
                        }

                        if (mf.bnd.boundarySelected >= 0 && i == mf.bnd.boundarySelected)
                        {
                            aa.ForeColor = Color.Red;
                            bb.ForeColor = Color.Red;
                        }
                        else
                        {
                            aa.ForeColor = default;
                            bb.ForeColor = default;
                        }

                        if (mf.isMetric)
                        {
                            bb.Text = Math.Round(mf.bnd.bndArr[i].area * 0.0001, 2) + " Ha";
                        }
                        else
                        {
                            bb.Text = Math.Round(mf.bnd.bndArr[i].area * 0.000247105, 2) + " Ac";
                        }

                        if (mf.bnd.bndArr[i].isOwnField)
                        {
                            tableLayoutPanel1.SetColumnSpan(dd, 2);


                            cc.Text = "Field";
                            aa.Text = string.Format("Bound {0}", i + 1);
                            //aa.Text = string.Format(gStr.gsOuter + " {0}", field);
                            //field += 1;
                            mf.bnd.bndArr[i].isDriveThru = false;
                            mf.bnd.bndArr[i].isDriveAround = false;
                            dd.Text = "Select!!!";


                            if (mf.bnd.boundarySelected >= 0 && mf.bnd.boundarySelected < mf.bnd.bndArr.Count && !mf.bnd.bndArr[mf.bnd.boundarySelected].isOwnField) dd.Enabled = true;
                            else dd.Enabled = false;
                        }
                        else
                        {
                            tableLayoutPanel1.SetColumnSpan(dd, 1);
                            aa.Text = string.Format("Bound {0}", i + 1);
                            //aa.Text = string.Format(gStr.gsInner + " {0}", inner);
                            //inner += 1;
                            cc.Text = "Inner";
                            cc.Text = (mf.bnd.bndArr[i].OuterField+1).ToString();
                            dd.Enabled = true;
                            ee.Visible = true;
                            dd.Text = mf.bnd.bndArr[i].isDriveThru ? gStr.gsYes : gStr.gsNo;
                            ee.Text = mf.bnd.bndArr[i].isDriveAround ? gStr.gsYes : gStr.gsNo;
                        }
                    }
                    else
                    {
                        tableLayoutPanel1.SetColumnSpan(aa, 2);
                        while (true)
                        {
                            Control ee = tableLayoutPanel1.GetNextControl(aa, true);
                            if (ee == null)
                            {
                                break;
                            }
                            else
                            {
                                ee.Dispose();
                            }
                        }


                        aa.Text = string.Format("Create Boundary", i - position + 1);

                        if (mf.bnd.boundarySelected >= 0 && i == mf.bnd.boundarySelected)
                        {
                            aa.ForeColor = Color.Red;
                        }
                        else
                        {
                            aa.ForeColor = default;
                        }
                        //break;
                    }
                }
            }
        }

        private void FormBoundary_Load(object sender, EventArgs e)
        {
            btnLeftRight.Image = mf.bnd.isDrawRightSide ? Properties.Resources.BoundaryRight : Properties.Resources.BoundaryLeft;
            btnLeftRight.Enabled = false;
            btnOuter.Enabled = false;
            btnLoadBoundaryFromGE.Enabled = false;
            btnGo.Enabled = false;
            btnDelete.Enabled = false;

            //update the list view with real data
            UpdateChart();
            UpdateScroll(-1);
        }

        private void btnOuter_Click(object sender, EventArgs e)
        {
            btnLeftRight.Enabled = true;
            btnLoadBoundaryFromGE.Enabled = false;
            btnLoadMultiBoundaryFromGE.Enabled = false;
            btnOuter.Enabled = false;
            btnGo.Enabled = true;

            UpdateChart();
        }

        void DriveThru_Click(object sender, EventArgs e)
        {
            if (sender is Button b)
            {
                if (mf.bnd.bndArr[Convert.ToInt32(b.Name) + position].isOwnField)
                {
                    mf.bnd.bndArr[mf.bnd.boundarySelected].OuterField = Convert.ToInt32(b.Name) + position;

                }
                else mf.bnd.bndArr[Convert.ToInt32(b.Name) + position].isDriveThru = !mf.bnd.bndArr[Convert.ToInt32(b.Name) + position].isDriveThru;
                UpdateChart();
            }
        }

        void OwnField_Click(object sender, EventArgs e)
        {
            if (sender is Button b)
            {
                mf.bnd.bndArr[Convert.ToInt32(b.Name) + position].isOwnField = !mf.bnd.bndArr[Convert.ToInt32(b.Name) + position].isOwnField;

                mf.bnd.bndArr[Convert.ToInt32(b.Name) + position].OuterField = -1;
                UpdateChart();
            }
        }
        
        void DriveAround_Click(object sender, EventArgs e)
        {
            if (sender is Button b)
            {
                mf.bnd.bndArr[Convert.ToInt32(b.Name) + position].isDriveAround = !mf.bnd.bndArr[Convert.ToInt32(b.Name) + position].isDriveAround;
                UpdateChart();
            }
        }

        void B_Click(object sender, EventArgs e)
        {
            if (sender is Button b)
            {

                mf.bnd.boundarySelected = Convert.ToInt32(b.Name) + position;

                if (mf.bnd.bndArr.Count > mf.bnd.boundarySelected && mf.bnd.boundarySelected >= 0)
                {
                    btnOuter.Enabled = false;
                    btnLoadBoundaryFromGE.Enabled = false;
                    btnGo.Enabled = false;
                    btnDelete.Enabled = true;
                    btnLeftRight.Enabled = false;

                }
                else if (mf.bnd.boundarySelected > 0)
                {
                    btnOuter.Enabled = true;
                    btnLoadBoundaryFromGE.Enabled = true;
                    btnGo.Enabled = false;
                    btnDelete.Enabled = false;
                    btnLeftRight.Enabled = false;
                }
                else mf.bnd.boundarySelected = -1;

            }
            UpdateChart();
        }

        private void btnSerialCancel_Click(object sender, EventArgs e)
        {
            mf.bnd.isOkToAddPoints = false;
            mf.turn.BuildTurnLines();
            mf.gf.BuildGeoFenceLines();
            mf.mazeGrid.BuildMazeGridArray();
        }

        private void btnLeftRight_Click(object sender, EventArgs e)
        {
            mf.bnd.isDrawRightSide = !mf.bnd.isDrawRightSide;
            btnLeftRight.Image = mf.bnd.isDrawRightSide ? Properties.Resources.BoundaryRight : Properties.Resources.BoundaryLeft;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {


            btnLeftRight.Enabled = false;
            btnOuter.Enabled = false;
            btnLoadBoundaryFromGE.Enabled = false;
            btnGo.Enabled = false;
            btnDelete.Enabled = false;

            if (mf.bnd.bndArr.Count > mf.bnd.boundarySelected && mf.bnd.boundarySelected >= 0)
            {
                mf.bnd.bndArr.RemoveAt(mf.bnd.boundarySelected);
                mf.turn.turnArr.RemoveAt(mf.bnd.boundarySelected);
                mf.gf.geoFenceArr.RemoveAt(mf.bnd.boundarySelected);
            }

            mf.FileSaveBoundary();
            mf.bnd.boundarySelected = -1;
            if (position + items >= mf.bnd.bndArr.Count) position--;
            if (position < 0) position = 0;
            UpdateChart();

            UpdateScroll(-1);
        }

        private double easting, northing, latK, lonK;

        private void ResetAllBoundary()
        {
            position = 0;

            mf.bnd.bndArr.Clear();
            mf.turn.turnArr.Clear();
            mf.gf.geoFenceArr.Clear();

            mf.FileSaveBoundary();
            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.RowStyles.Clear();

            UpdateChart();
            UpdateScroll(-1);

            btnLeftRight.Enabled = false;
            btnOuter.Enabled = false;
            btnLoadBoundaryFromGE.Enabled = false;
            btnGo.Enabled = false;
            btnDelete.Enabled = false;
        }

        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            ResetAllBoundary();

            mf.bnd.boundarySelected = -1;

            mf.bnd.isOkToAddPoints = false;
            mf.turn.BuildTurnLines();
            mf.gf.BuildGeoFenceLines();
            mf.mazeGrid.BuildMazeGridArray();
        }

        private void Down_Scroll_Click(object sender, EventArgs e)
        {
            if (position + items < mf.bnd.bndArr.Count) position++;
            UpdateChart();
            UpdateScroll(-1);
        }

        private void Up_Scroll_Click(object sender, EventArgs e)
        {
            if (position > 0) position--;
            UpdateChart();
            UpdateScroll(-1);
        }
        void MouseWheel_Scroll(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                if (position > 0) position -= (e.Delta / 120);
            }
            else
            {
                if (position + items < mf.bnd.bndArr.Count) position -= (e.Delta / 120);
            }
            UpdateChart();
            UpdateScroll(-1);
        }

        void Mouse_Down(object sender, MouseEventArgs e)
        {
            oldY = MousePosition.Y;
            scroll = true;
        }

        void Mouse_Up(object sender, MouseEventArgs e)
        {
            scroll = false;
        }

        void Mouse_Leave(object sender, EventArgs e)
        {
            scroll = false;
        }

        private void Field_Click(object sender, EventArgs e)
        {
            MultipleFieldLocked = !MultipleFieldLocked;
            UpdateChart();
        }

        void Mouse_Move(object sender, MouseEventArgs e)
        {
            if (scroll == true && viewableRatio < 1)
            {
                if (!(oldY == MousePosition.Y))
                {
                    if (button4.Location.Y + (MousePosition.Y - oldY) > startscrollY)
                    {
                        if ((button4.Location.Y + (MousePosition.Y - oldY)) < (startscrollY + scrollmaxheight - thumbHeight))
                        {

                            position = (int)(((button4.Location.Y + MousePosition.Y - oldY) - startscrollY) / ((scrollmaxheight - thumbHeight) / (mf.bnd.bndArr.Count - items)) + 0.5);



                            UpdateScroll((button4.Location.Y + (MousePosition.Y - oldY) - startscrollY));
                        }
                        else
                        {
                            position = mf.bnd.bndArr.Count - items;
                            UpdateScroll(scrollmaxheight - thumbHeight);
                        }
                    }
                    else
                    {
                        position = 0;
                        UpdateScroll(0);
                    }
                    if (position != oldposition)
                    {
                        oldposition = position;
                        UpdateChart();
                    }
                    oldY = MousePosition.Y;
                }
            }
        }

        private void btnLoadBoundaryFromGE_Click(object sender, EventArgs e)
        {

            if (sender is Button button)
            {
                btnLeftRight.Enabled = false;
                btnOuter.Enabled = false;
                btnLoadBoundaryFromGE.Enabled = false;
                btnGo.Enabled = false;
                btnDelete.Enabled = false;

                string fileAndDirectory;
                {
                    //create the dialog instance
                    OpenFileDialog ofd = new OpenFileDialog
                    {
                        //set the filter to text KML only
                        Filter = "KML files (*.KML)|*.KML",

                        //the initial directory, fields, for the open dialog
                        InitialDirectory = mf.fieldsDirectory + mf.currentFieldDirectory
                    };

                    //was a file selected
                    if (ofd.ShowDialog() == DialogResult.Cancel) return;
                    else fileAndDirectory = ofd.FileName;
                }

                //start to read the file
                string line = null;
                string coordinates = null;
                int startIndex;
                int i = 0;

                using (System.IO.StreamReader reader = new System.IO.StreamReader(fileAndDirectory))
                {

                    if (button.Name == "btnLoadMultiBoundaryFromGE") ResetAllBoundary();
                    else i = mf.bnd.boundarySelected;

                    try
                    {
                        while (!reader.EndOfStream)
                        {
                            line = reader.ReadLine();

                            startIndex = line.IndexOf("<coordinates>");

                            if (startIndex != -1)
                            {
                                while (true)
                                {
                                    int endIndex = line.IndexOf("</coordinates>");

                                    if (endIndex == -1)
                                    {
                                        //just add the line
                                        if (startIndex == -1) coordinates = coordinates + line.Substring(0);
                                        else coordinates = coordinates + line.Substring(startIndex + 13);
                                    }
                                    else
                                    {
                                        if (startIndex == -1) coordinates = coordinates + line.Substring(0, endIndex);
                                        else coordinates = coordinates + line.Substring(startIndex + 13, endIndex - (startIndex + 13));
                                        break;
                                    }
                                    line = reader.ReadLine();
                                    line = line.Trim();
                                    startIndex = -1;
                                }

                                line = coordinates;
                                char[] delimiterChars = { ' ', '\t', '\r', '\n' };
                                string[] numberSets = line.Split(delimiterChars);

                                //at least 3 points
                                if (numberSets.Length > 2)
                                {
                                    mf.bnd.bndArr.Add(new CBoundaryLines());
                                    mf.turn.turnArr.Add(new CTurnLines());
                                    mf.gf.geoFenceArr.Add(new CGeoFenceLines());

                                    foreach (var item in numberSets)
                                    {
                                        string[] fix = item.Split(',');
                                        double.TryParse(fix[0], NumberStyles.Float, CultureInfo.InvariantCulture, out lonK);
                                        double.TryParse(fix[1], NumberStyles.Float, CultureInfo.InvariantCulture, out latK);
                                        double[] xy = mf.pn.DecDeg2UTM(latK, lonK);

                                        //match new fix to current position
                                        easting = xy[0] - mf.pn.utmEast;
                                        northing = xy[1] - mf.pn.utmNorth;

                                        //fix the azimuth error
                                        easting = (Math.Cos(-mf.pn.convergenceAngle) * easting) - (Math.Sin(-mf.pn.convergenceAngle) * northing);
                                        northing = (Math.Sin(-mf.pn.convergenceAngle) * easting) + (Math.Cos(-mf.pn.convergenceAngle) * northing);

                                        //add the point to boundary
                                        CBndPt bndPt = new CBndPt(easting, northing, 0);
                                        mf.bnd.bndArr[i].bndLine.Add(bndPt);
                                    }

                                    //fix the points if there are gaps bigger then
                                    mf.bnd.bndArr[i].CalculateBoundaryHeadings();
                                    mf.bnd.bndArr[i].PreCalcBoundaryLines();
                                    mf.bnd.bndArr[i].FixBoundaryLine(i, mf.vehicle.toolWidth);

                                    //boundary area, pre calcs etc
                                    mf.bnd.bndArr[i].CalculateBoundaryArea();
                                    mf.bnd.bndArr[i].PreCalcBoundaryLines();
                                    mf.bnd.bndArr[i].isSet = true;
                                    if (i == 0) mf.bnd.bndArr[i].isOwnField = true;
                                    else mf.bnd.bndArr[i].isOwnField = false;
                                    coordinates = "";
                                    i++;
                                }
                                else
                                {
                                    mf.TimedMessageBox(2000, gStr.gsErrorreadingKML, gStr.gsChooseBuildDifferentone);
                                }
                                if (button.Name == "btnLoadBoundaryFromGE")
                                {
                                    break;
                                }
                            }
                        }
                        mf.FileSaveBoundary();
                        UpdateChart();
                        UpdateScroll(-1);
                    }
                    catch (Exception)
                    {
                        return;
                    }
                }
            }
        }

        private void btnOpenGoogleEarth_Click(object sender, EventArgs e)
        {
            //save new copy of kml with selected flag and view in GoogleEarth
            mf.FileMakeCurrentKML(mf.pn.latitude, mf.pn.longitude);
            System.Diagnostics.Process.Start(mf.fieldsDirectory + mf.currentFieldDirectory + "\\CurrentPosition.KML");
        }
    }
}
