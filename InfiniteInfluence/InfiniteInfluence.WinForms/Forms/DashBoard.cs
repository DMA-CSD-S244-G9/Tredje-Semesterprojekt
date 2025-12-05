using FontAwesome.Sharp;
using InfiniteInfluence.WinFormsApp.Components;
using InfiniteInfluence.WinFormsApp.Forms;


namespace InfiniteInfluence.WinFormsApp;


/// <summary>
/// This is a dashboard window that is shown to users after using the login system.
/// Provides navigation to the different sections such as influencer, company and announcement management
/// and handles logging out and navigating back to the login form.
/// </summary>
public partial class DashBoard : Form
{
    // A reference to the login form so we can show it again when logging out
    private MainForm _loginForm;
    
    // The chosen form window to be displayed inside of the designated area for different content
    private Form currentFormWindow;

    // The components to make up the left navigation sidebar interaction behavior
    private IconButton currentlySelectedButton;


    /// <summary>
    /// Initializes a new instance of the <see cref="DashBoard"/> class.
    /// </summary>
    /// 
    /// <param name="loginForm">
    /// The login form that opened this dashboard. 
    /// The login form will be hidden while the dashboard is open and then shown again when the user logs out.
    /// </param>
    public DashBoard(MainForm loginForm)
    {
        InitializeComponent();

        // Store a reference to the login form and hides the login form while the dashboard is active
        AssignAndHideLoginForm(loginForm);

        // Highlights Home as the default selection
        HighlightHomeButton();

        // Defines a minimum window size so the layout does not break
        SetFormSize();

        // Adds a yellow tint animation to the picture box elements which are the white stars
        AddHoverTintAnimationsToPictureBoxes();
    }


    ///////////////////////////////
    // - User Interface Events - //
    ///////////////////////////////


    /// <summary>
    /// Clicking the company logo behaves like clicking the Home button.
    /// </summary>
    private void PictureBoxCompanyLogo_Click(object sender, EventArgs e)
    {
        HighlightHomeButton();
    }


    /// <summary>
    /// Highlights the Home button and attempts to close any child form that may be open
    /// </summary>
    private void IconButtonHome_Click(object sender, EventArgs e)
    {
        HighlightHomeButton();
    }


    /// <summary>
    /// Navigates to the announcement management view, and highlights the announcement in the navigation panel.
    /// Opens the <see cref="AnnouncementsForm"/> inside the dashboard's content area.
    /// </summary>
    private void IconButtonAnnouncement_Click(object sender, EventArgs e)
    {
        // Visually highlight the clicked sidebar button and unhighlight the previously selected button one
        HighlightCurrentlySelectedButton(sender);

        // Creates a new instance of the announcement management form
        AnnouncementsForm announcementForm = new AnnouncementsForm();

        // Display the announcements form inside the dashboard's main content panel
        OpenChildForm(announcementForm);
    }


    /// <summary>
    /// Logs out the user by closing the dashboard window and showing the stored login form window again
    /// </summary>
    private void IconButtonLogOut_Click(object sender, EventArgs e)
    {
        CloseDashBoardAndOpenLoginForm();
    }



    ///////////////////////
    //  Helper Methods   //
    ///////////////////////

    /// <summary>
    /// Stores a reference to the login form and hides it while the dashboard is displayed.
    /// </summary>
    /// <param name="loginForm">The login form that was logged in to, in order to open this dashboard.</param>
    private void AssignAndHideLoginForm(MainForm loginForm)
    {
        // Sets the form that was
        _loginForm = loginForm;

        // Turns the MainForm (Login Window) invisible
        _loginForm.Hide();
    }


    /// <summary>
    /// Highlights the home navigation button and closes any currently open child form, thereby resetting the 
    /// dashboard content area to the default home view.
    /// </summary>
    private void HighlightHomeButton()
    {
        // Highlight the Home button in the sidebar and reset any previously selected button
        HighlightCurrentlySelectedButton(iconButtonHome);

        // If a child form is currently open inside the dashboard then execute this section
        if (currentFormWindow != null)
        {
            // closes the child form to return the dashboard to its default home / welcome page state
            currentFormWindow.Close();
        }
    }


    /// <summary>
    /// Highlights the clicked navigation button and resets the previously highlighted one. This also updates the title
    /// label to indicate which section is the currently active one to the user.
    /// </summary>
    /// <param name="clickedButton">The button that was clicked.</param>
    private void HighlightCurrentlySelectedButton(object clickedButton)
    {
        // If no button is clicked then execute this section
        if (clickedButton == null)
        {
            return;
        }

        // Resets the button back to its initial state to ensure only one button is highlighted at a time
        ResetButtonToInitialState();

        // Changes the button's background, fore and icon colors to their highlighted colors
        currentlySelectedButton = (IconButton)clickedButton;
        currentlySelectedButton.BackColor = Color.FromArgb(42, 9, 85);
        currentlySelectedButton.ForeColor = Color.FromArgb(234, 219, 255);
        currentlySelectedButton.IconColor = Color.FromArgb(234, 219, 255);

        // Obtains the padding of the button and stores it within the variable, and then  sets the left padding to 10 to move the text
        Padding currentPadding = currentlySelectedButton.Padding;
        currentlySelectedButton.Padding = new Padding(20, currentPadding.Top, currentPadding.Right, currentPadding.Bottom);

        // Updates the text in the header field indicating which navigation element is currently chosen
        labelSelectedSideMenuTitle.Text = currentlySelectedButton.Text;
    }


    /// <summary>
    /// Resets the currently active navigation button back to its default styling and padding if another button was already highlighted.
    /// </summary>
    private void ResetButtonToInitialState()
    {
        // If no button is currently selected then execute this section
        if (currentlySelectedButton == null)
        {
            return;
        }

        // Changes the button's background, fore and icon colors to their initial colors
        currentlySelectedButton.BackColor = Color.FromArgb(44, 33, 56);
        currentlySelectedButton.ForeColor = Color.FromArgb(222, 222, 222);
        currentlySelectedButton.IconColor = Color.FromArgb(222, 222, 222);

        // Obtains the padding of the button and stores it within the variable, and then  sets the left padding to 10 to move it to the original place
        Padding currentPadding = currentlySelectedButton.Padding;
        currentlySelectedButton.Padding = new Padding(10, currentPadding.Top, currentPadding.Right, currentPadding.Bottom);
    }


    /// <summary>
    /// Opens a child form inside the dashboard's main content panel and closes any existing child form 
    /// if it is different from the requested child form.
    /// </summary>
    /// <param name="childForm">The form to display inside the dashboard's content panel.</param>
    private void OpenChildForm(Form childForm)
    {
        // If the currentFormWindow is not null
        if (currentFormWindow != null)
        {
            // If the currentFormWindow is not disposed currently, and is of the same datatype e.g. AnnouncementsForm then execute this section
            if (!currentFormWindow.IsDisposed && currentFormWindow.GetType() == childForm.GetType())
            {
                return;
            }

            // Elsewise close the current child form
            currentFormWindow.Close();
        }

        // Sets the current form that is being displayed to the Form objects specified in the method parameter
        currentFormWindow = childForm;

        // Makes the form a non-toplevel form displayable window (Setting this to true will cause an error)
        childForm.TopLevel = false;

        // Removes the edge of the form
        childForm.FormBorderStyle = FormBorderStyle.None;

        // Changes the dock property to make it fill out the panel container
        childForm.Dock = DockStyle.Fill;

        // Adds the child form to the panel container
        panelDesktop.Controls.Add(childForm);

        // Associates the form data to the panel
        panelDesktop.Tag = childForm;

        // Lastly brings the form to the front and also shows it
        childForm.BringToFront();

        // Turns the child form visible
        childForm.Show();
    }


    /// <summary>
    /// Logs out by closing the dashboard and showing the stored login form window again
    /// </summary>
    private void CloseDashBoardAndOpenLoginForm()
    {
        // Turns the MainForm (Login Window) visible again
        _loginForm.Show();

        // Closes the Dashboard form and disposes of it thereafter
        this.Close();
        this.Dispose();
    }


    /// <summary>
    /// Sets the minimum size of the dashboard window.
    /// This prevents the layout from breaking and looking weird when the window is resized to a too small size.
    /// </summary>
    private void SetFormSize()
    {
        // Defines the minimum size for the form
        MinimumSize = new Size(1248, 740);
    }



    /// <summary>
    /// Adds hover tint animations to the PictureBox elements and sets their parent to the top frame so
    /// that the transparency and layering work correctly
    /// </summary>
    private void AddHoverTintAnimationsToPictureBoxes()
    {
        // Apply orange tint to PictureBox objects over half a second's duration
        HoverTintAnimator.Attach(pictureBoxStarTiny, Color.Orange, 500);
        HoverTintAnimator.Attach(pictureBoxStarSmall1, Color.Orange, 500);
        HoverTintAnimator.Attach(pictureBoxStarSmall2, Color.Orange, 500);
        HoverTintAnimator.Attach(pictureBoxStarMedium, Color.Orange, 500);
        HoverTintAnimator.Attach(pictureBoxStarLarge, Color.Orange, 500);
        HoverTintAnimator.Attach(pictureBoxCompanyLogo, Color.Orange, 500);

        // Attaches the pictureboxes of stars and the company logo to the dashboard's top frame to have transparency work correctly
        pictureBoxStarTiny.Parent = pictureBoxTopFrame;
        pictureBoxStarSmall1.Parent = pictureBoxTopFrame;
        pictureBoxStarSmall2.Parent = pictureBoxTopFrame;
        pictureBoxStarMedium.Parent = pictureBoxTopFrame;
        pictureBoxStarLarge.Parent = pictureBoxTopFrame;
        labelSelectedSideMenuTitle.Parent = pictureBoxTopFrame;
    }
}
