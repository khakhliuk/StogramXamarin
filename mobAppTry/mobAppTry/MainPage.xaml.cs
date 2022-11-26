using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json;
using Plugin.FilePicker;
using System.Net;
using Xamarin.Essentials;

namespace mobAppTry
{
    public partial class MainPage : ContentPage
    {
        private UserModel loggedUser;
        private NetWorker netWorker = new NetWorker();
        private IEnumerable<PostModel> postsFeed;
        public MainPage(UserModel user)
        {
            InitializeComponent();
            loggedUser = user;
            UpdateFeed(false);

            refreshButton.Clicked += (s, e) =>
            {
                UpdateFeed(false);
            };

            myPostsButton.Clicked += (s, e) =>
            {
                UpdateFeed(true);
            };

            addPostButton.Clicked += AddPostButtonClicked;
        }

        private async void AddPostButtonClicked(object sender, EventArgs e)
        {
            var pickResult = await FilePicker.PickAsync(new PickOptions()
            {
                FileTypes = FilePickerFileType.Images,
                PickerTitle = "Pick an Image"
            });

            if (pickResult != null)
            {
                using (FileStream fsSource = new FileStream(pickResult.FullPath,
                    FileMode.Open, FileAccess.Read))
                {
                    byte[] imageBytes = new byte[fsSource.Length];
                    int numBytesToRead = (int)fsSource.Length;
                    int numBytesRead = 0;
                    while (numBytesToRead > 0)
                    {
                        int n = fsSource.Read(imageBytes, numBytesRead, numBytesToRead);
                        
                        if (n == 0)
                            break;

                        numBytesRead += n;
                        numBytesToRead -= n;
                    }

                    string json = JsonConvert.SerializeObject(new PostModel()
                    {
                        UserName = loggedUser.Login,
                        ImageBytes = Convert.ToBase64String(imageBytes)
                    });
                    netWorker.Post("Posts", json);
                    UpdateFeed(false);
                }
            }
        }

        public void UpdateFeed(bool myPosts)
        {
            string answer;

            if (myPosts)
            {
                answer = netWorker.Get("Posts/my/" + loggedUser.Login);
            }
            else
            {
                answer = netWorker.Get("Posts");
            }
            
            postsFeed = JsonConvert.DeserializeObject<IEnumerable<PostModel>>(answer);
            StackLayout mainStack = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.FillAndExpand, 
                VerticalOptions = LayoutOptions.FillAndExpand,
                Spacing = 0
            };

            foreach (var post in postsFeed.Reverse())
            {
                Frame postFrame = new Frame()
                {
                    BorderColor = Color.Black,
                    CornerRadius = 0,
                    Padding = new Thickness(0, 0)
                };

                StackLayout postStack = new StackLayout();

                StackLayout postTopStack = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal
                };

                Frame userImageFrame = new Frame()
                {
                    BackgroundColor = Color.Black,
                    WidthRequest = 15,
                    HeightRequest = 15,
                    VerticalOptions = LayoutOptions.Center,
                    CornerRadius = 40,
                    Margin = new Thickness(10,5,0,0)
                };

                Label labelName = new Label()
                {
                    Text = post.UserName,
                    FontSize = 18,
                    TextColor = Color.Black,
                    HorizontalTextAlignment = TextAlignment.Start,
                    Padding = new Thickness(0, 5, 0 , 0)
                };

                postTopStack.Children.Add(userImageFrame);
                postTopStack.Children.Add(labelName);
                postStack.Children.Add(postTopStack);
                
                var imageBytes = Convert.FromBase64String(post.ImageBytes);
                MemoryStream stream = new MemoryStream(imageBytes);

                Image image = new Image()
                {
                    Source = ImageSource.FromStream(() => stream),
                    Aspect = Aspect.AspectFill
                };

                postStack.Children.Add(image);
                
                StackLayout likesStack = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.Center
                };

                Label likesLabel = new Label()
                {
                    Text = "Dislikes: " + post.Likes,
                    FontSize = 16,
                    TextColor = Color.Black,
                    Padding = new Thickness(10, 0, 0, 5)
                };

                postStack.Children.Add(likesLabel);

                if (post.UserName != loggedUser.Login)
                {
                    Button likeButton = new Button()
                    {
                        Text = "Bruh it",
                        FontSize = 20,
                        TextColor = Color.White,
                        BackgroundColor = Color.DodgerBlue
                    };

                    likeButton.Clicked += (s, e) =>
                    {
                        var answPost = JsonConvert.DeserializeObject<PostModel>(netWorker.Get("Posts/like/" + post.Id));
                        likesLabel.Text = "Dislikes: " + answPost.Likes;
                    };
                    postStack.Children.Add(likeButton);
                }

                postFrame.Content = postStack;
                mainStack.Children.Add(postFrame);
            }

            if (postsFeed.Count() == 25)
            {
                Button buttonLoadMore = new Button()
                {
                    Text = "Load more"
                };
                mainStack.Children.Add(buttonLoadMore);
            }
            scrollFeed.Content = mainStack;
        }

        public void LikePost(object sender, System.EventArgs e)
        {

        }
    }
}
