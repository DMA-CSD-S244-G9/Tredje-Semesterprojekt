using InfiniteInfluence.ApiClient;
using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Model;
using InfiniteInfluence.WinFormsApp.Components;


namespace InfiniteInfluence.WinFormsApp.Forms;


/// <summary>
/// WinForms dashboard child form responsible for displaying, searching,
/// and deleting announcements using a reusable <see cref="AnnouncementGridControl"/>.
/// This form communicates with the REST Web API (via <see cref="IAnnouncementDao"/>)
/// to load and delete announcements, while delegating all grid-related UI logic
/// to the announcement grid control.
/// </summary>
public partial class AnnouncementsForm : Form
{
    // API client to retrieve announcements from the REST API
    private readonly IAnnouncementDao _announcementApiClient;

    // Our new reusable grid control
    private readonly AnnouncementGridControl _announcementGridControl;


    /// <summary>
    /// Initializes a new instance of the <see cref="AnnouncementsForm"/> class.
    /// This sets up the API client, creates and docks the announcements data grid view, and adds the user interface events
    /// for selection and search / filtering and loads the announcement data that are retrieved from the REST Web API.
    /// </summary>
    public AnnouncementsForm()
    {
        InitializeComponent();

        // Initializes the API client with the url and port matching the web api
        //_announcementApiClient = new AnnouncementApiClient("https://localhost:7777");
        _announcementApiClient = new AnnouncementApiClient("https://localhost:32771");

        // Creates and adds the grid data view controls to the panel
        _announcementGridControl = new AnnouncementGridControl();
        _announcementGridControl.Dock = DockStyle.Fill;
        panelAnnouncementContent.Controls.Add(_announcementGridControl);

        // Enable/disable delete button when selection changes
        _announcementGridControl.AnnouncementSelectionChanged += (s, e) =>
        {
            buttonAnnouncementDelete.Enabled = _announcementGridControl.SelectedAnnouncement != null;
        };

        // Calls upon the API to retrieve all announcements and send them to the AnnouncementGridController
        LoadAnnouncementsFromApi();

        // Hook up search bar
        AddSearchBarEvent();
    }



    ///////////////////////////////
    // - User Interface Events - //
    ///////////////////////////////


    /// <summary>
    /// Handles the click event of the "Delete" button for announcements.
    /// Delegates to <see cref="InitiateDeleteSelectedAnnouncement"/> to perform the deletion process where the cursor state changes, 
    /// the API call occurs and refreshing of the data grid view's contents occur.
    /// </summary>
    private void buttonAnnouncementDelete_Click(object sender, EventArgs e)
    {
        // Deletes an announcement from the grid
        InitiateDeleteSelectedAnnouncement();
    }


    /// <summary>
    /// Subscribes to the search text box's <see cref="Control.TextChanged"/> event so that whenever the system administrator types, then the announcement grid is 
    /// filtered according to the current search query which filters based on the announcementId, Title and company name.
    /// </summary>
    private void AddSearchBarEvent()
    {
        // When the user types in the search box we filter announcements in the data grid view
        roundedTextBoxAnnouncementsSearch.TextChanged += (s, e) =>
        {
            _announcementGridControl.ApplyFilter(roundedTextBoxAnnouncementsSearch.Text);
        };
    }



    //////////////////////////
    // - Deletion Methods - //
    //////////////////////////


    /// <summary>
    /// Initiates the deletion of the currently selected announcement.
    /// Temporarily disables the delete button and shows a wait cursor to indicate that work is being done, then proceeds to call <see cref="DeleteSelectedAnnouncement"/>.
    /// After the operation, then the cursor is changed back to the initial state and button is once again enabled for interaction.
    /// </summary>
    private void InitiateDeleteSelectedAnnouncement()
    {
        // Locks the button by changing it to disabled
        buttonAnnouncementDelete.Enabled = false;

        // Obtains the current cursor's state and stores it in the previousCursor variable
        Cursor previousCursor = Cursor.Current;

        // Changes the cursor to show an hour glass illustrating waiting
        Cursor.Current = Cursors.WaitCursor;

        try
        {
            // Attempts to delete the selected announcement from the database and datagridview
            DeleteSelectedAnnouncement();
        }
        finally
        {
            // Changes the cursor back to its initial state as waiting is now over
            Cursor.Current = previousCursor;

            // Unlocks the button by changing it back to an enabled state
            buttonAnnouncementDelete.Enabled = true;
        }
    }


    /// <summary>
    /// Deletes the currently selected announcement if one is selected / highlighted in the data grid view.
    /// 
    /// This method first retrieves the selected <see cref="Announcement"/> from the grid control.
    /// Then shows a confirmation dialog to the user, awaiting a choice, if yes is chosen then it proceeds to
    /// call upon the REST Web API to delete the announcement with the matching announcementId from the database.
    /// After the deletion it reloads the announcements from the API and reapplies the existing search filter if anything was entered.
    /// If anythin gfails during the deletion process an error dialog is shown to the system administrator.
    /// </summary>
    private void DeleteSelectedAnnouncement()
    {
        // Retrieves the announcement object that was selected in the datagrid view
        Announcement? currentAnnouncement = _announcementGridControl.SelectedAnnouncement;

        // If no announcement object is highlighted / selected then execute this section
        if (currentAnnouncement == null)
        {
            return;
        }

        // Builds the confirmation text that will be shown to the user in the modal dialog window
        string deleteDialogMessage = $"Do you want to delete announcement #{currentAnnouncement.AnnouncementId} - '{currentAnnouncement.Title}' from the database?";

        // Creates the modal popup which contains a Yes and No option
        DialogResult modalDialogWindow = MessageBox.Show(deleteDialogMessage, "Confirm deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

        // If the selected option is not yes then execute this section
        if (modalDialogWindow != DialogResult.Yes)
        {
            return;
        }

        try
        {
            // Calls upon the API to delete the announcement from the database
            bool isAnnouncementDeleted = _announcementApiClient.Delete(currentAnnouncement.AnnouncementId);

            // If the deletion of the announcement failed then execute this section
            if (!isAnnouncementDeleted)
            {
                MessageBox.Show("The announcement was unable to be deleted from the database", "Announcement deletion failed", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            // Remembers the currently used search text in order to keep the filter after reloading
            string currentSearchQuery = roundedTextBoxAnnouncementsSearch.Text;

            // Reloads the data by re-retrieving it from the API so WinForms correctly reflects the database
            LoadAnnouncementsFromApi();

            // Reapplies the search criteria to the filtering as to not ruin the user experience
            _announcementGridControl.ApplyFilter(currentSearchQuery);
        }

        catch (Exception exception)
        {
            // If something went wrong while calling the API or deleting, notify the user
            MessageBox.Show("The following exception occured while trying to delete the announcement:\n" + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }



    /////////////////////
    // - API Methods - //
    /////////////////////


    /// <summary>
    /// Retrieves all of the announcements through the REST Web API using the <see cref="IAnnouncementDao"/>  and passes the retrieved list of announcements, 
    /// and passes them to the <see cref="AnnouncementGridControl"/> which then adds the announcement objects to the data grid view
    /// If the API call fails, an error dialog is shown to the system administrator.
    /// </summary>
    private void LoadAnnouncementsFromApi()
    {
        try
        {
            // Calls upon the API to retrieve all of the announcements in the form of an IEnumerable
            IEnumerable<Announcement> announcementsFromApi = _announcementApiClient.GetAll();

            // Supplies the retrieved announcements to the AnnouncementGridControl
            _announcementGridControl.SetAnnouncements(announcementsFromApi);
        }

        catch (Exception exception)
        {
            MessageBox.Show($"Unable to retrieve the list of announcements from the API. Report the following error: {exception.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
