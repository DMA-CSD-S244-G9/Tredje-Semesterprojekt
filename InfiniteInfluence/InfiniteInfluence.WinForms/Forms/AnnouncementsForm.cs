using InfiniteInfluence.ApiClient;
using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Model;
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


public partial class AnnouncementsForm : Form
{

    // Binding list that the grid binds to which contain Announcement objects
    private readonly BindingList<Announcement> _announcements = new();

    // BindingSource used as adapter between list and DataGridView
    private readonly BindingSource _bindingSource = new();

    private DataGridView dataGridView;

    // Custom scrollbars for the grid
    private ColorScrollBar verticalScrollBar;
    private ColorScrollBar horizontalScrollBar;

    // Holds the full list of announcements for filtering purposes
    private List<Announcement> _allAnnouncements = new();

    // API client to retrieve announcements from the REST API
    private readonly IAnnouncementDao _announcementApiClient;


    public AnnouncementsForm()
    {
        InitializeComponent();

        // Initializes the API client with the url and port matching the web api
        _announcementApiClient = new AnnouncementApiClient("https://localhost:32769");

        BuildDataGridView();

        // When the user types in the search box we filter announcements in the grid
        roundedTextBoxAnnouncementsSearch.TextChanged += (s, e) =>
        {
            // TO DO:
        };

        // Loads the announcements retrieved from the API
        LoadAnnouncementsFromApi();

        _allAnnouncements = _announcements.ToList();
    }


    private void BuildDataGridView()
    {
        dataGridView = new DataGridView();

        SpecifyDataGridRules(dataGridView);

        ApplyStylingToColumnHeader(dataGridView);

        ApplyGeneralDataGridStyling(dataGridView);

        AddDataGridColumns(dataGridView);

        panelAnnouncementContent.Controls.Add(dataGridView);
        dataGridView.ScrollBars = ScrollBars.None;
        dataGridView.Dock = DockStyle.Fill;

        // Binding of the data
        _bindingSource.DataSource = _announcements;
        dataGridView.DataSource = _bindingSource;

        // Adjustments of height when a row is removed, added or the list is changed
        _bindingSource.ListChanged += (s, e) => FitGridHeightToContent();
        dataGridView.RowsAdded += (s, e) => FitGridHeightToContent();
        dataGridView.RowsRemoved += (s, e) => FitGridHeightToContent();

        // Enables the delete button only if a row is selected / there is a current Announcement
        dataGridView.SelectionChanged += (s, e) => buttonAnnouncementDelete.Enabled = _bindingSource.Current is Announcement || dataGridView.SelectedRows.Count > 0;

        // Formatting of specific cells (for example date formatting)
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

        panelAnnouncementContent.Controls.Add(horizontalScrollBar);
        panelAnnouncementContent.Controls.Add(verticalScrollBar);

        // Sync: user scrolls with the custom bar -> grid scrolls
        verticalScrollBar.ValueChanged += (s, e) =>
        {
            if (dataGridView.RowCount == 0)
            {
                return;
            }

            int idx = Math.Max(0, Math.Min(dataGridView.RowCount - 1, verticalScrollBar.Value));

            // Protect against exception when the grid does not yet have a "first" row
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

            foreach (DataGridViewColumn dataGridViewColumn in dataGridView.Columns)
            {
                if (dataGridViewColumn.Name == "_spacer")
                {
                    continue;
                }

                fixedWidth += dataGridViewColumn.Width;
            }

            int client = dataGridView.ClientSize.Width;
            int overflow = Math.Max(0, fixedWidth - client);

            horizontalScrollBar.Minimum = 0;
            horizontalScrollBar.Maximum = overflow;
            horizontalScrollBar.LargeChange = Math.Max(1, client);
            horizontalScrollBar.Value = Math.Max(horizontalScrollBar.Minimum, Math.Min(horizontalScrollBar.Maximum, dataGridView.HorizontalScrollingOffset));
        }


        // Hooks that keep the scrollbars and grid in synchronization
        dataGridView.Scroll += (s, e) => RefreshScrollModels();
        dataGridView.Resize += (s, e) => RefreshScrollModels();
        dataGridView.ColumnWidthChanged += (s, e) => RefreshScrollModels();
        dataGridView.DataBindingComplete += (s, e) => RefreshScrollModels();
        _bindingSource.ListChanged += (s, e) => RefreshScrollModels();
        dataGridView.RowsAdded += (s, e) => RefreshScrollModels();
        dataGridView.RowsRemoved += (s, e) => RefreshScrollModels();


        // (optional) make the mouse wheel control vertical scrolling on our custom bar
        dataGridView.MouseWheel += (s, e) =>
        {
            // e.Delta is +/-120 per notch; adjust step size as desired
            int step = Math.Max(1, verticalScrollBar.LargeChange / 3);

            if (e is HandledMouseEventArgs h)
            {
                h.Handled = true;
            }

            verticalScrollBar.Value = Math.Max(verticalScrollBar.Minimum, Math.Min(verticalScrollBar.Maximum, verticalScrollBar.Value + (e.Delta > 0 ? -step : step)));
        };

        // Initial synchronization after all has been added and bound
        RefreshScrollModels();
    }


    private void SpecifyDataGridRules(DataGridView dataGridView)
    {
        // Prevents users from adjusting the height of a row
        dataGridView.AllowUserToResizeRows = false;

        // Users cannot add new rows directly in the grid
        dataGridView.AllowUserToAddRows = false;

        // Users cannot delete rows directly in the grid
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

        dataGridView.BackgroundColor = panelAnnouncementContent.BackColor;
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
        // Table Columns matching the Announcement object properties
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn
        {
            Width = 40,
            HeaderText = "Id",
            DataPropertyName = "AnnouncementId"
        });

        dataGridView.Columns.Add(new DataGridViewTextBoxColumn
        {
            Width = 320,
            HeaderText = "Title",
            DataPropertyName = "Title"
        });

        dataGridView.Columns.Add(new DataGridViewTextBoxColumn
        {
            Width = 150,
            HeaderText = "Company",
            DataPropertyName = "CompanyName"
        });

        dataGridView.Columns.Add(new DataGridViewTextBoxColumn
        {
            Width = 120,
            HeaderText = "Language",
            DataPropertyName = "AnnouncementLanguage"
        });


        // TODO: How is this working exactly with the (cur/max) ?
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn
        {
            Width = 100,
            HeaderText = "Applicants (cur/max)",
            DataPropertyName = "CurrentApplicants" // we format it in CellFormatting to show cur/max
        });

        dataGridView.Columns.Add(new DataGridViewTextBoxColumn
        {
            Width = 175,
            HeaderText = "Start Display",
            DataPropertyName = "StartDisplayDateTime",
            DefaultCellStyle = { Format = "dd-MM-yyyy HH:mm" }
        });

        dataGridView.Columns.Add(new DataGridViewTextBoxColumn
        {
            Width = 175,
            HeaderText = "End Display",
            DataPropertyName = "EndDisplayDateTime",
            DefaultCellStyle = { Format = "dd-MM-yyyy HH:mm" }
        });

        DataGridViewTextBoxColumn spacer = new DataGridViewTextBoxColumn
        {
            Name = "_spacer",
            HeaderText = "",
            ReadOnly = true,
            SortMode = DataGridViewColumnSortMode.NotSortable,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, // <- makes the grid responsive
            Resizable = DataGridViewTriState.False
        };

        dataGridView.Columns.Add(spacer);
    }


    // Formatting of cells – for example date formatting and custom "cur/max" text
    private void dataGridView_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs eventArgs)
    {
        DataGridViewColumn col = dataGridView.Columns[eventArgs.ColumnIndex];

        // The "Applicants (cur/max)" column is bound to CurrentApplicants,
        // but we display "current/max" instead of just current.
        if (col.HeaderText == "Applicants (cur/max)")
        {
            if (dataGridView.Rows[eventArgs.RowIndex].DataBoundItem is Announcement announcement)
            {
                eventArgs.Value = $"{announcement.CurrentApplicants} / {announcement.MaximumApplicants}";
                eventArgs.FormattingApplied = true;
            }
        }

        else if ((col.DataPropertyName == "StartDisplayDateTime" || col.DataPropertyName == "EndDisplayDateTime") && eventArgs.Value is DateTime dt)
        {
            eventArgs.Value = dt.ToString("dd-MM-yyyy HH:mm");
            eventArgs.FormattingApplied = true;
        }
    }


    void FitGridHeightToContent()
    {
        int rowsHeight = dataGridView.Rows.GetRowsHeight(DataGridViewElementStates.Visible);
        int headers = dataGridView.ColumnHeadersVisible ? dataGridView.ColumnHeadersHeight : 0;

        // small buffer
        int border = 2;

        dataGridView.Height = rowsHeight + headers + border;
    }





    private void buttonAnnouncementDelete_Click(object sender, EventArgs e)
    {
        // TO DO:
    }




    private void LoadAnnouncementsFromApi()
    {
        try
        {
            IEnumerable<Announcement> announcementsFromApi = _announcementApiClient.GetAll();

            // Clears the existing announcements from the list used to bind the datagridview
            _announcements.Clear();

            // Iterates over each announcement within the announecments retrieved from the api
            foreach (Announcement currentAnnouncement in announcementsFromApi)
            {
                // Adds the currently iterated announcement to the list that is used by the data grid view
                _announcements.Add(currentAnnouncement);
            }
        }

        catch (Exception exception)
        {
            MessageBox.Show($"Unable to retrieve the list of announcements from the API. Report the following error: {exception.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
