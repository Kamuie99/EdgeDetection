using OpenCvSharp;
using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;


using DPoint = System.Drawing.Point;  // System.Drawing.Point 별칭
using CvPoint = OpenCvSharp.Point;    // OpenCvSharp.Point 별칭

using DSize = System.Drawing.Size;  // System.Drawing.Size 별칭
using CvSize = OpenCvSharp.Size;    // OpenCvSharp.Size 별칭

namespace EdgeDetection
{
    public partial class Form1 : Form
    {
        private string[] imagePaths = new string[4]; // 이미지 경로 저장

        private Rectangle[] roiRects = new Rectangle[4];
        private bool isDragging = false;
        private int currentRoiIndex = -1;
        private DPoint dragStartPoint;


        public Form1()
        {
            InitializeComponent();
        }

        #region ROI 표시 및 마우스 조작 함수
        // ROI 표시 및 마우스 조작 이벤트 등록
        private void btnSetROI_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 4; i++)
            {
                roiRects[i] = new Rectangle(30, 30, 70, 70); // 초기 ROI
                PictureBox picBox = Controls.Find($"pictureBox{i + 1}", true)[0] as PictureBox;

                picBox.Paint += PictureBox_Paint;
                picBox.MouseDown += PictureBox_MouseDown;
                picBox.MouseMove += PictureBox_MouseMove;
                picBox.MouseUp += PictureBox_MouseUp;
                picBox.Invalidate(); // 강제 그리기
            }
        }

        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            PictureBox pic = sender as PictureBox;
            int index = int.Parse(pic.Name.Replace("pictureBox", "")) - 1;

            if (roiRects[index] != Rectangle.Empty)
            {
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    e.Graphics.DrawRectangle(pen, roiRects[index]);
                }
            }
        }

        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            PictureBox pic = sender as PictureBox;
            int index = int.Parse(pic.Name.Replace("pictureBox", "")) - 1;

            if (roiRects[index].Contains(e.Location))
            {
                isDragging = true;
                dragStartPoint = e.Location;
                currentRoiIndex = index;
            }
        }

        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && currentRoiIndex != -1)
            {
                PictureBox pic = sender as PictureBox;
                int dx = e.X - dragStartPoint.X;
                int dy = e.Y - dragStartPoint.Y;

                roiRects[currentRoiIndex].Offset(dx, dy);
                dragStartPoint = e.Location;
                pic.Invalidate();
            }
        }

        private void PictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
            currentRoiIndex = -1;
        }
        #endregion



        // 이미지 불러오기 (Bitmap)
        private void btnLoadImages_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Image Files (*.jpg; *.png; *.bmp)|*.jpg;*.png;*.bmp",
                Title = "4개의 꼭짓점 이미지를 선택하세요"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (ofd.FileNames.Length != 4)
                {
                    MessageBox.Show("반드시 4장의 이미지를 선택해야 합니다.");
                    return;
                }

                // 이미지 표시 및 저장
                for (int i = 0; i < 4; i++)
                {
                    imagePaths[i] = ofd.FileNames[i];

                    // PictureBox에 이미지 표시할 때 Image Lock 방지용 Bitmap 생성
                    using (var tempImg = Image.FromFile(imagePaths[i]))
                    {
                        PictureBox picBox = Controls.Find($"pictureBox{i + 1}", true)[0] as PictureBox;
                        picBox.Image?.Dispose(); // 기존 이미지 리소스 해제
                        picBox.Image = new Bitmap(tempImg);
                    }

                    // 이미지 저장 경로 설정
                    string saveDir = Path.Combine(Application.StartupPath, "SavedImages");
                    Directory.CreateDirectory(saveDir);
                    string fileName = $"corner_{i + 1}.bmp";
                    string savePath = Path.Combine(saveDir, fileName);

                    // 이미지 저장 (덮어쓰기)
                    File.Copy(imagePaths[i], savePath, true);
                }

                MessageBox.Show("이미지 4장이 성공적으로 저장되었습니다.");
            }
        }

        // 엣지 검출 함수
        private void btnEdgeDetect_Click(object sender, EventArgs e)
        {
            string folderPath = Path.Combine(Application.StartupPath, "SavedImages");

            for (int i = 0; i < 4; i++)
            {
                string imagePath = Path.Combine(folderPath, $"corner_{i + 1}.bmp");

                if (!File.Exists(imagePath))
                {
                    MessageBox.Show($"이미지 파일이 존재하지 않습니다: corner_{i + 1}.bmp");
                    return;
                }

                using (Bitmap bitmap = new Bitmap(imagePath))
                using (Mat mat = BitmapToMat(bitmap))
                using (Mat gray = new Mat())
                using (Mat blurred = new Mat())
                using (Mat edges = new Mat())
                {
                    // 1. Grayscale 변환
                    Cv2.CvtColor(mat, gray, ColorConversionCodes.BGR2GRAY);

                    // 2. 히스토그램 평활화 (선명도 향상)
                    //Cv2.EqualizeHist(gray, gray);

                    // 3. 블러링
                    Cv2.BilateralFilter(gray, blurred, 9, 75, 75);

                    // 4. Canny 엣지 검출 (민감도 약간 증가)
                    Cv2.Canny(blurred, edges, 50, 150);

                    //// 5. Morphology Close 연산으로 끊긴 선 연결
                    //Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(3, 3));
                    //Cv2.MorphologyEx(edges, edges, MorphTypes.Close, kernel);

                    // 6. 결과 표시
                    Bitmap edgeBitmap = MatToBitmap(edges);
                    PictureBox picBox = Controls.Find($"pictureBox{i + 1}", true)[0] as PictureBox;
                    picBox.Image?.Dispose(); // 기존 이미지 메모리 해제
                    picBox.Image = edgeBitmap;
                }
            }

            MessageBox.Show("엣지 검출 완료!");
        }


        // Bitmap → Mat 변환
        private Mat BitmapToMat(Bitmap bmp)
        {
            using (var ms = new MemoryStream())
            {
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                byte[] imageData = ms.ToArray();
                return Mat.FromImageData(imageData, ImreadModes.Color);
            }
        }

        // Mat → Bitmap 변환
        private Bitmap MatToBitmap(Mat mat)
        {
            byte[] imageData = mat.ToBytes(".png");
            using (var ms = new MemoryStream(imageData))
            {
                Bitmap bmp = new Bitmap(ms);
                return new Bitmap(bmp);
            }
        }

        private void btnSaveRoiOnly_Click(object sender, EventArgs e)
        {
            string folderPath = Path.Combine(Application.StartupPath, "SavedImages");

            for (int i = 0; i < 4; i++)
            {
                PictureBox picBox = Controls.Find($"pictureBox{i + 1}", true)[0] as PictureBox;
                if (picBox.Image == null)
                {
                    MessageBox.Show($"PictureBox{i + 1}에 이미지가 없습니다.");
                    continue;
                }

                using (Bitmap edgeBitmap = new Bitmap(picBox.Image)) // ← 현재 표시 중인 엣지 이미지 사용
                {
                    Rectangle roiOnPicBox = roiRects[i];

                    // 픽쳐박스 및 이미지 크기
                    int pbWidth = picBox.ClientSize.Width;
                    int pbHeight = picBox.ClientSize.Height;
                    int imgWidth = edgeBitmap.Width;
                    int imgHeight = edgeBitmap.Height;

                    // Zoom 비율 계산 (비율 유지)
                    float ratio = Math.Min((float)pbWidth / imgWidth, (float)pbHeight / imgHeight);

                    // 실제 픽쳐박스 내에서 이미지가 그려지는 위치 (중앙 정렬 오프셋)
                    int displayWidth = (int)(imgWidth * ratio);
                    int displayHeight = (int)(imgHeight * ratio);
                    int offsetX = (pbWidth - displayWidth) / 2;
                    int offsetY = (pbHeight - displayHeight) / 2;

                    // ROI 좌표를 이미지 좌표로 변환
                    int x = (int)((roiOnPicBox.X - offsetX) / ratio);
                    int y = (int)((roiOnPicBox.Y - offsetY) / ratio);
                    int width = (int)(roiOnPicBox.Width / ratio);
                    int height = (int)(roiOnPicBox.Height / ratio);

                    Rectangle roiOnImage = new Rectangle(x, y, width, height);
                    roiOnImage.Intersect(new Rectangle(0, 0, edgeBitmap.Width, edgeBitmap.Height)); // 이미지 범위 제한

                    if (roiOnImage.Width <= 0 || roiOnImage.Height <= 0)
                    {
                        MessageBox.Show($"ROI 영역이 유효하지 않습니다 (이미지 {i + 1})");
                        continue;
                    }

                    Bitmap roiBmp = edgeBitmap.Clone(roiOnImage, edgeBitmap.PixelFormat);

                    string roiPath = Path.Combine(folderPath, $"roi_{i + 1}.bmp");
                    roiBmp.Save(roiPath);

                    // 픽쳐박스에 ROI 이미지 표시
                    picBox.Image?.Dispose();
                    picBox.Image = new Bitmap(roiBmp);
                    roiBmp.Dispose();

                    // 🔴 ROI 사각형 제거 및 다시 그리기
                    roiRects[i] = Rectangle.Empty;
                    picBox.Invalidate();
                }
            }

            MessageBox.Show("엣지 이미지에서 ROI 영역이 잘 저장되었습니다.");
        }

        private void btnDetectChamfer_Click(object sender, EventArgs e)
        {
            string folderPath = Path.Combine(Application.StartupPath, "SavedImages");
            double chamferAngleTolerance = 10.0;

            for (int i = 0; i < 4; i++)
            {
                string roiPath = Path.Combine(folderPath, $"roi_{i + 1}.bmp");
                if (!File.Exists(roiPath))
                {
                    MessageBox.Show($"ROI 이미지가 없습니다: roi_{i + 1}.bmp");
                    continue;
                }

                using (Mat src = Cv2.ImRead(roiPath))
                using (Mat gray = new Mat())
                using (Mat blurred = new Mat())
                using (Mat edges = new Mat())
                using (Mat result = src.Clone())
                {
                    Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
                    Cv2.GaussianBlur(gray, blurred, new CvSize(3, 3), 0.5);
                    Cv2.Canny(blurred, edges, 50, 150);

                    LineSegmentPoint[] lines = Cv2.HoughLinesP(edges, 1, Math.PI / 180, 20, 5, 20);

                    List<LineSegmentPoint> verticalLines = new List<LineSegmentPoint>();
                    List<LineSegmentPoint> horizontalLines = new List<LineSegmentPoint>();
                    List<Tuple<LineSegmentPoint, double>> chamferCandidates = new List<Tuple<LineSegmentPoint, double>>();

                    foreach (var line in lines)
                    {
                        double dx = line.P2.X - line.P1.X;
                        double dy = line.P2.Y - line.P1.Y;
                        double angleDeg = Math.Abs(Math.Atan2(dy, dx) * 180.0 / Math.PI);
                        double length = Math.Sqrt(dx * dx + dy * dy);

                        if (angleDeg > 180)
                            angleDeg -= 180;

                        if (angleDeg <= 10 || angleDeg >= 170)
                            verticalLines.Add(line);
                        else if (angleDeg >= 80 && angleDeg <= 100)
                            horizontalLines.Add(line);
                        else if (angleDeg >= 45 - chamferAngleTolerance && angleDeg <= 45 + chamferAngleTolerance)
                            chamferCandidates.Add(Tuple.Create(line, length));
                    }

                    var chamferLine = chamferCandidates.OrderByDescending(t => t.Item2).FirstOrDefault();

                    if (verticalLines.Count > 0 && horizontalLines.Count > 0 && chamferLine != null)
                    {
                        LineSegmentPoint vLine = verticalLines.OrderByDescending(l =>
                        {
                            double dx = l.P2.X - l.P1.X;
                            double dy = l.P2.Y - l.P1.Y;
                            return Math.Sqrt(dx * dx + dy * dy);
                        }).First();

                        LineSegmentPoint hLine = horizontalLines.OrderByDescending(l =>
                        {
                            double dx = l.P2.X - l.P1.X;
                            double dy = l.P2.Y - l.P1.Y;
                            return Math.Sqrt(dx * dx + dy * dy);
                        }).First();

                        LineSegmentPoint chamferSeg = chamferLine.Item1;

                        // 교차점 계산 함수
                        Point2f? GetIntersection(CvPoint p1, CvPoint p2, CvPoint p3, CvPoint p4)
                        {
                            float A1 = p2.Y - p1.Y;
                            float B1 = p1.X - p2.X;
                            float C1 = A1 * p1.X + B1 * p1.Y;

                            float A2 = p4.Y - p3.Y;
                            float B2 = p3.X - p4.X;
                            float C2 = A2 * p3.X + B2 * p3.Y;

                            float denominator = A1 * B2 - A2 * B1;
                            if (Math.Abs(denominator) < 1e-5)
                                return null;

                            float x = (B2 * C1 - B1 * C2) / denominator;
                            float y = (A1 * C2 - A2 * C1) / denominator;
                            return new Point2f(x, y);
                        }

                        // 교차점 계산
                        Point2f? inter1 = GetIntersection(vLine.P1, vLine.P2, chamferSeg.P1, chamferSeg.P2);
                        Point2f? inter2 = GetIntersection(hLine.P1, hLine.P2, chamferSeg.P1, chamferSeg.P2);

                        if (inter1.HasValue && inter2.HasValue)
                        {
                            // 연장 직선 그리기
                            var (vStart, vEnd) = ExtendLine(vLine.P1, vLine.P2, result.Size());
                            Cv2.Line(result, vStart, vEnd, Scalar.Blue, 1);

                            var (hStart, hEnd) = ExtendLine(hLine.P1, hLine.P2, result.Size());
                            Cv2.Line(result, hStart, hEnd, Scalar.Green, 1);

                            var (cStart, cEnd) = ExtendLine(chamferSeg.P1, chamferSeg.P2, result.Size());
                            Cv2.Line(result, cStart, cEnd, Scalar.Gray, 1);

                            // 교차점 잇는 선분만 빨간색으로
                            Cv2.Line(result,
                                new CvPoint((int)inter1.Value.X, (int)inter1.Value.Y),
                                new CvPoint((int)inter2.Value.X, (int)inter2.Value.Y),
                                Scalar.Red, 2);
                        }
                    }
                    else
                    {
                        MessageBox.Show($"챔퍼 기준 각도에 부합하는 선이 충분하지 않습니다 (이미지 {i + 1})");
                    }

                    string resultPath = Path.Combine(folderPath, $"chamfer_{i + 1}.bmp");
                    Cv2.ImWrite(resultPath, result);

                    PictureBox picBox = Controls.Find($"pictureBox{i + 1}", true)[0] as PictureBox;
                    picBox.Image?.Dispose();
                    picBox.Image = new Bitmap(resultPath);
                }
            }
        }

        // 직선 연장 함수 (시작점, 끝점 반환)
        private (CvPoint, CvPoint) ExtendLine(CvPoint p1, CvPoint p2, CvSize imageSize)
        {
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;

            double length = Math.Sqrt(dx * dx + dy * dy);
            if (length == 0)
                return (p1, p2);

            double scale = 1000; // 충분히 긴 선 연장

            CvPoint pt1 = new CvPoint(
                (int)(p1.X - dx / length * scale),
                (int)(p1.Y - dy / length * scale));

            CvPoint pt2 = new CvPoint(
                (int)(p1.X + dx / length * scale),
                (int)(p1.Y + dy / length * scale));

            return (pt1, pt2);
        }




    }
}



