﻿using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;


namespace Instagram
{
    public partial class Home : Form
    {
        bool lightModeOn = false;
        DBHandlingUtilities dbHandler;
        UIUtilities UI;
        List<string[]> dataTable;
        string ImageNewName = "";
        int listViewItemsCount = 0;
        ImageList listViewImageList = new ImageList();
        Post[] postList;

        public Home()
        {
            InitializeComponent();
            dbHandler = new DBHandlingUtilities();
            UI = new UIUtilities(this, true);
            Configure_Theme();
            Import_All_Following();
        }
        private void Configure_Theme()
        {
            Color backColor, foreColor;
            if (lightModeOn)
            {
                backColor = Color.FromArgb(255, 255, 255);
                foreColor = Color.FromArgb(242, 242, 242);
            }
            else
            {
                backColor = Color.FromArgb(43, 43, 43);
                foreColor = Color.FromArgb(0, 0, 0);
            }
            this.BackColor = foreColor;
            storyListView.BackColor = backColor;
            feedPanel.BackColor = foreColor;
            
        }


        private void Import_All_Following()
        {
            dataTable = dbHandler.Import_Data_Using_SQL("5", "Ahmad", "FollowingTable");
            dbHandler.Truncate_Temporary_Post_Table();
            Generate_Posts_And_Stories(dataTable);
            postList = dbHandler.Generate_Posts(lightModeOn);
            Display_Posts();
        }

        private void Display_Posts()
        {
            feedPanel.AutoScroll = true;
            for(int i=0; i< postList.Length;i++)
            {
                postList[i].TopLevel = false;
                feedPanel.Controls.Add(postList[i]);
                postList[i].Show();
            }
        }

        private void Add_All_Stories(string userID, string userName)
        {
            dbHandler.Create_Valid_Story_View(userID, userName);
            if (dbHandler.Check_If_Story_Exists(userID, userName))
            {
                try
                {
                    listViewImageList.Images.Add(dbHandler.Retrieve_Profile_Picture_Using_SQL(Int32.Parse(userID)));
                    storyListView.Items.Add(new ListViewItem
                    {
                        ImageIndex = listViewItemsCount,
                        Text = userName,
                        Tag = userName + '_' + userID
                    }); ; ; ; ; ; ;
                    listViewItemsCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void Add_All_Post(string userID, string userName)
        {
            dbHandler.Create_View_For_Following_Posts(userID,userName);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dbHandler.Add_Post("3", "Eqan", "Hello", "Hyderabad");
            feedPanel.Refresh();
        }

        public void Generate_Posts_And_Stories(List<string[]> usersList)
        {
            listViewImageList.ImageSize = new Size(50, 50);
            for (int i = 0; i < usersList.Count; i++)
            {
                Add_All_Stories(usersList[i][1], usersList[i][2]);
                Add_All_Post(usersList[i][1], usersList[i][2]);
            }
            storyListView.LargeImageList = listViewImageList; 
        }

        private void listView1_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (e.Label == null)
                return;
            ImageNewName = Convert.ToString(e.Label);
            ListViewItem item1 = storyListView.SelectedItems[0];
            FileInfo fileInfo = new FileInfo(item1.Tag.ToString());
            fileInfo.MoveTo(fileInfo.Directory.FullName + "\\" + ImageNewName + fileInfo.Extension);
            storyListView.Items[listViewItemsCount].Text = ImageNewName;
        }


        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (storyListView.SelectedIndices.Count <= 0)
            {
                Console.WriteLine(storyListView.SelectedItems.Count);
                return;
            }
            if (storyListView.SelectedIndices[0] >= 0)
            {
                var item = storyListView.SelectedItems[0];
                Image img = listViewImageList.Images[item.ImageIndex];
                this.Hide();
                Story story = new Story(storyListView.SelectedItems[0].Tag.ToString(), img, false);
                story.Show();
            }
        }

        private void feedPanel_Paint(object sender, PaintEventArgs e)
        {
        }
    }
}
