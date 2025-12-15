namespace InfiniteInfluence.WinFormsApp;


/// <summary>
/// This is the main forms that will be launched and is a pseudo-login window for system administrators.
/// The window handles the login flow, although no actual validation is added, and upon clicking the login 
/// button it opens the dashboard where influencers can be managed.
/// </summary>
public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();

        // Defines the minimum and maximum size for the form forcing it to be the same size all the time
        SetFormSize();
    }


    //////////////////////
    //  Event handlers  //
    //////////////////////

    /// <summary>
    /// Resets the Forgot password link color to its default coloring when the mouse leaves the link.
    /// </summary>
    private void LinkLabelForgotPassword_Normal(object? sender, EventArgs e)
    {
        ChangeLinkLabelColor(Color.FromArgb(134, 129, 158));
    }


    /// <summary>
    /// Changes the forgot password link color when the mouse hovers over it to provide visual feedback to the user.
    /// </summary>
    private void LinkLabelForgotPassword_Hover(object? sender, EventArgs e)
    {
        ChangeLinkLabelColor(Color.FromArgb(190, 170, 255));
    }


    /// <summary>
    /// Handles when the login button is clicked.
    /// Currently no login validation is in place, but if it were to be then it should be performed here
    /// before giving the user access to the dashboard.
    /// </summary>
    private void buttonLogin_Click(object sender, EventArgs e)
    {
        LogInToDashBoard();
    }



    /////////////////
    // - Methods - //
    /////////////////

    /// <summary>
    /// Simply creates and opens the dashboard window.
    /// </summary>
    private void LogInToDashBoard()
    {
        DashBoard dashBoard = new DashBoard(this);
        dashBoard.Show();
    }


    /// <summary>
    /// Changes the link color for the forgot password link.
    /// </summary>
    /// <param name="color">The color to apply to the link.</param>
    private void ChangeLinkLabelColor(Color color)
    {
        linkLabelForgotPassword.LinkColor = color;
    }


    /// <summary>
    /// Sets a fixed form size by setting both minimum and maximum size to the same width and height.
    /// </summary>
    private void SetFormSize()
    {
        // Defines the minimum and maximum size for the form forcing it to be the same size all the time
        MaximumSize = new Size(950, 660);
        MinimumSize = new Size(950, 660);
    }
}
