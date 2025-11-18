using InfiniteInfluence.WinFormsApp.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace InfiniteInfluence.WinFormsApp.Forms;


public partial class InfluencersForm : Form
{
    private readonly BindingList<Influencer> _influencers = new();
    private readonly BindingSource _bindingSource = new();

    private DataGridView dataGridView;


    private ColorScrollBar verticalScrollBar;
    private ColorScrollBar horizontalScrollBar;



    private List<Influencer> _allInfluencers = new();


    public InfluencersForm()
    {
        InitializeComponent();

        BuildDataGridView();


        roundedTextBoxInfluencerSearch.TextChanged += (s, e) =>
        {
            FilterInfluencers(roundedTextBoxInfluencerSearch.Text);
        };


        // Creates test data to populate the table with
        LoadTestData();

        _allInfluencers = _influencers.ToList();
    }






    private void BuildDataGridView()
    {
        dataGridView = new DataGridView();

        SpecifyDataGridRules(dataGridView);

        ApplyStylingToColumnHeader(dataGridView);

        ApplyGeneralDataGridStyling(dataGridView);

        AddDataGridColumns(dataGridView);


        panelInfluencerContent.Controls.Add(dataGridView);
        dataGridView.ScrollBars = ScrollBars.None;
        dataGridView.Dock = DockStyle.Fill;

        // Binding of data
        _bindingSource.DataSource = _influencers;
        dataGridView.DataSource = _bindingSource;


        // Adjustments of height when a row is removed, added or the list is changed
        _bindingSource.ListChanged += (s, e) => FitGridHeightToContent();
        dataGridView.RowsAdded += (s, e) => FitGridHeightToContent();
        dataGridView.RowsRemoved += (s, e) => FitGridHeightToContent();


        dataGridView.SelectionChanged += (s, e) => buttonInfluencerDelete.Enabled = _bindingSource.Current is Influencer || dataGridView.SelectedRows.Count > 0;


        // Masks the contents of the CPR cell
        dataGridView.CellFormatting += dataGridView_CellFormatting;


        // This should first be called after the table has been made
        FitGridHeightToContent();

















        // Add bars to the panel in the same container as the dataGridView
        verticalScrollBar = new ColorScrollBar
        {
            Direction = ColorScrollBar.Orientation.Vertical,
            Dock = DockStyle.Right,
            Width = 12
        };
        horizontalScrollBar = new ColorScrollBar
        {
            Direction = ColorScrollBar.Orientation.Horizontal,
            Dock = DockStyle.Bottom,
            Height = 12
        };
        
        panelInfluencerContent.Controls.Add(horizontalScrollBar);
        panelInfluencerContent.Controls.Add(verticalScrollBar);


        // Sync: bruger ruller -> grid scroller
        verticalScrollBar.ValueChanged += (s, e) =>
        {
            if (dataGridView.RowCount == 0)
            {
                return;
            }
                
            int idx = Math.Max(0, Math.Min(dataGridView.RowCount - 1, verticalScrollBar.Value));

            // Beskyt mod exception når grid ikke har nogen "første" endnu
            try
            {
                dataGridView.FirstDisplayedScrollingRowIndex = idx;
            }
            
            catch
            {
                // TODO: this is illegal we need to add data for the catch
            }
        };

        horizontalScrollBar.ValueChanged += (s, e) =>
        {
            dataGridView.HorizontalScrollingOffset = Math.Max(0, Math.Min(horizontalScrollBar.Maximum, horizontalScrollBar.Value));
        };




        void RefreshScrollModels()
        {
            // Vertical
            int first = 0;
            try
            { 
                first = Math.Max(0, dataGridView.FirstDisplayedScrollingRowIndex); 
            }
            
            catch 
            { 
                first = 0;
            }

            int visibleRows = Math.Max(1, dataGridView.DisplayedRowCount(false));
            verticalScrollBar.Minimum = 0;
            verticalScrollBar.Maximum = Math.Max(0, dataGridView.RowCount - 1);
            verticalScrollBar.LargeChange = visibleRows;
            verticalScrollBar.Value = Math.Max(verticalScrollBar.Minimum, Math.Min(verticalScrollBar.Maximum, first));

            // Horizontal scroll
            int fixedWidth = (dataGridView.RowHeadersVisible ? dataGridView.RowHeadersWidth : 0);

            foreach (DataGridViewColumn c in dataGridView.Columns)
            {
                if (c.Name == "_spacer")
                {
                    continue;
                }

                fixedWidth += c.Width;
            }

            int client = dataGridView.ClientSize.Width;
            int overflow = Math.Max(0, fixedWidth - client);

            horizontalScrollBar.Minimum = 0;
            horizontalScrollBar.Maximum = overflow;
            horizontalScrollBar.LargeChange = Math.Max(1, client);
            horizontalScrollBar.Value = Math.Max(horizontalScrollBar.Minimum, Math.Min(horizontalScrollBar.Maximum, dataGridView.HorizontalScrollingOffset));
        }



        // Hooks der holder tingene i sync
        dataGridView.Scroll += (s, e) => RefreshScrollModels();
        dataGridView.Resize += (s, e) => RefreshScrollModels();
        dataGridView.ColumnWidthChanged += (s, e) => RefreshScrollModels();
        dataGridView.DataBindingComplete += (s, e) => RefreshScrollModels();
        _bindingSource.ListChanged += (s, e) => RefreshScrollModels();
        dataGridView.RowsAdded += (s, e) => RefreshScrollModels();
        dataGridView.RowsRemoved += (s, e) => RefreshScrollModels();


        // (valgfrit) gør musehjulet til vertikal scroll på vores bar
        dataGridView.MouseWheel += (s, e) =>
        {
            // e.Delta er +/-120 pr. notch; justér step efter smag
            int step = Math.Max(1, verticalScrollBar.LargeChange / 3);
            if (e is HandledMouseEventArgs h) h.Handled = true;
            verticalScrollBar.Value = Math.Max(verticalScrollBar.Minimum, Math.Min(verticalScrollBar.Maximum, verticalScrollBar.Value + (e.Delta > 0 ? -step : step)));
        };

        // Initial synchronization after all has been added and bound
        RefreshScrollModels();
    }



    private void SpecifyDataGridRules(DataGridView dataGridView)
    {
        // Prevents users from adjusting the height of a row
        dataGridView.AllowUserToResizeRows = false;

        // Allows users to add new rows to the data grid
        dataGridView.AllowUserToAddRows = false;

        // Allows users to delete rows from the data grid
        dataGridView.AllowUserToDeleteRows = false;

        // Makes the cells read-only preventing users from editing directly in the cells 
        dataGridView.ReadOnly = true;

        // Determines if the columns should be created automatically when the grid's datasource and data members are set
        dataGridView.AutoGenerateColumns = false;

        // Clicking a cell will select the entire row of the cell
        dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

        // Only one row at a time can be selected
        dataGridView.MultiSelect = false;
    }



    private void ApplyStylingToColumnHeader(DataGridView dataGridView)
    {
        // By setting this the user's currently set theme style is not being used
        dataGridView.EnableHeadersVisualStyles = false;

        // Aligns the header content to be vertically in the middle and horizontally to the left
        dataGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        // Sets the header column's border style to be single border making it thinner than the default
        dataGridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

        // Allows users to resize the columns by pulling the headers borders
        dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;

        // Specifies the initial height of the header column
        dataGridView.ColumnHeadersHeight = 36;

        // Font and general color styling
        dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f);
        dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(44, 33, 56);
        dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(222, 222, 222);

        // The coloring for selection is set to the same as the header's default cell style to
        // prevent highlighting of the header when clicking on a cell
        dataGridView.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(44, 33, 56);
        dataGridView.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.FromArgb(222, 222, 222);
    }



    private void ApplyGeneralDataGridStyling(DataGridView dataGridView)
    {
      //dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;



        dataGridView.BackgroundColor = panelInfluencerContent.BackColor;
        dataGridView.BorderStyle = BorderStyle.None;


        // The coloring used for when a row is being highlighted
        dataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(109, 84, 181);
        dataGridView.DefaultCellStyle.SelectionForeColor = Color.FromArgb(240, 240, 240);


        // Removes the entire white column tot he left
        dataGridView.RowHeadersVisible = false;


        // Applies header styling to make the header stand out
        dataGridView.DefaultCellStyle.BackColor = Color.FromArgb(46, 42, 58);
        dataGridView.DefaultCellStyle.ForeColor = Color.FromArgb(220, 220, 230);
        dataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(42, 38, 54);
        dataGridView.GridColor = Color.FromArgb(80, 76, 96);
    }



    private void AddDataGridColumns(DataGridView dataGridView)
    {
        // Table Columns
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn
        {
            Width = 70,
            HeaderText = "Id",
            DataPropertyName = "Id"
        });
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn
        {
            Width = 220,
            HeaderText = "Fornavn",
            DataPropertyName = "Fornavn"
        });
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn
        {
            Width = 260,
            HeaderText = "Efternavn",
            DataPropertyName = "Efternavn"
        });
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn
        {
            Width = 70,
            HeaderText = "Alder",
            DataPropertyName = "Alder",
            ReadOnly = true
        });
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn
        {
            Width = 240,
            HeaderText = "Fødselsdato",
            DataPropertyName = "Fødselsdato",
            DefaultCellStyle = { Format = "dd-MM-yyyy" }
        });
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn
        {
            Width = 240,
            HeaderText = "CPR-nummer",
            // maskeres i CellFormatting
            DataPropertyName = "CPRNummer"
        });
        dataGridView.Columns.Add(new DataGridViewCheckBoxColumn
        {
            Width = 110,
            HeaderText = "Verificeret",
            DataPropertyName = "IsVerified",
            FlatStyle = FlatStyle.Flat,

            ThreeState = false
        });

        var spacer = new DataGridViewTextBoxColumn
        {
            Name = "_spacer",
            HeaderText = "",
            ReadOnly = true,
            SortMode = DataGridViewColumnSortMode.NotSortable,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, // <- gør griddet responsivt
            Resizable = DataGridViewTriState.False
        };

        dataGridView.Columns.Add(spacer);

    }




    // Maskér CPR (vis kun sidste 4 tegn)
    private void dataGridView_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        var col = dataGridView.Columns[e.ColumnIndex];

        if (col.DataPropertyName == "CPRNummer" && e.Value is string s)
        {
            e.Value = MaskCpr(s);
            e.FormattingApplied = true;
        }

        else if (col.DataPropertyName == "Fødselsdato" && e.Value is DateTime dt)
        {
            e.Value = dt.ToString("dd-MM-yyyy");
            e.FormattingApplied = true;
        }
    }



    private static string MaskCpr(string cpr)
    {
        if (string.IsNullOrWhiteSpace(cpr))
        {
            return "";
        }

        var keep = Math.Min(4, cpr.Length);

        return new string('•', Math.Max(0, cpr.Length - keep)) + cpr[^keep..];
    }



    void FitGridHeightToContent()
    {
        int rowsHeight = dataGridView.Rows.GetRowsHeight(DataGridViewElementStates.Visible);
        int headers = dataGridView.ColumnHeadersVisible ? dataGridView.ColumnHeadersHeight : 0;

        // lille buffer
        int border = 2;

        dataGridView.Height = rowsHeight + headers + border;
    }










    private void DeleteSelectedInfluencers()
    {
        // Ingen række valgt?
        if (_bindingSource.Current is not Influencer && dataGridView.SelectedRows.Count == 0)
        {
            return;
        }

        // Saml de/den valgte som Influencer-objekter
        var toDelete = new List<Influencer>();

        if (dataGridView.SelectedRows.Count > 0)
        {
            foreach (DataGridViewRow row in dataGridView.SelectedRows)
            {
                if (row.DataBoundItem is Influencer inf)
                {
                    toDelete.Add(inf);
                }
            }
        }
        else if (_bindingSource.Current is Influencer current)
        {
            toDelete.Add(current);
        }

        if (toDelete.Count == 0)
        {
            return;
        }

        // Bekræft
        string msg = toDelete.Count == 1
            ? $"Slet #{toDelete[0].Id} {toDelete[0].Fornavn} {toDelete[0].Efternavn}?"
            : $"Slet {toDelete.Count} valgte influencere?";

        
        var ok = MessageBox.Show(msg, "Bekræft sletning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

        if (ok != DialogResult.Yes)
        {
            return;
        }

        // Fjern fra kilden (BindingSource/BindingList) → grid opdaterer automatisk
        foreach (var inf in toDelete)
        {
            _bindingSource.Remove(inf);                   // svarer til _influencers.Remove(inf)
        }
    }


    private void buttonInfluencerDelete_Click(object sender, EventArgs e)
    {
        DeleteSelectedInfluencers();
    }








    private void FilterInfluencers(string query)
    {
        // If the string is empty then clear the search query
        if (string.IsNullOrWhiteSpace(query))
        {
            _bindingSource.DataSource = new BindingList<Influencer>(_allInfluencers);
            dataGridView.DataSource = _bindingSource;

            return;
        }

        query = query.Trim().ToLower();

        List<Influencer> filtered = _allInfluencers.Where(i => i.Id.ToString().Contains(query) || $"{i.Fornavn} {i.Efternavn}".ToLower().Contains(query)).ToList();

        _bindingSource.DataSource = new BindingList<Influencer>(filtered);
        dataGridView.DataSource = _bindingSource;
    }












    private void LoadTestData()
    {
        Influencer i1 = new Influencer();
        i1.Id = 1;
        i1.Fornavn = "Sofie";
        i1.Efternavn = "Mikkelsen";
        i1.Fødselsdato = new DateTime(1998, 5, 3);
        i1.CPRNummer = "030598-1123";
        i1.IsVerified = true;
        _influencers.Add(i1);

        Influencer i2 = new Influencer();
        i2.Id = 2;
        i2.Fornavn = "Oliver";
        i2.Efternavn = "Kristensen";
        i2.Fødselsdato = new DateTime(1995, 7, 18);
        i2.CPRNummer = "180795-2214";
        i2.IsVerified = false;
        _influencers.Add(i2);

        Influencer i3 = new Influencer();
        i3.Id = 3;
        i3.Fornavn = "Emma";
        i3.Efternavn = "Lund";
        i3.Fødselsdato = new DateTime(2000, 9, 27);
        i3.CPRNummer = "270900-3345";
        i3.IsVerified = true;
        _influencers.Add(i3);

        Influencer i4 = new Influencer();
        i4.Id = 4;
        i4.Fornavn = "Lucas";
        i4.Efternavn = "Andersen";
        i4.Fødselsdato = new DateTime(1999, 2, 14);
        i4.CPRNummer = "140299-4567";
        i4.IsVerified = false;
        _influencers.Add(i4);

        Influencer i5 = new Influencer();
        i5.Id = 5;
        i5.Fornavn = "Freja";
        i5.Efternavn = "Olsen";
        i5.Fødselsdato = new DateTime(2002, 11, 6);
        i5.CPRNummer = "061102-5678";
        i5.IsVerified = true;
        _influencers.Add(i5);

        Influencer i6 = new Influencer();
        i6.Id = 6;
        i6.Fornavn = "William";
        i6.Efternavn = "Nielsen";
        i6.Fødselsdato = new DateTime(1997, 4, 21);
        i6.CPRNummer = "210497-6789";
        i6.IsVerified = true;
        _influencers.Add(i6);

        Influencer i7 = new Influencer();
        i7.Id = 7;
        i7.Fornavn = "Ida";
        i7.Efternavn = "Jørgensen";
        i7.Fødselsdato = new DateTime(1996, 1, 10);
        i7.CPRNummer = "100196-7890";
        i7.IsVerified = false;
        _influencers.Add(i7);

        Influencer i8 = new Influencer();
        i8.Id = 8;
        i8.Fornavn = "Noah";
        i8.Efternavn = "Mortensen";
        i8.Fødselsdato = new DateTime(1994, 8, 4);
        i8.CPRNummer = "040894-8910";
        i8.IsVerified = true;
        _influencers.Add(i8);

        Influencer i9 = new Influencer();
        i9.Id = 9;
        i9.Fornavn = "Alma";
        i9.Efternavn = "Hansen";
        i9.Fødselsdato = new DateTime(2001, 12, 9);
        i9.CPRNummer = "091201-9123";
        i9.IsVerified = false;
        _influencers.Add(i9);

        Influencer i10 = new Influencer();
        i10.Id = 10;
        i10.Fornavn = "Elias";
        i10.Efternavn = "Rasmussen";
        i10.Fødselsdato = new DateTime(1993, 3, 30);
        i10.CPRNummer = "300393-1234";
        i10.IsVerified = true;
        _influencers.Add(i10);

        Influencer i11 = new Influencer();
        i11.Id = 11;
        i11.Fornavn = "Clara";
        i11.Efternavn = "Thomsen";
        i11.Fødselsdato = new DateTime(1999, 6, 11);
        i11.CPRNummer = "110699-2345";
        i11.IsVerified = true;
        _influencers.Add(i11);

        Influencer i12 = new Influencer();
        i12.Id = 12;
        i12.Fornavn = "Victor";
        i12.Efternavn = "Larsen";
        i12.Fødselsdato = new DateTime(1998, 10, 22);
        i12.CPRNummer = "221098-3456";
        i12.IsVerified = false;
        _influencers.Add(i12);

        Influencer i13 = new Influencer();
        i13.Id = 13;
        i13.Fornavn = "Laura";
        i13.Efternavn = "Poulsen";
        i13.Fødselsdato = new DateTime(2000, 5, 17);
        i13.CPRNummer = "170500-4567";
        i13.IsVerified = true;
        _influencers.Add(i13);

        Influencer i14 = new Influencer();
        i14.Id = 14;
        i14.Fornavn = "Sebastian";
        i14.Efternavn = "Knudsen";
        i14.Fødselsdato = new DateTime(1995, 9, 8);
        i14.CPRNummer = "080995-5678";
        i14.IsVerified = false;
        _influencers.Add(i14);

        Influencer i15 = new Influencer();
        i15.Id = 15;
        i15.Fornavn = "Anna";
        i15.Efternavn = "Christensen";
        i15.Fødselsdato = new DateTime(1997, 2, 25);
        i15.CPRNummer = "250297-6789";
        i15.IsVerified = true;
        _influencers.Add(i15);
    }
}