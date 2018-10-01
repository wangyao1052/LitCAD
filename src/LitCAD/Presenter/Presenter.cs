using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using LitCAD.DatabaseServices;
using LitCAD.ApplicationServices;
using LitCAD.Commands;
using LitCAD.UI;

namespace LitCAD
{
    internal class Presenter : IPresenter
    {
        private ICanvas _canvas = null;
        private Document _document = null;

        //
        private CommandsMgr _cmdsMgr = null;
        private EntityDrawerMgr _entityDrawerMgr = null;

        private Pointer _pointer = null;
        internal Pointer pointer
        {
            get { return _pointer; }
        }

        private DynamicInputer _dynamicInputer = null;
        internal DynamicInputer dynamicInputer
        {
            get { return _dynamicInputer; }
        }

        // 原点
        private Origin _origin;

        //
        public Document document
        {
            get { return _document; }
        }

        public Selections selections
        {
            get { return _document.selections; }
        }

        public ICanvas canvas
        {
            get { return _canvas; }
        }

        internal bool canUndo
        {
            get { return _cmdsMgr.canUndo; }
        }

        internal bool canRedo
        {
            get { return _cmdsMgr.canRedo; }
        }

        //
        private Bitmap _bufferBitmap = null;
        private bool _bufferBitmapToRedraw = true;

        //
        private LitMath.Vector2 _screenPan = new LitMath.Vector2();
        private LitMath.Vector2 _screenDrag = new LitMath.Vector2();
        private LitMath.Vector2 screenPan
        {
            get { return _screenPan + _screenDrag; }
        }
        private float _resolution = 96.0f;
        private double _zoom = 1.0f;
        private double _zoomMin = 1e-4;
        private double _zoomMax = 1e4;

        /// <summary>
        /// Current block
        /// </summary>
        public Block currentBlock
        {
            get
            {
                return _document.database.blockTable[_document.currentBlockName] as Block;
            }
        }
        
        public Presenter(ICanvas canvas, Document doc)
        {
            _canvas = canvas;
            _document = (Document)doc;
            _canvas.SetPresenter(this);

            doc.selections.changed += this.OnSelectionChanged;

            _cmdsMgr = new CommandsMgr(this);
            _cmdsMgr.commandFinished += this.OnCommandFinished;
            _cmdsMgr.commandCanceled += this.OnCommandCanceled;

            _entityDrawerMgr = new EntityDrawerMgr(this);

            _pointer = new Pointer(this);

            _dynamicInputer = new DynamicInputer(this);
            _dynamicInputer.cmdInput.finish += this.OnCmdInputResurn;
            _dynamicInputer.cmdInput.cancel += this.OnCmdInputResurn;

            _origin = new Origin(this);

            TestData();
        }

        private void TestData()
        {
            Block modelSpace = _document.database.blockTable["ModelSpace"] as Block;

            Line line = new Line();
            line.startPoint = new LitMath.Vector2(0, 0);
            line.endPoint = new LitMath.Vector2(100, 100);
            line.color = LitCAD.Colors.Color.FromColor(Color.Green); ;
            modelSpace.AppendEntity(line);

            Circle circle = new Circle();
            circle.center = new LitMath.Vector2(0, 0);
            circle.radius = 20;
            circle.color = LitCAD.Colors.Color.FromColor(Color.Blue);
            modelSpace.AppendEntity(circle);

            Polyline polyline = new Polyline();
            polyline.color = LitCAD.Colors.Color.FromColor(Color.Yellow);
            polyline.AddVertexAt(polyline.NumberOfVertices, new LitMath.Vector2(0, 0));
            polyline.AddVertexAt(polyline.NumberOfVertices, new LitMath.Vector2(10, 20));
            polyline.AddVertexAt(polyline.NumberOfVertices, new LitMath.Vector2(20, 5));
            //polyline.SetPointAt(polyline.NumberOfVertices - 1, new LitMath.Vector2(100, 100));
            modelSpace.AppendEntity(polyline);

            Arc arc = new Arc();
            arc.color = LitCAD.Colors.Color.FromColor(Color.Blue);
            arc.center = new LitMath.Vector2(20, 20);
            arc.radius = 6;
            arc.startAngle = Math.PI / 4;
            arc.endAngle = Math.PI * 1.4f;
            modelSpace.AppendEntity(arc);

            Xline xline = new Xline();
            xline.basePoint = new LitMath.Vector2(0, 50);
            xline.direction = new LitMath.Vector2(-1, -1);
            modelSpace.AppendEntity(xline);

            Ray ray = new Ray();
            ray.basePoint = new LitMath.Vector2(-10, 20);
            ray.direction = new LitMath.Vector2(2, -5);
            modelSpace.AppendEntity(ray);

            Text text = new Text();
            text.color = Colors.Color.FromRGB(255, 0, 0);
            text.text = "gabc 北京\n efg";
            text.height = 5;
            text.font = "Arial";
            text.position = new LitMath.Vector2(0, 0);
            text.alignment = TextAlignment.RightBottom;
            modelSpace.AppendEntity(text);

            //Text text2 = new Text();
            //text2.text = "我爱北京天安门\nhello kitty";
            //text2.height = 5;
            //text2.font = "1234bbb";
            //text2.position = new LitMath.Vector2(100, 100);
            //modelSpace.AppendEntity(text2);
        }

        public void DrawEntity(Graphics graphics, Entity entity, Pen pen = null)
        {
            _entityDrawerMgr.DrawEntity(graphics, entity, pen);
        }

        public void AppendEntity(Entity entity)
        {
            Block modelSpace = _document.database.blockTable["ModelSpace"] as Block;
            modelSpace.AppendEntity(entity);
        }

        /// <summary>
        /// 绘制画布
        /// </summary>
        public void OnPaintCanvas(PaintEventArgs e)
        {
            int canvasWidth = (int)_canvas.width;
            int canvasHeight = (int)_canvas.height;
            Rectangle clipRectangle = e.ClipRectangle;
            Rectangle canvasRectangle = new Rectangle(0, 0, canvasWidth, canvasHeight);

            if (_bufferBitmap == null)
            {
                clipRectangle = canvasRectangle;
                _bufferBitmap = new Bitmap(canvasWidth, canvasHeight);
                _bufferBitmapToRedraw = true;
            }

            if (_bufferBitmapToRedraw)
            {
                _bufferBitmapToRedraw = false;

                Graphics graphics = Graphics.FromImage(_bufferBitmap);
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // 绘制背景
                graphics.Clear(Color.FromArgb(33, 40, 48));

                // 绘制数据库图元对象
                Block modelSpace = _document.database.blockTable[_document.currentBlockName] as Block;
                foreach (Entity entity in modelSpace)
                {
                    if (_document.selections.IsObjectSelected(entity.id))
                    {
                        this.DrawEntity(graphics, entity, GDIResMgr.Instance.GetEntitySelectedPen(entity));
                    }
                    else
                    {
                        this.DrawEntity(graphics, entity);
                    }
                }
            }
            
            // 双缓冲:将图片绘制到画布
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
            e.Graphics.DrawImage(_bufferBitmap, clipRectangle, clipRectangle, GraphicsUnit.Pixel);

            // 命令管理器绘制
            _cmdsMgr.OnPaint(e.Graphics);

            // 原点
            _origin.OnPaint(e.Graphics);

            // 绘制Pointer
            _pointer.OnPaint(e.Graphics);

            return;

            //LitMath.Vector2 origin = this.ModelToCanvas(new LitMath.Vector2(0, 0));
            //e.Graphics.DrawArc(GDIResMgr.Instance.GetPen(Color.Red, 0),
            //    (float)origin.x - 50, (float)origin.y - 50, 100, 100,
            //    45, -45);

            string text = "1234567890\n北京\nabcdefg";
            FontStyle fontStyle = FontStyle.Bold | FontStyle.Italic | FontStyle.Strikeout | FontStyle.Underline;
            Font font = new Font("Arial", 16, fontStyle);
            SolidBrush brush = new SolidBrush(Color.White);
            PointF position = new PointF(500, 500);

            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Near;
            format.LineAlignment = StringAlignment.Near;
            //format.FormatFlags = StringFormatFlags.DirectionRightToLeft;

            e.Graphics.DrawString(text, font, brush, position, format);

            //brush = new SolidBrush(Color.Green);
            //RectangleF rect = new RectangleF(position, new SizeF(200, 65));
            //e.Graphics.DrawString(text, font, brush, rect, format);
            //e.Graphics.DrawRectangle(new Pen(Color.White, 1), position.X, position.Y, rect.Width, rect.Height);

            SizeF size = e.Graphics.MeasureString(text, font, position, format);

            PointF pos1 = position;
            PointF pos2 = new PointF(pos1.X + size.Width, pos1.Y);
            PointF pos3 = new PointF(pos2.X, pos2.Y + size.Height);
            PointF pos4 = new PointF(pos3.X - size.Width, pos3.Y);
            e.Graphics.DrawLine(GDIResMgr.Instance.GetPen(Color.Red, 0),
                pos1, pos2);
            e.Graphics.DrawLine(GDIResMgr.Instance.GetPen(Color.Red, 0),
                pos2, pos3);
            e.Graphics.DrawLine(GDIResMgr.Instance.GetPen(Color.Red, 0),
                pos3, pos4);
            e.Graphics.DrawLine(GDIResMgr.Instance.GetPen(Color.Red, 0),
                pos4, pos1);

            e.Graphics.DrawLine(GDIResMgr.Instance.GetPen(Color.Green, 0),
                position, new PointF(1000, 0));
        }

        internal void RepaintCanvas(bool bufferBitmapToRedraw = false)
        {
            if (bufferBitmapToRedraw)
                _bufferBitmapToRedraw = true;
            _canvas.Repaint();
        }

        public void OnResize(EventArgs e)
        {
            _bufferBitmap = null;
            RepaintCanvas(true);
        }

        public void OnMouseDown(MouseEventArgs e)
        {
            Commands.Command cmd = _pointer.OnMouseDown(e);

            _mouseDownPoint.x = e.X;
            _mouseDownPoint.y = e.Y;

            if (_cmdsMgr.CurrentCmd != null)
            {
                _cmdsMgr.OnMouseDown(e);
                RepaintCanvas(true);
            }
            else
            {
                if (cmd != null)
                {
                    _cmdsMgr.DoCommand(cmd);
                    RepaintCanvas();
                }
            }
        }

        private LitMath.Vector2 _mouseDownPoint = new LitMath.Vector2();
        public void OnMouseUp(MouseEventArgs e)
        {
            _pointer.OnMouseUp(e);

            if (e.Button == MouseButtons.Middle)
            {
                _screenPan += _screenDrag;
                _screenDrag.x = 0;
                _screenDrag.y = 0;

                RepaintCanvas(true);

                return;
            }

            if (_cmdsMgr.CurrentCmd != null)
            {
                _cmdsMgr.OnMouseUp(e);
                RepaintCanvas();
            }
        }

        public void OnMouseMove(MouseEventArgs e)
        {
            _pointer.OnMouseMove(e);
            _dynamicInputer.OnMouseMove(e);

            if (e.Button == MouseButtons.Middle)
            {
                LitMath.Vector2 ePoint = new LitMath.Vector2(e.X, e.Y);
                _screenDrag = ePoint - _mouseDownPoint;
                RepaintCanvas(true);
                return;
            }

            if (_cmdsMgr.CurrentCmd != null)
            {
                _cmdsMgr.OnMouseMove(e);
                RepaintCanvas();
            }
        }

        public void OnMouseDoubleClick(MouseEventArgs e)
        {
            _pointer.OnMouseDoubleClick(e);
        }

        public void OnMouseWheel(MouseEventArgs e)
        {
            LitMath.Vector2 mousePosInCanvas = _canvas.GetMousePosition();
            LitMath.Vector2 mousePosInModel = this.CanvasToModel(mousePosInCanvas);

            float zoomDelta = 1.25f * (float)Math.Abs(e.Delta) / 120.0f;
            if (e.Delta < 0)
            {
                if (_zoom >= _zoomMin)
                {
                    _zoom = _zoom / zoomDelta;
                }
            }
            else
            {
                if (_zoom <= _zoomMax)
                {
                    _zoom = _zoom * zoomDelta;
                }
            }

            MoveModelPositionToCanvasPosition(mousePosInModel, mousePosInCanvas);

            RepaintCanvas(true);
        }

        public void OnKeyDown(KeyEventArgs e)
        {
            if (_cmdsMgr.CurrentCmd != null)
            {
                _cmdsMgr.OnKeyDown(e);
            }
            else
            {
                if (_dynamicInputer.StartCmd(e))
                {
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    _document.selections.Clear();
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    if (document.selections.Count > 0)
                    {
                        Commands.Modify.DeleteCmd cmd = new Commands.Modify.DeleteCmd();
                        this.OnCommand(cmd);
                    }
                }
            }
        }

        private CommandsFactory _cmdsFactory = new CommandsFactory();
        private void OnCmdInputResurn(DynInputCtrl sender, DynInputResult result)
        {
            switch (result.status)
            {
                case DynInputStatus.OK:
                    {
                        DynInputResult<string> cmdInputRet = result as DynInputResult<string>;
                        Command cmd = _cmdsFactory.NewCommand(cmdInputRet.value.ToLower());
                        if (cmd != null)
                        {
                            this.OnCommand(cmd);
                        }
                    }
                    break;

                case DynInputStatus.Cancel:
                    break;

                case DynInputStatus.Error:
                    break;

                default:
                    break;
            }
        }

        public void OnKeyUp(KeyEventArgs e)
        {
            if (_cmdsMgr != null)
            {
                _cmdsMgr.OnKeyUp(e);
            }
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        public void OnCommand(Commands.Command cmd)
        {
            _cmdsMgr.DoCommand(cmd);
        }

        /// <summary>
        /// 命令完成
        /// </summary>
        public void OnCommandFinished(Commands.Command cmd)
        {
            this.RepaintCanvas(true);
        }

        /// <summary>
        /// 命令取消
        /// </summary>
        public void OnCommandCanceled(Commands.Command cmd)
        {
            this.RepaintCanvas(false);
        }

        /// <summary>
        /// 选择集变更
        /// </summary>
        public void OnSelectionChanged()
        {
            _pointer.OnSelectionChanged();
            this.RepaintCanvas(true);
        }

        /// <summary>
        /// 绘制线段
        /// </summary>
        public void DrawLine(
            Graphics graphics, Pen pen,
            LitMath.Vector2 p1, LitMath.Vector2 p2,
            CSYS csys = CSYS.Model)
        {
            if (csys == CSYS.Model)
            {
                LitMath.Vector2 startInCanvas = ModelToCanvas(p1);
                LitMath.Vector2 endInCanvas = ModelToCanvas(p2);
                graphics.DrawLine(pen,
                    (float)startInCanvas.x, (float)startInCanvas.y,
                    (float)endInCanvas.x, (float)endInCanvas.y);
            }
            else
            {
                graphics.DrawLine(pen,
                    (float)p1.x, (float)p1.y,
                    (float)p2.x, (float)p2.y);
            }
        }

        /// <summary>
        /// 绘制圆
        /// </summary>
        public void DrawCircle(
            Graphics graphics, Pen pen,
            LitMath.Vector2 center, double radius,
            CSYS csys = CSYS.Model)
        {
            if (csys == CSYS.Model)
            {
                LitMath.Vector2 centerInCanvas = ModelToCanvas(center);
                double radiusInCanvas = ModelToCanvas(radius);
                graphics.DrawEllipse(pen,
                    (float)(centerInCanvas.x - radiusInCanvas), (float)(centerInCanvas.y - radiusInCanvas),
                    (float)radiusInCanvas * 2, (float)radiusInCanvas * 2);
            }
            else
            {
                graphics.DrawEllipse(pen,
                    (float)(center.x - radius), (float)(center.y - radius),
                    (float)radius * 2, (float)radius * 2);
            }
        }

        /// <summary>
        /// 绘制圆弧
        /// </summary>
        public void DrawArc(
            Graphics graphics, Pen pen,
            LitMath.Vector2 center, double radius,
            double startAngle, double endAngle,
            CSYS csys = CSYS.Model)
        {
            if (csys == CSYS.Model)
            {
                LitMath.Vector2 centerInCanvas = ModelToCanvas(center);
                double radiusInCanvas = ModelToCanvas(radius);
                double startAngleInCanvas = 360 - endAngle;
                double endAngleInCanvas = 360 - startAngle;

                DrawArcInCanvas(graphics, pen,
                    centerInCanvas, radiusInCanvas, startAngleInCanvas, endAngleInCanvas);
            }
            else
            {
                DrawArcInCanvas(graphics, pen,
                    center, radius, startAngle, startAngle);
            }
        }

        private void DrawArcInCanvas(Graphics graphics, Pen pen,
            LitMath.Vector2 center, double radius,
            double startAngle, double endAngle)
        {
            if (endAngle < startAngle)
                endAngle += 360;

            graphics.DrawArc(pen,
                (float)(center.x - radius), (float)(center.y - radius),
                (float)radius * 2, (float)radius * 2,
                (float)startAngle, (float)(endAngle - startAngle));
        }

        /// <summary>
        /// 绘制矩形
        /// </summary>
        public void DrawRectangle(
            Graphics graphics, Pen pen,
            LitMath.Vector2 position, double width, double height,
            CSYS csys = CSYS.Model)
        {
            if (csys == CSYS.Model)
            {
                double widthInCanvas = this.ModelToCanvas(width);
                double heightInCanvas = this.ModelToCanvas(height);
                LitMath.Vector2 posInCanvas = this.ModelToCanvas(position);
                posInCanvas.y -= heightInCanvas;

                graphics.DrawRectangle(pen,
                    (float)posInCanvas.x, (float)posInCanvas.y,
                    (float)widthInCanvas, (float)heightInCanvas);
            }
            else
            {
                graphics.DrawRectangle(pen,
                    (float)position.x, (float)position.y,
                    (float)width, (float)height);
            }
        }

        /// <summary>
        /// 绘制矩形
        /// </summary>
        public void DrawRectangle(
            Graphics graphics, Pen pen,
            LitMath.Rectangle2 rectangle,
            CSYS csys = CSYS.Model)
        {
            this.DrawRectangle(graphics, pen, rectangle.location, rectangle.width, rectangle.height, csys);
        }

        /// <summary>
        /// 绘制矩形
        /// </summary>
        public void DrawRectangle(
            Graphics graphics, Pen pen,
            double x, double y, double width, double height,
            CSYS csys = CSYS.Model)
        {
            this.DrawRectangle(graphics, pen, new LitMath.Vector2(x, y), width, height, csys);
        }

        /// <summary>
        /// 绘制点
        /// </summary>
        public void DrawPoint(Graphics graphics, Brush brush, LitMath.Vector2 point)
        {
        }

        /// <summary>
        /// 绘制文本
        /// </summary>
        public LitMath.Vector2 DrawString(
            Graphics graphics, Brush brush,
            string text, string fontName, double fontHeight, TextAlignment textAlign,
            LitMath.Vector2 position,
            CSYS csys = CSYS.Model)
        {
            if (csys == CSYS.Model)
            {
                fontHeight = (int)this.ModelToCanvas(fontHeight);
                position = this.ModelToCanvas(position);
            }
                
            SizeF textSizeInCanvas = this.DrawStringInCanvas(
                graphics, brush, text, fontName, (int)fontHeight, textAlign,
                (float)position.x, (float)position.y);

            double w = textSizeInCanvas.Width;
            double h = textSizeInCanvas.Height;
            if (csys == CSYS.Model)
            {
                w = this.CanvasToModel(w);
                h = this.CanvasToModel(h);
            }
            return new LitMath.Vector2(w, h);
        }

        public SizeF DrawStringInCanvas(
             Graphics graphics, Brush brush,
             string text, string fontName, int fontHeight, TextAlignment textAlign,
             float x, float y)
        {
            if (fontHeight <= 0)
            {
                return new SizeF(0, 0);
            }

            FontStyle fontStyle = FontStyle.Regular;
            Font font = new Font(fontName, (int)fontHeight, fontStyle);
            StringFormat format = new StringFormat();
            switch (textAlign)
            {
                case TextAlignment.LeftBottom:
                    format.Alignment = StringAlignment.Near;
                    format.LineAlignment = StringAlignment.Far;
                    break;

                case TextAlignment.LeftMiddle:
                    format.Alignment = StringAlignment.Near;
                    format.LineAlignment = StringAlignment.Center;
                    break;

                case TextAlignment.LeftTop:
                    format.Alignment = StringAlignment.Near;
                    format.LineAlignment = StringAlignment.Near;
                    break;

                case TextAlignment.CenterBottom:
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Far;
                    break;

                case TextAlignment.CenterMiddle:
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    break;

                case TextAlignment.CenterTop:
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Near;
                    break;

                case TextAlignment.RightBottom:
                    format.Alignment = StringAlignment.Far;
                    format.LineAlignment = StringAlignment.Far;
                    break;

                case TextAlignment.RightMiddle:
                    format.Alignment = StringAlignment.Far;
                    format.LineAlignment = StringAlignment.Center;
                    break;

                case TextAlignment.RightTop:
                    format.Alignment = StringAlignment.Far;
                    format.LineAlignment = StringAlignment.Near;
                    break;
            }

            PointF pos = new PointF(x, y);
            graphics.DrawString(text, font, brush, pos, format);

            SizeF size = graphics.MeasureString(text, font, pos, format);
            return size;

            //PointF pos1 = new PointF((float)posInCanvas.x, (float)posInCanvas.y - size.Height);
            //PointF pos2 = new PointF(pos1.X + size.Width, pos1.Y);
            //PointF pos3 = new PointF(pos2.X, pos2.Y + size.Height);
            //PointF pos4 = new PointF(pos3.X - size.Width, pos3.Y);
            //graphics.DrawLine(GDIResMgr.Instance.GetPen(Color.Red, 0),
            //    pos1, pos2);
            //graphics.DrawLine(GDIResMgr.Instance.GetPen(Color.Red, 0),
            //    pos2, pos3);
            //graphics.DrawLine(GDIResMgr.Instance.GetPen(Color.Red, 0),
            //    pos3, pos4);
            //graphics.DrawLine(GDIResMgr.Instance.GetPen(Color.Red, 0),
            //    pos4, pos1);
        }

        /// <summary>
        /// 填充矩形
        /// </summary>
        public void FillRectangle(
            Graphics graphics, Brush brush,
            LitMath.Vector2 position, double width, double height,
            CSYS csys = CSYS.Model)
        {
            if (csys == CSYS.Model)
            {
                double widthInCanvas = this.ModelToCanvas(width);
                double heightInCanvas = this.ModelToCanvas(height);
                LitMath.Vector2 posInCanvas = this.ModelToCanvas(position);
                posInCanvas.y -= heightInCanvas;

                graphics.FillRectangle(brush,
                    (float)posInCanvas.x, (float)posInCanvas.y,
                    (float)widthInCanvas, (float)heightInCanvas);
            }
            else
            {
                graphics.FillRectangle(brush,
                    (float)position.x, (float)position.y,
                    (float)width, (float)height);
            }
        }

        /// <summary>
        /// 填充矩形
        /// </summary>
        public void FillRectangle(
            Graphics graphics, Brush brush,
            LitMath.Rectangle2 rectangle,
            CSYS csys = CSYS.Model)
        {
            this.FillRectangle(graphics, brush, rectangle.location, rectangle.width, rectangle.height, csys);
        }

        /// <summary>
        /// 填充矩形
        /// </summary>
        public void FillRectangle(
            Graphics graphics, Brush brush,
            double x, double y, double width, double height,
            CSYS csys = CSYS.Model)
        {
            this.FillRectangle(graphics, brush, new LitMath.Vector2(x, y), width, height, csys);
        }

        /// <summary>
        /// 填充椭圆
        /// </summary>
        public void FillEllipse(
            Graphics graphics, Brush brush,
            LitMath.Vector2 center, double width, double height,
            CSYS csys = CSYS.Model)
        {
            if (csys == CSYS.Model)
            {
                LitMath.Vector2 centerInCanvas = ModelToCanvas(center);
                double widthInCanvas = ModelToCanvas(width);
                double heightInCanvas = ModelToCanvas(height);

                graphics.FillEllipse(brush,
                    (float)centerInCanvas.x, (float)centerInCanvas.y,
                    (float)widthInCanvas, (float)heightInCanvas);
            }
            else
            {
                graphics.FillEllipse(brush,
                    (float)center.x, (float)center.y,
                    (float)width, (float)height);
            }
        }

        /// <summary>
        /// 填充多边形
        /// </summary>
        public void FillPolygon(
            Graphics graphics, Brush brush,
            List<LitMath.Vector2> points,
            CSYS csys = CSYS.Model)
        {
            if (csys == CSYS.Model)
            {
                List<LitMath.Vector2> pointsInCanvas = new List<LitMath.Vector2>(points.Count);
                for (int i = 0; i < points.Count; ++i)
                {
                    pointsInCanvas.Add(ModelToCanvas(points[i]));
                }
 
                PointF[] pnts = new PointF[points.Count];
                for (int i = 0; i < points.Count; ++i)
                {
                    pnts[0] = new PointF((float)pointsInCanvas[i].x, (float)pointsInCanvas[i].y);
                }

                graphics.FillPolygon(brush, pnts);
            }
            else
            {
                PointF[] pnts = new PointF[points.Count];
                for (int i = 0; i < points.Count; ++i)
                {
                    pnts[i] = new PointF((float)points[i].x, (float)points[i].y);
                }
                graphics.FillPolygon(brush, pnts);
            }
        }

        /// <summary>
        /// 绘制选择框
        /// </summary>
        public void DrawSelectRect(Graphics g, SelectRectangle selRect)
        {
            Color color = Color.White;
            if (selRect.selectMode == SelectRectangle.SelectMode.Window)
            {
                color = Color.Green;
            }
            else
            {
                color = Color.Blue;
            }

            XorGDI.DrawRectangle(g, XorGDI.PenStyles.PS_DOT, color, 1,
                selRect.startPoint, selRect.endPoint);
        }

        /// <summary>
        /// 将Model下的坐标移动到和Canvas下的坐标对应
        /// </summary>
        private void MoveModelPositionToCanvasPosition(
            LitMath.Vector2 modelPos,
            LitMath.Vector2 canvasPos)
        {
            LitMath.Vector2 modelPosInCanvas = this.ModelToCanvas(modelPos);
            _screenPan += canvasPos - modelPosInCanvas;
        }

        /// <summary>
        /// Model <> Screen 坐标变换
        /// </summary>
        #region Model<>Screen
        public double ModelToCanvas(double value)
        {
            return value * _resolution * _zoom;
        }

        public LitMath.Vector2 ModelToCanvas(LitMath.Vector2 pointInModel)
        {
            LitMath.Vector2 pan = this.screenPan;
            double x = pointInModel.x * _resolution * _zoom + pan.x;
            double y = _canvas.height - pointInModel.y * _resolution * _zoom + pan.y;

            return new LitMath.Vector2(x, y);
        }

        public double CanvasToModel(double value)
        {
            return value / (_resolution * _zoom);
        }

        public LitMath.Vector2 CanvasToModel(LitMath.Vector2 pointInCanvas)
        {
            LitMath.Vector2 pan = this.screenPan;
            double x = pointInCanvas.x - pan.x;
            double y = _canvas.height - (pointInCanvas.y - pan.y);

            return new LitMath.Vector2(x, y) / (_resolution * _zoom);
        }
        #endregion
    }
}
