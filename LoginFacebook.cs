using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json.Linq;
using Xamarin.Auth;

namespace App_Teste
{
    class LoginFacebook
    {
        public void LoginFacebook()
        {

            var activity = this as Activity;

            var auth = new OAuth2Authenticator(
                clientId: "159718315402370",
                scope: "",
                authorizeUrl: new Uri("https://m.facebook.com/dialog/oauth"),
                redirectUrl: new Uri("http://www.facebook/com/connect/login_sucess.html"));

            auth.Completed += async (sender, eventArgs) =>
            {
                if (eventArgs.IsAuthenticated)
                {
                    var accessToken = eventArgs.Account.Properties["access_token"].ToString();
                    var expiresIn = Convert.ToDouble(eventArgs.Account.Properties["expires_in"]);
                    var expiryDate = DateTime.Now + TimeSpan.FromSeconds(expiresIn);

                    var request = new OAuth2Request("GET", new Uri("https://graph.facebook.com/me?fields=name,picture"), null, eventArgs.Account);
                    ///https://graph.facebook.com/me?fields=email,first_name,last_name,gender,picture
                    var response = await request.GetResponseAsync();
                    var obj = JObject.Parse(response.GetResponseText());

                    Intent intent = new Intent(this, typeof(Perfil));
                    intent.PutExtra("ID", obj["id"].ToString());
                    intent.PutExtra("Nome", obj["name"].ToString());
                    ImageService.SaveToDisk("imagemFacebook", await ImageService.DownloadImage("https://graph.facebook.com/" + obj["id"].ToString() + "/picture?type=large"));
                    StartActivity(intent);
                }
                else
                {
                    Console.WriteLine("Usuário cancelou");
                }
            };

            activity.StartActivity(auth.GetUI(activity));

        }
    }
}