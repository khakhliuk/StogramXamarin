using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace mobAppTry
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LogInPage : ContentPage
    {
        private string adress = "http://10.0.2.2:4000/api/";
        public LogInPage()
        {
            InitializeComponent();
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += async (s, e) => {
                loginFrame.BackgroundColor = Color.MediumBlue;
                await ProceedLogin();
                loginFrame.BackgroundColor = Color.DodgerBlue;
            };
            labelLogin.GestureRecognizers.Add(tapGestureRecognizer);

            tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) => {
                signInFrame.BackgroundColor = Color.DimGray;
                ProceedSignIn();
            };
            labelSignIn.GestureRecognizers.Add(tapGestureRecognizer);
        }

        public async Task ProceedLogin()
        {
            if (loginEntry.Text == null || loginEntry.Text == "" || passwordEntry.Text == null || passwordEntry.Text == "")
            {
                await DisplayAlert("Ошибка", "Не все поля заполнены", "OK");
                loginFrame.BackgroundColor = Color.DodgerBlue;
                return;
            }

            UserModel user = new UserModel()
            {
                Login = loginEntry.Text,
                Password = passwordEntry.Text
            };
            loginFrame.BackgroundColor = Color.DodgerBlue;

            try
            {
                WebClient webClient = new WebClient();
                webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
                string json = JsonConvert.SerializeObject(user);
                var userAnswer = JsonConvert.DeserializeObject<UserModel>(webClient.UploadString(adress + "Login", json));
                loginFrame.BackgroundColor = Color.DodgerBlue;
                if (userAnswer != null)
                {
                    App.Current.MainPage = new MainPage(userAnswer);
                }
                else
                {
                    await DisplayAlert("Ошибка", "Логин или пароль не верный", "OK");
                    loginFrame.BackgroundColor = Color.DodgerBlue;
                    return;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "OK");
            }

        }

        public async Task ProceedSignIn()
        {
            if (loginEntry.Text == null || loginEntry.Text == "" || passwordEntry.Text == null || passwordEntry.Text == "")
            {
                await DisplayAlert("Ошибка", "Не все поля заполнены", "OK");
                signInFrame.BackgroundColor = Color.DarkGray;
                return;
            }

            UserModel user = new UserModel()
            {
                Login = loginEntry.Text,
                Password = passwordEntry.Text
            };

            try
            {
                WebClient webClient = new WebClient();
                webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
                string json = JsonConvert.SerializeObject(user);
                var userAnswer = JsonConvert.DeserializeObject<UserModel>(webClient.UploadString(adress + "Login/create", json));
                if (userAnswer != null)
                {
                    App.Current.MainPage = new MainPage(userAnswer);
                }
                else
                {
                    await DisplayAlert("Ошибка", "Чел с таким именем уже существует", "OK");
                    signInFrame.BackgroundColor = Color.DarkGray;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "OK");
            }

            
        }
    }
}