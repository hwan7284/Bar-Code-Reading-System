using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel;
using OfficeOpenXml;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.IO;
using Newtonsoft.Json;

namespace Test1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            autoSaveTimer.Interval = 60000; // 1분 간격으로 저장 (밀리초 단위)
            autoSaveTimer.Tick += AutoSaveTimer_Tick;
            autoSaveTimer.Start();

            // 프로그램 실행 시 Load
            // LoadDataGridViewFromJSON(dataGridView1);
        }

        private string previousText = ""; // 이전 텍스트 저장
        private string afterText = ""; // 이후 텍스트 저장
        private bool spaceKeyPressed = false; // 스페이스바 입력 여부
        private int line_A = 0; // 바코드, 바코드2 행 저장
        private int line_B = 0; // 바코드3 행 저장
        private int line_C = 0;

        private void textBox3_KeyDown_1(object sender, KeyEventArgs e) // 바코드 입력
        {
            if (e.KeyCode == Keys.Space && !string.IsNullOrWhiteSpace(textBox3.Text))
            {
                if (e.KeyCode == Keys.Space)
                {
                    if (!spaceKeyPressed)
                    {
                        previousText = textBox3.Text.Trim();
                        textBox3.Clear();
                    }
                    spaceKeyPressed = true;
                    e.SuppressKeyPress = true;
                }
            }
            else if (e.KeyCode == Keys.Enter)
            {
                string text = textBox3.Text;

                if (!string.IsNullOrWhiteSpace(text)) // 공백이나 공백 문자만 있는 경우에만 처리
                {
                    if (spaceKeyPressed)
                    {
                        afterText = text.Trim();

                        string input1 = previousText; // 여기에 확인할 문자열을 넣으세요
                        string input2 = afterText; // 여기에 확인할 문자열을 넣으세요

                        bool isNumeric16_1 = long.TryParse(input1, out long result1) && input1.Length == 16;
                        bool isNumeric16_2 = long.TryParse(input2, out long result2) && input2.Length == 16;

                        if (isNumeric16_1 && isNumeric16_2)
                        {
                            if (dataGridView1.Rows.Count > 0)
                            {
                                if (line_A >= dataGridView1.Rows.Count - 1)
                                {
                                    dataGridView1.Rows.Add(); // 행이 부족할 때만 추가
                                }
                                dataGridView1.Rows[line_A].Cells[0].Value = previousText;
                                dataGridView1.Rows[line_A].Cells[1].Value = afterText;
                                dataGridView1.Rows[line_A].Cells["연도"].Value = DateTime.Now.ToString("yyyy");
                                dataGridView1.Rows[line_A].Cells["월"].Value = DateTime.Now.ToString("MM");
                                dataGridView1.Rows[line_A].Cells["일"].Value = DateTime.Now.ToString("dd");
                                dataGridView1.Rows[line_A].Cells["시간"].Value = DateTime.Now.ToString("HH:mm:ss");
                                line_A++; // 행 증가
                                dataGridView2.Rows[0].Cells[0].Value = line_A;
                                label5.Text = "정상 !";
                            }
                        }
                        else
                        {
                            string combinedText = $"{previousText} {afterText}";
                            label5.Text = "인식 오류 확인 !";
                            dataGridView3.Rows.Add();
                            dataGridView3.Rows[line_C].Cells[0].Value = combinedText;
                            line_C++; // 행 증가
                            combinedText = "";
                        }
                    }
                    else
                    {
                        if (line_B >= dataGridView1.Rows.Count -1)
                        {
                            dataGridView1.Rows.Add(); // 행이 부족할 때만 추가
                        }
                        dataGridView1.Rows[line_B].Cells[2].Value = text;
                        dataGridView1.Rows[line_B].Cells["연도"].Value = DateTime.Now.ToString("yyyy");
                        dataGridView1.Rows[line_B].Cells["월"].Value = DateTime.Now.ToString("MM");
                        dataGridView1.Rows[line_B].Cells["일"].Value = DateTime.Now.ToString("dd");
                        dataGridView1.Rows[line_B].Cells["시간"].Value = DateTime.Now.ToString("HH:mm:ss");
                        line_B++; // 행 증가
                        dataGridView2.Rows[0].Cells[1].Value = line_B;
                        label5.Text = "정상 !";
                    }

                }
                textBox3.Clear();
                spaceKeyPressed = false;
                previousText = "";
                afterText = "";
            }
        }
        private void button1_Click(object sender, EventArgs e) // 파일 저장
        {
            // DataGridView의 내용을 DataTable로 로드
            DataTable dt = new DataTable();
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                dt.Columns.Add(column.HeaderText);
            }

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                DataRow dataRow = dt.NewRow();
                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    dataRow[i] = row.Cells[i].Value;
                }
                dt.Rows.Add(dataRow);
            }

            // DataGridView2의 내용을 DataTable로 로드
            DataTable dt2 = new DataTable();
            foreach (DataGridViewColumn column in dataGridView2.Columns)
            {
                dt2.Columns.Add(column.HeaderText);
            }

            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                DataRow dataRow = dt2.NewRow();
                for (int i = 0; i < dataGridView2.Columns.Count; i++)
                {
                    dataRow[i] = row.Cells[i].Value;
                }
                dt2.Rows.Add(dataRow);
            }

            // DataGridView3의 내용을 DataTable로 로드
            DataTable dt3 = new DataTable();
            foreach (DataGridViewColumn column in dataGridView3.Columns)
            {
                dt3.Columns.Add(column.HeaderText);
            }

            foreach (DataGridViewRow row in dataGridView3.Rows)
            {
                DataRow dataRow = dt3.NewRow();
                for (int i = 0; i < dataGridView3.Columns.Count; i++)
                {
                    dataRow[i] = row.Cells[i].Value;
                }
                dt3.Rows.Add(dataRow);
            }

            string currentTime = DateTime.Now.ToString("yyyyMMdd_HH：mm：ss");
            string fileName = $"{currentTime}_Bar-Code.xlsx";

            // 폴더 브라우저 대화상자를 통해 저장할 경로 선택
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = folderDialog.SelectedPath;
                    string savePath = Path.Combine(selectedPath, fileName);

                    // DataTable의 내용을 Excel 파일로 저장
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Sheet1");
                        worksheet.Column(1).Width = 18;
                        worksheet.Column(2).Width = 18;
                        worksheet.Column(12).Width = 18;
                        worksheet.Column(13).Width = 18;
                        worksheet.Cell(1, 1).InsertTable(dt);
                        worksheet.Cell(1, 9).InsertTable(dt2);
                        worksheet.Cell(1, 12).InsertTable(dt3);
                        workbook.SaveAs(savePath);
                    }


                    MessageBox.Show("Excel 파일로 내보내기 완료!");
                }
            }
        }
        private void button2_Click(object sender, EventArgs e) // 데이터 삭제
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                dataGridView1.Rows.Remove(selectedRow);
                line_A--;
                line_B--;
                dataGridView2.Rows[0].Cells[0].Value = line_A;
                dataGridView2.Rows[0].Cells[1].Value = line_B;
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            MoveSelectedRow(-1); // 위로 이동 (-1 인덱스)
        }
        private void button4_Click(object sender, EventArgs e)
        {
            MoveSelectedRow(1); // 아래로 이동 (1 인덱스)
        }
        private void MoveSelectedRow(int offset) // 행 이동
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int currentIndex = dataGridView1.SelectedRows[0].Index;
                int newIndex = currentIndex + offset;

                if (newIndex >= 0 && newIndex < dataGridView1.Rows.Count)
                {
                    DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                    dataGridView1.Rows.Remove(selectedRow);
                    dataGridView1.Rows.Insert(newIndex, selectedRow);
                    dataGridView1.ClearSelection();
                    dataGridView1.Rows[newIndex].Selected = true;
                }
            }
        }
        private void button5_Click(object sender, EventArgs e) // 시트 비우기
        {
            dataGridView1.Rows.Clear();
            line_A = 0;
            line_B = 0;
            dataGridView2.Rows[0].Cells[0].Value = line_A;
            dataGridView2.Rows[0].Cells[1].Value = line_B;
        }
        private Timer autoSaveTimer = new Timer();
        private void AutoSaveTimer_Tick(object sender, EventArgs e)
        {
            string currentTime = DateTime.Now.ToString("yyyyMMdd_HH：mm：ss");
            string fileName = $"{currentTime}_AutoSave.json";
            string currentTime1 = DateTime.Now.ToString("yyyy년 MM월 dd일_HH시 mm분 ss초");
            label6.Text = $"{currentTime1} 자동 저장 !";

            SaveDataGridViewToJSON(dataGridView1, fileName);
        }
        private void SaveDataGridViewToJSON(DataGridView dataGridView, string fileName) // 데이터 저장
        {
            List<List<string>> data = new List<List<string>>();

            List<string> line_AData = new List<string> { line_A.ToString() };
            data.Add(line_AData);

            List<string> line_BData = new List<string> { line_B.ToString() };
            data.Add(line_BData);

            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                List<string> rowData = new List<string>();
                foreach (DataGridViewCell cell in row.Cells)
                {
                    rowData.Add(cell.Value?.ToString() ?? "");
                }
                data.Add(rowData);
            }

            // 실행 파일이 위치한 디렉토리에 데이터를 저장
            //string savePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

            string savePath = @"C:" + fileName;

            string jsonData = JsonConvert.SerializeObject(data);
            File.WriteAllText(savePath, jsonData);
        }
        private void LoadDataGridViewFromJSON(DataGridView dataGridView) // 데이터 로드
        {
            int line_AValue = 0;
            int line_BValue = 0;

            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "JSON 파일 (*.json)|*.json|모든 파일 (*.*)|*.*";
                openFileDialog.FilterIndex = 1;

                // 기본 디렉토리를 설정
                openFileDialog.InitialDirectory = @"C:";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFilePath = openFileDialog.FileName;

                    if (File.Exists(selectedFilePath))
                    {
                        string jsonData = File.ReadAllText(selectedFilePath);
                        List<List<string>> data = JsonConvert.DeserializeObject<List<List<string>>>(jsonData);

                        dataGridView.Rows.Clear(); // 기존의 행을 모두 삭제

                        foreach (List<string> rowData in data)
                        {
                            // 데이터가 비어있지 않다면 행을 추가
                            if (rowData.Any(cellData => !string.IsNullOrEmpty(cellData)))
                            {
                                int rowIndex = dataGridView.Rows.Add(rowData.ToArray());

                                // 시간 열 데이터를 이전 시간 데이터로 복원
                                if (rowData.Count >= dataGridView.Columns.Count)
                                {
                                    dataGridView.Rows[rowIndex].Cells["시간"].Value = rowData[dataGridView.Columns["시간"].Index];
                                }
                            }
                        }
                        if (data.Count > 0 && data[0].Count > 0)
                        {
                            // JSON 데이터를 파싱하여 line_A 값을 읽어옴
                            if (int.TryParse(data[0][0], out int parsed1Value))
                            {
                                line_AValue = parsed1Value;
                            }
                            // JSON 데이터를 파싱하여 line_B 값을 읽어옴
                            if (int.TryParse(data[1][0], out int parsed2Value))
                            {
                                line_BValue = parsed2Value;
                            }
                        }
                        // 첫 번째 행을 삭제
                        for (int i = 0; i < 2; i++)
                        {
                            if (dataGridView.Rows.Count > 0)
                            {
                                dataGridView.Rows.RemoveAt(0);
                            }
                        }

                        line_A = line_AValue;
                        line_B = line_BValue;
                        dataGridView2.Rows[0].Cells[0].Value = line_A;
                        dataGridView2.Rows[0].Cells[1].Value = line_B;
                    }
                    else
                    {
                        MessageBox.Show("선택한 파일이 존재하지 않습니다.");
                    }
                }
            }
        }

        //private void button6_Click(object sender, EventArgs e) // 데이터 저장 버튼시 경로 지정
        //{
        //    // 파일 저장 대화상자 열기
        //    using (var saveFileDialog = new SaveFileDialog())
        //    {
        //        saveFileDialog.Filter = "JSON 파일 (*.json)|*.json";
        //        saveFileDialog.FilterIndex = 1;

        //        if (saveFileDialog.ShowDialog() == DialogResult.OK)
        //        {
        //            string fileName = saveFileDialog.FileName;

        //            // DataGridView의 내용을 선택한 파일로 저장
        //            SaveDataGridViewToJSON(dataGridView1, fileName);
        //        }
        //    }
        //}
        private void button6_Click(object sender, EventArgs e) // 데이터 저장 버튼 시 경로 지정 X
        {
            string currentTime = DateTime.Now.ToString("yyyyMMdd_HH：mm：ss");
            string fileName = $"{currentTime}_Bar-Code.json";

            SaveDataGridViewToJSON(dataGridView1, fileName);

            MessageBox.Show("JSON 데이터 저장 완료!");
        }
        private void button7_Click(object sender, EventArgs e) // 데이터 로드 버튼
        {
            LoadDataGridViewFromJSON(dataGridView1);
            dataGridView2.Rows[0].Cells[0].Value = line_A;
            dataGridView2.Rows[0].Cells[1].Value = line_B;
        }
        private void dataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
             dataGridView2.Rows[0].Cells[0].Value = line_A;
             dataGridView2.Rows[0].Cells[1].Value = line_B;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            dataGridView3.Rows.Clear();
            line_C = 0;
        }
    }
}
