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
                roiRects[i] = new Rectangle(30, 30, 100, 100); // 초기 ROI
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

                // 1. Bitmap으로 이미지 불러오기
                using (Bitmap bitmap = new Bitmap(imagePath))
                {
                    // 2. Bitmap → Mat 변환
                    using (Mat mat = BitmapToMat(bitmap))
                    {
                        // 3. 그레이스케일 변환
                        using (Mat gray = new Mat())
                        {
                            Cv2.CvtColor(mat, gray, ColorConversionCodes.BGR2GRAY);

                            // 4. 블러링으로 노이즈 제거
                            using (Mat blurred = new Mat())
                            {
                                Cv2.GaussianBlur(gray, blurred, new OpenCvSharp.Size(5, 5), 1.5);

                                // 5. Canny 엣지 검출
                                using (Mat edges = new Mat())
                                {
                                    Cv2.Canny(blurred, edges, 50, 150);

                                    // 6. Mat → Bitmap 변환
                                    Bitmap edgeBitmap = MatToBitmap(edges);

                                    // 7. PictureBox에 표시
                                    PictureBox picBox = Controls.Find($"pictureBox{i + 1}", true)[0] as PictureBox;
                                    picBox.Image?.Dispose(); // 기존 이미지 메모리 해제
                                    picBox.Image = edgeBitmap;
                                }
                            }
                        }
                    }
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

            double DistancePointToPoint(CvPoint a, CvPoint b)
            {
                return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
            }

            //bool PointsEqual(CvPoint a, CvPoint b) => a.X == b.X && a.Y == b.Y;

            double maxRedLineLength = -1;
            int maxRedLineIndex = -1;

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

                    Cv2.FindContours(edges, out CvPoint[][] contours, out _, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

                    var largest = contours.OrderByDescending(c => Cv2.ContourArea(c)).FirstOrDefault();
                    if (largest == null || largest.Length < 5) continue;

                    CvPoint[] approx = Cv2.ApproxPolyDP(largest, 10, true);
                    if (approx.Length < 4) continue;

                    //! 디버깅용 노란점 찍기 주석처리
                    //foreach (var pt in approx)
                    //{
                    //    Cv2.Circle(result, pt, 5, Scalar.Yellow, -1);
                    //}

                    List<(CvPoint p1, CvPoint p2, double len)> lines = new List<(CvPoint, CvPoint, double)>();
                    for (int j = 0; j < approx.Length; j++)
                    {
                        var p1 = approx[j];
                        var p2 = approx[(j + 1) % approx.Length];
                        double len = DistancePointToPoint(p1, p2);
                        lines.Add((p1, p2, len));
                    }

                    var sorted = lines.OrderByDescending(l => l.len).ToList();

                    var longLine1 = sorted[0];

                    // 중복 제거 적용해서 두 번째 긴 선분 찾기
                    (CvPoint p1, CvPoint p2, double len)? longLine2 = null;
                    for (int idx = 1; idx < sorted.Count; idx++)
                    {
                        var candidate = sorted[idx];

                        var mid1 = new CvPoint((longLine1.p1.X + longLine1.p2.X) / 2, (longLine1.p1.Y + longLine1.p2.Y) / 2);
                        var mid2 = new CvPoint((candidate.p1.X + candidate.p2.X) / 2, (candidate.p1.Y + candidate.p2.Y) / 2);
                        double dist = DistancePointToPoint(mid1, mid2);

                        if (dist >= 20)
                        {
                            longLine2 = candidate;
                            break;
                        }
                    }
                    if (longLine2 == null) longLine2 = sorted[1];

                    CvPoint[] points = new CvPoint[] { longLine1.p1, longLine1.p2, longLine2.Value.p1, longLine2.Value.p2 };

                    var candidateChamferLines = new List<(CvPoint p1, CvPoint p2, double len)>();
                    for (int a = 0; a < points.Length; a++)
                    {
                        for (int b = a + 1; b < points.Length; b++)
                        {
                            double dist = DistancePointToPoint(points[a], points[b]);
                            candidateChamferLines.Add((points[a], points[b], dist));
                        }
                    }

                    var chamferLine = candidateChamferLines.OrderBy(l => l.len).First();

                    // 빨간선 길이 체크
                    if (chamferLine.len > maxRedLineLength)
                    {
                        maxRedLineLength = chamferLine.len;
                        maxRedLineIndex = i;
                    }

                    Cv2.Line(result, longLine1.p1, longLine1.p2, Scalar.Green, 2);
                    Cv2.Line(result, longLine2.Value.p1, longLine2.Value.p2, Scalar.Blue, 2);
                    Cv2.Line(result, chamferLine.p1, chamferLine.p2, Scalar.Red, 2);

                    string resultPath = Path.Combine(folderPath, $"chamfer_{i + 1}.bmp");
                    Cv2.ImWrite(resultPath, result);

                    PictureBox picBox = Controls.Find($"pictureBox{i + 1}", true)[0] as PictureBox;
                    picBox.Image?.Dispose();
                    picBox.Image = new Bitmap(resultPath);
                }
            }

            if (maxRedLineIndex >= 0)
            {
                MessageBox.Show($" 모따기 C0.7은 현재 {maxRedLineIndex+1}사분면");
            }
            else
            {
                MessageBox.Show("유효한 빨간선이 감지되지 않았습니다.");
            }

        }




    }
}
