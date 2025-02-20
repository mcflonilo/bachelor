using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CsvHelper;
using CsvHelper.Configuration;
using PostSharp.Patterns.Model;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace UltraBend.Controls
{
    public class CsvGrid : RadGridView
    {
        [WeakEvent]
        public event EventHandler OnPaste;
        public CsvGrid() : base()
        {
            //this.CellEditorInitialized += CsvGrid_CellEditorInitialized;
            this.EditorRequired += CsvGrid_EditorRequired;
        }

        private void CsvGrid_EditorRequired(object sender, EditorRequiredEventArgs e)
        {
            if (e.EditorType == typeof(GridSpinEditor))
            {
                e.EditorType = typeof(CsvRadSpinEditor);
            }
        }

        


        void TextBoxItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                GridNavigator.SelectPreviousRow(1);
            }
            else if (e.KeyCode == Keys.Down)
            {
                GridNavigator.SelectNextRow(1);
            }
        }

        protected override RadGridViewElement CreateGridViewElement()
        {
            var val =  new CsvRadGridViewElement();
            val.OnPaste += Val_OnPaste;
            return val;
        }

        private void Val_OnPaste(object sender, EventArgs e)
        {
            if (OnPaste != null)
                OnPaste(sender, e);
        }

        public override string ThemeClassName => typeof(RadGridView).FullName;


    }

    public class CsvRadSpinEditor : GridSpinEditor
    {
        public override object Value
        {
            get
            {
                CsvRadSpinEditorElement editor = (CsvRadSpinEditorElement)this.EditorElement;
                return editor.Value;
            }
            set
            {
                CsvRadSpinEditorElement editor = (CsvRadSpinEditorElement)this.EditorElement;

                if (value == null)
                    editor.Value = 0;
                else
                {
                    Decimal test;
                    if (Decimal.TryParse(value.ToString(), NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint,
                        CultureInfo.CurrentCulture, out test))
                    {
                        editor.Value = test;
                    }
                    //else if (value.ToS)
                    //{
                    //    editor.Value = 0;
                    //}
                }
            }
        }

        protected override RadElement CreateEditorElement()
        {
            return (RadElement)new CsvRadSpinEditorElement(){ShowUpDownButtons = false, EnableValueChangingOnTextChanging = false};
        }

    }

    public class CsvRadSpinEditorElement : RadSpinEditorElement
    {
        protected override decimal GetValueFromText()
        {
            int num1 = this.Hexadecimal ? 1 : 0;
            try
            {
                if (string.IsNullOrEmpty(this.Text) || this.Text.Length == 1 && !(this.Text != "-"))
                    return this.internalValue;
                Decimal num2 = new Decimal(0);
                return !this.Hexadecimal ? 
                    this.Constrain(Decimal.Parse(this.Text, NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint, (IFormatProvider)CultureInfo.CurrentCulture)) 
                    : 
                    this.Constrain(Convert.ToDecimal(Convert.ToInt64(this.Text, 16)));
            }
            catch
            {
                return this.internalValue;
            }

        }

        protected override string GetNumberText(decimal num)
        {
            if (Hexadecimal)
                return $"{ (long)num:X}";
            return Convert.ToDouble(num).ToString("g");
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            this.ValidateOnKeyPress(e);
            
            // not calling base key press...

            this.OnTextBoxKeyPress(e);
        }

        protected new void OnTextBoxKeyPress(KeyPressEventArgs e)
        {
            NumberFormatInfo numberFormat = CultureInfo.CurrentCulture.NumberFormat;
            string decimalSeparator = numberFormat.NumberDecimalSeparator;
            string numberGroupSeparator = numberFormat.NumberGroupSeparator;
            string negativeSign = numberFormat.NegativeSign;
            if ((int)e.KeyChar == 46)
                e.KeyChar = decimalSeparator[0];
            string str = e.KeyChar.ToString();
            if (char.IsDigit(e.KeyChar) 
                || str.Equals(decimalSeparator) 
                || (str.Equals(numberGroupSeparator) 
                || str.Equals(negativeSign)) 
                || (int)e.KeyChar == 8 
                || this.Hexadecimal && ((int)e.KeyChar >= 97 && (int)e.KeyChar <= 102 || (int)e.KeyChar >= 65 && (int)e.KeyChar <= 70) 
                || (Control.ModifierKeys & (Keys.Control | Keys.Alt)) != Keys.None
                || (int)e.KeyChar == 101 // e key
                || (int)e.KeyChar == 69 // E key
                )
                return;
            e.Handled = true;
            Telerik.WinControls.NativeMethods.MessageBeep(0);
        }
    }

    public class CsvRadGridViewElement : RadGridViewElement
    {
        [WeakEvent]
        public event EventHandler OnPaste;

        protected override MasterGridViewTemplate CreateTemplate()
        {
            var val = new CsvMasterGridViewTemplate();
            val.OnPaste += Val_OnPaste;
            return val;
        }

        private void Val_OnPaste(object sender, EventArgs e)
        {
            if (OnPaste != null)
                OnPaste(sender, e);
        }

        protected override Type ThemeEffectiveType => typeof(RadGridViewElement);
    }

    public class CsvMasterGridViewTemplate : MasterGridViewTemplate
    {
        [WeakEvent]
        public event EventHandler OnPaste;

        public static bool CanChangeType(object value, Type conversionType)
        {
            if (conversionType == null)
            {
                return false;
            }

            if (value == null)
            {
                return false;
            }

            IConvertible convertible = value as IConvertible;

            if (convertible == null)
            {
                return false;
            }

            return true;
        }

        public override void Paste()
        {
            if (Clipboard.ContainsData(DataFormats.Text))
            {
                var dataSource = this.Owner.DataSource as IList;

                var bindingSource = this.Owner.DataSource as ICsvBindingList;

                if (bindingSource != null)
                {
                    bindingSource.RaiseListChangedEvents = false;
                }

                if (dataSource != null)
                {
                    string data = Clipboard.GetData(DataFormats.Text).ToString();
                    if (!string.IsNullOrWhiteSpace(data))
                    {
                        using (TextReader reader = new StringReader(data))
                        {
                            var csv = new CsvReader(reader, new Configuration() { HasHeaderRecord = false, Delimiter = "\t" });
                            IEnumerable<object> records = null;
                            var type = dataSource.GetType().GetGenericArguments().SingleOrDefault();
                            try
                            {
                                records = csv.GetRecords<dynamic>().ToList();
                                var count = records.Count();
                            }
                            catch { }

                            if (records != null && type != null)
                            {
                                var rowIndex = 0;
                                var colIndex = 0;

                                if (this.Owner.SelectedCells.Any())
                                {
                                    rowIndex = this.Owner.SelectedCells.Select(c => c.RowInfo.Index).Min();
                                    colIndex = this.Owner.SelectedCells.Select(c => c.ColumnInfo.Index).Min();
                                }
                                if (this.Owner.SelectedRows.Any())
                                    rowIndex = this.Owner.SelectedRows.Select(r => r.Index).Min();
                                if (rowIndex == -1)
                                    rowIndex = this.Owner.RowCount;
                                
                                foreach (var record in records)
                                {
                                    var item = Activator.CreateInstance(type);
                                    if (rowIndex < dataSource.Count)
                                    {
                                        item = dataSource[rowIndex];
                                    }
                                    else
                                    {
                                        dataSource.Add(item);
                                    }

                                    //var newItem = Activator.CreateInstance(type);
                                    var propertyValues = (IDictionary<string, object>)record;
                                    var properties = item.GetType().GetProperties().Where(p => !Attribute.IsDefined(p, typeof(BrowsableAttribute))).ToArray();
                                    var propertyIndex = 0;
                                    foreach (var propertyValue in propertyValues)
                                    {
                                        while ((!CanChangeType(propertyValue.Value, properties[propertyIndex].PropertyType) || colIndex > propertyIndex) && propertyIndex < properties.Length)
                                        {
                                            propertyIndex++;
                                        }

                                        if (propertyIndex < properties.Length)
                                        {
                                            var value = Activator.CreateInstance(properties[propertyIndex].PropertyType);

                                            try
                                            {
                                                value = Convert.ChangeType(propertyValue.Value, properties[propertyIndex].PropertyType);
                                            }
                                            catch (System.FormatException ex)
                                            {
                                                // use default from above
                                            }

                                            properties[propertyIndex].SetValue(item, value);
                                            propertyIndex++;
                                        }
                                    }
                                    rowIndex++;
                                }

                                if (bindingSource != null)
                                {
                                    bindingSource.RaiseListChangedEvents = true;
                                    bindingSource.ResetBindings();
                                }

                                this.Refresh();

                                if (OnPaste != null)
                                    OnPaste(this, null);

                                return;
                            }
                        }
                    }
                }


                if (bindingSource != null)
                {
                    bindingSource.RaiseListChangedEvents = true;
                    bindingSource.ResetBindings();
                }
            }
            base.Paste();
        }
    }
}
