using InfiniteInfluence.DataAccessLibrary.Model;
using System.ComponentModel;


namespace InfiniteInfluence.WinFormsApp.Components;


/// <summary>
/// A reusable WinForms <see cref="UserControl"/> that encapsulates all of the DataGridView related
/// logic for displaying a list of <see cref="Announcement"/> objects in a dashboard style grid with a fitting theme.
///
/// This control is responsible for configuring the <see cref="DataGridView"/> columns, styling, and selection behavior.
/// Binding announcements using the <see cref="BindingSource"/> as well as setting and filtering announcements.
/// It also provides a <see cref="SelectedAnnouncement"/> property which allows for finding the currently selected announcement in the AnnouncementsForm.
///
/// The goal is to keep all of the data grid view specific behavior in one place so that the AnnouncementsForm
/// does not get cluttered, and only contain essentials such as API calls, dialogs, and other UI logic.
/// </summary>
public class AnnouncementGridControl : UserControl
{
    // BindingSource used as adapter between list and DataGridView.
    // The BindingSource knows which list is the "current data" and the DataGridView listens to it.
    private readonly BindingSource _bindingSource = new();

    // The DataGridView that displays the announcements
    private readonly DataGridView dataGridView;

    // Holds the full list of announcements for filtering purposes.
    // We never bind this list directly; instead we wrap it in a BindingList for BindingSource.
    private List<Announcement> _allAnnouncements = new();

    /// <summary>
    /// Exposes the BindingSource in case the parent form wants to subscribe
    /// or inspect the current item.
    /// </summary>
    public BindingSource BindingSource => _bindingSource;

    /// <summary>
    /// Convenience property to get the currently selected announcement,
    /// or null if there is none.
    /// 
    /// This works because the DataGridView is bound to the BindingSource,
    /// and BindingSource.Current references the currently selected row's data object.
    /// </summary>
    public Announcement? SelectedAnnouncement => _bindingSource.Current as Announcement;

    /// <summary>
    /// Raised when the selection in the grid changes.
    /// The form can subscribe to this to enable/disable buttons.
    /// </summary>
    public event EventHandler? AnnouncementSelectionChanged;


    /// <summary>
    /// Initializes a new instance of the <see cref="AnnouncementGridControl"/> class.
    /// 
    /// The constructor:
    /// <list type="number">
    /// <item>Creates and configures the internal <see cref="DataGridView"/>.</item>
    /// <item>Applies rules, styling, and adds the announcement-specific columns.</item>
    /// <item>Sets up data binding via the internal <see cref="BindingSource"/>.</item>
    /// <item>Hooks up events to keep grid size and formatting in sync.</item>
    /// </list>
    /// 
    /// After construction the control is ready to receive data via the <see cref="SetAnnouncements"/> method.
    /// </summary>
    public AnnouncementGridControl()
    {
        // Make the control fill its parent by default
        Dock = DockStyle.Fill;

        // Create the grid instance
        dataGridView = new DataGridView();

        // Initialize the grid (rules, styling, columns, binding, basic events)
        InitializeDataGridView();
    }


    /// <summary>
    /// Sets the full data set of announcements that the grid will display.
    /// 
    /// This method:
    /// <list type="bullet">
    /// <item>Stores the supplied announcements in an internal list (<see cref="_allAnnouncements"/>).</item>
    /// <item>Wraps them in a <see cref="BindingList{T}"/> and assigns it as the data source of the <see cref="BindingSource"/>.</item>
    /// <item>Causes the <see cref="DataGridView"/> to refresh and render the current list of announcements.</item>
    /// </list>
    /// 
    /// Typical usage is for a parent form to call this after receiving data from a REST Web API.
    /// </summary>
    /// <param name="announcements">The announcements to display in the grid.</param>
    public void SetAnnouncements(IEnumerable<Announcement> announcements)
    {
        // Converts the incoming IEnumerable to a concrete List so we can reuse it for filtering.
        _allAnnouncements = announcements.ToList();

        // Wraps the list in a BindingList, which supports change notifications (add/remove).
        // The BindingSource will use this list as its current data source.
        _bindingSource.DataSource = new BindingList<Announcement>(_allAnnouncements);

        // Because the DataGridView is already bound to _bindingSource (in InitializeDataGridView),
        // changing the BindingSource.DataSource automatically updates the grid.
    }




    ////////////////////////////////
    // - Search Field Filtering - //
    ////////////////////////////////


    /// <summary>
    /// Filters the grid based on a search query string
    /// 
    /// The filtering matches:
    /// <list type="bullet">
    /// <item><see cref="Announcement.AnnouncementId"/> converted to string,</item>
    /// <item><see cref="Announcement.Title"/>,</item>
    /// <item><see cref="Announcement.CompanyName"/>.</item>
    /// </list>
    /// 
    /// If the query is blank or whitespace, the filter is cleared and the full list of announcements is shown.
    /// </summary>
    /// <param name="query">The search text entered by the user.</param>
    public void ApplyFilter(string query)
    {
        // If the string is empty then clear the search query and show the original full list
        if (string.IsNullOrWhiteSpace(query))
        {
            _bindingSource.DataSource = new BindingList<Announcement>(_allAnnouncements);
            return;
        }

        // Removes any leading and trailing whitespaces and makes the supplied query lower case
        string normalizedQuery = query.Trim().ToLower();

        // Filter announcements based on id (as a string), title or company name
        List<Announcement> filteredAnnouncements = _allAnnouncements
            .Where(announcement =>

                // Match if the Id (converted to string) contains the query
                announcement.AnnouncementId.ToString().Contains(normalizedQuery)

                // Match if the title is not null/empty and contains the query
                || (!string.IsNullOrWhiteSpace(announcement.Title)
                && announcement.Title.ToLower().Contains(normalizedQuery))

                // Match if the company name is not null/empty and contains the query
                || (!string.IsNullOrWhiteSpace(announcement.CompanyName)
                    && announcement.CompanyName.ToLower().Contains(normalizedQuery)))

            .ToList();

        // Update the BindingSource to use the filtered list.
        // The DataGridView will automatically update because it is data-bound to _bindingSource.
        _bindingSource.DataSource = new BindingList<Announcement>(filteredAnnouncements);
    }



    ///////////////////////////////////
    // - Columns & Cell Formatting - //
    ///////////////////////////////////

    /// <summary>
    /// Adds and configures all of the columns of the <see cref="DataGridView"/> that is used to display 
    /// announcement information such as Id, Title, Company, Language, Applicants, Start/End display, and a spacer column.
    /// The spacer column uses <see cref="DataGridViewAutoSizeColumnMode.Fill"/> to make the grid responsive.
    /// </summary>
    /// <param name="dataGridView">The grid to which columns are added.</param>
    private void AddDataGridColumns(DataGridView dataGridView)
    {
        // Column for announcement Id
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn
        {
            Width = 40,
            HeaderText = "Id",
            DataPropertyName = "AnnouncementId"
        });

        // Column for announcement title
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn
        {
            Width = 320,
            HeaderText = "Title",
            DataPropertyName = "Title"
        });

        // Column for company name
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn
        {
            Width = 150,
            HeaderText = "Company",
            DataPropertyName = "CompanyName"
        });

        // Column for language
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn
        {
            Width = 120,
            HeaderText = "Language",
            DataPropertyName = "AnnouncementLanguage"
        });


        // Column that will display "current / maximum" applicants (formatted in CellFormatting)
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn
        {
            Width = 110,
            HeaderText = "Applicants (cur/max)",
            // DataPropertyName is bound to CurrentApplicants, but we will combine
            // CurrentApplicants + MaximumApplicants in the CellFormatting event.
            DataPropertyName = "CurrentApplicants"
        });

        // Column for start display date/time
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn
        {
            Width = 175,
            HeaderText = "Start Display",
            DataPropertyName = "StartDisplayDateTime",
            // Sets default format for date time values in this column
            DefaultCellStyle = { Format = "dd-MM-yyyy HH:mm" }
        });

        // Column for end display date/time
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn
        {
            Width = 175,
            HeaderText = "End Display",
            DataPropertyName = "EndDisplayDateTime",
            DefaultCellStyle = { Format = "dd-MM-yyyy HH:mm" }
        });

        // Spacer column: this column does not show data but expands to fill remaining space.
        // This is used to make the grid layout responsive and avoid awkward empty areas.
        DataGridViewTextBoxColumn spacerColumn = new DataGridViewTextBoxColumn
        {
            Name = "_spacer",
            HeaderText = "",
            ReadOnly = true,
            SortMode = DataGridViewColumnSortMode.NotSortable,

            // Fill mode makes this column grow/shrink to fill leftover width.
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            Resizable = DataGridViewTriState.False
        };

        // Adds the spacer column as the last column
        dataGridView.Columns.Add(spacerColumn);
    }



    /// <summary>
    /// Handles cell formatting for the <see cref="DataGridView"/> like displaying "current / maximum" in the Applicants column 
    /// instead of just the raw values in two columns, formatting of start and end display dates using the "dd-MM-yyyy HH:mm" format.
    /// This event keeps the visual representation user friendly while the underlying Announcement data is still intact.
    /// </summary>
    /// <param name="sender">The source of the event which in this case is the grid.</param>
    /// <param name="eventArgs">Formatting information for the current cell.</param>
    private void dataGridView_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs eventArgs)
    {
        // Get the column metadata for the cell being formatted
        DataGridViewColumn dataGridViewColumn = dataGridView.Columns[eventArgs.ColumnIndex];

        // The "Applicants (cur/max)" column is bound to CurrentApplicants,
        // but we display "current/max" instead of just current.
        if (dataGridViewColumn.HeaderText == "Applicants (cur/max)")
        {
            // Retrieve the Announcement object bound to the current row
            if (dataGridView.Rows[eventArgs.RowIndex].DataBoundItem is Announcement announcement)
            {
                // Build text like "3 / 10" based on current and max applicants
                eventArgs.Value = $"{announcement.CurrentApplicants} / {announcement.MaximumApplicants}";

                // Tell the grid that we have handled the formatting for this cell
                eventArgs.FormattingApplied = true;
            }
        }

        // Handles date formatting for StartDisplayDateTime and EndDisplayDateTime columns
        else if ((dataGridViewColumn.DataPropertyName == "StartDisplayDateTime" || dataGridViewColumn.DataPropertyName == "EndDisplayDateTime") && eventArgs.Value is DateTime dateTime)
        {
            // Format the DateTime value as "dd-MM-yyyy HH:mm"
            eventArgs.Value = dateTime.ToString("dd-MM-yyyy HH:mm");

            eventArgs.FormattingApplied = true;
        }
    }



    ////////////////////////////////
    // - Data Grid View Styling - //
    ////////////////////////////////


    /// <summary>
    /// Configures behavioral rules for the <see cref="DataGridView"/>, such as disabling row resizing, disallowing editing, and forcing only entire row selections.
    /// This helps ensure that the grid is used as a read only list display rather than an actually editable data grid view.
    /// </summary>
    /// <param name="dataGridView">The grid to configure.</param>
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



    /// <summary>
    /// Applies styling to the column headers of the <see cref="DataGridView"/> such as alignment, colors, border style, and height.
    /// This gives the grid a consistent visual appearance that matches the dashboard and website's general aesthetic theme.
    /// </summary>
    /// <param name="dataGridView">The grid whose column headers are to be styled.</param>
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



    /// <summary>
    /// Applies general styling to the <see cref="DataGridView"/> cells and grid such as background color, selection colors, alternating row colors, and the lines between the grid.
    /// This makes the announcement list visually consistent with the rest of the application and MVC website's theme.
    /// </summary>
    /// <param name="dataGridView">The grid to style.</param>
    private void ApplyGeneralDataGridStyling(DataGridView dataGridView)
    {
        // Disables the auto-sizing feature of the columns allowing us to set our own widths without grid rendering issues
        dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

        // Changes the background color of the data grid view to avoid having a white box below the entries
        dataGridView.BackgroundColor = Color.FromArgb(46, 42, 58);

        // Removes the borders of the data grid view
        dataGridView.BorderStyle = BorderStyle.None;

        // The coloring used for when a row is being highlighted
        dataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(109, 84, 181);
        dataGridView.DefaultCellStyle.SelectionForeColor = Color.FromArgb(240, 240, 240);

        // Removes the entire white column to the left
        dataGridView.RowHeadersVisible = false;

        // Applies row styling to make the grid blend into the theme
        dataGridView.DefaultCellStyle.BackColor = Color.FromArgb(46, 42, 58);
        dataGridView.DefaultCellStyle.ForeColor = Color.FromArgb(220, 220, 230);
        dataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(42, 38, 54);
        dataGridView.GridColor = Color.FromArgb(80, 76, 96);
    }



    /// <summary>
    /// Adjusts the <see cref="DataGridView.Height"/> so that it fits the sum of all visible rows plus the header height, with a small extra border.
    /// This helps with avoiding empty space below the grid and keeps the layout looking mostly the same when it is being scaled up.
    /// 
    /// In other words: the grid becomes "tight" around its contents instead of leaving a lot of blank space.
    /// </summary>
    private void FitGridHeightToContent()
    {
        // Get the combined height of all visible rows
        int rowsHeight = dataGridView.Rows.GetRowsHeight(DataGridViewElementStates.Visible);

        // Include header height if headers are visible
        int headerHeight = dataGridView.ColumnHeadersVisible ? dataGridView.ColumnHeadersHeight : 0;

        // Small extra border to avoid clipping
        int extraBorderPixels = 2;

        // Set the DataGridView height to match content + header + border
        dataGridView.Height = rowsHeight + headerHeight + extraBorderPixels;
    }



    ////////////////////////////////
    // - Private Helper Methods - //
    ////////////////////////////////

    /// <summary>
    /// Initializes and configures the internal <see cref="DataGridView"/>:
    /// sets rules, styling, columns, binds the <see cref="BindingSource"/>,
    /// and hooks up basic events such as selection changes and cell formatting.
    /// </summary>
    private void InitializeDataGridView()
    {
        // Applies non-visual behavioral rules (read-only, selection mode, etc.)
        SpecifyDataGridRules(dataGridView);

        // Applies styling to the header row
        ApplyStylingToColumnHeader(dataGridView);

        // Applies styling to the rest of the grid (rows, selection, etc.)
        ApplyGeneralDataGridStyling(dataGridView);

        // Adds all announcement-specific columns to the grid
        AddDataGridColumns(dataGridView);

        // Add grid to this UserControl so it becomes visible
        Controls.Add(dataGridView);

        // Use built-in DataGridView scrollbars (vertical + horizontal)
        dataGridView.ScrollBars = ScrollBars.Both;

        // Make the grid fill all available space inside this control
        dataGridView.Dock = DockStyle.Fill;

        // Bind the DataGridView to the BindingSource.
        // This is the key data binding link: DataGridView <-> BindingSource <-> (BindingList<Announcement>)
        dataGridView.DataSource = _bindingSource;

        // When the BindingSource list changes, adjust grid height to match new content
        _bindingSource.ListChanged += (sender, eventArgs) => FitGridHeightToContent();

        // When rows are added to the DataGridView, re-fit height
        dataGridView.RowsAdded += (sender, eventArgs) => FitGridHeightToContent();

        // When rows are removed from the DataGridView, re-fit height
        dataGridView.RowsRemoved += (sender, eventArgs) => FitGridHeightToContent();

        // Bubble selection-changed event up to a parent form that subscribes to AnnouncementSelectionChanged
        dataGridView.SelectionChanged += (sender, eventArgs) =>
        {
            AnnouncementSelectionChanged?.Invoke(this, EventArgs.Empty);
        };

        // Attach cell formatting event handler (for dates and applicants "cur/max")
        dataGridView.CellFormatting += dataGridView_CellFormatting;

        // Initial height fitting after the grid is fully configured
        FitGridHeightToContent();
    }
}
