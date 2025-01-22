using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Accord.Math;
using Telerik.Charting;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using Telerik.Windows.Documents.Spreadsheet.Model;
using Telerik.Windows.Documents.Spreadsheet.Model.Protection;
using Telerik.Windows.Documents.Spreadsheet.Utilities;
using Telerik.WinForms.Controls.Spreadsheet;
using Telerik.WinForms.Spreadsheet;
using UltraBend.Common.Math;
using UltraBend.Databases;
using UltraBend.Databases.Material;
using UltraBend.Services;
using Material = UltraBend.Data.Material.Material;
using MaterialData = UltraBend.Data.Material.MaterialData;

namespace UltraBend
{
    public partial class MaterialSpreadsheetForm : RadForm
    {
        public MaterialSpreadsheetForm()
        {
            Material = new Material
            {
                Id = Guid.NewGuid(),
                Data = new BindingList<MaterialData> {new MaterialData {Index = 0, Stress = 0, Strain = 0}},
                RegressionOrder = 6
            };
            Regression = new PolynomialRegression {Order = Material.RegressionOrder};
            InitializeComponent();
        }

        public MaterialSpreadsheetForm(Material material)
        {
            Material = material;
            Regression = new PolynomialRegression(){Order = Material.RegressionOrder};
            InitializeComponent();
        }

        public Material Material { get; set; }

        public PolynomialRegression Regression { get; set; }

        private void WriteData()
        {
            var worksheet = radSpreadsheetData.SpreadsheetElement.ActiveWorksheet;
            var range = worksheet.UsedCellRange;
            worksheet.Cells[range.FromIndex, range.ToIndex].SetValue("");
            for (var i = 0; i < Material.Data.Count; i++)
            {
                worksheet.Cells[i, 0, i, 0].SetValue(Material.Data[i].Strain);
                worksheet.Cells[i, 1, i, 1].SetValue(Material.Data[i].Stress);
            }
        }

        private void WriteCoefficients()
        {
            var worksheet = radSpreadsheetCoefficients.SpreadsheetElement.ActiveWorksheet;
            var range = worksheet.UsedCellRange;
            worksheet.Cells[range.FromIndex, range.ToIndex].SetValue("");
            worksheet.Cells[0, 0, 0, 0].SetValueAsText("R^2");
            worksheet.Cells[0, 1, 0, 1].SetValueAsText(Regression.RSquared.ToString("G"));
            for (var i = 0; i < Regression.a.Length; i++)
            {
                worksheet.Cells[i + 1, 0, i + 1, 0].SetValue($"C{i}");
                worksheet.Cells[i + 1, 1, i + 1, 1].SetValueAsText(Regression.a[i].ToString("G"));
            }
        }

        private void MaterialSpreadsheetForm_Load(object sender, EventArgs e)
        {
            radSpinEditorOrder.Value = Regression.Order;

            radChartView1.Series[0].PointSize = new SizeF(1, 1);
            radChartView1.Series[0].Shape = new RoundRectShape(0);
            radChartView1.Series[1].DrawLinesToLabels = false;

            radTextBoxMaterialId.Text = Material.Id.ToString();
            radTextBoxMaterialName.Text = Material.Name;
            radTextBoxDescription.Text = Material.Description;
            SetupDataSpreadsheet();
            SetupCoefficientSpreadsheet();

            WriteData();
            Update();
        }

        private void SetupCoefficientSpreadsheet()
        {
            var workbook = new Workbook();
            workbook.Protect("internal");
            radSpreadsheetCoefficients.SpreadsheetElement.Workbook = workbook;
            radSpreadsheetCoefficients.SpreadsheetElement.MessageShowing += SpreadsheetElement_MessageShowing;
            var worksheet = workbook.ActiveWorksheet;
            var options = new WorksheetProtectionOptions(allowInsertRows: true, allowDeleteRows: true);
            worksheet.Protect("internal", options);
            worksheet.Columns[2, worksheet.Columns.Count - 1].SetHidden(true);
            worksheet.Columns[0, 0]
                .SetWidth(new ColumnWidth((radSpreadsheetCoefficients.Width - (25 + 17 + 2)) / 4, true));
            worksheet.Columns[1, 1]
                .SetWidth(new ColumnWidth((radSpreadsheetCoefficients.Width - (25 + 17 + 2)) * (3.0 / 4), true));
            //worksheet.Columns[0, 1].SetIsLocked(false);
            worksheet.HeaderNameRenderingConverter = new CoefficientNameConverter();
        }

        private void SpreadsheetElement_MessageShowing(object sender, MessageShowingEventArgs e)
        {
            if (e.Content.Equals(LocalizationManager.GetString("Spreadsheet_ProtectedWorksheet_Error"),
                StringComparison.OrdinalIgnoreCase))
                e.IsHandled = true;
        }

        private void SetupDataSpreadsheet()
        {
            var workbook = new Workbook();
            workbook.Protect("internal");
            radSpreadsheetData.SpreadsheetElement.Workbook = workbook;
            var worksheet = workbook.ActiveWorksheet;
            var options = new WorksheetProtectionOptions(allowInsertRows: true, allowDeleteRows: true);
            worksheet.Protect("internal", options);
            worksheet.Columns[2, worksheet.Columns.Count - 1].SetHidden(true);
            worksheet.Columns[0, 1].SetWidth(new ColumnWidth((radSpreadsheetData.Width - (25 + 17 + 2)) / 2, true));
            worksheet.Columns[0, 1].SetIsLocked(false);
            worksheet.HeaderNameRenderingConverter = new DataNameConverter();
            workbook.WorkbookContentChanged += Workbook_WorkbookContentChanged;
        }

        private void Workbook_WorkbookContentChanged(object sender, EventArgs e)
        {
            var worksheet = radSpreadsheetData.SpreadsheetElement.ActiveWorksheet;
            for (var i = 0; i < worksheet.Rows.Count; i++)
            {
                var col1 = worksheet.Cells[i, 0, i, 0];
                var col2 = worksheet.Cells[i, 1, i, 1];
                var col1val = col1.GetValue().Value;
                var col2val = col2.GetValue().Value;
                if (col1val.ValueType == CellValueType.Number && col2val.ValueType == CellValueType.Number)
                {
                    // note, always store SI units
                    var strain = double.Parse(col1val.GetResultValueAsString(col1.GetFormat().Value));
                    var stress = double.Parse(col2val.GetResultValueAsString(col2.GetFormat().Value)) * 1E6;
                    if (Material.Data.Count > i)
                    {
                        Material.Data[i].Strain = strain;
                        Material.Data[i].Stress = stress;
                    }
                    else
                    {
                        Material.Data.Add(new MaterialData {Strain = strain, Stress = stress, Index = i, Id = Guid.NewGuid()});
                    }
                }
                else
                {
                    while (Material.Data.Count > i)
                        Material.Data.RemoveAt(i);
                    break;
                }
            }


            Regression.x = Material.Data.Select(i => i.Strain).ToArray();
            Regression.y = Material.Data.Select(i => i.Stress).ToArray();
            Regression.Compute();
            WriteCoefficients();
            Plot();
            //radGridView1.MasterTemplate.BestFitColumns(BestFitColumnMode.AllCells);

            Plot();
        }

        private void Plot()
        {
            var scatterSeries = radChartView1.Series[0] as ScatterLineSeries;
            scatterSeries.DataPoints.Clear();
            if (Regression.x.Any())
            {
                var xmin = Regression.x.Min();
                var xmax = Regression.x.Max();
                for (var i = 0; i < 100; i++)
                {
                    var xi = xmin + i * (xmax - xmin) / 100.0;
                    var yfit = Regression.Eval(xi);
                    scatterSeries.DataPoints.Add(new ScatterDataPoint(xi, yfit / 1e6));
                }
            }

            var counter = 0;
            foreach (var dataItem in Material.Data)
            {
                dataItem.Index = counter;
                counter++;
            }

            var scatterLineSeries = radChartView1.Series[1] as ScatterLineSeries;
            scatterLineSeries.DataPoints.Clear();
            scatterLineSeries.PointSize = new SizeF(10, 10);
            scatterLineSeries.DataPoints.Clear();
            scatterLineSeries.DataPoints.AddRange(Material.Data.OrderBy(d => d.Index)
                .Select(d => new ScatterDataPoint(d.Strain, d.Stress / 1E6)).ToArray());
        }

        public class DataNameConverter : HeaderNameRenderingConverterBase
        {
            protected override string ConvertColumnIndexToNameOverride(HeaderNameRenderingConverterContext context,
                int columnIndex)
            {
                switch (columnIndex)
                {
                    case 0:
                        return "Strain [-]";
                    case 1:
                        return "Stress [MPa]";
                    default:
                        return base.ConvertColumnIndexToNameOverride(context, columnIndex);
                }
            }
        }

        public class CoefficientNameConverter : HeaderNameRenderingConverterBase
        {
            protected override string ConvertColumnIndexToNameOverride(HeaderNameRenderingConverterContext context,
                int columnIndex)
            {
                switch (columnIndex)
                {
                    case 0:
                        return "Var.";
                    case 1:
                        return "Value";
                    default:
                        return base.ConvertColumnIndexToNameOverride(context, columnIndex);
                }
            }
        }

        private void radButtonCancel_Click(object sender, EventArgs e)
        {
            //DialogResult = DialogResult.Cancel;

            Close();
        }

        private void radButtonOK_Click(object sender, EventArgs e)
        {
            //DialogResult = DialogResult.OK;

            //Material.RegressionCoefficients = Regression.a;
            //Material.RegressionOrder = Regression.Order;

            var materialsDatabaseFileName =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "UltraDeep\\UltraBend\\global.ubm");
            using (var dataFile = DataFile<MaterialDbContext>.Load(materialsDatabaseFileName, connection => new MaterialDbContext(connection, true, false)))
            {
                var service = new MaterialsService(dataFile);
                service.UpsertMaterial(Material);
                service.Save();
            }

            Close();
        }

        private void radSpinEditorOrder_ValueChanged_1(object sender, EventArgs e)
        {
            Regression.Order = Convert.ToInt32(Math.Floor(radSpinEditorOrder.Value));
            Update();
        }

        private void Update()
        {
            Regression.Compute();
            WriteCoefficients();
            Plot();

            Material.RegressionCoefficients = Regression.a;
            Material.RegressionOrder = Regression.Order;
        }

        private void radTextBoxMaterialName_TextChanged(object sender, EventArgs e)
        {
            Material.Name = radTextBoxMaterialName.Text;

        }

        private void radTextBoxDescription_TextChanged(object sender, EventArgs e)
        {
            Material.Description = radTextBoxDescription.Text;

        }
    }
}